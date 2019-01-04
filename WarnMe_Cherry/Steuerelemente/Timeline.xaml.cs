using System.Windows.Controls;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für Timeline.xaml
    /// </summary>
    public partial class Timeline : UserControl, Interfaces.IUpdateable
    {
        public double MaxValue { get; set; } = 100d;

        double fadeOutLength = 0.005f;

        private double value = 0;
        public double Value
        {
            get => value;
            set
            {
                this.value = value;
                ProgressColor1.Offset = Progress;
                ProgressColor2.Offset = Progress - fadeOutLength;
            }
        }

        /// <summary>
        /// Gibt den relativen Fortschritt der Timeline zurück.
        /// Der <see cref="double"/> Wert liegt zwischen 0 und 1
        /// </summary>
        public double Progress
        {
            get
            {
                if (MaxValue > 0)
                {
                    double tmp = (value) / (MaxValue);
                    if (tmp <= 1 && tmp >= 0)
                        return tmp;
                }
                return 0;
            }
        }


        public Timeline()
        {
            InitializeComponent();
        }

        public void Update()
        {
            double p = Progress;
            ToolTip = string.Format("", p);
        }
    }
}
