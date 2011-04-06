using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;

namespace exprodtoolbox
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        private void FolderFind(object sender, EventArgs e)
        {

            if (sender.GetType().Equals(typeof(TextBox)))
            {
              TextBox tb = sender as TextBox;
                  FolderBrowserDialog dlg = new FolderBrowserDialog();
                  dlg.ShowNewFolderButton = false;
                  dlg.SelectedPath = tb.Text;
                  if (dlg.ShowDialog().Equals(DialogResult.Cancel)) return;
                  tb.Text = dlg.SelectedPath;
                  if (!tb.Text.EndsWith("\\")) tb.Text += "\\";//workaround
              }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            tbDBPass.Text = Properties.Settings.Default.DefaultPassword;
            tbDocs.Text = Properties.Settings.Default.DefaultDocs;
            tbTemplates.Text = Properties.Settings.Default.DefaultTemplates;
            tbArchives.Text = Properties.Settings.Default.DefaultArchives;
            cbDetailedLog.Checked = Properties.Settings.Default.DetailedLog;

            //Resources
            ResourceManager rm = Properties.Resources.ResourceManager;
            ResourceSet rs = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);
            System.Collections.IDictionaryEnumerator de = rs.GetEnumerator();
            de.Reset(); 
            Type tmp;
            Byte[] xxx = new Byte[0];
            tmp = xxx.GetType();
            tvResources.BeginUpdate();
            TreeNode root=tvResources.Nodes.Add("РЕСУРСЫ");
            while (de.MoveNext())
            {
                if (root.Nodes.ContainsKey(de.Value.GetType().ToString()))
                {
                    root.Nodes.Find(de.Value.GetType().ToString(), false)[0].Nodes.Add(de.Value.ToString(), de.Key.ToString());
                }
                else
                {
                    root.Nodes.Add(de.Value.GetType().ToString(), de.Value.GetType().ToString()).Nodes.Add(de.Value.ToString(), de.Key.ToString());
                }
            }
            tvResources.Sort();
            tvResources.EndUpdate();
            //MessageBox.Show(txt);

        }

        private void bOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DefaultPassword = tbDBPass.Text;
            Properties.Settings.Default.DefaultDocs = tbDocs.Text;
            Properties.Settings.Default.DefaultTemplates = tbTemplates.Text;
            Properties.Settings.Default.DefaultArchives = tbArchives.Text;
            Properties.Settings.Default.DetailedLog = cbDetailedLog.Checked;
            Properties.Settings.Default.Save();

        }

        private void tbArchives_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format(Properties.Settings.Default.DefaultArchives, ".GBK", "ИМЯБД", DateTime.Today),"Архив пишем в файл на сервере");
        }

        private void tvResources_DoubleClick(object sender, EventArgs e)
        {
            if (Uri.IsWellFormedUriString(tvResources.SelectedNode.Name, UriKind.Absolute))
            {
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = tvResources.SelectedNode.Name;
                prc.Start();
            }
            else
            MessageBox.Show(tvResources.SelectedNode.Name, tvResources.SelectedNode.FullPath);
        }

        


    }
}
