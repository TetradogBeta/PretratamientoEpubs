using CommonEbookPretractament;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookStandaritzedGUI
{
    /// <summary>
    /// Lógica de interacción para VisorCapitiloSpliter.xaml
    /// </summary>
    public partial class VisorCapitiloSpliter : UserControl
    {
        private List<Spliter> capitulo;

        public event EventHandler<SpliterEventArgs> SpliterSelected;

        public VisorCapitiloSpliter()
        {
            InitializeComponent();
        }
        public List<Spliter> Capitulo { 
            get => capitulo;
            set {

                capitulo = value;
                Refresh();
            }
        }

        public void Refresh()
        {
            parrafosCapitulo.Children.Clear();
            //añado los capitulos
            //onClick SpliterSelected
        }
    }

    public class SpliterEventArgs:EventArgs
    {
        public SpliterEventArgs(Spliter spliter) => Spliter = spliter;
        public Spliter Spliter { get; private set; }
    }
}
