using CommonEbookPretractament;
using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Lógica de interacción para VisorSpliter.xaml
    /// </summary>
    public partial class VisorSpliter : UserControl
    {

        public event EventHandler<TextIndexEventArgs> IndexClick;
        public VisorSpliter()
        {
            InitializeComponent();
        }
        public Spliter Spliter { get; set; }

        public EbookStandaritzed Ebook { get; set; }
        public int Chapter { get; set; }

        public void Refresh()
        {
            const string JOIN = "<join párrafo>";

            int pos;
            Capitulo capitulo;
            List<string> parrafos;
            List<string> parrafosAux;
            List<Spliter> splitersUsados;
            string[] partesParrafo;
            int indexParrafo;
            int indexParrafoOri;
            Run parrafo;
            int indexParrafoVer=0;

            tbSpliterTextVer.Inlines.Clear();
            tbSpliterTextVer.Text = "";
            tbSpliterTextOri.Text = "";
            tbSpliterTextOri.Tag = default;
            tbSpliterTextVer.Tag = default;
            if (Spliter.IsValid)
            {
                if (Spliter.IsRelevant)
                {
                    if (Spliter.Saltar)
                    {
                        tbSpliterTextVer.Inlines.Add(new Run() { Text = $"Saltado párrafo {Spliter.EditIndexInicio}", Foreground = Brushes.Gold });
                    }
                    else
                    {
                        pos = 1;
                        capitulo = Ebook.GetCapitulo(Chapter);
                        parrafos = Spliter.GetParts(capitulo.ParrafosEditados, Ebook.Version.GetContentElements(Chapter), JOIN, true).ToList();
                        splitersUsados = capitulo.ParrafosEditados.Filtra((s) => s.IsRelevant && !s.Saltar);
                        indexParrafo =splitersUsados.IndexOf(Spliter);
                        parrafosAux = Spliter.GetParts(capitulo.ParrafosEditados, Ebook.Version.GetContentElements(Chapter),JOIN).ToList();
                        indexParrafoOri =parrafosAux.IndexOf(parrafos[indexParrafo]);
                        partesParrafo = parrafos[indexParrafo].Split(JOIN);
                        tbSpliterTextVer.Tag = Ebook.Version.GetContentElementsArray(Chapter).ToList().IndexOf(partesParrafo[0]);
                        foreach (string str in partesParrafo)
                        {
                            parrafo = new Run() { Text = (string.IsNullOrEmpty(str) ? "-SIN TEXTO A MOSTRAR-" : string.IsNullOrWhiteSpace(str) ? "-ESPACIO EN BLANCO-" : str) + "\n", Foreground = (pos++) % 2 == 0 ? Brushes.Gray : Brushes.Black };
                            parrafo.PreviewMouseLeftButtonDown += Parrafo_MouseButtonDown;
                            parrafo.PreviewMouseRightButtonDown += Parrafo_MouseButtonDown;
                            parrafo.Tag = indexParrafoVer;
                            indexParrafoVer++;


                            tbSpliterTextVer.Inlines.Add(parrafo);
                        }
                            tbSpliterTextOri.Text = Ebook.Reference.GetContentElementsArray(Chapter)[indexParrafoOri];
                        tbSpliterTextOri.Tag = indexParrafoOri;
                    }
                }
                else
                {
                    tbSpliterTextVer.Inlines.Add(new Run() { Text = "No se usará este spliter, no es relevante", Foreground = Brushes.LightGreen });
                }
            }
            else
            {
                tbSpliterTextVer.Inlines.Add(new Run() { Text = "No se usará este spliter, no es valido", Foreground = Brushes.LightBlue });
            }
           
                
        }


        private void Parrafo_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool isOri = ReferenceEquals(sender,tbSpliterTextOri);
            TextBlock tb = isOri ? tbSpliterTextOri : tbSpliterTextVer;
            int indexParrafo = 0;

            if (!Equals(tb.Tag, default))
            {
                if (!isOri)
                  indexParrafo=(int)(sender as Run).Tag;
                if (IndexClick != null)
                    IndexClick(this, new TextIndexEventArgs((int)tb.Tag+indexParrafo, isOri));
            }
        }
    }
    public class TextIndexEventArgs : EventArgs
    {
        public int Index { get; private set; }
        public bool IsOri { get; private set; }
        public TextIndexEventArgs(int index, bool isOri) 
        { Index = index; IsOri = isOri; }
    }
}
