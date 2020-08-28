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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Timeline_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel vm;
        public bool WindowInitialized;
        //Grid grid;
        Line mainLine;

        public MainWindow()
        {
            InitializeComponent();
            vm = new ViewModel();
            WindowInitialized = false;
        }

        private void Initialize()
        {
            //Initialize ViewModel
            vm.MainLineY = 250;
            vm.MarkerHeight = 15;
            vm.MarkerOrigin = (int)this.Width / 2;
            vm.MarkerInterval = 80;

            mainLine = new Line();
            mainLine.Stroke = Brushes.Black;
            mainLine.StrokeThickness = 2;

            grid.Children.Add(mainLine);
            Grid.SetRow(mainLine, 1);

            WindowInitialized = true;
        }

        private void Update()
        {
            //Draw line
            mainLine.X1 = 0;
            mainLine.X2 = Width;
            mainLine.Y1 = vm.MainLineY;
            mainLine.Y2 = vm.MainLineY;

            //Draw Markers
            int nMarkers = 0;
            for (int i = 0; i < 5; i++)
            {
                Line marker = new Line();
                marker.X1 = vm.MarkerOrigin + vm.MarkerInterval * i;
                marker.X2 = vm.MarkerOrigin + vm.MarkerInterval * i;
                marker.Y1 = vm.MainLineY - vm.MarkerHeight / 2;
                marker.Y2 = vm.MainLineY + vm.MarkerHeight / 2;
                marker.StrokeThickness = 2;
                marker.Stroke = Brushes.Black;

                //TextBlock markerText = new TextBlock();
                //markerText.Text = i.ToString();

                //Canvas canvas = new Canvas();
                

                grid.Children.Add(marker);
                Grid.SetRow(marker, 1);

                nMarkers++;
            }

            //Draw marker count for testing
            TextBlock nMarkersTextblock = new TextBlock();
            nMarkersTextblock.Text = "Number of Markers: " + nMarkers.ToString();
            nMarkersTextblock.Padding = new Thickness(10.0);
            nMarkersTextblock.HorizontalAlignment = HorizontalAlignment.Right;
            nMarkersTextblock.VerticalAlignment = VerticalAlignment.Top;

            grid.Children.Add(nMarkersTextblock);
            Grid.SetRow(nMarkersTextblock, 1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Initialize();
            this.Update();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(WindowInitialized) this.Update();
        }
    }
}
