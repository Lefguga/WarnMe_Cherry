using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarnMe_Cherry.Datenbank;
using static WarnMe_Cherry.Global;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für GridTable.xaml
    /// </summary>
    public partial class WorkTable : UserControl, Interfaces.IUpdateable
    {
        public delegate void ValueUpdate(DateTime date, Arbeitstag arbeitstag);
        public event ValueUpdate ValueUpdated;

        public string MonthString => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);
        public int Month { get; set; } = DateTime.Now.Month;
        public int Year { get; set; } = DateTime.Now.Year;
        public ColumnDefinitionCollection ColumnDefinition => Data.ColumnDefinitions;

        public Dictionary<DateTime, Arbeitstag> TableData = new Dictionary<DateTime, Arbeitstag>();

        internal static WorkDayPropWindow window = new WorkDayPropWindow();

        /// <summary>
        /// Returns the first <see cref="DateTime.DayOfWeek"/> as <see cref="int"/> with Sunday set as 6 not 0
        /// </summary>
        int FirstDayInMonth => ((int)new DateTime(Year, Month, 1).DayOfWeek + 6) % 7;

        public WorkTable()
        {
#if TRACE
            INFO("WorkTable");
#endif
            InitializeComponent();
            AddHandler(MouseDownEvent, new MouseButtonEventHandler(Focus_On_MouseDown), true);

            window.WorkdayUpdated += WorkdayUpdated;

            foreach (var child in Data.Children)
            {
                if (child is Workday)
                {
                    ((Workday)child).MouseUp += Open_Workday_Promt;
                }
            }
            //FillGridWithEmpty();
        }

        public void Update()
        {
#if TRACE
            INFO("WorkTable.Update");
#endif
            // Konvertierung zur sortierten Tabelle
            TableData = InternalVariables.Datenbank.Select<Arbeitstag>(InternalVariables.WORKINGDAYS.NAME).ToDictionary(value => DateTime.Parse(value.Key), value => value.Value);
            UpdateTable();
        }

        /// <summary>
        /// Update Workday, ValueTable and Trigger updated Event...
        /// </summary>
        /// <param name="date"></param>
        /// <param name="arbeitstag"></param>
        private void WorkdayUpdated(DateTime date, Arbeitstag arbeitstag)
        {
#if TRACE
            INFO("WorkTable.WorkdayUpdated date[{date}]");
#endif
            if (TableData.ContainsKey(date))
                TableData[date] = arbeitstag;
            else
                TableData.Add(date, arbeitstag);
            UpdateWorkday(date, arbeitstag);
            // trigger event
            ValueUpdated?.Invoke(date, arbeitstag);
        }

        private void Open_Workday_Promt(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("WorkTable.Open_Workday_Promt");
#endif
            Workday w_day = (Workday)sender;
            if (w_day.Opacity > 0d)
                ShowPropWindow(w_day.PointToScreen(new Point(w_day.RenderSize.Width * -0.2d, 0d)), w_day);
            // throw new NotImplementedException();
        }

        private static void ShowPropWindow(Point position, Workday workday)
        {
#if TRACE
            INFO("WorkTable.ShowPropWindow");
#endif
            window.Top = position.Y;
            window.Left = position.X;
            window.Width = workday.ActualWidth * 1.5d;
            window.Height = workday.ActualHeight * 1.1d;

            if (!window.IsVisible)
                window.Show(workday.Arbeitstag, workday.DateTime);
        }

        /// <summary>
        /// Fills all possible fields with placeholders of <see cref="Workday"/> with empty <see cref="Arbeitstag"/>
        /// </summary>
        private void FillGridWithEmpty()
        {
#if TRACE
            INFO("WorkTable.FillGridWithEmpty");
#endif
            for (int i = 0; i < 42; i++)
            {
                Workday workday = new Workday()
                {
                    Arbeitstag = Arbeitstag.Zero,
                    Opacity = 0.2d,
                    Focusable = true
                };
                //workday.MouseDown += Focus_On_MouseDown;
                AddElementToCell(workday, i / 7, i % 7);
            }
        }

        public void UpdateTable()
        {
#if TRACE
            INFO("WorkTable.UpdateTable");
#endif
            FillGridForMonth();
            //Data.RowDefinitions.Clear();

            foreach (var item in TableData)
            {
                UpdateWorkday(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Update the <see cref="Workday"/> Values with <see cref="DateTime"/>.Value
        /// <paramref name="arbeitstag"/>
        /// <paramref name="dateTime"/>
        /// </summary>
        public void UpdateWorkday(DateTime dateTime, Arbeitstag arbeitstag)
        {
#if TRACE
            INFO("WorkTable.UpdateWorkday");
#endif
            if (dateTime.Month == Month & dateTime.Year == Year)
            {
                int day = dateTime.Day + (FirstDayInMonth) - 1;
                if (GetWorkday(day / 7, day % 7, out Workday wDay))
                {
                    wDay.Arbeitstag = arbeitstag;
                    wDay.Opacity = 1d;
                }
                //AddElementToCell(workday, day / 7, day % 7);
            }
        }

        private void FillGridForMonth()
        {
#if TRACE
            INFO("WorkTable.FillGridForMonth");
#endif
            Dictionary<Tuple<int, int>, Workday> workdays = GetWorkdays();
            int daysInMonth = DateTime.DaysInMonth(Year, Month);
            int _firstDay = FirstDayInMonth;

            //hide days from prev month
            for (int i = 0; i < _firstDay; i++)
            {
                ((Workday)Data.Children[i]).Arbeitstag = Arbeitstag.Zero;
                ((Workday)Data.Children[i]).Opacity = 0d;
            }
            //update existing days
            for (int i = _firstDay; i < daysInMonth + _firstDay; i++)
            {
                ((Workday)Data.Children[i]).DateTime = new DateTime(Year, Month, 1 + i - _firstDay);
                ((Workday)Data.Children[i]).Arbeitstag = Arbeitstag.Zero;
                ((Workday)Data.Children[i]).Opacity = 0.4d;
            }
            //hide days from next month
            for (int i = daysInMonth + _firstDay; i < 42; i++)
            {
                ((Workday)Data.Children[i]).Arbeitstag = Arbeitstag.Zero;
                ((Workday)Data.Children[i]).Opacity = 0d;
            }
        }

        private bool GetWorkday(int row, int column, out Workday workday)
        {
#if TRACE
            INFO($"WorkTable.GetWorkday from grid row:{row} col:{column}");
#endif
            foreach (UIElement child in Data.Children)
            {
                if (Grid.GetRow(child) == row && Grid.GetColumn(child) == column)
                {
                    workday = (Workday)child;
                    return true;
                }
            }
            workday = default;
            return false;
        }

        private Dictionary<Tuple<int, int>, Workday> GetWorkdays()
        {
#if TRACE
            INFO($"WorkTable.GetWorkdays from grid");
#endif
            Dictionary<Tuple<int, int>, Workday> elementArray = new Dictionary<Tuple<int, int>, Workday>();
            foreach (UIElement child in Data.Children)
            {
                if (child is Workday)
                    elementArray.Add(new Tuple<int, int>(Grid.GetRow(child), Grid.GetColumn(child)), (Workday)child);
#if DEBUG
            DEBUG($"  add from grid row:{Grid.GetRow(child)} col:{Grid.GetColumn(child)}");
#endif
            }
            return elementArray;
        }
        
        private void AddElementToCell(UIElement uIElement, int tableRow, int tableColumn)
        {
#if TRACE
            INFO("WorkTable.AddElementToCell");
#endif
            Data.Children.Add(uIElement);
            Grid.SetColumn(uIElement, tableColumn);
            Grid.SetRow(uIElement, tableRow);
        }

        private void Focus_On_MouseDown(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("WorkTable.Focus_On_MouseDown");
#endif
            ((FrameworkElement)sender).Focus();
        }
    }
}
