using System;
using System.Drawing;

namespace MicaForEveryone.Win32.PInvoke
{
    [Serializable]
    public struct RECT : IEquatable<RECT>, IEquatable<Rectangle>
    {
        /// <summary>The x-coordinate of the upper-left corner of the rectangle.</summary>
        public int left;

        /// <summary>The y-coordinate of the upper-left corner of the rectangle.</summary>
        public int top;

        /// <summary>The x-coordinate of the lower-right corner of the rectangle.</summary>
        public int right;

        /// <summary>The y-coordinate of the lower-right corner of the rectangle.</summary>
        public int bottom;

        /// <summary>Initializes a new instance of the <see cref="RECT"/> struct.</summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        /// <summary>Initializes a new instance of the <see cref="RECT"/> struct.</summary>
        /// <param name="r">The rectangle.</param>
        public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
        {
        }

        /// <summary>The x-coordinate of the upper-left corner of the rectangle.</summary>
        public int Left { get => left; set => left = value; }

        /// <summary>The x-coordinate of the lower-right corner of the rectangle.</summary>
        public int Right { get => right; set => right = value; }

        /// <summary>The y-coordinate of the upper-left corner of the rectangle.</summary>
        public int Top { get => top; set => top = value; }

        /// <summary>The y-coordinate of the lower-right corner of the rectangle.</summary>
        public int Bottom { get => bottom; set => bottom = value; }

        /// <summary>Gets or sets the x-coordinate of the upper-left corner of this <see cref="RECT"/> structure.</summary>
        /// <value>The x-coordinate of the upper-left corner of this <see cref="RECT"/> structure. The default is 0.</value>
        public int X
        {
            get => left;
            set
            {
                right -= left - value;
                left = value;
            }
        }

        /// <summary>Gets or sets the y-coordinate of the upper-left corner of this <see cref="RECT"/> structure.</summary>
        /// <value>The y-coordinate of the upper-left corner of this <see cref="RECT"/> structure. The default is 0.</value>
        public int Y
        {
            get => top;
            set
            {
                bottom -= top - value;
                top = value;
            }
        }

        /// <summary>Gets or sets the height of this <see cref="RECT"/> structure.</summary>
        /// <value>The height of this <see cref="RECT"/> structure. The default is 0.</value>
        public int Height
        {
            get => bottom - top;
            set => bottom = value + top;
        }

        /// <summary>Gets or sets the width of this <see cref="RECT"/> structure.</summary>
        /// <value>The width of this <see cref="RECT"/> structure. The default is 0.</value>
        public int Width
        {
            get => right - left;
            set => right = value + left;
        }

        /// <summary>Gets or sets the coordinates of the upper-left corner of this <see cref="RECT"/> structure.</summary>
        /// <value>A Point that represents the upper-left corner of this <see cref="RECT"/> structure.</value>
        public POINT Location
        {
            get => new(left, top);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>Gets or sets the size of this <see cref="RECT"/> structure.</summary>
        /// <value>A Size that represents the width and height of this <see cref="RECT"/> structure.</value>
        public Size Size
        {
            get => new(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>Tests whether all numeric properties of this <see cref="RECT"/> have values of zero.</summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty => left == 0 && top == 0 && right == 0 && bottom == 0;

        /// <summary>Performs an implicit conversion from <see cref="RECT"/> to <see cref="Rectangle"/>.</summary>
        /// <param name="r">The <see cref="RECT"/> structure.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Rectangle(RECT r) => new(r.left, r.top, r.Width, r.Height);

        /// <summary>Performs an implicit conversion from <see cref="Rectangle"/> to <see cref="RECT"/>.</summary>
        /// <param name="r">The Rectangle structure.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RECT(Rectangle r) => new(r);

        /// <summary>Tests whether two <see cref="RECT"/> structures have equal values.</summary>
        /// <param name="r1">The first <see cref="RECT"/> structure.</param>
        /// <param name="r2">The second <see cref="RECT"/> structure.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RECT r1, RECT r2) => r1.Equals(r2);

        /// <summary>Tests whether two <see cref="RECT"/> structures have different values.</summary>
        /// <param name="r1">The first <see cref="RECT"/> structure.</param>
        /// <param name="r2">The second <see cref="RECT"/> structure.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RECT r1, RECT r2) => !r1.Equals(r2);

        /// <summary>Determines whether the specified <see cref="RECT"/>, is equal to this instance.</summary>
        /// <param name="r">The <see cref="RECT"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="RECT"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(RECT r) => r.left == left && r.top == top && r.right == right && r.bottom == bottom;

        /// <summary>Determines whether the specified <see cref="Rectangle"/>, is equal to this instance.</summary>
        /// <param name="r">The <see cref="Rectangle"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="Rectangle"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(Rectangle r) => r.Left == left && r.Top == top && r.Right == right && r.Bottom == bottom;

        /// <summary>Determines whether the specified <see cref="object"/>, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj switch
        {
            null => false,
            RECT r => Equals(r),
            Rectangle r => Equals(r),
            _ => false,
        };

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => ((Rectangle)this).GetHashCode();

        /// <summary>Returns a <see cref="string"/> that represents this instance.</summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString() => $"{{left={left},top={top},right={right},bottom={bottom}}}";

        /// <summary>Represents an empty instance where all values are set to 0.</summary>
        public static readonly RECT Empty = new();
    }
}
