using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace WarnMe_Cherry.Datenbank
{
    internal class COLORS : Interfaces.IDataEntry
    {
        private Color[] colors =
        {
            Color.FromArgb(255, 0, 0, 204),     // AccentColor
            Color.FromArgb(255, 38, 38, 38),    // MainColor
            Color.FromArgb(255, 234, 234, 234)  // FontColor
        };

        const string ACCENT_COLOR_STR = "Accent Color";
        public Color ACCENT_COLOR { get => colors[0]; set => colors[0] = value; }

        const string MAIN_COLOR_STR = "Main Color";
        public Color MAIN_COLOR { get => colors[1]; set => colors[1] = value; }
        internal Color MAIN_COLOR_WEAK => Color.Multiply(MAIN_COLOR, AVG_COLOR(MAIN_COLOR) < 128 ? 1.5f : 0.667f);

        const string FONT_COLOR_STR = "Font Color";
        public Color FONT_COLOR { get => colors[2]; set => colors[2] = value; }

        public string DATA_ID => "Colors";

        private static byte AVG_COLOR(Color c)
        {
            return (byte)((c.R + c.G + c.B) / 3);
        }

        public void New()
        {
            throw new System.NotImplementedException();
        }

        public void Load(JObject data)
        {
            if (data.TryGetValue(ACCENT_COLOR_STR, out JToken color))
            {
                AccentColor = color.
            }
        }

        public JObject Save()
        {
            throw new System.NotImplementedException();
        }
    }

}
