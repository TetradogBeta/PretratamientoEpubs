using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PretractamentDeEpubs.Controls
{

    /// <summary>
    /// Lógica de interacción para ViewPresentation.xaml
    /// </summary>
    public partial class ViewPresentation : Grid,INotifyPropertyChanged,IViewPresentation
    {
        public static TimeSpan TickTackTime { get; set; } = TimeSpan.FromMilliseconds(100);

        private TimeSpan actualTime;

        public event EventHandler? ViewFinished;
        public event PropertyChangedEventHandler? PropertyChanged;
        public ViewPresentation()
        {
            Timer = new System.Timers.Timer();
            Timer.Elapsed += async (s, e) =>
            {
                ActualTime += TickTackTime;
                try
                {
                    await Refresh();
                }
                catch { }
            };

            ActualItems = new List<IItemView>();
            InitializeComponent();

            _ = Reset();


        }
        
        public TimeSpan ActualTime { 
            get => actualTime; 
            set { 
                actualTime = value;
                OnPropertyChanged();
            }
        }
        List<IItemView> ActualItems { get; set; }
        System.Timers.Timer Timer { get; set; }

        public IEnumerable<IItemView> Items => Children.OfType<IItemView>();

        public bool Finished => ActualTime >= Items.Max((item) => item.FinishTime) && !IsUnlimitedViewPresentation;
        public bool IsUnlimitedViewPresentation => Items.Any(i => Equals(i.FinishTime, ItemView.UnlimitFinishTime));

        public ViewPresentation View => this;
        public async Task Reset()
        {
            ActualTime = new TimeSpan();
            await Refresh();
        }

        public async Task Refresh()
        {
            Action act = async () =>
            {
                for (int i = ActualItems.Count - 1; i >= 0; i--)
                {
                    if (ActualTime < ActualItems[i].InitTime || !Equals(ActualItems[i].FinishTime, ItemView.UnlimitFinishTime) && ActualTime <= ActualItems[i].FinishTime)
                    {


                        ActualItems[i].Element.Visibility = Visibility.Collapsed;
                        ActualItems.RemoveAt(i);

                    }
                }

                foreach (IItemView item in Items)
                {
                    if (!ReferenceEquals(item, default))
                    {

                        if (ActualTime >= item.InitTime && !Equals(item.FinishTime, ItemView.UnlimitFinishTime) && ActualTime < item.FinishTime)
                        {


                            if (item.Element.Visibility != Visibility.Visible)
                                item.Element.Visibility = Visibility.Visible;
                            if (!ActualItems.Contains(item))
                            {

                                SetRow(item.Element, item.Row);
                                SetColumn(item.Element, item.Column);
                                ActualItems.Add(item);

                            }
                        }
                        else if (item.Element.Visibility == Visibility.Visible && !Equals(item.FinishTime,ItemView.UnlimitFinishTime))
                            item.Element.Visibility = Visibility.Collapsed;


                    }
                }
                if (Finished)
                {
                    await Next();
                }
            };
            await Dispatcher.BeginInvoke(act);

        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Action act;
            if (Visibility == Visibility.Visible)
            {
                //si es visible es que se quiere ejecutar
                act = async () =>
                 {
                     await Start();
                 };
            }
            else
            {
                //si es invisible es que se tiene que parar y resetear
                act = async () =>
                  {
                      await Stop();

                  };
            }
            Dispatcher.BeginInvoke(act);


        }

        public async Task Start(TimeSpan init = default)
        {
            if (Equals(init, default))
            {
                init = new TimeSpan();
            }
            ActualTime = init;
            await Refresh();
            //inicio el temporizador
            if (Timer.Enabled)
            {
                Timer.Stop();
            }
            Timer.Start();
        }

        public async Task Stop()
        {
            //paro el temporizador
            if (Timer.Enabled)
            {
                Timer.Stop();
            }
            
            await Reset();
        }

        public async Task Next()
        {
            Presentation? parent = Parent as Presentation;

            if (!ReferenceEquals(parent, default))
            {
                await parent.Next();
            }
            if (ViewFinished != null)
            {
                ViewFinished(this, new EventArgs());
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
