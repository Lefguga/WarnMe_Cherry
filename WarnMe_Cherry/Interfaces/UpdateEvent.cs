using System.Windows.Controls;

namespace WarnMe_Cherry.Interfaces
{
    public class UpdateEvent
    {
        public delegate void ValueChange();
        public event ValueChange ValueUpdated;
    }
    public class UC_UpdateEvent: UserControl
    {
        public delegate void ValueChange();
        public event ValueChange ValueUpdated;
    }
}
