using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        // Поле для отрисовки элемента закрытия вкладки.
        private static Bitmap closeImage;
        // Текущий индекс вкладки.
        private static int currentInd = 0;
        // Текущий отображаемый бокс.
        private static RichTextBox currentRtb;
        private static string currentText = "";
        private static bool flag = true;
        public Form1()
        {
            InitializeComponent();
            MinimumSize = new Size(420, 360);
            timer1.Tick += Timer1Tick;
            // Задаю иконки нужным кнопкам.
            toolStripButton1.Image = Properties.Resource.Bold;
            toolStripButton2.Image = Properties.Resource.Italic_Image;
            toolStripButton3.Image = Properties.Resource.underline_Image;
            toolStripButton4.Image = Properties.Resource.strikethrough;
            newToolStripMenuItem.Image = Properties.Resource.new_Image;
            openToolStripMenuItem.Image = Properties.Resource.open_Image;
            saveToolStripMenuItem.Image = Properties.Resource.save_Image;
            newToolStripButton.Image = Properties.Resource.new_Image;
            openToolStripButton.Image = Properties.Resource.open_Image;
            saveToolStripButton.Image = Properties.Resource.save_Image;
            closeImage = Properties.Resource.Close;
        }

        private void Form1Load(object sender, EventArgs e)
        {
            tabControl1.Padding = new Point(12, 4);
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            currentRtb = rtb;
            // Начальный файл.
            tabControl1.SelectedTab.Name = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\"))
                + "README.txt";
            Text = tabControl1.SelectedTab.Name + " - Notepad+";
            currentRtb.Text = File.ReadAllText(tabControl1.SelectedTab.Name);
            tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Substring(0, tabControl1.SelectedTab.Text.Length - 1);
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /// <summary>
        /// Метод создает новую вкладку на tabControl.
        /// Потом новая вкладка выберается текущей, чтобы редактирование проходило в ней.
        /// Новый файл имеет имя new, пока пользователь его не сохранит.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("new" + tabControl1.TabCount.ToString());
            newPage.Margin = new Padding(0, 0, 0, 0);
            newPage.Padding = new Padding(0, 0, 0, 0);
            tabControl1.TabPages.Add(newPage);
            fileType.Text = "New file";
            newPage.Controls.Add(CreateNewRichTextBox());
            tabControl1.SelectedIndex = tabControl1.TabCount - 1;
            Text = newPage.Text + " - Notepad+";
            // Выбор текущего бокса для редактирвония.
            currentRtb = (RichTextBox)tabControl1.SelectedTab.Controls["rtb"];
            tabControl1.SelectedTab.Name = tabControl1.SelectedTab.Text;

        }

        /// <summary>
        /// Метод создает новую вкладку, в которой открывает выбранный файл.
        /// Формат файла сохраняется.
        /// Есди файл с таким именем уже был открыт, то он выберется текущим.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Normal text files (*.txt)|*.txt|Rich text files (*.rtf)|*.rtf";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            try {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // Итерация по текущим вкладкам.
                    TabControl.TabPageCollection pages = tabControl1.TabPages;
                    foreach (TabPage page in pages)
                    {
                        if (page.Name == openFileDialog1.FileName)
                        {
                            tabControl1.SelectedTab = page;
                            return;
                        }
                    }
                    // Создание новой вкладки.
                    NewToolStripMenuItemClick(sender, e);
                    string path = openFileDialog1.FileName;
                    // В зависимости от формата файла.
                    if (Path.GetExtension(path) == ".txt")
                    {
                        currentRtb.Text = File.ReadAllText(path);
                        fileType.Text = "Normal text file";
                    }
                    else
                    {
                        currentRtb.LoadFile(path, RichTextBoxStreamType.RichText);
                        fileType.Text = "Rich text file";
                    }
                    tabControl1.SelectedTab.Text = path.Split('\\')[path.Split('\\').Length - 1];
                    Text = openFileDialog1.FileName + " - Notepad+";
                    tabControl1.SelectedTab.Name = path;
                }
            } catch(Exception ex) { MessageBox.Show(ex.Message); }

        }

        /// <summary>
        /// Отрисовка элемента для закрытия вкладки.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Закрытие вкладки.
        /// Если вкладка была несохранена, пользователю дают возможнсть ее сохранить.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    // Если вкладка не сохранена.
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
        /// <summary>
        /// Метод, который отрабатывает при переключения между вкладками.
        /// При переключении вкладки измениться текущий бокс для редактирования и поля length & lines.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            // Выбор текущего бокса.
            currentRtb = (RichTextBox)tabControl1.SelectedTab.Controls["rtb"];
            currentInd = tabControl1.SelectedIndex;
            if (tabControl1.SelectedTab.Name.Contains(" - Notepad+"))
                Text = tabControl1.SelectedTab.Name;
            else
                Text = tabControl1.SelectedTab.Name + " - Notepad+";
            currentText = currentRtb.Text;
            length.Text = "Length: " + currentRtb.Text.Length.ToString();
            lines.Text = "Lines: " + currentRtb.Text.Count(c => c == '\n').ToString();
            string[] exc = tabControl1.SelectedTab.Text.Split('.');
            if (exc.Length > 1 && exc[1] == "txt")
                fileType.Text = "Normal text file";
            else if (exc.Length > 1 && exc[1] == "rtf")
                fileType.Text = "Rich text file";
            else
                fileType.Text = "New file";

        }

        /// <summary>
        /// Изменение отображения количества символов и строк.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtbTextChanged(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Text.Contains("*"))
                tabControl1.SelectedTab.Text += "*";
            length.Text = "Length: " + currentRtb.Text.Length.ToString();
            lines.Text = "Lines: " + (currentRtb.Text.Count(c => c == '\n') + 1).ToString();
        }
        /// <summary>
        /// Создает новый richtextbox, который потом поместиться в TabPage.
        /// </summary>
        /// <returns></returns>
        private RichTextBox CreateNewRichTextBox()
        {
            RichTextBox newBox = new RichTextBox();
            newBox.Dock = DockStyle.Fill;
            newBox.BorderStyle = BorderStyle.None;
            newBox.Margin = new Padding(3, 3, 3, 3);
            // У всех боксов одно имя - для простоты.
            newBox.Name = "rtb";
            newBox.TextChanged += RtbTextChanged;
            newBox.SelectionChanged += RtbSelectionChanged;
            newBox.MouseUp += RtbMouseUp;
            newBox.ZoomFactor = 1.3f;
            newBox.WordWrap = false;
            return newBox;
        }

        /// <summary>
        /// Сохраняет новый файл в выбранном формате.
        /// Если файл уже существовал, то данные запишуться в файл.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Normal text files (*.txt)|*.txt|Rich text files (*.rtf)|*.rtf";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            try
            {
                // Если файл новый.
                if (!tabControl1.SelectedTab.Text.Contains("txt") && !tabControl1.SelectedTab.Text.Contains("rtf"))
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string path = saveFileDialog1.FileName;
                        if (Path.GetExtension(path) == ".txt")
                        {
                            File.WriteAllText(path, currentRtb.Text);
                            fileType.Text = "Normal text file";
                        }
                        else
                        { currentRtb.SaveFile(path, RichTextBoxStreamType.RichText); fileType.Text = "Rich text file"; }
                        tabControl1.SelectedTab.Text = saveFileDialog1.FileName.Split('\\')[saveFileDialog1.FileName.Split('\\').Length - 1];
                        Text = saveFileDialog1.FileName + " - Notepad+";
                        tabControl1.SelectedTab.Name = saveFileDialog1.FileName;
                    }
                }
                else
                {
                    string path = Text.Split('-')[0].Trim();
                    if (tabControl1.SelectedTab.Text.Contains("txt"))
                        File.WriteAllText(path, currentRtb.Text);
                    else
                        currentRtb.SaveFile(path, RichTextBoxStreamType.RichText);
                    if (tabControl1.SelectedTab.Text.Contains("*") && !tabControl1.SelectedTab.Text.Contains("new"))
                        tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Substring(0, tabControl1.SelectedTab.Text.Length - 1);
                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Сохраняет файл с возможностью изменить его имя и формат.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Normal text files (*.txt)|*.txt|Rich text files (*.rtf)|*.rtf";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            try
            {

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string path = saveFileDialog1.FileName;
                    if (Path.GetExtension(path) == ".txt")
                    {
                        File.WriteAllText(path, currentRtb.Text);
                        fileType.Text = "Normal text file";
                    }
                    else
                    { currentRtb.SaveFile(path, RichTextBoxStreamType.RichText); fileType.Text = "Rich text file"; }
                    tabControl1.SelectedTab.Text = saveFileDialog1.FileName.Split('\\')[saveFileDialog1.FileName.Split('\\').Length - 1];
                    Text = saveFileDialog1.FileName;
                    tabControl1.SelectedTab.Name = saveFileDialog1.FileName;
                }
            } catch(Exception ex) { MessageBox.Show(ex.Message); }
        }


        /// <summary>
        /// Сохраняет все файлы разом.
        /// Если среди открытых файлов есть новый, то будет вызван диалог сохранения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                TabControl.TabPageCollection pages = tabControl1.TabPages;
                // Итерация по всем вкладкам.
                foreach (TabPage page in pages)
                {
                    RichTextBox tempBox = page.Controls[0] as RichTextBox;
                    if (!page.Text.Contains("txt") && !page.Text.Contains("rtf"))
                        SaveNewFile(page, tempBox.Text);
                    else
                    {
                        if (tempBox != null)
                        {
                            if (Path.GetExtension(page.Name) == ".txt")
                            {
                                File.WriteAllText(page.Name, tempBox.Text);
                                fileType.Text = "Normal text file";
                            }
                            else
                            { tempBox.SaveFile(page.Name, RichTextBoxStreamType.RichText); fileType.Text = "Rich text file"; }
                            if (page.Text.Contains("*"))
                                page.Text = page.Text.Substring(0, page.Text.Length - 1);
                        }
                    }
                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Задает таймеру автосохранения нужный интервал.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FifteenSecToolStripMenuItemClick(object sender, EventArgs e)
        {
            timer1.Interval = 15000;
            timer1.Start();
        }

        /// <summary>
        /// Задает таймеру автосохранения нужный интервал.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FourtySecToolStripMenuItem2Click(object sender, EventArgs e)
        {
            timer1.Interval = 45000;
            timer1.Start();
        }
        /// <summary>
        /// Задает таймеру автосохранения нужный интервал.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinToolStripMenuItemClick(object sender, EventArgs e)
        {
            timer1.Interval = 60000;
            timer1.Start();
        }

        /// <summary>
        /// Задает таймеру автосохранения нужный интервал.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FiveMinToolStripMenuItemClick(object sender, EventArgs e)
        {
            timer1.Interval = 300000;
            timer1.Start();
        }

        /// <summary>
        /// При тике таймера вызывается функция автосохранения.
        /// ТОЛЬКО когда среди файлов нет новых.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            bool flag = true;
            TabControl.TabPageCollection pages = tabControl1.TabPages;
            foreach (TabPage page in pages)
            {
                if (!page.Text.Contains("txt") && !page.Text.Contains("txt"))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
                SaveAllToolStripMenuItemClick(sender, e);
        }

        /// <summary>
        /// Метод отдельно сохраняет новый файл.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="text"></param>
        private void SaveNewFile(TabPage page, string text)
        {
            saveFileDialog1.Filter = "Normal text files (*.txt)|*.txt|Rich text files (*.rtf)|*.rtf";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = page.Text;
            try
            {

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string path = saveFileDialog1.FileName;
                    if (Path.GetExtension(path) == ".txt")
                    {
                        File.WriteAllText(path, currentRtb.Text);
                        fileType.Text = "Normal text file";
                    }
                    else
                    { currentRtb.SaveFile(path, RichTextBoxStreamType.RichText); fileType.Text = "Rich text file"; }
                    page.Text = saveFileDialog1.FileName.Split('\\')[saveFileDialog1.FileName.Split('\\').Length - 1];
                    page.Name = saveFileDialog1.FileName;

                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Вызов метода при нажатии на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewToolStripButtonClick(object sender, EventArgs e)
        {
            NewToolStripMenuItemClick(sender, e);
        }

        /// <summary>
        /// Изменение шрифта на жирный.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton1Click(object sender, EventArgs e)
        {
            if (toolStripButton1.Checked == false) toolStripButton1.Checked = true;
            else toolStripButton1.Checked = false;

            if (currentRtb.SelectionFont == null) return;

            FontStyle style = currentRtb.SelectionFont.Style;
            if (currentRtb.SelectionFont.Bold)
            {
                style &= ~FontStyle.Bold;
            }
            else
            {
                style |= FontStyle.Bold;

            }
            currentRtb.SelectionFont = new Font(currentRtb.SelectionFont, style);
        }


        private void RtbSelectionChanged(object sender, EventArgs e)
        {
            if (rtb.SelectionFont != null)
            {
                toolStripButton1.Checked = currentRtb.SelectionFont.Bold;
                toolStripButton2.Checked = currentRtb.SelectionFont.Italic;
                toolStripButton3.Checked = currentRtb.SelectionFont.Underline;
                toolStripButton4.Checked = currentRtb.SelectionFont.Strikeout;
            }
        }

        /// <summary>
        /// Изменение шрифта на курсив.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton2Click(object sender, EventArgs e)
        {
            if (toolStripButton2.Checked == false) toolStripButton2.Checked = true;
            else toolStripButton2.Checked = false;

            if (currentRtb.SelectionFont == null) return;

            FontStyle style = currentRtb.SelectionFont.Style;
            if (currentRtb.SelectionFont.Italic)
            {
                style &= ~FontStyle.Italic;
            }
            else
            {
                style |= FontStyle.Italic;

            }
            currentRtb.SelectionFont = new Font(currentRtb.SelectionFont, style);
        }

        /// <summary>
        /// Изменение шрифта на подчеркнутый.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton3Click(object sender, EventArgs e)
        {
            if (toolStripButton3.Checked == false) toolStripButton3.Checked = true;
            else toolStripButton3.Checked = false;

            if (currentRtb.SelectionFont == null) return;

            FontStyle style = currentRtb.SelectionFont.Style;
            if (currentRtb.SelectionFont.Underline)
            {
                style &= ~FontStyle.Underline;
            }
            else
            {
                style |= FontStyle.Underline;

            }
            currentRtb.SelectionFont = new Font(currentRtb.SelectionFont, style);
        }

        /// <summary>
        /// Изменение шрифта на зачеркнутый.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton4Click(object sender, EventArgs e)
        {
            if (toolStripButton4.Checked == false) toolStripButton4.Checked = true;
            else toolStripButton4.Checked = false;

            if (currentRtb.SelectionFont == null) return;

            FontStyle style = currentRtb.SelectionFont.Style;
            if (currentRtb.SelectionFont.Strikeout)
            {
                style &= ~FontStyle.Strikeout;
            }
            else
            {
                style |= FontStyle.Strikeout;

            }
            currentRtb.SelectionFont = new Font(currentRtb.SelectionFont, style);
        }

        /// <summary>
        /// Метод вызывается при нажатии кнопки закрытия формы.
        /// Перед закрытием пользователю предлагается сохранить все файлы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            TabControl.TabPageCollection pages = tabControl1.TabPages;
            string unsaved = "";
            foreach (TabPage page in pages)
            {
                //RichTextBox tempBox = page.Controls[0] as RichTextBox;
                if (page.Text.Contains("*"))
                    unsaved += page.Text + "\n";
            }
            if (unsaved.Length > 0)
            {
                DialogResult result = MessageBox.Show("Есть несохраненённые файлы:\n" + unsaved + "Сохранить?", 
                    "Сохранение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveAllToolStripMenuItemClick(sender, e);
                    Application.Exit();
                }
                else if (result == DialogResult.No)
                    Application.Exit();
                else { flag = false; return; }
            }
            Application.Exit();
        }

        /// <summary>
        /// Изменение шрифта в боксе.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Font oldFont = Font;
                DialogResult res = fontDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    FontApply(currentRtb, e);
                }
                else if (res == DialogResult.Cancel)
                {
                    this.Font = oldFont;

                    foreach (Control containedControl in rtb.Controls)
                    {
                        containedControl.Font = oldFont;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Измение шрифта в каждом компоненте бокса.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontApply(object sender, EventArgs e)
        {
            fontDialog1.FontMustExist = true;

            currentRtb.Font = fontDialog1.Font;
            currentRtb.ForeColor = fontDialog1.Color;

            foreach (Control containedControl in currentRtb.Controls)
            {
                containedControl.Font = fontDialog1.Font;
            }
        }

        /// <summary>
        ///  Вызов контекстного меню.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RtbMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                currentRtb.ContextMenuStrip = contextMenuStrip1;
            }
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentRtb.SelectAll();
        }

        private void CutToolStripMenuItemClick(object sender, EventArgs e)
        {
            currentRtb.Cut();

        }

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            currentRtb.Copy();
        }

        private void PasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            currentRtb.Paste();
        }

        /// <summary>
        /// Вызов метода при нажатии на кнопку закрытия на форме.
        /// Почему-то я не смог связать этот метод с событием из другого.
        /// Поэтому пришлось два раза прописывать метод закрытия формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1FormClosing(object sender, FormClosingEventArgs e)
        {
            TabControl.TabPageCollection pages = tabControl1.TabPages;
            string unsaved = "";
            foreach (TabPage page in pages)
            {
                //RichTextBox tempBox = page.Controls[0] as RichTextBox;
                if (page.Text.Contains("*"))
                    unsaved += page.Text + "\n";
            }
            if (unsaved.Length > 0)
            {
                DialogResult result = MessageBox.Show("Есть несохраненённые файлы:\n" + unsaved + "Сохранить?",
                    "Сохранение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveAllToolStripMenuItemClick(sender, e);
                    Application.Exit();
                }
                else if (result == DialogResult.No)
                    Application.Exit();
                else { e.Cancel = true; return; }
            }
            Application.Exit();
        }
        /// <summary>
        /// Изменение темы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LightToolStripMenuItemClick(object sender, EventArgs e)
        {
            currentRtb.BackColor = Color.White;
            currentRtb.ForeColor = Color.Black;
        }
        /// <summary>
        /// Изменение темы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DarkToolStripMenuItemClick(object sender, EventArgs e)
        {
            currentRtb.BackColor = Color.FromArgb(28, 28, 28);
            currentRtb.ForeColor = Color.White;
        }
        /// <summary>
        /// Изменение темы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlueToolStripMenuItemClick(object sender, EventArgs e)
        {
            currentRtb.BackColor = Color.FromArgb(217, 247, 239);

            currentRtb.ForeColor = Color.Black;
        }
    }
}


