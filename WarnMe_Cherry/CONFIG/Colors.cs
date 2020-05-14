using System;
using System.Windows.Media;

namespace WarnMe_Cherry.CONFIG
{
    [Serializable]
    internal class COLORS
    {
        private Color[] colors =
        {
            Color.FromArgb(255, 0, 0, 204),     // AccentColor
            Color.FromArgb(255, 38, 38, 38),    // MainColor
            Color.FromArgb(255, 234, 234, 234)  // FontColor
        };


        [Newtonsoft.Json.JsonProperty(PropertyName = "Accent Color")]
        public Color ACCENT_COLOR { get => colors[0]; set => colors[0] = value; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "Main Color")]
        public Color MAIN_COLOR { get => colors[1]; set => colors[1] = value; }
        internal Color MAIN_COLOR_WEAK => Color.Multiply(MAIN_COLOR, AVG_COLOR(MAIN_COLOR) < 128 ? 1.5f : 0.667f);

        [Newtonsoft.Json.JsonProperty(PropertyName = "Font Color")]
        public Color FONT_COLOR { get => colors[2]; set => colors[2] = value; }

        private static byte AVG_COLOR(Color c)
        {
            return (byte)((c.R + c.G + c.B) / 3);
        }
    }

}
