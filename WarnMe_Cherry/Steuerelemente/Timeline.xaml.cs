using System.Windows.Controls;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für Timeline.xaml
    /// </summary>
    public partial class Timeline : UserControl, IUpdateable
    {

        private double maxValue = 100;
        public double MaxValue { get => maxValue; set => maxValue = value; }

        float fadeOutLength = 0.02f;

        private double value = 0;
        public double Value
        {
            get => value;
            set
            {
                this.value = value;
                double progress = Progress;
                if (progress >= fadeOutLength)
                {
                    ProgressColor1.Offset = progress;
                    ProgressColor2.Offset = progress - fadeOutLength;
                }
            }
        }

        public double Progress
        {
            get
            {
                if (MaxValue > 0)
                {
                    double tmp = (Value) / (MaxValue);
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
            ToolTip = p;
        }
    }
}
