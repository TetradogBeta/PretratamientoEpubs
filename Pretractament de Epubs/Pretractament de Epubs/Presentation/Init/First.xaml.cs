using PretractamentDeEpubs.Controls;
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

namespace PretractamentDeEpubs.Presentation.Init
{
    /// <summary>
    /// Lógica de interacción para First.xaml
    /// </summary>
    public partial class First : UserControl,IViewPresentation
    {
        public First()
        {
            InitializeComponent();
        }

        public ViewPresentation View => view;
    }
}
