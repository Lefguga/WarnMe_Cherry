using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für Timeline.xaml
    /// </summary>
    public partial class Timeline : UserControl, IUpdateable
    {

        private double maxValue = 100;
        public double MaxValue { get => maxValue; set => maxValue = value; }

        private double value = 0;
        public double Value
        {
            get => value;
            set
            {
                this.value = value;
                double progress = Progress;
                if (progress >= 0.05)
                {
                    ProgressColor1.Offset = progress;
                    ProgressColor2.Offset = progress - 0.05;
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
