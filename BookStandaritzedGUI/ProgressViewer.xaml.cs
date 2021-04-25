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
using System.Windows.Shapes;

namespace BookStandaritzedGUI
{
    /// <summary>
    /// Lógica de interacción para ProgressViewer.xaml
    /// </summary>
    public partial class ProgressViewer : Window
    {
        int pos;


        public ProgressViewer()
        {
            InitializeComponent();
        }
        public ProgressViewer(IEnumerable<string> txtVersion, IEnumerable<string> txtReference) : this()
        {
            pos = 0;
            stkVersion.Children.AddRange(txtVersion.ToArray().Convert(StringToView));
            pos = 0;
            stkReference.Children.AddRange(txtReference.ToArray().Convert(StringToView));
        }

        public ProgressViewer(EbookStandaritzed ebook, int chapter) : this(ebook.GetContentElements(chapter), ebook.Reference.GetContentElements(chapter))
        {
            string[] parrafos;
            TextBlock tb;
            int pos;

            Title = $"Resultado       {ebook.Version.SaveName}      ~       {ebook.Reference.Version.SaveName}";
            pos = 0;
            parrafos = ebook.Version.GetContentElementsArray(chapter);
            foreach (UIElement element in stkVersion.Children)
            {
                tb = element as TextBlock;
                while (!tb.Text.Contains(parrafos[pos])) pos++;
                tb.Text = $"{(pos + 1).ToString().PadLeft(3, '0')}:{tb.Text}";
            }

        }

        UIElement StringToView(string str)
        {
            pos++;
            TextBlock tb = new TextBlock() { Text = $"{(pos).ToString().PadLeft(3, '0')} ~ {str}", Background = pos % 2 == 0 ? Brushes.LightGreen : Brushes.Transparent, Tag = pos };
            tb.MouseRightButtonDown += (s, e) =>
            {
                double offset;
                TextBlock tbFound;
                StackPanel stkAMover, stkElementOrigen;

                TextBlock tbClicked = s as TextBlock;
                int itemPos = (int)tbClicked.Tag;

                if (ReferenceEquals(tbClicked.Parent, stkReference))
                {       
                    //mover scroll Version
                    stkAMover = stkVersion;
                    stkElementOrigen = stkReference;
                }
                else
                {
                    //mover scroll Referencia
                    stkAMover = stkReference;
                    stkElementOrigen = stkVersion;
                }

                if (Equals(stkAMover.Tag, default))
                    stkAMover.Tag=stkAMover.Children.ToArray();

                tbFound = ((object[])stkAMover.Tag).FirstOrDefault((item) => Equals((item as TextBlock).Tag, itemPos)) as TextBlock;
                if (!Equals(tbFound, default))
                {
                    offset = ((stkElementOrigen.Parent as ScrollViewer).VerticalOffset - (tbClicked.ActualHeight * itemPos)) + tbFound.ActualHeight * (stkAMover.Children.IndexOf(tbFound) + 1);
            
                    (stkAMover.Parent as ScrollViewer).ScrollToVerticalOffset(offset);
                }
                else
                    (stkAMover.Parent as ScrollViewer).ScrollToVerticalOffset(stkAMover.ActualHeight);
                (stkAMover.Parent as ScrollViewer).UpdateLayout();
            };
            return tb;
        }
    }
}
