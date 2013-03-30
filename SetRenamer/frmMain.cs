using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApplication1
{
    public partial class frmMain : Form
    {
        static string mtgDirectory;
        static string[] imagePath;
        public frmMain()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            listView1.Items.Clear();
            openFileDialog1.Filter = "OCTGN XML Image List (*.rels) | *.rels";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mtgDirectory = openFileDialog1.FileName;
                listView1.BeginUpdate();
                using (XmlReader reader = XmlReader.Create(openFileDialog1.FileName))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "Relationship":
                                    reader.MoveToFirstAttribute();
                                    ListViewItem subitem = new ListViewItem();
                                    subitem.Text = reader.Value;
                                    reader.MoveToNextAttribute();
                                    Guid id = new Guid();
                                    Guid.TryParseExact(reader.Value.Substring(1), "N", out id);
                                    subitem.SubItems.Add(id.ToString());
                                    listView1.Items.Add(subitem);
                                    break;
                            }
                        }
                    }
                }
                listView1.EndUpdate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = listView1.Items.Count;
            progressBar1.Minimum = 0;
            progressBar1.Step = 1;
            foreach (ListViewItem item in listView1.Items)
            {
                string s = System.IO.Directory.GetParent(mtgDirectory).FullName;
                imagePath = item.Text.Split('/');
                s = System.IO.Directory.GetParent(s).FullName + "\\" + imagePath[1] + "\\";
                string oldName = s + imagePath[2];
                string extension = System.IO.Path.GetExtension(oldName);
                string newName = s + item.SubItems[1].Text + extension;
                try
                {
                    System.IO.File.Move(oldName, newName);
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                progressBar1.PerformStep();
            }
        }
    }
}
