using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.IO;
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
using CommonEbookPretractament;
using Notifications.Wpf.Core;

namespace BooksSplitedGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static SortedList<string, EbookSplited> DicBooksSaved { get; set; }= new SortedList<string, EbookSplited>();
    public static string NameNotificationControl { get; private set; }
        EbookSplited Selected { get; set; }

        public MainWindow()
        {
         
            InitializeComponent();
            Notificaciones.NameControlNotifications=nameof(notificacionManager);

            UpdateFolders();
            
        }

        private void UpdateFolders()
        {
            UpdateDicBooks();

            lstFolders.Items.Clear();
            lstFolders.Items.AddRange(
                                        System.IO.Directory.GetDirectories(Ebook.Directory)
                                                           .Convert((d) =>
                                                                System.IO.Path.GetRelativePath(Ebook.Directory, d))
                                      );
            lstFolders.SelectedIndex = 0;
        }


        private void lstFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EbookSplited[] newsEbooks;
            Task<EbookSplited[]> task;
            if (!Equals(lstFolders.SelectedItem, default))
            {
                txtNameFile.Text = string.Empty;
          
                task = EbookSplited.GetEbookSplitedNewer((string)lstFolders.SelectedItem);
                task.ContinueWith((r) =>
                {
                    Action act = () =>
                    {
                        newsEbooks = task.Result;
                        lstBooksFolder.Items.Clear();
                        for (int i = 0; i < newsEbooks.Length; i++)
                        {
                            if (DicBooksSaved.ContainsKey(newsEbooks[i].RelativeEbookPath))
                                lstBooksFolder.Items.Add(DicBooksSaved[newsEbooks[i].RelativeEbookPath]);
                            else lstBooksFolder.Items.Add(newsEbooks[i]);
                        }
                        lstBooksFolder.SelectedIndex = 0;
                    };
                    Dispatcher.BeginInvoke(act);
                });

            }
        }
        private void lstBooksFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Selected = lstBooksFolder.SelectedItem as EbookSplited;
           

            if (!Equals(Selected, default))
            {
                if (DicBooksSaved.ContainsKey(Selected.RelativeEbookPath))
                {
                    txtNameFile.Text = Selected.OriginalTitle + ";" + Selected.Idioma;
                }
                ugChaptersBook.Children.Clear();
                ugChaptersBook.Children.AddRange(ChapterViwer.GetChapters(Selected));
            }
        }   
        private void btnLoadChapters_Click(object sender, RoutedEventArgs e)
        {
            string[] campos;
        

            if (!string.IsNullOrEmpty(txtNameFile.Text) && txtNameFile.Text.Contains(';'))
            {
                campos = txtNameFile.Text.Split(';');
                Selected.OriginalTitle = campos[0];
                Selected.Idioma = campos[1];
                lstBooksFolder.Items.Refresh();
                if (Selected.IsRelevant)
                {
                    Selected.Save();
                }
            }
            else
            {
                _ = Notificaciones.ShowMessage("Atención", "Se requiere un títuloOriginal;idioma");
            }

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.F5)
            {
                UpdateFolders();
            }
        }

        public static void UpdateDicBooks()
        {
            DicBooksSaved.Clear();
            foreach (EbookSplited ebookSplited in EbookSplited.GetEbookSpliteds())
            {
                if (!DicBooksSaved.ContainsKey(ebookSplited.RelativeEbookPath))
                    DicBooksSaved.Add(ebookSplited.RelativeEbookPath, ebookSplited);
                else
                    DicBooksSaved[ebookSplited.RelativeEbookPath] = ebookSplited;
            }
        }

    }
}
