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
        public ProgressViewer(IEnumerable<string> txtVersion,IEnumerable<string> txtReference):this()
        {
            pos = -1;
            stkVersion.Children.AddRange(txtVersion.ToArray().Convert(StringToView));
            pos = -1;
            stkReference.Children.AddRange(txtReference.ToArray().Convert(StringToView));
        }

        public ProgressViewer(EbookStandaritzed ebook, int chapter):this(ebook.GetContentElements(chapter), ebook.Reference.GetContentElements(chapter))
        {
    
        }

        UIElement StringToView(string str)
        {
            pos++;
            return new TextBlock() { Text =$"{pos} ~ {str}", Background=pos%2 == 0?Brushes.LightGreen:Brushes.Transparent };
        }
    }
}
