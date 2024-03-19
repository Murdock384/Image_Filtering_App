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
using System.Windows.Shapes;

namespace Image_Filtering
{
    /// <summary>
    /// Interaction logic for MedianCutColorPalleteSelector.xaml
    /// </summary>
    public partial class MedianCutColorPalleteSelector : Window
    {
        public int SelectedPaletteCount { get; private set; }
        public MedianCutColorPalleteSelector()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PaletteCountTextBox.Text, out int paletteCount))
            {
                SelectedPaletteCount = paletteCount;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
