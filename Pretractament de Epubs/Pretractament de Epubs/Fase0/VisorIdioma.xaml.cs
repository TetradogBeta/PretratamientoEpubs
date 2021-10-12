using Gabriel.Cat.S.Extension;
using PretractamentDeEpubs.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

namespace PretractamentDeEpubs.Fase0
{
    /// <summary>
    /// Lógica de interacción para VisorIdioma.xaml
    /// </summary>
    public partial class VisorIdioma : UserControl
    {
        public VisorIdioma()
        {
            InitializeComponent();
        }
        public Idioma? Idioma { get; set; }
        public void Refresh()
        {
            if (!ReferenceEquals(Idioma, default))
            {
                txtNombre.Text = Idioma.Nombre;
                txtCulture.Text = Idioma.Culture;
                if (!ReferenceEquals(Idioma.PathIcono, default))
                {
                    imgIcono.Source = new BitmapImage(new Uri(Idioma.PathIcono));
                }
                else
                {
                    imgIcono.SetImage(new Bitmap(1, 1));
                }
                if (!ReferenceEquals(Idioma.PathTraduccionApp, default))
                {
                    imgHayTraduccionApp.SetImage((Bitmap)Bitmap.FromStream(new MemoryStream(Properties.Resources.estrella)));
                }
                else
                {
                    imgHayTraduccionApp.SetImage(new Bitmap(1, 1));
                }
            }
        }
    }
}
