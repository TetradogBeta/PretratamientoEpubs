using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Gabriel.Cat.S.Extension
{
    public static class RichTextBoxExtension
    {
        public static Point GetSelectionRange(this RichTextBox rtb)
        {
            int inicio = -1;
            int fin = -1;
           
            if (rtb.IsSelectionActive)
            {
                fin =rtb.Document.ContentStart.GetOffsetToPosition(rtb.Selection.End)-2;
                inicio =fin - rtb.Selection.Text.Length;


            }


            return new Point(inicio, fin);
        }
    }
}
