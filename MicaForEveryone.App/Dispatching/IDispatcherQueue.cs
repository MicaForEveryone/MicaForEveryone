using Microsoft.UI.Dispatching;
using System.Runtime.CompilerServices;

namespace MicaForEveryone.App.Dispatching;

#pragma warning disable CS0649

/// <summary>
/// A struct mapping the native WinRT <c>IDispatcherQueue</c> interface.
/// </summary>
public unsafe struct IDispatcherQueue
{
    /// <summary>
    /// The vtable pointer for the current instance.
    /// </summary>
    private readonly void** vtbl;

    /// <summary>
    /// Native API for <see cref="DispatcherQueue.TryEnqueue(DispatcherQueueHandler)"/>.
    /// </summary>
    /// <param name="callback">A pointer to an <c>IDispatcherQueueHandler</c> object.</param>
    /// <param name="result">The result of the operation (the <see cref="bool"/> WinRT retval).</param>
    /// <returns>The <c>HRESULT</c> for the operation.</returns>
    /// <remarks>The <paramref name="callback"/> parameter is assumed to be a pointer to an <c>IDispatcherQueueHandler</c> object.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int TryEnqueue(void* callback, bool* result)
    {
        return ((delegate* unmanaged<IDispatcherQueue*, void*, byte*, int>)vtbl[7])((IDispatcherQueue*)Unsafe.AsPointer(ref this), callback, (byte*)result);
    }
}