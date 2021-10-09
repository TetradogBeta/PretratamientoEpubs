using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PretractamentDeEpubs.Controls
{
    public interface IItemView
    {
        public int Row
        {
            get;
            set;
        }

        public int Column
        {
            get;set;
        }

        public TimeSpan InitTime
        {
            get;
            set;
        }

        public TimeSpan FinishTime
        {
            get;
            set;
        }
        public UIElement Element { get;  }
    }
}
