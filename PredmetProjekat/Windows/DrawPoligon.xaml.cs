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
    /// Interaction logic for DrawPoligon.xaml
    /// </summary>
    public partial class DrawPoligon : Window
    {
        List<System.Windows.Point> pointss = new List<System.Windows.Point>();
        int num;
        public DrawPoligon(List<System.Windows.Point> points, int id)
        {
            pointss = points;
            num = id;
            InitializeComponent();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Polygon polygon = new Polygon();
            
            if (num != -1)
            {
                ((MainWindow)Application.Current.MainWindow).elementi.Children.RemoveAt(num);
                polygon.Name = "objekat" + num.ToString();
            }
            else
            {
                polygon.Name = "objekat" + ((MainWindow)Application.Current.MainWindow).elementi.Children.Count.ToString();
            }
            double minX = pointss.Min(x => x.X);
            double minY = pointss.Min(x => x.Y);
            double maxY = pointss.Max(x => x.Y);
            for (int i = 0; i < pointss.Count(); i++)
            {
                pointss[i] = new System.Windows.Point(pointss[i].X  , pointss[i].Y );

                polygon.Points.Add(pointss[i]);
            }
            polygon.StrokeThickness = Int32.Parse(textBox1.Text);
            
            if (cp.SelectedColor.HasValue)
            {
               
                polygon.Name = "objekat" + (((MainWindow)Application.Current.MainWindow).elementi.Children.Count).ToString();
                polygon.MouseLeftButtonDown += new MouseButtonEventHandler(PolyGon_MouseDown);

                Color C = cp.SelectedColor.Value;
                polygon.Stroke = new SolidColorBrush(C);
                if (checkBoxTransparent.IsChecked == true)
                {
                    byte alpha = 150; 
                    SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(alpha, C.R, C.G, C.B));
                    polygon.Stroke = brush;
                }

                ((MainWindow)Application.Current.MainWindow).elementi.Children.Add(polygon);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = textBox2.Text;

                if (checkBox.IsChecked == true)
                {
                    if (cp2.SelectedColor.HasValue)
                    {
                        textBlock.Foreground = new SolidColorBrush(cp2.SelectedColor.Value);
                    }
                    else
                    {
                        textBlock.Foreground = System.Windows.Media.Brushes.Black;
                    }
                   
                    Canvas.SetLeft(textBlock, minX  ); 
                    Canvas.SetTop(textBlock, minY  +  (maxY- minY)/2);
                    textBlock.Name = "text" + (((MainWindow)Application.Current.MainWindow).elementi.Children.Count - 1).ToString();

                    ((MainWindow)Application.Current.MainWindow).elementi.Children.Add(textBlock);
                }
            }
             ((MainWindow)Application.Current.MainWindow).points = new List<System.Windows.Point>();
            this.Close();

        }
        private void PolyGon_MouseDown(object sender, EventArgs e)
        {
            Polygon p = sender as Polygon;
            DrawPoligon dp = new DrawPoligon(p.Points.ToList(), int.Parse(p.Name.Split('t')[1]));
            dp.textBox1.Text = p.StrokeThickness.ToString();

            dp.Show();
        }
        private void cp_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }
    }
}
