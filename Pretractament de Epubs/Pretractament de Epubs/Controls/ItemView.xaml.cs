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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PretractamentDeEpubs.Controls
{

    [ContentProperty(nameof(Content))]
    /// <summary>
    /// Lógica de interacción para ItemView.xaml
    /// </summary>
    public partial class ItemView : UserControl, INotifyPropertyChanged,IItemView
    {
        public static TimeSpan UnlimitFinishTime = new TimeSpan();
        public static int DefaultTimeFinish = 10;
        public static int DefaultTimeInit = 0;
        private TimeSpan finishTime;
        private TimeSpan initTime;
        private int column;
        private int row;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ItemView()
        {
            Row = 0;
            Column = 0;
            InitTime = TimeSpan.FromSeconds(DefaultTimeInit);
            FinishTime = TimeSpan.FromSeconds(DefaultTimeFinish);
            InitializeComponent();
        }
        public int Row
        {
            get => row;
            set
            {
                row = value;
                OnPropertyChanged();
            }
        }

        public int Column
        {
            get => column;
            set
            {
                column = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan InitTime
        {
            get => initTime;
            set
            {
                initTime = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan FinishTime { 
            get => finishTime; 
            set { 
                finishTime = value;
                OnPropertyChanged();
            }
        }
        public UIElement? Element
        {
            get => this;

        }
        public TimeSpan Duration => FinishTime - InitTime;



        public new object? Content
        {
            get => base.Content;
            set {
                
                base.Content = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
