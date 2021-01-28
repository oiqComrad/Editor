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
        private static Bitmap closeImage;
        private static int currentInd = 0;
        private static RichTextBox currentRtb;
        private static string currentText = "";
        public Form1()
        {
            InitializeComponent();
            MinimumSize = new Size(420, 360);
        }

        private void Form1Load(object sender, EventArgs e)
        {
            tabControl1.Padding = new Point(12, 4);
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            currentRtb = rtb;
            Text = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\")) + "README.txt" + " - Notepad+";
            tabControl1.SelectedTab.Name = Text;
            closeImage = Properties.Resource.Close;
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
            newPage.Controls.Add(CreateNewRichTextBox());
            tabControl1.SelectedIndex = tabControl1.TabCount - 1;
            Text = newPage.Text + " - Notepad+";
            currentRtb = (RichTextBox)tabControl1.SelectedTab.Controls["rtb"];
            tabControl1.SelectedTab.Name = tabControl1.SelectedTab.Text;

        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentRtb.Text = File.ReadAllText(openFileDialog1.FileName);
                tabControl1.SelectedTab.Text = openFileDialog1.FileName.Split('\\')[openFileDialog1.FileName.Split('\\').Length - 1];
                Text = openFileDialog1.FileName + " - Notepad+";
                tabControl1.SelectedTab.Name = openFileDialog1.FileName;
            }

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
                var imageRect = new Rectangle(
                    (tabRect.Right - closeImage.Width),
                    tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                    closeImage.Width,
                    closeImage.Height);
                if (imageRect.Contains(e.Location))
                {
                    TabPage page = tabControl1.TabPages[i];
                    if (page.Text.Contains("*"))
                    {
                        DialogResult result = MessageBox.Show("Сохранить файл " + page.Text.Substring(0, page.Text.Length - 1) + "?", 
                            "Сохранение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            tabControl1.SelectedIndex = i;
                            SaveToolStripMenuItemClick(sender, e);
                            tabControl1.TabPages.RemoveAt(i);
                        }
                        else if (result == DialogResult.No)
                        {
                            this.tabControl1.TabPages.RemoveAt(i);
                            break;
                        }
                        else break;

                    }
                    else
                    {
                        this.tabControl1.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void TabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            currentRtb = (RichTextBox)tabControl1.SelectedTab.Controls["rtb"];
            currentInd = tabControl1.SelectedIndex;
            if (tabControl1.SelectedTab.Name.Contains(" - Notepad+"))
                Text = tabControl1.SelectedTab.Name;
            else
                Text = tabControl1.SelectedTab.Name + " - Notepad+";
            currentText = currentRtb.Text;

        }

        private void RtbTextChanged(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Text.Contains("*"))
            tabControl1.SelectedTab.Text += "*";
        }

        private RichTextBox CreateNewRichTextBox()
        {
            RichTextBox newBox = new RichTextBox();
            newBox.Dock = DockStyle.Fill;
            newBox.BorderStyle = BorderStyle.None;
            newBox.Margin = new Padding(3, 3, 3, 3);
            newBox.Name = "rtb";
            newBox.TextChanged += RtbTextChanged;
            newBox.WordWrap = false;
            return newBox;
        }

        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Normal text files (*.txt)|*.txt|Rich text files (*.rtf)|*.rtf";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (tabControl1.SelectedTab.Text.Contains("new"))
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog1.FileName, currentRtb.Text);
                    tabControl1.SelectedTab.Text = saveFileDialog1.FileName.Split('\\')[saveFileDialog1.FileName.Split('\\').Length - 1];
                    Text = saveFileDialog1.FileName;
                    tabControl1.SelectedTab.Name = saveFileDialog1.FileName;
                }
            }
            else
            {
                string path = Text.Split('-')[0].Trim();
                File.WriteAllText(path, currentRtb.Text);
                if (tabControl1.SelectedTab.Text.Contains("*") && !tabControl1.SelectedTab.Text.Contains("new"))
                    tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Substring(0, tabControl1.SelectedTab.Text.Length - 1);
            }
        }

        private void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Normal text files (*.txt)|*.txt|Rich text files (*.rtf)|*.rtf";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, currentRtb.Text);
                tabControl1.SelectedTab.Text = saveFileDialog1.FileName.Split('\\')[saveFileDialog1.FileName.Split('\\').Length - 1];
                Text = saveFileDialog1.FileName;
                tabControl1.SelectedTab.Name = saveFileDialog1.FileName;
            }
        }

        private void SaveAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            TabControl.TabPageCollection pages = tabControl1.TabPages;
            foreach (TabPage page in pages)
            {
                RichTextBox tempBox = page.Controls[0] as RichTextBox;
                if (tempBox != null)
                {
                    File.WriteAllText(page.Name, tempBox.Text);
                    page.Text = page.Text.Substring(0, page.Text.Length - 1);
                }
            }
        }

        private void FifteenSecToolStripMenuItemClick(object sender, EventArgs e)
        {
            timer1.Interval = 15000;
        }


        private void FourtySecToolStripMenuItem2Click(object sender, EventArgs e)
        {
            timer1.Interval = 45000;
        }

        private void MinToolStripMenuItemClick(object sender, EventArgs e)
        {
            timer1.Interval = 60000;
        }

        private void FiveMinToolStripMenuItemClick(object sender, EventArgs e)
        {
            timer1.Interval = 300000;
        }

        private void Timer1Tick(object sender, EventArgs e)
        {
            SaveAllToolStripMenuItemClick(sender, e);
        }
    }



}

