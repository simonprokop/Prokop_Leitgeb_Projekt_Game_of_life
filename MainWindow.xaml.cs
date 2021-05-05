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

namespace CGOF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game = new Game();
        public Random rand = new Random();
        public static MainWindow Window;

        public MainWindow()
        {
            InitializeComponent();
            Window = this;
            game.CreateBoard();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string buttonContent = (sender as Button).Content.ToString();

            switch (buttonContent)
            {
                case "Play":
                    game.Start();
                    break;

                case "Stop":
                    game.Stop();
                    break;

                case "Reset":
                    game.Reset();
                    break;
                case "Blinker":
                    OSC1.IsEnabled = false;
                    break;

                case "Toad":
                    OSC2.IsEnabled = false;
                    break;

                case "Block":
                    STA1.IsEnabled = false;
                    break;

                case "Tub":
                    STA2.IsEnabled = false;
                    break;
              /*  case "Glider":
                    Space1.IsEnable = false;
                    break;*/
                case "Randomize":
                    game.Randomize();
                    break;
            }
        }
        public void RightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!game.GameStarted)
            {
                var cell = (Game.Cell)(sender as Rectangle).Tag;
                if (OSC1.IsEnabled == false)
                {
                    game.Osc1(cell);
                    OSC1.IsEnabled = true;
                }
                if (OSC2.IsEnabled == false)
                {
                    game.Osc2(cell);
                    OSC2.IsEnabled = true;
                }
                if (STA1.IsEnabled == false)
                {
                    game.Sta1(cell);
                    STA1.IsEnabled = true;
                }
                if (STA2.IsEnabled == false)
                {
                    game.Sta2(cell);
                    STA2.IsEnabled = true;
                }
                else
                {
                    game.ChangeCellStatus(cell);
                }
            }
        }
    }
}