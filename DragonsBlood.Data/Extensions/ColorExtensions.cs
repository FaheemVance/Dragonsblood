using System;
using System.Drawing;
using System.Globalization;

namespace DragonsBlood.Data.Extensions
{
    public static class ColorExtensions
    {
        public static Color GetSystemDrawingColorFromHexString(this Color color, string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return Color.Yellow;

            KnownColor kc;

            var success = Enum.TryParse(hexString, out kc);

            if (success)
                return Color.FromKnownColor(kc);

            hexString = hexString.Remove(0, 2);

            if (!hexString.Contains("#"))
                hexString = "#" + hexString;

            if (!System.Text.RegularExpressions.Regex.IsMatch(hexString, @"[#]([0-9]|[a-f]|[A-F]){6}\b"))
                throw new ArgumentException();
            int red = int.Parse(hexString.Substring(1, 2), NumberStyles.HexNumber);
            int green = int.Parse(hexString.Substring(3, 2), NumberStyles.HexNumber);
            int blue = int.Parse(hexString.Substring(5, 2), NumberStyles.HexNumber);

            return Color.FromArgb(red, green, blue);
        }

    }
}