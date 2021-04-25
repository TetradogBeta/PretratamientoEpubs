using CommonEbookPretractament;
using Gabriel.Cat.S.Extension;
using Notifications.Wpf.Core;
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
        class DummyEbookSpliter
        {
            public DummyEbookSpliter(EbookSplited ebook) => Ebook = ebook;
            public EbookSplited Ebook { get; private set; }

            public override bool Equals(object obj)
            {
                bool equals = obj is DummyEbookSpliter;

                if (equals)
                {
                    equals = Ebook.Equals((obj as DummyEbookSpliter).Ebook);
                }
                else
                {
                    equals = Ebook.Equals(obj);
                }
                return equals;
            }

            public override string ToString()
            {
                return $"Referencia {Ebook.SaveName}";
            }
        }

        static List<System.Threading.Tasks.Task> ToDo = new List<System.Threading.Tasks.Task>();

        private Spliter parrafoActual;

        public event EventHandler HasChanges;

        public CapituloViewer()
        {
            InitializeComponent();
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

                txtIndexFin.Text = parrafoActual.EditIndexFin + "";
                txtIndexInicio.Text = parrafoActual.EditIndexInicio + "";

                chkbSaltarParrafo.IsChecked = parrafoActual.Saltar;

                if (!visorCapitiloSpliter.Parrafos.Exists((p) => p.CompareTo(parrafoActual) == 0))
                {
                    visorCapitiloSpliter.Parrafos.Add(parrafoActual);
                    visorCapitiloSpliter.Refresh();
                }
                else
                {
                    parrafoActual = visorCapitiloSpliter.Parrafos.First((p) => p.CompareTo(parrafoActual) == 0);
                }

            }
        }
        public EbookStandaritzed EbookActual
        {
            get => visorCapitiloSpliter.Ebook;
            set
            {
                visorCapitiloSpliter.Ebook = value;
                ParrafoActual = new Spliter();
                if (!Equals(MainWindow.Group, default))
                {
                    //cargo el libro actual
                    cmbEbookOriginal.SelectionChanged -= cmbEbookOriginal_SelectionChanged;
                    cmbEbookOriginal.ItemsSource = MainWindow.Group.Items.ToArray().Convert((l) => new DummyEbookSpliter((EbookSplited)l));
                    cmbEbookOriginal.SelectedIndex = GetIndexEbookOriginal(EbookActual.Reference.Version);
                    cmbEbookOriginal.SelectionChanged += cmbEbookOriginal_SelectionChanged;
                    cmbChapters.Items.Clear();
                    cmbChapters.Items.AddRange(Enumerable.Range(1, EbookActual.TotalChapters).ToArray().Convert((p) => $"Capítulo {p}"));
                    cmbEbookOriginal_SelectionChanged();
                }

            }
        }
        private int GetIndexEbookOriginal(EbookSplited ebook)
        {
            return cmbEbookOriginal.Items.IndexOf(cmbEbookOriginal.Items.ToArray().First((e) => e.Equals(ebook)));
        }
        private void CheckAndSave()
        {

            EbookActual.Save();
            visorCapitiloSpliter.Refresh();

            CheckChapterFinished();

            if (HasChanges != null)
                HasChanges(this, new EventArgs());

        }

        private void CheckChapterFinished()
        {
            if (cmbChapters.SelectedIndex >= 0 && cmbChapters.SelectedIndex < EbookActual.TotalChapters)
            {
                if (EbookActual.Finished(cmbChapters.SelectedIndex))
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
                ParrafoActual.EditIndexInicio = index.Value;
                CheckAndSave();
            }
        }

        private void txtIndexFin_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? index = GetIfIsNumberValid(txtIndexFin);
            if (index.HasValue && ParrafoActual.IndexFin != index.Value)
            {
                ParrafoActual.EditIndexFin = index.Value;
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
                ParrafosCapitulosReference = EbookActual.Reference.GetContentElementsArray(cmbChapters.SelectedIndex);
                cmbParrafosReference.Items.Clear();
                //el +1 me ha dado un problema porque el Index Inicio es confuso...tengo que elegir entre empezar por 0 o restar 1 si es > 0
                //restare
                cmbParrafosReference.Items.AddRange(Enumerable.Range(1, ParrafosCapitulosReference.Length).ToArray().Convert((p) => $"párrafo referencia {p}"));

                cmbParrafosVersion.Items.Clear();
                cmbParrafosVersion.Items.AddRange(Enumerable.Range(1, ParrafosCapitulosVersion.Length).ToArray().Convert((p) => $"párrafo versión {p}"));

                cmbParrafosReference.SelectedIndex = 0;
                cmbParrafosVersion.SelectedIndex = 0;

                visorCapitiloSpliter.Chapter = cmbChapters.SelectedIndex;
                CheckChapterFinished();
                tbInfo.Text = "";

            }
        }

        private void cmbEbookOriginal_SelectionChanged(object sender = null, SelectionChangedEventArgs e = null)
        {
            NotificationManager notificationManager;
            EbookStandaritzed parent;
            string tituloError;
            if (cmbEbookOriginal.SelectedIndex >= 0)
            {
                parent = MainWindow.GetReference((cmbEbookOriginal.SelectedItem as DummyEbookSpliter).Ebook);
                if (EbookActual.IsParentValid(parent))
                    EbookActual.Reference = parent;
                else
                {
                    cmbEbookOriginal.SelectionChanged -= cmbEbookOriginal_SelectionChanged;
                    if (!Equals(EbookActual.Reference, default))
                    {
                        tituloError = "Pongo el que habia";
                        cmbEbookOriginal.SelectedIndex = GetIndexEbookOriginal(EbookActual.Reference.Version);
                    }
                    else
                    {
                        tituloError = "No pongo ninguno,elige uno valido";
                        cmbEbookOriginal.SelectedIndex = -1;
                    }
                    cmbEbookOriginal.SelectionChanged += cmbEbookOriginal_SelectionChanged;
                    notificationManager = new NotificationManager();

                    ToDo.Add(notificationManager.ShowAsync(new NotificationContent
                    {
                        Title = tituloError,
                        Message = "Atención! Has intentado poner como base un descendiente de este mismo...",
                       
                        Type = NotificationType.Error
                    }));
                }

                CheckAndSave();
            }
            cmbChapters.SelectionChanged -= cmbChapters_SelectionChanged;
            cmbChapters.SelectedIndex = 0;
            cmbChapters.SelectionChanged += cmbChapters_SelectionChanged;
            cmbChapters_SelectionChanged();




        }

        private void visorCapitiloSpliter_SpliterSelected(object sender, SpliterEventArgs e)
        {
            ParrafoActual = e.Spliter;
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            Spliter spliter = new Spliter();

            visorCapitiloSpliter.Parrafos.Add(spliter);
            visorCapitiloSpliter.Refresh();

            ParrafoActual = spliter;

        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UnsafeMode || MessageBox.Show("¿Está seguro de borrarlo?", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                visorCapitiloSpliter.Parrafos.Remove(ParrafoActual);
                visorCapitiloSpliter.Refresh();
                ParrafoActual = new Spliter();
                CheckAndSave();
            }
        }

        private void rtbVersion_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Point ptrSelection = rtbVersion.GetSelectionRange();
            tbInfo.Text = $": Index Inicio = {cmbParrafosVersion.SelectedIndex+1}, Inicio = {ptrSelection.X}, Fin = {ptrSelection.Y}";
        }
    }
}
