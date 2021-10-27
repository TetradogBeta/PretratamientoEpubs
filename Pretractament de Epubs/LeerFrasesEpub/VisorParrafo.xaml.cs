using ImportarEPUBS;
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

namespace LeerFrasesEpub
{
    /// <summary>
    /// Lógica de interacción para VisorParrafo.xaml
    /// </summary>
    public partial class VisorParrafo : UserControl
    {
        public VisorParrafo()
        {
            InitializeComponent();
        }
        public ImportarEPUBS.Version? Version { get; set; }
        public int Capitulo { get; set; }
        public int Parrafo { get; set; }

        public IEnumerable<Frase?> FrasesOk => lstFrases.Items.Cast<CheckBox>().Where(f => f.IsChecked.GetValueOrDefault()).Select(f => f.Tag as Frase);

        public void Refresh()
        {
            Parrafo? parrafo = Version?.Capitulos?[Capitulo]?.Parrafos?[Parrafo];
            lstFrases.Items.Clear();
            if (!ReferenceEquals(parrafo, default))
            {
                parrafo.Load();
                foreach (Frase frase in parrafo.Frases)
                {
                    lstFrases.Items.Add(new CheckBox() { Content = frase.Texto, Tag = frase, IsChecked = true });
                }
            }
        }

    }
}
