using System;
using System.Collections.Generic;
using System.IO;
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
using TestReadEpub;
using Gabriel.Cat.S.Extension;

namespace BooksSplitedGUI
{
    /// <summary>
    /// Lógica de interacción para ChapterViwer.xaml
    /// </summary>
    public partial class ChapterViwer : UserControl
    {

        public ChapterViwer()
        {
            InitializeComponent();
            txtContent.PreviewMouseLeftButtonDown += txtContent_MouseLeftButtonDown;

        }
        public ChapterViwer(EbookSplited ebookSplited,int chapter):this()
        {

            EbookSplited = ebookSplited;
            Chapter = chapter;

            txtContent.Text =string.Join('\n', EbookSplited.Ebook.GetContentElementsArray(Chapter));
            Update();
        }
        public int Chapter { get; set; }
        public EbookSplited EbookSplited { get; set; }


        private void Update()
        {
            if (!EbookSplited.CapitulosAOmitir[Chapter])
            {
                txtContent.Background = Brushes.LightGreen;
            }
            else
            {
                txtContent.Background = Brushes.LightSalmon;
            }
        }
        private void txtContent_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EbookSplited.CapitulosAOmitir[Chapter] = !EbookSplited.CapitulosAOmitir[Chapter];
            Update();
            EbookSplited.Save();
            if (!MainWindow.DicBooksSaved.ContainsKey(EbookSplited.RelativePath))
            {
                MainWindow.DicBooksSaved.Add(EbookSplited.RelativePath, EbookSplited);
            }
        }
        public static ChapterViwer[] GetChapters(EbookSplited ebookSplited)
        {
            ChapterViwer[] chapters = new ChapterViwer[ebookSplited.Ebook.TotalChapters];
            for(int i=0;i<chapters.Length;i++)
            {
                chapters[i] = new ChapterViwer(ebookSplited, i);
            }
            return chapters;
        }

      
    }
}
