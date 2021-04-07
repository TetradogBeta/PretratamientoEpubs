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

        EbookStandaritzed EbookActual { get; set; }
        string[] ParrafosCapitulosReference { get;  set; }
        string[] ParrafosCapitulosVersion { get; set; }

        public  MainWindow()
        {
            DicStandard = new SortedList<string, EbookStandaritzed>();
            DicSplited = new SortedList<string, EbookSplited>();
            ParrafoActual = new Parrafo();
            EbookActual = new EbookStandaritzed();
            ParrafosCapitulosReference = new string[]{ string.Empty };
            ParrafosCapitulosVersion  = new string[]{ string.Empty };
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
                    if (!Equals(EbookActual.Version, e.Object))
                    {
                        if (Group != default)
                        {
                            Group.UnselectItem();
                        }
                        Group = s as GroupItem;
                        SetEbookSplited(e.Object as EbookSplited);
                    }
                };
                lstEbookSplited.Items.Add(Group);
            }
            if (!Equals(Group, default))
            {
                SetEbookSplited(Group.FirstOrDefault as EbookSplited);
            }
        }

        private void SetEbookSplited(EbookSplited ebookSpited)
        {
            if (!Equals(ebookSpited, default) && !Equals(EbookActual.Version,ebookSpited))
            {
                if (!DicStandard.ContainsKey(ebookSpited.ToString()))
                {
                    EbookActual = new EbookStandaritzed(ebookSpited);
                    DicStandard.Add(ebookSpited.ToString(), EbookActual);
                }
                else
                {
                    EbookActual = DicStandard[ebookSpited.ToString()];
                }

                cmbEbookOriginal.SelectedIndex = cmbEbookOriginal.Items.IndexOf(ebookSpited);
                cmbChapters.Items.Clear();
                cmbChapters.Items.AddRange(Enumerable.Range(0, ebookSpited.TotalChapters).ToArray().Convert((c)=>$"capitulo {c}"));
                cmbChapters.SelectedIndex = 0;
                cmbEbookOriginal.Items.Clear();
                cmbEbookOriginal.Items.AddRange(Group.Items);
                cmbEbookOriginal.SelectedItem = ebookSpited;
                Group.SelectedItem = ebookSpited;
            }
          
        }

        private void cmbParrafosReference_SelectionChanged(object sender=null, SelectionChangedEventArgs e=null)
        {
            if(cmbParrafosReference.SelectedIndex>=0)
            rtbOriginal.SetText(ParrafosCapitulosReference[cmbParrafosReference.SelectedIndex]);
        }

        private void cmbParrafosVersion_SelectionChanged(object sender = null, SelectionChangedEventArgs e=null)
        {
            if (cmbParrafosVersion.SelectedIndex >= 0)
                rtbVersion.SetText(ParrafosCapitulosVersion[cmbParrafosVersion.SelectedIndex]);
        }

        private void cmbChapters_SelectionChanged(object sender = null, SelectionChangedEventArgs e = null)
        {

            if (cmbChapters.SelectedIndex >= 0)
            {

                ParrafosCapitulosVersion = EbookActual.Version.GetContentElementsArray(cmbChapters.SelectedIndex);


                cmbParrafosVersion.Items.Clear();

                cmbParrafosVersion.Items.AddRange(Enumerable.Range(0, ParrafosCapitulosVersion.Length).ToArray().Convert((p) => $"párrafo a mirar {p}"));

                cmbParrafosVersion.SelectedIndex = 0;

                cmbParrafosVersion_SelectionChanged();
                SetReference(EbookActual.Reference);
            }
        }

        private void cmbEbookOriginal_SelectionChanged(object sender = null, SelectionChangedEventArgs e = null)
        {

            if (cmbEbookOriginal.Items.Count > 0)
            {
                SetReference(cmbEbookOriginal.SelectedItem as EbookStandaritzed);


                if (!Equals(EbookActual.CapitulosEditados, default) && EbookActual.CapitulosEditados.Any((c) => !Equals(c, default)))
                {
                    //a ver si quiere borrar los capitulos editados
                    if (MessageBox.Show("Desea eliminar los capitulos editados?podria ser que no coincidieran con el anterior...", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        EbookActual.CapitulosEditados = new Capitulo[EbookActual.CapitulosEditados.Length];
                    }
                }
                EbookActual.Save();
            }
        }

        private void SetReference(EbookStandaritzed ebookReference)
        {
            if (!Equals(ebookReference, default))
            {
                EbookActual.Reference = ebookReference;
                if (cmbChapters.SelectedIndex >= EbookActual.Reference.TotalChapters)
                {
                    cmbChapters.SelectionChanged -= cmbChapters_SelectionChanged;
                    cmbChapters.SelectedIndex = 0;
                    cmbChapters.SelectionChanged += cmbChapters_SelectionChanged;
                }
                ParrafosCapitulosReference = EbookActual.Reference.Version.GetContentElementsArray(cmbChapters.SelectedIndex);
                cmbParrafosReference.Items.Clear();
                cmbParrafosReference.Items.AddRange(Enumerable.Range(0, ParrafosCapitulosReference.Length).ToArray().Convert((p) => $"párrafo referencia {p}"));
                cmbParrafosReference.SelectedIndex = 0;
                cmbParrafosReference_SelectionChanged();

                cmbParrafosVersion.SelectedIndex = 0;

                cmbParrafosVersion_SelectionChanged();
                Title = $"Editando {EbookActual}";

            }
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
                EbookActual.Save();
               
            }
        }

     

        private void txtFin_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.Fin = number.Value;
                EbookActual.Save();
            }
        }

        private void txtInicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.Inicio = number.Value;
                EbookActual.Save();
            }
        }

        private void txtIndexFin_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.IndexFin = number.Value;
                EbookActual.Save();
            }
        }

        private void txtIndexInicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? number = GetIfIsNumberValid(sender as TextBox);
            if (number.HasValue)
            {
                ParrafoActual.IndexInicio = number.Value;
                EbookActual.Save();
            }
        }

        private void chkbSaltarParrafo_Checked(object sender, RoutedEventArgs e)
        {
            ParrafoActual.Saltar = true;
            EbookActual.Save();
        }

        private void chkbSaltarParrafo_Unchecked(object sender, RoutedEventArgs e)
        {
            ParrafoActual.Saltar = false;
            EbookActual.Save();
        }
    }
}
