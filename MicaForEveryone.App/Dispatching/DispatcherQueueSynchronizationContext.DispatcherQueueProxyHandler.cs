using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using WinRT;

#nullable enable

namespace MicaForEveryone.App.Dispatching;

/// <inheritdoc />
public partial class DispatcherQueueSynchronizationContext
{
    /// <summary>
    /// A custom <c>IDispatcherQueueHandler</c> object, that internally stores a captured <see cref="SendOrPostCallback"/> instance and the
    /// input captured state. This allows consumers to enqueue a state and a cached stateless delegate without any managed allocations.
    /// </summary>
    private unsafe struct DispatcherQueueProxyHandler
    {
        /// <summary>
        /// The shared vtable pointer for <see cref="DispatcherQueueProxyHandler"/> instances.
        /// </summary>
        private static readonly void** Vtbl = InitVtbl();

        /// <summary>
        /// Setups the vtable pointer for <see cref="DispatcherQueueProxyHandler"/>.
        /// </summary>
        /// <returns>The initialized vtable pointer for <see cref="DispatcherQueueProxyHandler"/>.</returns>
        /// <remarks>
        /// The vtable itself is allocated with <see cref="RuntimeHelpers.AllocateTypeAssociatedMemory(Type, int)"/>,
        /// which allocates memory in the high frequency heap associated with the input runtime type. This will be
        /// automatically cleaned up when the type is unloaded, so there is no need to ever manually free this memory.
        /// </remarks>
        private static void** InitVtbl()
        {
            void** vtbl = (void**)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(DispatcherQueueProxyHandler), sizeof(void*) * 4);

            vtbl[0] = (delegate* unmanaged<DispatcherQueueProxyHandler*, Guid*, void**, int>)&Impl.QueryInterface;
            vtbl[1] = (delegate* unmanaged<DispatcherQueueProxyHandler*, uint>)&Impl.AddRef;
            vtbl[2] = (delegate* unmanaged<DispatcherQueueProxyHandler*, uint>)&Impl.Release;
            vtbl[3] = (delegate* unmanaged<DispatcherQueueProxyHandler*, int>)&Impl.Invoke;

            return vtbl;
        }

        /// <summary>
        /// The vtable pointer for the current instance.
        /// </summary>
        private void** vtbl;

        /// <summary>
        /// The <see cref="GCHandle"/> to the captured <see cref="SendOrPostCallback"/>.
        /// </summary>
        private GCHandle callbackHandle;

        /// <summary>
        /// The <see cref="GCHandle"/> to the captured state (if present, or a <see langword="null"/> handle otherwise).
        /// </summary>
        private GCHandle stateHandle;

        /// <summary>
        /// The current reference count for the object (from <c>IUnknown</c>).
        /// </summary>
        private volatile uint referenceCount;

        /// <summary>
        /// Creates a new <see cref="DispatcherQueueProxyHandler"/> instance for the input callback and state.
        /// </summary>
        /// <param name="handler">The input <see cref="SendOrPostCallback"/> callback to enqueue.</param>
        /// <param name="state">The input state to capture and pass to the callback.</param>
        /// <returns>A pointer to the newly initialized <see cref="DispatcherQueueProxyHandler"/> instance.</returns>
        public static DispatcherQueueProxyHandler* Create(SendOrPostCallback handler, object? state)
        {
            DispatcherQueueProxyHandler* @this = (DispatcherQueueProxyHandler*)NativeMemory.Alloc((nuint)sizeof(DispatcherQueueProxyHandler));

            @this->vtbl = Vtbl;
            @this->callbackHandle = GCHandle.Alloc(handler);
            @this->stateHandle = state is not null ? GCHandle.Alloc(state) : default;
            @this->referenceCount = 1;

            return @this;
        }

        /// <summary>
        /// Devirtualized API for <c>IUnknown.Release()</c>.
        /// </summary>
        /// <returns>The updated reference count for the current instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release()
        {
            uint referenceCount = Interlocked.Decrement(ref this.referenceCount);

            if (referenceCount == 0)
            {
                callbackHandle.Free();

                if (stateHandle.IsAllocated)
                {
                    stateHandle.Free();
                }

                NativeMemory.Free(Unsafe.AsPointer(ref this));
            }

            return referenceCount;
        }

        /// <summary>
        /// A private type with the implementation of the unmanaged methods for <see cref="DispatcherQueueProxyHandler"/>.
        /// These methods will be set into the shared vtable and invoked by WinRT from the object passed to it as an interface.
        /// </summary>
        private static class Impl
        {
            /// <summary>
            /// The HRESULT for a successful operation.
            /// </summary>
            private const int S_OK = 0;

            /// <summary>
            /// The HRESULT for an invalid cast from <c>IUnknown.QueryInterface</c>.
            /// </summary>
            private const int E_NOINTERFACE = unchecked((int)0x80004002);

            /// <summary>
            /// The GUID for the <c>IUnknown</c> COM interface.
            /// </summary>
            private static readonly Guid GuidOfIUnknown = new(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

            /// <summary>
            /// The GUID for the <c>IAgileObject</c> WinRT interface.
            /// </summary>
            private static readonly Guid GuidOfIAgileObject = new(0x94EA2B94, 0xE9CC, 0x49E0, 0xC0, 0xFF, 0xEE, 0x64, 0xCA, 0x8F, 0x5B, 0x90);

            /// <summary>
            /// The GUID for the <c>IDispatcherQueueHandler</c> WinRT interface.
            /// </summary>
            private static readonly Guid GuidOfIDispatcherQueueHandler = new(0x2E0872A9, 0x4E29, 0x5F14, 0xB6, 0x88, 0xFB, 0x96, 0xD5, 0xF9, 0xD5, 0xF8);

            /// <summary>
            /// Implements <c>IUnknown.QueryInterface(REFIID, void**)</c>.
            /// </summary>
            [UnmanagedCallersOnly]
            public static int QueryInterface(DispatcherQueueProxyHandler* @this, Guid* riid, void** ppvObject)
            {
                if (riid->Equals(GuidOfIUnknown) ||
                    riid->Equals(GuidOfIAgileObject) ||
                    riid->Equals(GuidOfIDispatcherQueueHandler))
                {
                    Interlocked.Increment(ref @this->referenceCount);

                    *ppvObject = @this;

                    return S_OK;
                }

                return E_NOINTERFACE;
            }

            /// <summary>
            /// Implements <c>IUnknown.AddRef()</c>.
            /// </summary>
            [UnmanagedCallersOnly]
            public static uint AddRef(DispatcherQueueProxyHandler* @this)
            {
                return Interlocked.Increment(ref @this->referenceCount);
            }

            /// <summary>
            /// Implements <c>IUnknown.Release()</c>.
            /// </summary>
            [UnmanagedCallersOnly]
            public static uint Release(DispatcherQueueProxyHandler* @this)
            {
                uint referenceCount = Interlocked.Decrement(ref @this->referenceCount);

                if (referenceCount == 0)
                {
                    @this->callbackHandle.Free();

                    if (@this->stateHandle.IsAllocated)
                    {
                        @this->stateHandle.Free();
                    }

                    NativeMemory.Free(@this);
                }

                return referenceCount;
            }

            /// <summary>
            /// Implements <c>IDispatcherQueueHandler.Invoke()</c>.
            /// </summary>
            [UnmanagedCallersOnly]
            public static int Invoke(DispatcherQueueProxyHandler* @this)
            {
                object callback = @this->callbackHandle.Target!;
                object? state = @this->stateHandle.IsAllocated ? @this->stateHandle.Target! : null;

                try
                {
                    Unsafe.As<SendOrPostCallback>(callback)(state);
                }
                catch (Exception e)
                {
                    ExceptionHelpers.SetErrorInfo(e);

                    return ExceptionHelpers.GetHRForException(e);
                }

                return S_OK;
            }
        }
    }
}