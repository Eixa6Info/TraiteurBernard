using System;
using System.Windows;
using System.Windows.Controls;


namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour MessageBoxWpf.xaml
    /// </summary>
    public partial class MessageBoxWpf : Window
    {

        public bool YesOrNo { get; set; }

        public MessageBoxWpf(String title, String message, MessageBoxButton messageBoxButton)
        {
            InitializeComponent();
            Title = title;
            tbMessage.Text = message;
            switch (messageBoxButton)
            {
                case MessageBoxButton.OK:

                    // button "ok"
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

                    // boutton "oui"
                    Button yes = new Button
                    {
                        Content = "Oui",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 10, 10)
                    };
                    yes.Click += new RoutedEventHandler(this.yes);
                    gridMain.Children.Add(yes);
                    
                    // boutton "non"
                    Button no = new Button
                    {
                        Content = "Non",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(10, 0, 75, 10)
                    };
                    no.Click += new RoutedEventHandler(this.no);
                    gridMain.Children.Add(no);
                    break;
                case MessageBoxButton.YesNoCancel:
                    break;

   

            }


        }

        private void ok(object sender, RoutedEventArgs e)
        {
            Close();
        } 
        
        private void yes(object sender, RoutedEventArgs e)
        {
            this.YesOrNo = true;
            Close();
        } 
        
        private void no(object sender, RoutedEventArgs e)
        {
            this.YesOrNo = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
