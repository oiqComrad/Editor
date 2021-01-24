using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace TextEditor
{
    class Roflan
    {
        public static RichTextBox CreateNewRichTextBox()
        {
            RichTextBox newBox = new RichTextBox();
            newBox.Dock = DockStyle.Fill;
            newBox.BorderStyle = BorderStyle.FixedSingle;
            newBox.Margin = new Padding(3, 3, 3, 3);
            return newBox;
        }
    }
}
