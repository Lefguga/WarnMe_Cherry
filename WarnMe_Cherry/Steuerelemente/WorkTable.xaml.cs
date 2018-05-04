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
using WarnMe_Cherry.ExterneKlassen;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für GridTable.xaml
    /// </summary>
    public partial class WorkTable : UserControl
    {

        public GridLength TitelHeight => Table.RowDefinitions[0].Height;
        public GridLength RowHeight { get; set; } = new GridLength(40);
        public int RowCount => Data.RowDefinitions.Count;
        public ColumnDefinitionCollection ColumnDefinition
        {
            get
            {
                return Data.ColumnDefinitions;
            }
            set
            {
                Data.ColumnDefinitions.Clear();
                foreach (var colDef in ColumnDefinition)
                {
                    Data.ColumnDefinitions.Add(colDef);
                }
            }
        }

        public SortedDictionary<DateTime, Arbeitstag> TableData = new SortedDictionary<DateTime, Arbeitstag>();

        public WorkTable()
        {
            InitializeComponent();
        }

        public void UpdateRow(DateTime dateTime, Arbeitstag arbeitstag)
        {
            if (TableData.ContainsKey(dateTime))
            {
                TableData[dateTime] = arbeitstag;
            }
            else
            {
                TableData.Add(dateTime, arbeitstag);
                //UpdateTable();
            }
            UpdateTable();
        }

        public void UpdateTable()
        {
            int i = 0;
            Data.Children.Clear();
            Data.RowDefinitions.Clear();

            foreach (var item in TableData.Reverse())
            {
                AddWorkRow(i++, item.Key, item.Value);
            }
        }

        ///// <summary>
        ///// Inserts an <see cref="Array"/> of <see cref="UIElement"/> in a new <see cref="RowDefinition"/>. Column index increases with every element in array.
        ///// </summary>
        ///// <param name="cellElements"></param>
        //public void AddRow(params UIElement[] cellElements) => AddRow(RowCount, cellElements);
        public void AddWorkRow(int rowIndex, DateTime dateTime, Arbeitstag arbeitstag)
        {
            Viewbox day = new Viewbox
            {
                Margin = new Thickness(0, 3, 0, 3),
                Child = new TextBlock() { Text = dateTime.ToShortDateString() }
            },
            start = new Viewbox
            {
                Margin = new Thickness(0, 3, 0, 3),
                Child = new Steuerelemente.DateTimePicker() { DateTime = arbeitstag.StartZeit, AllowInputs = true }
            },
            end = new Viewbox
            {
                Margin = new Thickness(0, 3, 0, 3),
                Child = new Steuerelemente.DateTimePicker() { DateTime = arbeitstag.EndZeit, AllowInputs = true }
            },
            duration = new Viewbox
            {
                Margin = new Thickness(0, 3, 0, 3),
                Child = new Steuerelemente.DateTimePicker() { DateTime = arbeitstag.Duration, AllowInputs = true }
            },
            comment = new Viewbox
            {
                Margin = new Thickness(0, 3, 0, 3),
                Child = new TextBox()
                {
                    Text = arbeitstag.Bemerkung,
                    BorderBrush = null,
                    Background = null,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    //Foreground = MainWindow.Foreground,
                    MinWidth = 150.0d
                }
            };

            Data.RowDefinitions.Add(new RowDefinition() { Height = RowHeight });

            AddElementToCell(day, rowIndex, 0);
            AddElementToCell(start, rowIndex, 1);
            AddElementToCell(end, rowIndex, 2);
            AddElementToCell(duration, rowIndex, 3);
            AddElementToCell(comment, rowIndex, 4);
        }

        /* ClearRow()
        public void ClearRow(int rowIndex)
        {
            int i = 0;
            foreach (var item in TableData)
            {
                if (i == rowIndex)
                {
                    ClearRow(item.Key);
                }
                i++;
            }
        }

        public void ClearRow(DateTime dateTime)
        {
            int index = TableData.IndexOf(dateTime);
            if (index > 0)
            {
                TableData.Remove(dateTime);
                UIElementCollection children = Data.Children;
                foreach (UIElement child in children)
                {
                    if (Grid.GetRow(child) == index)
                        Data.Children.Remove(child);
                }
            }
        }
        */

        private void AddElementToCell(UIElement uIElement, int tableRow, int tableColumn)
        {
            Data.Children.Add(uIElement);
            Grid.SetColumn(uIElement, tableColumn);
            Grid.SetRow(uIElement, tableRow);
        }
    }
}
