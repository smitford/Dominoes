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

namespace Dominoes.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Grid tile1;
        public MainWindow()
        {
            InitializeComponent();
            //tile1 = Tile1;
        }


        private void Background_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            ///Background.Children.Add(Tile1);
        }
        int i=0;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var x = Mouse.GetPosition(Application.Current.MainWindow).X;
            var y = Mouse.GetPosition(Application.Current.MainWindow).Y;

            TileModel tileModel = new TileModel();
            tileModel.Margin = new Thickness(x, y,0, 0);
            //Tile1.Margin = new Thickness( Mouse.GetPosition(Application.Current.MainWindow).Y, -Mouse.GetPosition(Application.Current.MainWindow).X, -Mouse.GetPosition(Application.Current.MainWindow).Y);
            Background.Children.Add(tileModel);
            i++;
        }
    }
}
