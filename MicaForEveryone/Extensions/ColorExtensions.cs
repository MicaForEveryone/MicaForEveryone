using Windows.UI;

namespace MicaForEveryone.Extensions
{
    public static class ColorExtensions
    {
        public static void Blend(this ref Color a, Color b)
        {
            a.A = (byte)(255 - ((255 - a.A) * (255 - b.A) / 255));
            a.R = (byte)((a.R * (255 - b.A) + b.R * b.A) / 255);
            a.G = (byte)((a.G * (255 - b.A) + b.G * b.A) / 255);
            a.B = (byte)((a.B * (255 - b.A) + b.B * b.A) / 255);
        }
    }
}
