using CommonEbookPretractament;
using Gabriel.Cat.S.Extension;
using Notifications.Wpf.Core;
using System;
using System.Collections;
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

namespace BookStandaritzedGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Version = "Book Standaritzed V2.3";
        public static MainWindow Main { get; set; }
        public static SortedList<string, EbookStandaritzed> DicStandard { get; set; }
        public static GroupItem Group { get; set; }
        public static bool UnsafeMode { get; set; } = false;

        static string UnsafeString => UnsafeMode ? "Unsafe" : "";

        Guid? LastSugerenciaMode { get; set; }
        Guid? LastNotificacionMode { get; set; }


        public MainWindow()
        {
            Main = this;
            Title = Version;
            DicStandard = new SortedList<string, EbookStandaritzed>();
            InitializeComponent();

            Load();
            if (SugerenciasOn)
            {
                Task.Delay(100).ContinueWith((t) => MostrarMensaje("Sugerencia", "Primero se empieza por el orignal para poner solo los párrafos con contenido.Así todas las traducciones/versiones podrán hacerse sin problemas, ya que no se puede añadir contenido."))
                                .ContinueWith((t) => MostrarMensaje("Información", "Pulsa F1 para desactivar/activar las sugerencias al inicio", TimeSpan.FromSeconds(7)))
                                .ContinueWith((t) => MostrarMensaje("Información", "Pulsa F5 para limpiar todas las notificaciones", TimeSpan.FromSeconds(7)))
                                .ContinueWith((t) => MostrarMensaje("Información", "Haz clic encima del texto de la previsualización de spliter para posicionar el párrafo versión o referencia depende de cual se hace clic"));

            }
            Task.Delay(100).ContinueWith((t) => MostrarMensaje("Información", $"Pulsa F2 para {(NotificacionesOn ? "desactivar" : "activar")} las notificaciones", TimeSpan.FromSeconds(7), forceNotification: !NotificacionesOn));
        }
        public bool SugerenciasOn
        {
            get => Properties.Settings.Default.SugerenciasOn;
            set
            {
                Properties.Settings.Default.SugerenciasOn = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool NotificacionesOn
        {
            get => Properties.Settings.Default.NotificacionesOn;
            set
            {
                Properties.Settings.Default.NotificacionesOn = value;
                Properties.Settings.Default.Save();
            }
        }
        private void Load()
        {

            EbookSplited[] ebooksSpited = EbookSplited.GetEbookSpliteds();
            EbookStandaritzed[] ebooksStandaritzed = EbookStandaritzed.GetEbookStandaritzeds();
            SortedList<string, List<EbookSplited>> dic = new SortedList<string, List<EbookSplited>>();


            lstEbookSplited.Items.Clear();

            for (int i = 0; i < ebooksSpited.Length; i++)
            {

                if (!dic.ContainsKey(ebooksSpited[i].OriginalTitle))
                    dic.Add(ebooksSpited[i].OriginalTitle, new List<EbookSplited>());
                dic[ebooksSpited[i].OriginalTitle].Add(ebooksSpited[i]);
            }
            DicStandard.Clear();
            for (int i = 0; i < ebooksStandaritzed.Length; i++)
                if (!Equals(ebooksStandaritzed[i].Version, default) && !DicStandard.ContainsKey(ebooksStandaritzed[i].Version.SaveName))
                    DicStandard.Add(ebooksStandaritzed[i].Version.SaveName, ebooksStandaritzed[i]);

            Group = default;
            foreach (var title in dic)
            {
                Group = new GroupItem(new KeyValuePair<string, IList>(title.Key, title.Value));
                Group.Selected += (s, e) =>
                {
                    if (!Equals(capituloViewer.EbookActual.Version, e.Object))
                    {
                        if (Group != default)
                        {
                            Group.UnselectItem();
                        }
                        Group = s as GroupItem;
                        SetEbookActual(e.Object as EbookSplited);
                    }
                };
                lstEbookSplited.Items.Add(Group);
            }
            if (!Equals(Group, default))
            {
                SetEbookActual(Group.FirstOrDefault as EbookSplited);
            }
        }
        private void SetEbookActual(EbookSplited ebook)
        {
            EbookStandaritzed ebookStandaritzed = GetReference(ebook);
            if (!Equals(ebookStandaritzed, default))
            {

                capituloViewer.EbookActual = ebookStandaritzed;
                UpdateTitle();

            }
        }

        private void UpdateTitle()
        {
            string principio = $"{UnsafeString} {Version}";
            if (!capituloViewer.EbookActual.Finished())
            {
                Title = $"{principio} #Trabajando# {capituloViewer.EbookActual.Version.SaveName}";
            }
            else
            {
                if (!capituloViewer.EbookActual.IsABase)
                    Title = $"{principio} #Finiquitado# {capituloViewer.EbookActual.Version.SaveName}";
                else Title = $"{principio} #Base# {capituloViewer.EbookActual.Version.SaveName}";
            }
        }

        public static EbookStandaritzed GetReference(EbookSplited ebook)
        {
            EbookStandaritzed ebookStandaritzed = default;
            if (!Equals(ebook, default))
            {
                if (!DicStandard.ContainsKey(ebook.SaveName))
                    DicStandard.Add(ebook.SaveName, new EbookStandaritzed(ebook));
                ebookStandaritzed = DicStandard[ebook.SaveName];

            }
            return ebookStandaritzed;
        }

        private void capituloViewer_HasChanges(object sender, EventArgs e)
        {
            UpdateTitle();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Task tAux;

            if (e.Key.Equals(Key.F12))
            {
                UnsafeMode = !UnsafeMode;
                UpdateTitle();
            }
            else if (e.Key.Equals(Key.F1))
            {
                SugerenciasOn = !SugerenciasOn;
                if (LastSugerenciaMode.HasValue)
                    tAux = CerrarMensaje(LastSugerenciaMode.Value);
                else tAux = Task.Delay(1);

                tAux.ContinueWith(t =>
                        MostrarMensaje("Sugerencias al inicio", SugerenciasOn ? "Están activadas" : "Están desactivadas", TimeSpan.FromSeconds(10), NotificationType.Success)
                       .ContinueWith((t) => LastSugerenciaMode = t.Result))
                       .ContinueWith((t) => MostrarMensaje("Información", $"Pulsa F1 para {(SugerenciasOn ? "desactivar" : "activar")} las sugerencias al inicio"));


            }
            else if (e.Key.Equals(Key.F2))
            {
                NotificacionesOn = !NotificacionesOn;
                if (LastNotificacionMode.HasValue)
                    tAux = CerrarMensaje(LastNotificacionMode.Value);
                else tAux = Task.Delay(1);

                tAux.ContinueWith(t =>
                        MostrarMensaje("Notificaciones", NotificacionesOn ? "Están activadas" : "Están desactivadas", TimeSpan.FromSeconds(10), NotificationType.Success, true)
                        .ContinueWith((t) => LastNotificacionMode = t.Result))
                        .ContinueWith((t) => MostrarMensaje("Información", $"Pulsa F2 para {(NotificacionesOn ? "desactivar" : "activar")} las notificaciones", default, NotificationType.Information, true));


            }
            else if (e.Key.Equals(Key.F5))
            {
                _=Notificaciones.CloseAllMessages();
            }
        }
        public async Task<Guid> MostrarMensaje(string title, string content, TimeSpan? time = default, NotificationType tipo = NotificationType.Information, bool forceNotification = false)
        {
            return await Notificaciones.ShowMessage(title, content, tipo, time, nameof(notificationArea), notificacionesOn: () => NotificacionesOn || forceNotification);
        }
        public async Task CerrarMensaje(Guid idMensaje)
        {
            await idMensaje.CloseMessage();
        }

    }

}
