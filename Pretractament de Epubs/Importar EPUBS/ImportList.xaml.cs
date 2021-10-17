using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace ImportarEPUBS
{
    /// <summary>
    /// Lógica de interacción para ImportList.xaml
    /// </summary>
    public partial class ImportList : UserControl
    {
        public event EventHandler? Selected;
        public ImportList()
        {
            Files = new SortedList<string, string>();
            InitializeComponent();
        }
        public object SelectedItem => lstEpubs.SelectedItem;

        SortedList<string,string> Files { get; set; }

        public void Remove(string? file=default)
        {
            if (Equals(file, default)){
                file = SelectedItem.ToString();
            }

            if (!Equals(file, default) && Files.ContainsKey(file))
            {
                lstEpubs.SelectionChanged -= lstEpubs_SelectionChanged;
                lstEpubs.Items.Remove(file);
                lstEpubs.SelectionChanged += lstEpubs_SelectionChanged;
                Files.Remove(file);
            }
        }
        public void Clear()
        {
            Files.Clear();
            lstEpubs.Items.Clear();
        }

        private void btnImportar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog opnFileDialog = new OpenFileDialog();
            opnFileDialog.CheckFileExists = true;
            opnFileDialog.CheckPathExists = true;

            opnFileDialog.Filter = "EPUB|*.epub";
            opnFileDialog.Multiselect = true;

            if (opnFileDialog.ShowDialog().GetValueOrDefault())
            {
                foreach(string file in opnFileDialog.FileNames)
                {
                    if (!Files.ContainsKey(file))
                    {
                        Files.Add(file, file);
                        lstEpubs.Items.Add(file);
                    }
                }
            }
        }

        private void lstEpubs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Selected != null)
            {
                Selected(this, new EventArgs());
            }
        }
    }
}
