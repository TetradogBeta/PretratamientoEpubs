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
        SortedList<string,EbookStandaritzed> DicStandard { get; set; }
        SortedList<string, EbookSplited> DicSplited { get; set; }
        GroupItem group;
        public  MainWindow()
        {
            group = default;
            InitializeComponent();
            DicStandard = new SortedList<string, EbookStandaritzed>();
            DicSplited = new SortedList<string, EbookSplited>();
            Load();
        }
        private void Load()
        {
            GroupItem group;
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

            foreach(var title in dic)
            {
                group = new GroupItem(new KeyValuePair<string, IList<object>>(title.Key, title.Value.Convert((item) => (object)item)));
                group.Selected += (s, e) =>
                {
                    if (group != default)
                    {
                        group.UnselectItem();
                    }
                    group = s as GroupItem;
                    SetEbookSplited(e.Object as EbookSplited);
                };
                lstEbookSplited.Items.Add(group);
            }
        }

        private void SetEbookSplited(EbookSplited ebookSpited)
        {
            throw new NotImplementedException();
        }

        private void cmbParrafosOriginal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void cmbParrafosVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void cmbChapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void cmbEbookOriginal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
