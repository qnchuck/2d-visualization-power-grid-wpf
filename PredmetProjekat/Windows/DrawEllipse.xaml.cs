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
using System.Text.RegularExpressions;
using PredmetProjekat;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for DrawEllipse.xaml
    /// </summary>
    public partial class DrawEllipse : Window
    {
        Ellipse elipsa = new Ellipse();
        double xx;
        double yy;
        int num;
        public DrawEllipse(double x, double y, int id)
        {
            xx = x;
            yy = y;
            num = id;
            InitializeComponent();
        }
       
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cp_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            elipsa.Width = Int32.Parse(textBox5.Text);
            elipsa.Height = Int32.Parse(textBox.Text);
            elipsa.StrokeThickness = Int32.Parse(textBox1.Text);

            Canvas.SetLeft(elipsa, xx );
            Canvas.SetTop(elipsa, yy );
            if (cp.SelectedColor.HasValue)
            {
                Color C = cp.SelectedColor.Value;
                if(num != -1)
                {
                    ((MainWindow)Application.Current.MainWindow).elementi.Children.RemoveAt(num);
                    elipsa.Name = "objekat" + num.ToString(); 
                }
                else
                {
                    elipsa.Name = "objekat" + ((MainWindow)Application.Current.MainWindow).elementi.Children.Count.ToString();
                }

                if (checkBoxTransparent.IsChecked == true)
                {
                    byte alpha = 150;
                    SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(alpha, C.R, C.G, C.B));

                    elipsa.Stroke = brush;
                }
                elipsa.Stroke= new SolidColorBrush(C);  //(SolidColorBrush)new BrushConverter().ConvertFrom(colorVal); 
                TextBlock textBlock = new TextBlock();
                textBlock.Text = textBox2.Text;
               
                elipsa.MouseLeftButtonDown += new MouseButtonEventHandler(Ellipse_MouseDown);
                
                ((MainWindow)Application.Current.MainWindow).elementi.Children.Add(elipsa);
                if (checkBoxText.IsChecked == true)
                {
                    if (cp2.SelectedColor.HasValue)
                    {
                        textBlock.Foreground = new SolidColorBrush(cp2.SelectedColor.Value);
                    }
                    else
                    {
                        textBlock.Foreground = System.Windows.Media.Brushes.Black;
                    }

                    Canvas.SetLeft(textBlock, xx );
                    Canvas.SetTop(textBlock, yy + elipsa.Height / 2 );

                    textBlock.Name = "text" + (((MainWindow)Application.Current.MainWindow).elementi.Children.Count - 1).ToString();
                    ((MainWindow)Application.Current.MainWindow).elementi.Children.Add(textBlock);
                }
            }
            this.Close();

        }
        private void Ellipse_MouseDown(object sender, EventArgs e)
        {
            Ellipse p = sender as Ellipse;
            DrawEllipse dp = new DrawEllipse(xx,yy, int.Parse(p.Name.Split('t')[1]));
            dp.textBox5.Text = p.Width.ToString();
            dp.textBox.Text = p.Height.ToString();
            dp.textBox1.Text = p.StrokeThickness.ToString();
            dp.Show();
        }
    }
}
