using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WarnMe_Cherry.NotifyIcon
{
    internal class NotifyIcon : INotifyIcon
    {

        public NotifyIcon()
        {

        }

        public string ToolTip
        {
            get;
            set;
        }

        public ContextMenu ContextMenu
        {
            get;
            set;
        }

        public Icon Icon
        {
            get;
            set;
        }

        public bool Visibility
        {
            get;
            set;
        }
    }
}
