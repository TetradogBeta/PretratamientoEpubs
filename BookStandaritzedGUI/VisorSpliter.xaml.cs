using CommonEbookPretractament;
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

            tbSpliterText.Inlines.Clear();
            tbSpliterText.Text = "";

            if (Spliter.IsValid)
            {
                if (Spliter.IsRelevant)
                {
                    if (Spliter.Saltar)
                    {
                        tbSpliterText.Inlines.Add(new Run() { Text = $"Saltado párrafo {Spliter.EditIndexInicio}", Foreground = Brushes.Gold });
                    }
                    else
                    {
                        pos = 1;
                        foreach (string str in Spliter.GetParts(new List<Spliter>() { Spliter }, Ebook.Version.GetContentElements(Chapter), JOIN, true).ToArray()[0].Split(JOIN))
                            tbSpliterText.Inlines.Add(new Run() { Text =(string.IsNullOrEmpty( str)?"-SIN TEXTO A MOSTRAR-":string.IsNullOrWhiteSpace(str) ?"-ESPACIO EN BLANCO-":str) +"\n", Foreground = (pos++) % 2 == 0 ? Brushes.Gray : Brushes.Black });
                    }
                }
                else
                {
                    tbSpliterText.Inlines.Add(new Run() { Text = "No se usará este spliter, no es relevante", Foreground = Brushes.LightGreen });
                }
            }
            else
            {
                tbSpliterText.Inlines.Add(new Run() { Text = "No se usará este spliter, no es valido", Foreground = Brushes.LightBlue });
            }
           
                
        }
    }
}
