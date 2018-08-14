using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für GridTable.xaml
    /// </summary>
    public partial class WorkTable : UserControl
    {
        public string MonthString => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);
        public int Month { get; set; } = DateTime.Now.Month;
        public int Year { get; set; } = DateTime.Now.Year;
        public ColumnDefinitionCollection ColumnDefinition => Data.ColumnDefinitions;

        public SortedDictionary<DateTime, Arbeitstag> TableData = new SortedDictionary<DateTime, Arbeitstag>();

        /// <summary>
        /// Returns the first <see cref="DateTime.DayOfWeek"/> as <see cref="int"/> with Sunday set as 6 not 0
        /// </summary>
        int FirstDayInMonth => ((int)new DateTime(Year, Month, 1).DayOfWeek + 6) % 7;

        public WorkTable()
        {
            InitializeComponent();
            FillGridWithEmpty();
        }

        /// <summary>
        /// Fills all possible fields with placeholders of <see cref="Workday"/> with empty <see cref="Arbeitstag"/>
        /// </summary>
        private void FillGridWithEmpty()
        {
            for (int i = 0; i < 42; i++)
            {
                Workday workday = new Workday()
                {
                    Arbeitstag = Arbeitstag.Zero,
                    Opacity = 0.2d
                };
                workday.MouseDown += Focus_On_MouseDown;
                AddElementToCell(workday, i / 7, i % 7);
            }
        }

        public void UpdateTable()
        {
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
            int daysInMonth = DateTime.DaysInMonth(Year, Month);
            //hide days from prev month
            for (int i = 0; i < FirstDayInMonth; i++)
            {
                if (GetWorkday(i / 7, i % 7, out Workday workday))
                {
                    workday.Arbeitstag = Arbeitstag.Zero;
                    workday.Opacity = 0d;
                }
            }
            //update existing days
            for (int i = 0; i < daysInMonth; i++)
            {
                int day = i + FirstDayInMonth;
                if (GetWorkday(day / 7, day % 7, out Workday workday))
                {
                    workday.DateTime = new DateTime(Year, Month, i + 1);
                    workday.Arbeitstag = Arbeitstag.Zero;
                    workday.Opacity = 0.4d;
                };
                //AddElementToCell(workday, day / 7, day % 7);
            }
            //hide days from next month
            for (int i = daysInMonth + FirstDayInMonth; i < 42; i++)
            {
                if (GetWorkday(i / 7, i % 7, out Workday workday))
                {
                    workday.Arbeitstag = Arbeitstag.Zero;
                    workday.Opacity = 0d;
                }
            }
        }

        private bool GetWorkday(int row, int column, out Workday workday)
        {
            foreach (UIElement child in Data.Children)
            {
                if (Grid.GetRow(child) == row && Grid.GetColumn(child) == column)
                {
                    workday = (Workday)child;
                    return true;
                }
            }
            workday = null;
            return false;
        }
        
        private void AddElementToCell(UIElement uIElement, int tableRow, int tableColumn)
        {
            Data.Children.Add(uIElement);
            Grid.SetColumn(uIElement, tableColumn);
            Grid.SetRow(uIElement, tableRow);
        }

        private void Focus_On_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((FrameworkElement)sender).Focus();
        }
    }
}
