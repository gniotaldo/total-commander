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
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private int minimumColumnWidth = 40;
        private string path1;
        private string path2;
        private int clickedColumnIndex = -1;
        private int listView1sorted = 0;
        private int listView2sorted = 0;

        public Form1()
        {
            InitializeComponent();
            MinimumSize = new Size(800, 600);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Autosize();
            LoadDrives(comboBox1);
            LoadDrives(comboBox2);
            LoadDIR(listView1);
            LoadDIR(listView2);

            comboBox1.SelectedIndexChanged += comboBox_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += comboBox_SelectedIndexChanged;
            SizeChanged += Form1_SizeChanged;
            listView1.ItemActivate += ListView_ItemActivate;
            listView1.ColumnWidthChanging += ListView_ColumnWidthChanging;
            listView1.ColumnWidthChanged += ListView_ColumnWidthChanged;
            listView2.ItemActivate += ListView_ItemActivate;
            listView2.ColumnWidthChanging += ListView_ColumnWidthChanging;
            listView2.ColumnWidthChanged += ListView_ColumnWidthChanged;
            listView1.ColumnClick += ListView_ColumnClick;
            listView2.ColumnClick += ListView_ColumnClick;
        }

        private void Autosize()
        {
            int listSize = (ClientSize.Width - 74) / 2;
            listView1.Width = listSize;
            listView2.Width = listSize;
            listView1.Left = 12;
            listView2.Left = listView1.Right + 50;
            label2.Left = listView2.Left;
            label1.Left = listView1.Left;
            label1.MaximumSize = new Size(listSize - 41, 15);
            label2.MaximumSize = new Size(listSize - 41, 15);
            comboBox1.Location = new Point(listView1.Right - comboBox1.Width, comboBox1.Top);
            comboBox2.Location = new Point(listView2.Right - comboBox2.Width, comboBox2.Top);
        }

        private void LoadDIR(ListView listView)
        {
            int listWidth = listView.ClientSize.Width;
            listView.Columns[0].Width = listWidth * 3 / 4;
            listView.Columns[1].Width = listWidth * 1 / 4;
            listView.Tag = (double)listView.Columns[0].Width / listView.Width;
            ShowDIR(comboBox1.SelectedItem.ToString(), listView);
        }
        private void LoadDrives(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                comboBox.Items.Add(drive.Name);
            }
            comboBox.SelectedIndex = 0;
        }


        private void ListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (e.ColumnIndex == 1)
            {
                e.Cancel = true;
                e.NewWidth = listView.Columns[1].Width;
            }
            else if (e.NewWidth < minimumColumnWidth)
            {
                e.Cancel = true;
                e.NewWidth = minimumColumnWidth;
            }
            else if (e.NewWidth > listView.ClientSize.Width - minimumColumnWidth)
            {
                e.Cancel = true;
                e.NewWidth = listView.ClientSize.Width - minimumColumnWidth;
            }
        }
        private void ListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (e.ColumnIndex == 0)
            {
                listView.Columns[1].Width = listView.ClientSize.Width - listView.Columns[0].Width;
                listView.Tag = (double)listView.Columns[0].Width / listView.Width;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            int listSize = (ClientSize.Width - 74) / 2;
            listView1.Width = listSize;
            listView2.Width = listSize;
            listView2.Left = listView1.Right + 50;
            label2.Left = listView2.Left;
            comboBox1.Location = new Point(listView1.Right - comboBox1.Width, comboBox1.Top);
            comboBox2.Location = new Point(listView2.Right - comboBox2.Width, comboBox2.Top);
            label1.MaximumSize = new Size(listSize - 41, 15);
            label2.MaximumSize = new Size(listSize - 41, 15);


            if ((int)(listSize * (double)listView1.Tag) >= minimumColumnWidth)
            {
                listView1.Columns[0].Width = (int)(listSize * (double)listView1.Tag);
            }
            else
            {
                listView1.Columns[0].Width = minimumColumnWidth;
            }
            listView1.Columns[1].Width = listSize - listView1.Columns[0].Width - 5;

            if ((int)(listSize * (double)listView2.Tag) >= minimumColumnWidth)
            {
                listView2.Columns[0].Width = (int)(listSize * (double)listView2.Tag);
            }
            else
            {
                listView2.Columns[0].Width = minimumColumnWidth;
            }
            listView2.Columns[1].Width = listSize - listView2.Columns[0].Width - 5;
        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       
        private void ShowDIR(string directoryPath, ListView listView)
        {
            listView1sorted = 0;
            listView2sorted = 0;
            listView.Items.Clear();
            if (listView == listView1)
            {
                label1.Text = "Path: " + directoryPath;
                path1 = directoryPath;
                toolTip1.SetToolTip(label1, directoryPath);
            }
            else
            {
                label2.Text = "Path: " + directoryPath;
                path2 = directoryPath;
                toolTip2.SetToolTip(label2, directoryPath);
            }


            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            if (directoryInfo.Parent != null)
            {
                ListViewItem parentItem = new ListViewItem("..");
                parentItem.SubItems.Add("");
                parentItem.ImageKey = "dir";
                listView.Items.Add(parentItem);
            }


            foreach (var dir in directoryInfo.GetDirectories())
            {
                ListViewItem item = new ListViewItem(dir.Name);
                item.ImageKey = "dir";
                item.SubItems.Add(dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                listView.Items.Add(item);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                ListViewItem item = new ListViewItem(file.Name);
                item.ImageKey = "file";
                item.SubItems.Add(file.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
                listView.Items.Add(item);
            }
        }
        private void ListView_ItemActivate(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewItem item = listView.SelectedItems[0];
            string itemName = item.Text;
            string itemPath;
            itemPath = listView == listView1 ? Path.Combine(path1, itemName): Path.Combine(path2, itemName);

            if (Directory.Exists(itemPath))
            {
                try
                {
                    if (itemName == "..")
                    {
                        itemPath = listView == listView1 ? Directory.GetParent(path1).FullName : Directory.GetParent(path2).FullName;
                    }
                    ShowDIR(itemPath, listView);
                }
                catch (Exception error)
                {
                    MessageBox.Show("ERROR: " + error.Message);
                    ShowDIR(Directory.GetParent(itemPath).FullName, listView);
                }

            }
            else if (File.Exists(itemPath))
            {
                Process.Start(itemPath);
            }
        }

        
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedDrive = comboBox.SelectedItem.ToString();

            if (comboBox == comboBox1)
            {
                path1 = selectedDrive;
                ShowDIR(path1, listView1);
            }
            else if (comboBox == comboBox2)
            {
                path2 = selectedDrive;
                ShowDIR(path2, listView2);
            }
        }
        private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView listView = (ListView)sender;
            clickedColumnIndex = e.Column;
            SortList(listView);
        }

        private void SortList(ListView listView)
        {
            ListViewItem parent = null;
            int itemCount = listView.Items.Count;
            int sortedBy = listView == listView1 ? listView1sorted : listView2sorted;
            if (itemCount <= 1) return;
            if (listView.Items[0].Text == "..")
            {
                parent = listView.Items[0];
                listView.Items.Remove(parent);
                itemCount -= 1;
            }
            if (clickedColumnIndex == sortedBy)
            {
                for (int i = itemCount - 2; i >= 0; i--)
                {
                    ListViewItem item = listView.Items[i];
                    listView.Items.Remove(item);
                    listView.Items.Add(item);
                }
            }
            else
            {
                for (int i = 0; i < itemCount - 1; i++)
                {
                    for (int j = 0; j < itemCount - 1 - i; j++)
                    {
                        ListViewItem currentItem = listView.Items[j];
                        ListViewItem nextItem = listView.Items[j + 1];
                        string currentText = currentItem.SubItems[clickedColumnIndex].Text;
                        string nextText = nextItem.SubItems[clickedColumnIndex].Text;
                        if (currentItem.ImageKey == nextItem.ImageKey)
                        {
                            if (string.Compare(currentText, nextText) > 0)
                            {
                                listView.Items.Remove(currentItem);
                                listView.Items.Insert(j + 1, currentItem);
                            }
                        }
                    }
                }
            }
            if (listView == listView1)
            {
                listView1sorted = clickedColumnIndex;
            }
            else
            {
                listView2sorted = clickedColumnIndex;
            }
            if (parent != null)
            {
                listView.Items.Insert(0, parent);
            }
            
        }
    }
}
