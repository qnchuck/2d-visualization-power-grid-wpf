using PredmetProjekat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for AddText.xaml
    /// </summary>
    public partial class AddText : Window
    {
        System.Windows.Point pp;
        public AddText(System.Windows.Point p, bool edit)
        {
            pp = p;
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Label labela = new Label();
            int fss;
            bool fs = Int32.TryParse(textBox1.Text, out fss);
            if(fs == true)
            {
                labela.FontSize = fss;
            }
            else
            {
                labela.FontSize = 10;
            }
            
            labela.Content =(textBox2.Text);
            labela.Name = "txt" + (((MainWindow)Application.Current.MainWindow).elementi.Children.Count).ToString();


            Canvas.SetLeft(labela, pp.X);
            Canvas.SetTop(labela, pp.Y);
            if (cp.SelectedColor.HasValue)
            {
                Color C = cp.SelectedColor.Value;
                labela.Foreground = new SolidColorBrush(C);
               
                ((MainWindow)Application.Current.MainWindow).elementi.Children.Add(labela);

            }
            else
            {
                labela.Foreground = System.Windows.Media.Brushes.Black;

                ((MainWindow)Application.Current.MainWindow).elementi.Children.Add(labela);

            }
            this.Close();

        }


    }
}
