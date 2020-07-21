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
using TraiteurBernardWPF.Data;
using TraiteurBernardWPF.Modele;
using TraiteurBernardWPF.Utils;

namespace TraiteurBernardWPF.Gui
{
    /// <summary>
    /// Logique d'interaction pour BoiteDialogueCompoWpf.xaml
    /// </summary>
    public partial class BoiteDialogueCompoWpf : Window
    {
        private SaisieData edite;
        int type = 0;
        Dictionary<int, string> ListCommentaire = new Dictionary<int, string>();
        
        public BoiteDialogueCompoWpf(int Type)
        {
            InitializeComponent();
            this.edite = new SaisieData();
            type = Type;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int jourId = short.Parse(txtNumJour.Text);

            try
            {
               
                if (txtNumJour.Text != "")
                {
                    ListCommentaire.Add(jourId, txtBoxComment.Text);
                }
                
                if (type == 1)
                {
                    PdfCreerWpf wpf = new PdfCreerWpf(1, 2, ListCommentaire);
                    WinFormWpf.CornerTopLeftToParent(wpf, this);
                    wpf.ShowDialog();
                }
                else
                {
                    PdfCreerWpf wpf = new PdfCreerWpf(1, 6, ListCommentaire);
                    WinFormWpf.CornerTopLeftToParent(wpf, this);
                    wpf.ShowDialog();
                }
                Close();         
            }
            catch (System.IO.IOException a)
            {
                LogHelper.WriteToFile(a.Message, "MainWindow.xaml.cs");
                Console.WriteLine(a.Message);
                return;
            }
        }

        private void BtnSuivant(object sender, RoutedEventArgs e)
        {
            int jourId = short.Parse(txtNumJour.Text);
            if (jourId < 7)
            {
                if (jourId != 0)
                {
                    if (txtBoxComment.Text != "")
                    {
                        ListCommentaire.Add(jourId, txtBoxComment.Text);
                    }
                    txtBoxComment.Text = "";
                    txtNumJour.Text = (jourId + 1).ToString();
                }
            }
            else
            {
                this.btnOK_Click(sender,e);
                Close();
            }         
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            txtBoxComment.Text = "";
            txtNumJour.Text = "1";                 
        }
    }
}
