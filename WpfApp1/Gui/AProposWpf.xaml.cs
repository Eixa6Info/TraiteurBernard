using System.Windows;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour AProposWpf.xaml
    /// </summary>
    public partial class AProposWpf : Window
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public AProposWpf()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fermer la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
