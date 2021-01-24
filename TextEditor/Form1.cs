﻿using System;
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
        private static Bitmap closeImage = new Bitmap(@"C:\programs\Editor\TextEditor\Resources\Close.png");
        private static int currectInd = 0;
        public Form1()
        {
            InitializeComponent();
            MinimumSize = new Size(420, 360);
        }

        private void Form1Load(object sender, EventArgs e)
        {
            tabControl1.Padding = new Point(12, 4);
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


        private void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("new" + tabControl1.TabCount.ToString());
            newPage.Margin = new Padding(3, 3, 3, 3);
            newPage.Padding = new Padding(3, 3, 3, 3);
            tabControl1.TabPages.Add(newPage);
            tabControl1.SelectedIndex = tabControl1.TabCount - 1;
            newPage.Controls.Add(Roflan.CreateNewRichTextBox());
            Text = newPage.Text + " - Notepad+";

        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var newBox = Roflan.CreateNewRichTextBox();
                newBox.Text = File.ReadAllText(openFileDialog1.FileName);
                tabControl1.TabPages[currectInd].Controls.Clear();
                tabControl1.SelectedTab.Controls.Add(newBox);
                tabControl1.SelectedTab.Text = openFileDialog1.FileName.Split('\\')[openFileDialog1.FileName.Split('\\').Length - 1];
                Text = openFileDialog1.FileName + " - Notepad+";

            }

            //var tabPage = tabControl1.TabPages[0];
            //tabPage.Text = "123434343434343433333333333333333333333333333333333";
        }

        private void TabControl1DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = this.tabControl1.TabPages[e.Index];
            var tabRect = this.tabControl1.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);

            e.Graphics.DrawImage(closeImage,
                (tabRect.Right - closeImage.Width),
                tabRect.Top + (tabRect.Height - closeImage.Height) / 2);
            TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                tabRect, tabPage.ForeColor, TextFormatFlags.Left);

        }

        private void TabControl1MouseDown(object sender, MouseEventArgs e)
        {
            for (var i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                var tabRect = this.tabControl1.GetTabRect(i);
                tabRect.Inflate(-2, -2);
                var closeImage = new Bitmap(@"C:\programs\Editor\TextEditor\Resources\Close.png");
                var imageRect = new Rectangle(
                    (tabRect.Right - closeImage.Width),
                    tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                    closeImage.Width,
                    closeImage.Height);
                if (imageRect.Contains(e.Location))
                {
                    this.tabControl1.TabPages.RemoveAt(i);
                    break;
                }
            }
        }

        private void TabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            currectInd = tabControl1.SelectedIndex;
        }
    }



}

