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
    public partial class Form1 : Form
    {
        private static int count = 1;
        public Form1()
        {
            InitializeComponent();
            MinimumSize = new Size(420, 360);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            //tabControl1.DrawItem += tabControl1_DrawItem();
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


        private void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("new" + count++.ToString());
            newPage.Margin = new Padding(3, 3, 3, 3);
            newPage.Padding = new Padding(3, 3, 3, 3);
            tabControl1.TabPages.Add(newPage);
            tabControl1.SelectedIndex = tabControl1.TabCount - 1;
            RichTextBox newBox = new RichTextBox();
            newPage.Controls.Add(newBox);
            newBox.Dock = DockStyle.Fill;
            newBox.BorderStyle = BorderStyle.FixedSingle;
            newBox.Margin = new Padding(3, 3, 3, 3);
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                richTextBox1.Text = File.ReadAllText(openFileDialog1.FileName);
        }
    }



}

