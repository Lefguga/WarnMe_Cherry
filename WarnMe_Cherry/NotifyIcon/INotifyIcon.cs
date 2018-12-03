using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WarnMe_Cherry.NotifyIcon
{
    public interface INotifyIcon
    {

        string ToolTip
        {
            get;
            set;
        }

        ContextMenu ContextMenu
        {
            get;
            set;
        }

        Icon Icon
        {
            get;
            set;
        }

        bool Visibility
        {
            get;
            set;
        }
    }
}
