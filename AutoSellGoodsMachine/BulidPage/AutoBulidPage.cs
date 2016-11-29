using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AutoSellGoodsMachine.BulidPage
{
    public class AutoBulidPage
    {
        public static void CreateRow(Grid grid, int rowNumber)
        {
            for (int i = 0; i <= rowNumber; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star)
                });
            }
        }

        public static void CreateColumn(Grid grid, int columnNumber)
        {
            for (int i = 0; i <= columnNumber; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star)
                });
            }
        }
    }
}
