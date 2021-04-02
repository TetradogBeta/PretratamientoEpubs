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
        GroupItem Group { get; set; }
        Parrafo ParrafoActual { get; set; }
        public  MainWindow()
        {
       
            InitializeComponent();
            DicStandard = new SortedList<string, EbookStandaritzed>();
            DicSplited = new SortedList<string, EbookSplited>();
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
                    if (Group != default)
                    {
                        Group.UnselectItem();
                    }
                    Group = s as GroupItem;
                    SetEbookSplited(e.Object as EbookSplited);
                };
                lstEbookSplited.Items.Add(Group);
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
        private int? GetIfIsNumberValid(TextBox textBox)
        {
            int aux;
            int? result;
            if (int.TryParse(textBox.Text.Trim(' ','\t','\n','\r'), out aux))
            {
                result = aux;
            }
            else
            {
                result = default;
            }
            return result;
        }
        private void txtPos_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.Posicion = number.Value;
            }
        }

     

        private void txtFin_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.Fin = number.Value;
            }
        }

        private void txtInicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.Inicio = number.Value;
            }
        }

        private void txtIndexFin_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.IndexFin = number.Value;
            }
        }

        private void txtIndexInicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.IndexInicio = number.Value;
            }
        }

        private void chkbSaltarParrafo_Checked(object sender, RoutedEventArgs e)
        {
            ParrafoActual.Saltar = true;
        }

        private void chkbSaltarParrafo_Unchecked(object sender, RoutedEventArgs e)
        {
            ParrafoActual.Saltar = false;
        }
    }
}
