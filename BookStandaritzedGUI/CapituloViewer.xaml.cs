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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookStandaritzedGUI
{
    /// <summary>
    /// Lógica de interacción para CapituloViewer.xaml
    /// </summary>
    public partial class CapituloViewer : UserControl
    {
        private EbookStandaritzed ebookActual;
        private Spliter parrafoActual;

        public CapituloViewer()
        {
            InitializeComponent();
            ParrafoActual = new Spliter();
            EbookActual = new EbookStandaritzed();
            ParrafosCapitulosReference = new string[] { string.Empty };
            ParrafosCapitulosVersion = new string[] { string.Empty };



        }
        string[] ParrafosCapitulosReference { get; set; }
        string[] ParrafosCapitulosVersion { get; set; }
        Spliter ParrafoActual
        {
            get => parrafoActual;
            set
            {
                parrafoActual = value;

                txtFin.Text = parrafoActual.CharFin + "";
                txtInicio.Text = parrafoActual.CharInicio + "";

                txtIndexFin.Text = parrafoActual.IndexFin + "";
                txtIndexInicio.Text = parrafoActual.IndexInicio + "";

                chkbSaltarParrafo.IsChecked = parrafoActual.Saltar;

            }
        }
        public EbookStandaritzed EbookActual
        {
            get => ebookActual;
            set
            {
                ebookActual = value;
                if (!Equals(MainWindow.Group, default))
                {
                    //cargo el libro actual
                    cmbEbookOriginal.SelectionChanged -= cmbEbookOriginal_SelectionChanged;
                    cmbEbookOriginal.ItemsSource = MainWindow.Group.Items;
                    cmbEbookOriginal.SelectedIndex = cmbEbookOriginal.Items.IndexOf(ebookActual.Reference.Version);
                    cmbEbookOriginal.SelectionChanged += cmbEbookOriginal_SelectionChanged;
                    cmbChapters.Items.Clear();
                    cmbChapters.Items.AddRange(Enumerable.Range(0, ebookActual.TotalChapters).ToArray().Convert((p) => $"Capitulo {p}"));
                    cmbEbookOriginal_SelectionChanged();
                }

            }
        }
        private void CheckAndSave()
        {

            EbookActual.Save();

            CheckChapterFinished();

        }

        private void CheckChapterFinished()
        {
            if (cmbChapters.SelectedIndex >= 0 && cmbChapters.SelectedIndex < EbookActual.TotalChapters)
            {
                if (EbookActual.GetCapitulo(cmbChapters.SelectedIndex).Finished(EbookActual.Reference, EbookActual.Version, cmbChapters.SelectedIndex))
                {
                    rtbVersion.Background = Brushes.LightGreen;
                }
                else
                {
                    rtbVersion.Background = Brushes.AntiqueWhite;
                }
            }
        }

        private int? GetIfIsNumberValid(TextBox textBox)
        {
            int aux;
            int? result;
            if (int.TryParse(textBox.Text.Trim(' ', '\t', '\n', '\r'), out aux))
            {
                result = aux;
            }
            else
            {
                result = default;
            }
            return result;
        }

        private void cmbParrafosVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbParrafosVersion.SelectedIndex != -1 && cmbParrafosVersion.SelectedIndex < ParrafosCapitulosVersion.Length)
                rtbVersion.SetText(ParrafosCapitulosVersion[cmbParrafosVersion.SelectedIndex]);
        }

        private void cmbParrafosReference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbParrafosReference.SelectedIndex != -1 && cmbParrafosReference.SelectedIndex < ParrafosCapitulosReference.Length)
                rtbOriginal.SetText(ParrafosCapitulosReference[cmbParrafosReference.SelectedIndex]);
        }

        private void chkbSaltarParrafo_Checked(object sender, RoutedEventArgs e)
        {
            if (!ParrafoActual.Saltar)
            {
                ParrafoActual.Saltar = true;
                CheckAndSave();
            }
        }

        private void chkbSaltarParrafo_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ParrafoActual.Saltar)
            {
                ParrafoActual.Saltar = false;
                CheckAndSave();
            }
        }

        private void txtIndexInicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? index = GetIfIsNumberValid(txtIndexInicio);
            if (index.HasValue && ParrafoActual.IndexInicio != index.Value)
            {
                ParrafoActual.IndexInicio = index.Value;
                CheckAndSave();
            }
        }

        private void txtIndexFin_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? index = GetIfIsNumberValid(txtIndexFin);
            if (index.HasValue && ParrafoActual.IndexFin != index.Value)
            {
                ParrafoActual.IndexFin = index.Value;
                CheckAndSave();
            }
        }
        private void txtInicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? index = GetIfIsNumberValid(txtInicio);
            if (index.HasValue && ParrafoActual.CharInicio != index.Value)
            {
                ParrafoActual.CharInicio = index.Value;
                CheckAndSave();
            }
        }

        private void txtFin_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? index = GetIfIsNumberValid(txtFin);
            if (index.HasValue && ParrafoActual.CharFin != index.Value)
            {
                ParrafoActual.CharFin = index.Value;
                CheckAndSave();
            }
        }


        private void cmbChapters_SelectionChanged(object sender = null, SelectionChangedEventArgs e = null)
        {
            if (cmbChapters.SelectedIndex != -1)
            {
                ParrafosCapitulosVersion = EbookActual.Version.GetContentElementsArray(cmbChapters.SelectedIndex);
                ParrafosCapitulosReference = EbookActual.Reference.Version.GetContentElementsArray(cmbChapters.SelectedIndex);
                cmbParrafosReference.Items.Clear();
                cmbParrafosReference.Items.AddRange(Enumerable.Range(0, ParrafosCapitulosReference.Length).ToArray().Convert((p) => $"párrafo referencia {p}"));

                cmbParrafosVersion.Items.Clear();
                cmbParrafosVersion.Items.AddRange(Enumerable.Range(0, ParrafosCapitulosVersion.Length).ToArray().Convert((p) => $"párrafo versión {p}"));

                cmbParrafosReference.SelectedIndex = 0;
                cmbParrafosVersion.SelectedIndex = 0;

                visorCapitiloSpliter.Capitulo = EbookActual.GetCapitulo(cmbChapters.SelectedIndex).ParrafosEditados;
                CheckChapterFinished();

            }
        }

        private void cmbEbookOriginal_SelectionChanged(object sender=null, SelectionChangedEventArgs e=null)
        {
            EbookActual.Reference = MainWindow.GetReference(cmbEbookOriginal.SelectedItem as EbookSplited);
            if (cmbChapters.SelectedIndex != 0)
                cmbChapters.SelectedIndex = 0;
            else cmbChapters_SelectionChanged();


        }

        private void visorCapitiloSpliter_SpliterSelected(object sender, SpliterEventArgs e)
        {
            ParrafoActual = e.Spliter;
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            Spliter spliter = new Spliter();

            visorCapitiloSpliter.Capitulo.Add(spliter);
            visorCapitiloSpliter.Refresh();

            ParrafoActual = spliter;

        }
    }
}
