using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct COLORREF
    {
        /// <summary>The DWORD value</summary>
        [FieldOffset(0)]
        private uint Value;

        /// <summary>The intensity of the red color.</summary>
        [FieldOffset(0)]
        public byte R;

        /// <summary>The intensity of the green color.</summary>
        [FieldOffset(1)]
        public byte G;

        /// <summary>The intensity of the blue color.</summary>
        [FieldOffset(2)]
        public byte B;

        /// <summary>The transparency.</summary>
        [FieldOffset(3)]
        public byte A;

        private const uint CLR_NONE = 0xFFFFFFFF;
        private const uint CLR_DEFAULT = 0xFF000000;

        /// <summary>Initializes a new instance of the <see cref="COLORREF"/> struct.</summary>
        /// <param name="r">The intensity of the red color.</param>
        /// <param name="g">The intensity of the green color.</param>
        /// <param name="b">The intensity of the blue color.</param>
        public COLORREF(byte r, byte g, byte b)
        {
            Value = 0;
            A = 0;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>Initializes a new instance of the <see cref="COLORREF"/> struct.</summary>
        /// <param name="value">The packed DWORD value.</param>
        public COLORREF(uint value)
        {
            R = 0;
            G = 0;
            B = 0;
            A = 0;
            Value = value & 0x00FFFFFF;
        }

        /// <summary>Initializes a new instance of the <see cref="COLORREF"/> struct.</summary>
        /// <param name="color">The color.</param>
        public COLORREF(Color color) : this(color.R, color.G, color.B)
        {
            if (color == Color.Transparent)
                Value = CLR_NONE;
            A = (byte)(255 - color.A);
        }

        /// <summary>A method to darken a color by a percentage of the difference between the color and Black.</summary>
        /// <param name="percent">The percentage by which to darken the original color.</param>
        /// <returns>
        /// The return color's Alpha value will be unchanged, but the RGB content will have been increased by the specified percentage. If
        /// percent is 100 then the returned Color will be Black with original Alpha.
        /// </returns>
        public COLORREF Darken(float percent)
        {
            if (percent < 0 || percent > 1.0)
                throw new ArgumentOutOfRangeException(nameof(percent));

            return new(Conv(R), Conv(G), Conv(B)) { Value = Value };

            byte Conv(byte c) => (byte)(c - (int)(c * percent));
        }

        /// <summary>A method to lighten a color by a percentage of the difference between the color and Black.</summary>
        /// <param name="percent">The percentage by which to lighten the original color.</param>
        /// <returns>
        /// The return color's Alpha value will be unchanged, but the RGB content will have been decreased by the specified percentage. If
        /// percent is 100 then the returned Color will be White with original Alpha.
        /// </returns>
        public COLORREF Lighten(float percent)
        {
            if (percent < 0 || percent > 1.0)
                throw new ArgumentOutOfRangeException(nameof(percent));

            return new(Conv(R), Conv(G), Conv(B)) { Value = Value };

            byte Conv(byte c) => (byte)(c + (int)((255f - c) * percent));
        }

        public COLORREF Blend(COLORREF rhs)
        {
            return new COLORREF
            {
                A = (byte)((255 - A) * (255 - rhs.A) / 255),
                R = (byte)((R * rhs.A + rhs.R * (255 - rhs.A)) / 255),
                G = (byte)((G * rhs.A + rhs.G * (255 - rhs.A)) / 255),
                B = (byte)((B * rhs.A + rhs.B * (255 - rhs.A)) / 255),
            };
        }

        /// <summary>Gets the 32-bit ARGB value of this <see cref="COLORREF"/> structure.</summary>
        /// <returns>The 32-bit ARGB value of this <see cref="COLORREF"/> structure.</returns>
        public int ToArgb() => unchecked((int)Value);

        /// <summary>Performs an implicit conversion from <see cref="COLORREF"/> to <see cref="Color"/>.</summary>
        /// <param name="cr">The <see cref="COLORREF"/> value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Color(COLORREF cr) => cr.Value == CLR_NONE ? Color.Transparent : Color.FromArgb(255 - cr.A, cr.R, cr.G, cr.B);

        /// <summary>Performs an implicit conversion from <see cref="Color"/> to <see cref="COLORREF"/>.</summary>
        /// <param name="clr">The <see cref="Color"/> value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator COLORREF(Color clr) => new(clr);

        /// <summary>Performs an implicit conversion from <see cref="System.ValueTuple{R, G, B}"/> to <see cref="COLORREF"/>.</summary>
        /// <param name="clr">The tuple containing the R, G, and B values.</param>
        /// <returns>The resulting <see cref="COLORREF"/> instance from the conversion.</returns>
        public static implicit operator COLORREF((byte r, byte g, byte b) clr) => new(clr.r, clr.g, clr.b);

        /// <summary>Performs an implicit conversion from <see cref="COLORREF"/> to <see cref="System.UInt32"/>.</summary>
        /// <param name="cr">The <see cref="COLORREF"/> value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator uint(COLORREF cr) => cr.Value;

        /// <summary>Performs an implicit conversion from <see cref="System.UInt32"/> to <see cref="COLORREF"/>.</summary>
        /// <param name="value">The <see cref="uint"/> value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator COLORREF(uint value) => new(value);

        /// <summary>Static reference for CLR_NONE representing no color.</summary>
        public static COLORREF None = new(CLR_NONE);

        /// <summary>Static reference for CLR_DEFAULT representing the default color.</summary>
        public static COLORREF Default = new(0xFF000000);

        /// <inheritdoc />
        public override string ToString() => ((Color)this).ToString();
    }
}
