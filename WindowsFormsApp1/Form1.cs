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
        private double ratio1;
        private double ratio2;
        private string path1;
        private string path2;

        
            
        public Form1()
        {
            InitializeComponent();
            MinimumSize = new Size(800, 600);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //autosize
            int listSize = (ClientSize.Width - 74) / 2;
            listView1.Width = listSize;
            listView2.Width = listSize;
            listView2.Left = listView1.Right + 50;
            label2.Left = listView2.Left;
            label1.Left = listView1.Left;

            SizeChanged += Form1_SizeChanged;

            //left list
            int list0Width = listView1.ClientSize.Width;
            listView1.Columns[0].Width = list0Width * 3 / 4;
            listView1.Columns[1].Width = list0Width * 1 / 4;
            ratio1 = (double)listView1.Columns[0].Width / listView1.Width;
            label1.Left = listView1.Left; 
            ShowDIR(@"C:\", listView1);
            path1 = @"C:\";

            listView1.ColumnWidthChanging += ListView_ColumnWidthChanging;
            listView1.ColumnWidthChanged += ListView1_ColumnWidthChanged;
            listView1.ItemActivate += ListView_ItemActivate;


            //right list
            int list1Width = listView2.ClientSize.Width;
            listView2.Columns[0].Width = list1Width * 3 / 4;
            listView2.Columns[1].Width = list1Width * 1 / 4;
            ratio2 = (double)listView2.Columns[0].Width / listView2.Width;
            label2.Left = listView2.Left;
            ShowDIR(@"D:\", listView2);
            path2 = @"D:\";

            listView2.ItemActivate += ListView_ItemActivate;
            listView2.ColumnWidthChanging += ListView_ColumnWidthChanging;
            listView2.ColumnWidthChanged += ListView2_ColumnWidthChanged;
            
        }
        

        private void ListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (e.ColumnIndex == 1)
            {
                e.Cancel = true;
                e.NewWidth = listView.Columns[1].Width;
            }
            if (e.NewWidth < minimumColumnWidth)
            {
                e.Cancel = true;
                e.NewWidth = minimumColumnWidth;
            }
            if (e.NewWidth > listView.ClientSize.Width - minimumColumnWidth)
            {
                e.Cancel = true;
                e.NewWidth = listView.ClientSize.Width - minimumColumnWidth;
            }
        }
        private void ListView1_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                listView1.Columns[1].Width = listView1.ClientSize.Width - listView1.Columns[0].Width;
                ratio1 = (double)listView1.Columns[0].Width / listView1.Width;
            }
        }
        
        private void ListView2_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                listView2.Columns[1].Width = listView2.ClientSize.Width - listView2.Columns[0].Width;
                ratio2 = (double)listView2.Columns[0].Width / listView2.Width;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            int listSize = (ClientSize.Width - 74) / 2;
            listView1.Width = listSize;
            listView2.Width = listSize;
            listView2.Left = listView1.Right + 50;
            label2.Left = listView2.Left;
            

            if ((int)(listSize * ratio1) >= minimumColumnWidth)
            {
                listView1.Columns[0].Width = (int)(listSize * ratio1);
            }
            else
            {
                listView1.Columns[0].Width = minimumColumnWidth;
            }
            listView1.Columns[1].Width = listSize - listView1.Columns[0].Width - 5;
        
            if ((int)(listSize * ratio2) >= minimumColumnWidth)
            {
                listView2.Columns[0].Width = (int)(listSize * ratio2);
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
            listView.Items.Clear();
            if (listView == listView1)
            {
                
                label1.Text = "Path: " + directoryPath;
                path1 = directoryPath;
            }
            else if (listView == listView2)
            {
                label2.Text = "Path: " + directoryPath;
                path2 = directoryPath;
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
            if (listView == listView1)
            {
                itemPath = Path.Combine(path1, itemName);
            }
            else
            {
                itemPath = Path.Combine(path2, itemName);
            }

            if (Directory.Exists(itemPath))
            {
                
                try
                {
                    if (itemName == "..")
                    {
                        if(listView == listView1)
                        {
                            itemPath = Directory.GetParent(path1).FullName;
                        }
                        else
                        {
                            itemPath = Directory.GetParent(path2).FullName;
                        }
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
    }
}
