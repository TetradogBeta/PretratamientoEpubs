using CommonEbookPretractament;
using Gabriel.Cat.S.Extension;
using System;
using System.Collections;
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

namespace BookStandaritzedGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Version = "Book Standaritzed V1.5";
        public static SortedList<string,EbookStandaritzed> DicStandard { get; set; }
        public static GroupItem Group { get; set; }
        public static bool UnsafeMode { get; set; } = false;

        static string UnsafeString => UnsafeMode ? "Unsafe" : "";



        public  MainWindow()
        {
            Title = Version;
            DicStandard = new SortedList<string, EbookStandaritzed>();
            InitializeComponent();

            Load();
        }
        private void Load()
        {

            EbookSplited[] ebooksSpited = EbookSplited.GetEbookSpliteds();
            EbookStandaritzed[] ebooksStandaritzed = EbookStandaritzed.GetEbookStandaritzeds();
            SortedList<string, List<EbookSplited>> dic = new SortedList<string, List<EbookSplited>>();
         

            lstEbookSplited.Items.Clear();

            for (int i = 0; i < ebooksSpited.Length; i++)
            {

                if (!dic.ContainsKey(ebooksSpited[i].OriginalTitle))
                    dic.Add(ebooksSpited[i].OriginalTitle, new List<EbookSplited>());
                dic[ebooksSpited[i].OriginalTitle].Add(ebooksSpited[i]);
            }
            DicStandard.Clear();
            for (int i = 0; i < ebooksStandaritzed.Length; i++)
                if(!Equals(ebooksStandaritzed[i].Version,default) && !DicStandard.ContainsKey(ebooksStandaritzed[i].Version.SaveName))
                   DicStandard.Add(ebooksStandaritzed[i].Version.SaveName, ebooksStandaritzed[i]);

            Group = default;
            foreach (var title in dic)
            {
                Group = new GroupItem(new KeyValuePair<string, IList>(title.Key, title.Value));
                Group.Selected += (s, e) =>
                {
                    if (!Equals(capituloViewer.EbookActual.Version, e.Object))
                    {
                        if (Group != default)
                        {
                            Group.UnselectItem();
                        }
                        Group = s as GroupItem;
                        SetEbookActual(e.Object as EbookSplited);
                    }
                };
                lstEbookSplited.Items.Add(Group);
            }
            if (!Equals(Group, default))
            {
               SetEbookActual(Group.FirstOrDefault as EbookSplited);
            }
        }
        private void SetEbookActual(EbookSplited ebook)
        {
            EbookStandaritzed ebookStandaritzed = GetReference(ebook);
            if (!Equals(ebookStandaritzed, default))
            {

                capituloViewer.EbookActual = ebookStandaritzed;
                UpdateTitle();

            }
        }

        private void UpdateTitle()
        {
            string principio = $"{UnsafeString} {Version}";
            if (!capituloViewer.EbookActual.Finished())
            {
                Title = $"{principio} #Trabajando# {capituloViewer.EbookActual.Version.SaveName}";
            }
            else
            {
                if(!capituloViewer.EbookActual.IsABase)
                     Title = $"{principio} #Finiquitado# {capituloViewer.EbookActual.Version.SaveName}";
                else Title = $"{principio} #Base# {capituloViewer.EbookActual.Version.SaveName}";
            }
        }

        public static EbookStandaritzed GetReference(EbookSplited ebook)
        {
            EbookStandaritzed ebookStandaritzed = default;
            if (!Equals(ebook, default))
            {
                if (!DicStandard.ContainsKey(ebook.SaveName))
                    DicStandard.Add(ebook.SaveName, new EbookStandaritzed(ebook));
               ebookStandaritzed= DicStandard[ebook.SaveName];

            }
            return ebookStandaritzed;
        }

        private void capituloViewer_HasChanges(object sender, EventArgs e)
        {
            UpdateTitle();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.F12))
            {
                UnsafeMode = !UnsafeMode;
                UpdateTitle();
            }
        }
    }

}
