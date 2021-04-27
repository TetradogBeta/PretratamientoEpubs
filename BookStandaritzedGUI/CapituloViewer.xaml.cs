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
            ContextMenu menu;
            MenuItem menuItem;
            InitializeComponent();

            menu = new ContextMenu();
            menuItem = new MenuItem() { Header = "Crear Spliters" };
            menuItem.Click += CrearEnSplitersSeleccion;
            menu.Items.Add(menuItem);
            menuItem = new MenuItem() { Header = "Dividir en Spliters" };
            menuItem.Click += DividirEnSplitersSeleccion;
            menu.Items.Add(menuItem);
            rtbVersion.ContextMenu = menu;
        }



        string[] ParrafosCapitulosReference { get; set; }
        string[] ParrafosCapitulosVersion { get; set; }
        Spliter ParrafoActual
        {
            get => parrafoActual;
            set
            {
                parrafoActual = value;

                RefreshInterficieParrafo();

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
        private void RefreshInterficieParrafo()
        {
            txtFin.Text = parrafoActual.CharFin + "";
            txtInicio.Text = parrafoActual.CharInicio + "";

            txtIndexFin.Text = parrafoActual.EditIndexFin + "";
            txtIndexInicio.Text = parrafoActual.EditIndexInicio + "";

            chkbSaltarParrafo.IsChecked = parrafoActual.Saltar;
        }
        private int GetIndexEbookOriginal(EbookSplited ebook)
        {
            return cmbEbookOriginal.Items.IndexOf(cmbEbookOriginal.Items.ToArray().First((e) => e.Equals(ebook)));
        }
        private void CheckAndSave()
        {
            List<Spliter> splitersNoValidos = visorCapitiloSpliter.Parrafos.Filtra((p) => !p.IsRelevant);
            for (int i = 0,f= splitersNoValidos.Count - 1; i < f; i++)
                visorCapitiloSpliter.Parrafos.Remove(splitersNoValidos[i]);
            if(splitersNoValidos.Count>0 && !visorCapitiloSpliter.Parrafos.Contains(ParrafoActual))
                ParrafoActual = splitersNoValidos.First<Spliter>();

            EbookActual.Save();


            CheckChapterFinished();
            RefreshInterficieParrafo();
            visorCapitiloSpliter.Refresh();

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
            tbInfo.Text = $": Index Inicio = {cmbParrafosVersion.SelectedIndex + 1}, Inicio = {ptrSelection.X}, Fin = {ptrSelection.Y}";
            tbInfo.Tag = new Nullable<Point>(ptrSelection);
        }
        private void DividirEnSplitersSeleccion(object sender, RoutedEventArgs e)
        {
            Spliter parrafo, parrafoAux;
            parrafo = GetParrafoSeleccionado();
            if (!Equals(parrafo, default))
            {
                if (parrafo.CharInicio != 0)
                {
                    parrafoAux = new Spliter() { EditIndexInicio = parrafo.EditIndexInicio, CharInicio = 0, CharFin = parrafo.CharInicio };
                    visorCapitiloSpliter.Parrafos.Add(parrafoAux);
                }
                if (parrafo.CharFin != rtbVersion.GetText().Length)
                {
                    if (!string.IsNullOrWhiteSpace(rtbVersion.GetText().Substring(parrafo.CharFin)))
                    {
                        parrafoAux = new Spliter() { EditIndexInicio = parrafo.EditIndexInicio, CharInicio = parrafo.CharFin, CharFin = -1 };
                        visorCapitiloSpliter.Parrafos.Add(parrafoAux);
                    }
                    else { 
                        parrafo.CharFin = -1;
                        ParrafoActual = parrafo;
                    }
                }
                visorCapitiloSpliter.Refresh();
                ParrafoActual = parrafo;
            }
        }
        private void CrearEnSplitersSeleccion(object sender, RoutedEventArgs e)
        {
            Spliter parrafo = GetParrafoSeleccionado();
            if (!Equals(parrafo, default))
            {
                ParrafoActual = parrafo;
                visorCapitiloSpliter.Refresh();
            }
        }
        Spliter GetParrafoSeleccionado()
        {
            Spliter parrafo;
            Nullable<Point> point;
            if (rtbVersion.IsSelectionActive && !Equals(tbInfo.Tag, default) && (tbInfo.Tag as Nullable<Point>).HasValue)
            {
                point = (tbInfo.Tag as Nullable<Point>);
                parrafo = new Spliter() { EditIndexInicio = cmbParrafosVersion.SelectedIndex + 1, CharInicio = int.Parse(point.Value.X + ""), CharFin = int.Parse(point.Value.Y + "") };
                visorCapitiloSpliter.Parrafos.Add(parrafo);

            }
            else parrafo = default;
            return parrafo;
        }
    }
}
