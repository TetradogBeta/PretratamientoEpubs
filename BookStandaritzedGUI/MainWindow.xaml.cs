using CommonEbookPretractament;
using Gabriel.Cat.S.Extension;
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

namespace BookStandaritzedGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SortedList<string,EbookStandaritzed> DicStandard { get; set; }
        public static SortedList<string, EbookSplited> DicSplited { get; set; }
        public static GroupItem Group { get; set; }




        public  MainWindow()
        {
            DicStandard = new SortedList<string, EbookStandaritzed>();
            DicSplited = new SortedList<string, EbookSplited>();

            InitializeComponent();

            Load();
        }
        private void Load()
        {

            EbookSplited[] ebooksSpited = EbookSplited.GetEbookSpliteds();
            EbookStandaritzed[] ebooksStandaritzed = EbookStandaritzed.GetEbookStandaritzeds();
            SortedList<string, List<EbookSplited>> dic = new SortedList<string, List<EbookSplited>>();
         

            lstEbookSplited.Items.Clear();
            DicSplited.Clear();
            for (int i = 0; i < ebooksSpited.Length; i++)
            {
                DicSplited.Add(ebooksSpited[i].EbookPath, ebooksSpited[i]);
                if (!dic.ContainsKey(ebooksSpited[i].OriginalTitle))
                    dic.Add(ebooksSpited[i].OriginalTitle, new List<EbookSplited>());
                dic[ebooksSpited[i].OriginalTitle].Add(ebooksSpited[i]);
            }
            DicStandard.Clear();
            for (int i = 0; i < ebooksStandaritzed.Length; i++)
                DicStandard.Add(ebooksStandaritzed[i].VersionPath, ebooksStandaritzed[i]);

            Group = default;
            foreach (var title in dic)
            {
                Group = new GroupItem(new KeyValuePair<string, IList<object>>(title.Key, title.Value.Convert((item) => (object)item)));
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
            if (!Equals(ebook, default))
            {
                if (!DicStandard.ContainsKey(ebook.RelativeEbookPath))
                    DicStandard.Add(ebook.RelativeEbookPath, new EbookStandaritzed(ebook));
                capituloViewer.EbookActual = DicStandard[ebook.RelativeEbookPath];

            }
        }

        public static EbookStandaritzed GetReference(EbookSplited ebook)
        {
            EbookStandaritzed ebookStandaritzed = default;
            if (!Equals(ebook, default))
            {
                if (!DicStandard.ContainsKey(ebook.RelativeEbookPath))
                    DicStandard.Add(ebook.RelativeEbookPath, new EbookStandaritzed(ebook));
               ebookStandaritzed= DicStandard[ebook.RelativeEbookPath];

            }
            return ebookStandaritzed;
        }
    }
}
