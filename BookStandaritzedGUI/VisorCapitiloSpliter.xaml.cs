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
            Notifications.Wpf.Core.NotificationManager manager;
            try
            {
                new ProgressViewer(Ebook, Chapter).ShowDialog();
            }
            catch
            {
                manager = new Notifications.Wpf.Core.NotificationManager();
                manager.ShowAsync("Revisa los Spliters!", "Error", new TimeSpan(20 * 1000));

            }
        }
    }

    public class SpliterEventArgs:EventArgs
    {
        public SpliterEventArgs(Spliter spliter) => Spliter = spliter;
        public Spliter Spliter { get; private set; }
    }
}
