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
        static readonly FontFamily FontFamilyDefault = new TextBlock().FontFamily;
        static readonly FontFamily FontFamilySelected = new FontFamily("Constantia");

        const string SEPARACION = " ~ ";
        int pos;
        List<TextBlock> tbSelecteds;

        public ProgressViewer()
        {
            InitializeComponent();
            tbSelecteds = new List<TextBlock>();
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
            Run line;
            string strTb;
            string text;

            Title = $"Resultado       {ebook.Version.SaveName}      ~       {ebook.Reference.Version.SaveName}";
            pos = 0;
            parrafos = ebook.Version.GetContentElementsArray(chapter);
            foreach (UIElement element in stkVersion.Children)
            {
                tb = element as TextBlock;
                strTb = tb.Tag.ToString();
                text = strTb.Split(SEPARACION)[1];
                while (!parrafos[pos].Contains(text) && !text.Contains(parrafos[pos])) pos++;
                line = new Run((pos + 1).ToString().PadLeft(3, '0'));
                line.Foreground = Brushes.Salmon;
                tb.Inlines.InsertBefore(tb.Inlines.FirstInline, line);
                tb.Inlines.InsertAfter(line, new Run(":") { Foreground = Brushes.Gray });
                tb.Tag = line.Text + ":" + strTb;
            }
            stkVersion.Tag = stkVersion.Children.ToArray();
            stkReference.Tag = stkReference.Children.ToArray();

        }

        UIElement StringToView(string str)
        {

            TextBlock tb;
            Run line;
            pos++;
            tb = new TextBlock() { Background = pos % 2 == 0 ? Brushes.LightGreen : Brushes.Transparent };
            line = new Run((pos).ToString().PadLeft(3, '0'));
            line.Foreground = Brushes.DarkBlue;
            tb.Inlines.Add(line);
            tb.Inlines.Add(new Run(SEPARACION) { Foreground = Brushes.Gray });
            tb.Inlines.Add(new Run(str));
            tb.Tag = line.Text + SEPARACION + str;
            tb.MouseRightButtonDown += (s, e) =>
            {
                double offset;
                TextBlock tbFound = default;
                StackPanel stkAMover, stkElementOrigen;
                ScrollViewer scAMover;
                TextBlock tbClicked = s as TextBlock;
                int itemPos;
                object[] items;

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
                itemPos = stkElementOrigen.Children.IndexOf(tbClicked);


                items = (object[])stkAMover.Tag;
                if (itemPos < items.Length)
                    tbFound = (TextBlock)items[itemPos];

                scAMover = stkAMover.Parent as ScrollViewer;

                if (!Equals(tbFound, default))
                {
                    offset = ((stkElementOrigen.Parent as ScrollViewer).VerticalOffset - (tbClicked.ActualHeight * itemPos)) + tbFound.ActualHeight * (stkAMover.Children.IndexOf(tbFound));

                    scAMover.ScrollToVerticalOffset(offset);
                }
                else
                    scAMover.ScrollToVerticalOffset(stkAMover.ActualHeight);
                scAMover.UpdateLayout();
            };
            tb.MouseLeftButtonDown += (s, e) =>
            {
                TextBlock tbClicked = (TextBlock)s;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (ReferenceEquals(tbClicked.Parent, stkVersion))
                    {
                        if (tbSelecteds.Contains(tbClicked))
                        {
                            tbSelecteds.Remove(tbClicked);
                            tbClicked.FontFamily = FontFamilyDefault;
                        }
                        else
                        {
                            tbSelecteds.Add(tbClicked);
                            tbClicked.FontFamily = FontFamilySelected;
                        }
                    }
                }

            };
            return tb;
        }
    }
}
