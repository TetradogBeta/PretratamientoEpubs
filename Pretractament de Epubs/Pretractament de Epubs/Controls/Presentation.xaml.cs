using System;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PretractamentDeEpubs.Controls
{
    /// <summary>
    /// Lógica de interacción para Presentation.xaml
    /// </summary>
    public partial class Presentation : Grid, INotifyPropertyChanged
    {
        private int actualViewIndex;
        private bool repeat;

        public event EventHandler? Finish;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Presentation()
        {
            ActualViewIndex = 0;
            Repeat = false;
            InitializeComponent();
            _ = Refresh();
        }
        public IEnumerable<IViewPresentation> Views => Children.OfType<IViewPresentation>();
        public int Total => Views.Count();
        public int ActualViewIndex { get => actualViewIndex; set { actualViewIndex = value; OnPropertyChanged(); } }

        public bool Repeat { get => repeat; set { repeat = value; OnPropertyChanged(); } }

        public bool CanArribeToEndPresentation
        {
            get
            {
                const int MAX = 1;
                int totalUnlimited = Views.Where(v => v.View.IsUnlimitedViewPresentation).Count();
                bool isAllRight = totalUnlimited <= MAX;
                
                if(totalUnlimited==MAX)
                    isAllRight= Views.Last().View.IsUnlimitedViewPresentation;

                return isAllRight;
            }
        }
        public async Task Refresh()
        {
           
            Action act = () =>
            {
                int pos = 0;
                //actualizo el index del selector
                //pongo la vista visible
                foreach (IViewPresentation view in Views)
                {
                    if (pos == ActualViewIndex)
                    {
                   
                        view.View.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        view.View.Visibility = Visibility.Collapsed;
                    }
                    pos++;
                }
            };
            await Dispatcher.BeginInvoke(act);

        }

        public async Task Next()
        {
            ActualViewIndex++;
            if (ActualViewIndex >= Total)
            {
                ActualViewIndex = 0;
                if (Finish != null)
                    Finish(this, new EventArgs());

            }
            if (ActualViewIndex != 0 || Repeat)
                await Refresh();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
