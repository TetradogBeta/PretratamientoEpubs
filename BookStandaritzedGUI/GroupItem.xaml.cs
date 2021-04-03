using Gabriel.Cat.S.Extension;
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
using System.Linq;

namespace BookStandaritzedGUI
{
    /// <summary>
    /// Lógica de interacción para GroupItem.xaml
    /// </summary>
    public partial class GroupItem : UserControl
    {
        public event EventHandler<ItemSelectedEventArgs> Selected;
        public GroupItem()
        {
            InitializeComponent();
        }

        public GroupItem(KeyValuePair<string,IList<object>> objs) : this()
        {
            tbGroupName.Text = objs.Key;
            lstItems.Items.AddRange(objs.Value);
        }
        public object FirstOrDefault => Items.Count > 0 ? Items[0] : default;
        public ItemCollection Items => lstItems.Items;
        public object SelectedItem
        {
            get => lstItems.SelectedItem;
            set=> lstItems.SelectedItem = value;
        }

        public void UnselectItem()
        {
            lstItems.SelectedIndex = -1;
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Selected != null && !Equals(lstItems.SelectedItem,default))
            {
                Selected(this, new ItemSelectedEventArgs() { Object = lstItems.SelectedItem });
            }
        }

        private void tbGroupName_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            const double INVISIBLE = 0;
            const int HEIGHT_ITEM = 20;
            lstItems.Height = lstItems.Height == INVISIBLE ? HEIGHT_ITEM * Items.Count:INVISIBLE ;
        }
    }
    public class ItemSelectedEventArgs : EventArgs
    {
        public object Object { get; set; }
    }
}
