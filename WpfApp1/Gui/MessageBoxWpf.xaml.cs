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

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour MessageBoxWpf.xaml
    /// </summary>
    public partial class MessageBoxWpf : Window
    {
        public MessageBoxWpf(String title, String message, MessageBoxButton messageBoxButton)
        {
            InitializeComponent();
            Title = title;
            tbMessage.Text = message;
            switch (messageBoxButton)
            {
                case MessageBoxButton.OK:
                    Button ok = new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 10, 10)
                    };
                    ok.Click += new RoutedEventHandler(this.ok);
                    gridMain.Children.Add(ok);
                    break;

                case MessageBoxButton.OKCancel:
                    break;
                case MessageBoxButton.YesNo:
                    break;
                case MessageBoxButton.YesNoCancel:
                    break;

   

            }


        }

        private void ok(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
