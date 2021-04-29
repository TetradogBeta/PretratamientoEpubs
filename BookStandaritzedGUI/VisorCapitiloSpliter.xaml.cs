using CommonEbookPretractament;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para VisorCapitiloSpliter.xaml
    /// </summary>
    public partial class VisorCapitiloSpliter : UserControl
    {

        private int chapter;

        public event EventHandler<SpliterEventArgs> SpliterSelected;

        public event EventHandler HasChanges;

        public VisorCapitiloSpliter()
        {
            InitializeComponent();
        }
        public List<Spliter> Parrafos => Ebook.GetCapitulo(Chapter).ParrafosEditados;
        public int Chapter { get => chapter; set { chapter = value; Refresh(); } }
        public EbookStandaritzed Ebook { get; set; }

        public void Refresh()
        {
            parrafosCapitulo.Children.Clear();
            //valido que los parrafos esten bien



            Parrafos.ForEach((p) =>
            {
                TextBlock tbParrafo = new TextBlock();
                tbParrafo.Height = 15;
                tbParrafo.Text = p.ToString();
                if (p.Saltar)
                    tbParrafo.Foreground = Brushes.DarkGray;
                tbParrafo.Tag = p;
                tbParrafo.MouseLeftButtonDown += (s, e) => {

                    if (!Equals(SpliterSelected, default))
                    {
                        SpliterSelected(this, new SpliterEventArgs((s as TextBlock).Tag as Spliter));
                    }
                    foreach (UIElement item in parrafosCapitulo.Children)
                        (item as TextBlock).Background = Brushes.White;
                    (s as TextBlock).Background = Brushes.LightBlue;
                
                };
                parrafosCapitulo.Children.Add(tbParrafo);
            });
            //añado los capitulos
            //onClick SpliterSelected
        }

        private void btnViewProgress_Click(object sender, RoutedEventArgs e)
        {
            ProgressViewer progress;
            try
            {
                progress = new ProgressViewer(Ebook, Chapter);
                progress.HasChanges += (s, e) => {
                    Action act = () =>
                    {
                        Refresh();
                        

                    };
                    Dispatcher.BeginInvoke(act);
                    if (HasChanges != null)
                        HasChanges(this, new EventArgs());
                
                
                };
                progress.ShowDialog();
            }
            catch(Exception ex)
            {
                _=MainWindow.Main.MostrarMensaje("Exception", ex.Message,TimeSpan.FromSeconds(25));

            }
        }
    }

    public class SpliterEventArgs:EventArgs
    {
        public SpliterEventArgs(Spliter spliter) => Spliter = spliter;
        public Spliter Spliter { get; private set; }
    }
}
