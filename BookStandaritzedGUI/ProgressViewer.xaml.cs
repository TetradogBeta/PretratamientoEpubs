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
using System.Windows.Shapes;

namespace BookStandaritzedGUI
{
    /// <summary>
    /// Lógica de interacción para ProgressViewer.xaml
    /// </summary>
    public partial class ProgressViewer : Window
    {
        static readonly FontFamily FontFamilyDefault = new TextBlock().FontFamily;
        static readonly FontFamily FontFamilySelected = new FontFamily("Constantia");
        static double DefaultHeight;
        const string SEPARACION = " ~ ";

        int Pos { get; set; }
        List<TextBlock> Selecteds { get; set; }
        EbookStandaritzed Ebook { get; set; }
        int Chapter { get; set; }

        bool IsPreviewOn { get; set; }
        bool? EstaUniendo { get; set; }
        bool ErrorUnir { get; set; }


        public event EventHandler HasChanges;

        public ProgressViewer()
        {
            InitializeComponent();
            Selecteds = new List<TextBlock>();
            KeyDown += (s, e) => { if (e.Key.HasFlag(Key.F5)) _=Notificaciones.CloseAllMessages(); };
        }

        public ProgressViewer(EbookStandaritzed ebook, int chapter) :this()
        {


            IsPreviewOn = false;
            Title = $"Resultado       {ebook.Version.SaveName}      ~       {ebook.Reference.Version.SaveName}";
            Ebook = ebook;
            Chapter = chapter;
            Reload();
            Task.Delay(100).ContinueWith((t) => {
                Action act = () =>
                {
                    if (stkReference.Children.Count > 0)
                        DefaultHeight = ((TextBlock)stkReference.Children[0]).ActualHeight;
                };
                Dispatcher.BeginInvoke(act);
                _ = Notificaciones.ShowMessage("Sugerencia", "Pulsa 'ctrl' para poder seleccionar/deseleccionar un parrafo", nameControl: nameof(notificationsManagerProgress), notificacionesOn: () => MainWindow.Main.SugerenciasOn);
                _ = Notificaciones.ShowMessage("Sugerencia", "Si unes se supone que va del indice más pequeño al más grande", nameControl: nameof(notificationsManagerProgress), notificacionesOn: () => MainWindow.Main.SugerenciasOn);
             });
        }

        private void Reload()
        {
            string[] parrafos;
            TextBlock tb;
            int pos;
            Run line;
            string strTb;
            string text;
            Capitulo capitulo;

            IsPreviewOn = false;
            Selecteds.Clear();
            RefreshButtons();
            ErrorUnir = false;
            stkVersion.Children.Clear();
            stkReference.Children.Clear();

            Pos = 0;
            stkVersion.Children.AddRange(Ebook.GetContentElementsArray(Chapter).Convert(StringToView));
            Pos = 0;
            stkReference.Children.AddRange(Ebook.Reference.GetContentElementsArray(Chapter).Convert(StringToView));


            pos = 0;
            parrafos = Ebook.Version.GetContentElementsArray(Chapter);
            capitulo = Ebook.GetCapitulo(Chapter);

            foreach (UIElement element in stkVersion.Children)
            {
                tb = element as TextBlock;
                strTb = tb.Tag.ToString();
                text = strTb.Split(SEPARACION)[1];
                while (!text.Contains(parrafos[pos])) pos++;

                line = new Run((pos + 1).ToString().PadLeft(3, '0'));
                line.Foreground = Spliter.IndexNotIn(capitulo.ParrafosEditados, pos+1)? Brushes.DarkRed:Brushes.Violet;
                tb.Inlines.InsertBefore(tb.Inlines.FirstInline, line);
                tb.Inlines.InsertAfter(line, new Run(":") { Foreground = Brushes.Gray });
                tb.Tag = line.Text + ":" + strTb;
            }
            stkVersion.Tag = stkVersion.Children.ToArray();
            stkReference.Tag = stkReference.Children.ToArray();
       
        }

        UIElement StringToView(string str)
        {

            TextBlock tb;
            Run line;

            Pos++;
            tb = new TextBlock() { Background = Pos % 2 == 0 ? Brushes.LightGreen : Brushes.Transparent };
            line = new Run((Pos).ToString().PadLeft(3, '0'));
            line.Foreground = Brushes.DarkBlue;
            tb.Inlines.Add(line);
            tb.Inlines.Add(new Run(SEPARACION) { Foreground = Brushes.Gray });
            tb.Inlines.Add(new Run(str));
            tb.Tag = line.Text + SEPARACION + str;
            tb.MouseRightButtonDown += (s, e) =>
            {
                double offset;
                TextBlock tbFound = default;
                StackPanel stkAMover, stkElementOrigen;
                ScrollViewer scAMover;
                TextBlock tbClicked = s as TextBlock;
                int itemPos;
                object[] items;

                if (ReferenceEquals(tbClicked.Parent, stkReference))
                {

                    //mover scroll Version
                    stkAMover = stkVersion;
                    stkElementOrigen = stkReference;
                }
                else
                {
                    //mover scroll Referencia
                    stkAMover = stkReference;
                    stkElementOrigen = stkVersion;
                }
                itemPos = stkElementOrigen.Children.IndexOf(tbClicked);


                items = (object[])stkAMover.Tag;
                if (itemPos < items.Length)
                    tbFound = (TextBlock)items[itemPos];

                scAMover = stkAMover.Parent as ScrollViewer;

                if (!Equals(tbFound, default))
                {
                    offset = ((stkElementOrigen.Parent as ScrollViewer).VerticalOffset - (tbClicked.ActualHeight * itemPos)) + tbFound.ActualHeight * (stkAMover.Children.IndexOf(tbFound));

                    scAMover.ScrollToVerticalOffset(offset);
                }
                else
                    scAMover.ScrollToVerticalOffset(stkAMover.ActualHeight);
                scAMover.UpdateLayout();
            };
            tb.MouseLeftButtonDown += (s, e) =>
            {
                int indexTb;
                TextBlock tbClicked = (TextBlock)s;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                   
                    if (ReferenceEquals(tbClicked.Parent, stkVersion))
                    {//mirar que un spliter no lo contenga ya!!
                        indexTb = GetIndexTb(tbClicked);
                        if (Spliter.IndexNotIn(Ebook.GetCapitulo(Chapter).ParrafosEditados, indexTb))
                        {
                            if (Selecteds.Contains(tbClicked))
                            {
                                Selecteds.Remove(tbClicked);
                                tbClicked.FontFamily = FontFamilyDefault;
                            }
                            else
                            {
                                Selecteds.Add(tbClicked);
                                tbClicked.FontFamily = FontFamilySelected;
                            }
                            RefreshButtons();
                        }
                        else {
                            Notificaciones.CloseAllMessages(nameof(notificationsManagerProgress))
                              .ContinueWith((t) => Notificaciones.ShowMessage("Atención", "Este párrafo es el resultado de un spliter, no se puede usar", notificationType: Notifications.Wpf.Core.NotificationType.Warning, nameControl: nameof(notificationsManagerProgress)));
                        }
                    }

                }

            };
            return tb;
        }

        private void RefreshButtons()
        {
            gButtons.IsEnabled = Selecteds.Count > 0;
        }

        private void btnUnir_Click(object sender, RoutedEventArgs e)
        {
            int inicio, fin;
            bool correcto = true;
            int  indexInicio;
            List<Spliter> parrafos;
            TextBlock tbAMirar;

            if (!EstaUniendo.HasValue || EstaUniendo.Value)
            {//hay un bug al saltarse uno o varios estos no desaparecen

                inicio = Selecteds.Min((tb) => GetIndexTb(tb));
                fin = Selecteds.Max((tb) => GetIndexTb(tb));
                parrafos = Ebook.GetCapitulo(Chapter).ParrafosEditados;
                //selecciono los que están en este rango
                //si hay alguno no valido paro e informo del problema
                indexInicio =stkVersion.Children.IndexOf(Selecteds.Find((tb) => GetIndexTb(tb) == inicio));
                for(int i = inicio+1,j=1; i < fin && correcto; i++,j++)
                {//me falla con los que se saltan tiene que dar error

                    correcto = Spliter.IndexNotIn(parrafos, i);
                    if (correcto)
                    {
                        tbAMirar = (TextBlock)stkVersion.Children[indexInicio + j];
                        if (!Equals(tbAMirar.FontFamily, FontFamilySelected))
                        {
                            tbAMirar.FontFamily = FontFamilySelected;
                            Selecteds.Add(tbAMirar);
                        }
                    }
                }
                ErrorUnir = !correcto;
                if (correcto)
                {
                    CommonUnirSaltar();
                    EstaUniendo = true;
                }
                else
                {
                    _ = Notificaciones.ShowMessage("Atención", "El rango seleccionado incluye un resultado de un spliter (son los que el parrafo original son de color violeta) o uno que se ha saltado, revísalo", nameControl: nameof(notificationsManagerProgress));
                }
            }
            else _ = Notificaciones.ShowMessage("Atención", "Ahora mismo estás saltando, no puedes unir, si quieres hacerlo tienes que aplicar o deshacer", nameControl: nameof(notificationsManagerProgress));

        }
        private void btnSaltar_Click(object sender, RoutedEventArgs e)
        {
            if (!EstaUniendo.HasValue || !EstaUniendo.Value)
            {
                CommonUnirSaltar(0);
                EstaUniendo = false;
                ErrorUnir = false;
            }
            else _ = Notificaciones.ShowMessage("Atención", "Ahora mismo estás uniendo, no puedes saltar, si quieres hacerlo tienes que aplicar o deshacer", nameControl: nameof(notificationsManagerProgress));
        }
        private void CommonUnirSaltar(int inicio = 1)
        {
           
                for (int i = inicio; i < Selecteds.Count; i++)
                {
                    Selecteds[i].Height = 0;
                }
                IsPreviewOn = true;

        }

       

        private void btnDeshacer_Click(object sender, RoutedEventArgs e)
        {
            Array arrayTbVersion = ((Array)stkVersion.Tag);
            Selecteds.ForEach((tb) => {

                tb.FontFamily = FontFamilyDefault;
                tb.Height = DefaultHeight;
                   
                
            });
            Selecteds.Clear();
            RefreshButtons();
            IsPreviewOn = false;
            EstaUniendo = null;
            ErrorUnir = false;
        }

        private void btnAplicar_Click(object sender, RoutedEventArgs e)
        {
            List<Spliter> parrafos;
            int inicio;
            int fin;
            if (!ErrorUnir)
            {
                if (IsPreviewOn)
                {
                    parrafos = Ebook.GetCapitulo(Chapter).ParrafosEditados;
                    //creo el spliter y lo añado al capitulo que toca
                    if (EstaUniendo.GetValueOrDefault())
                    {
                        //unir
                        inicio = Selecteds.Min((tb) => GetIndexTb(tb));
                        fin = Selecteds.Max((tb) => GetIndexTb(tb));
                        parrafos.Add(new Spliter() { EditIndexInicio = inicio, EditIndexFin = fin });
                    }
                    else
                    {
                        //saltar
                        Selecteds.ForEach((tbSaltar) =>
                        {
                            inicio = GetIndexTb(tbSaltar);
                            parrafos.Add(new Spliter() { EditIndexInicio = inicio, Saltar = true });
                        });
                    }

                    if (HasChanges != null)
                        HasChanges(this, new EventArgs());



                    Reload();
                    EstaUniendo = null;
                    Notificaciones.CloseAllMessages(nameof(notificationsManagerProgress))
                                  .ContinueWith((t) => Notificaciones.ShowMessage("Información", "Cambios aplicados con éxito!", notificationType: Notifications.Wpf.Core.NotificationType.Success, nameControl: nameof(notificationsManagerProgress)));
                }
                else _ = Notificaciones.ShowMessage("Atención", "Tienes que unir, saltar los parrafos actuales, sino quieres hacer nada con ellos dale a deshacer.", nameControl: nameof(notificationsManagerProgress));
            }
            else _ = Notificaciones.ShowMessage("Atención", "Tienes que arreglar la union de párrafos actual o darle a deshacer",Notifications.Wpf.Core.NotificationType.Error, nameControl: nameof(notificationsManagerProgress));

        }
        private int GetIndexTb(TextBlock tb)=> int.Parse((tb.Inlines.FirstInline as Run).Text);
    }
}
