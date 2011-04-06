using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data;
using FirebirdSql.Data.Services;

namespace exprodtoolbox
{
    public partial class EditDBInfo : Form
    {
        public EditDBInfo()
        {
            InitializeComponent();
        }

        private void tbDBPath_DoubleClick(object sender, EventArgs e)
        {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = "gdb"; dlg.Filter = "Interbase/Firebird database (*.gdb;*.fdb)|*.gdb;*.fdb";
                dlg.AddExtension = dlg.CheckFileExists = dlg.CheckPathExists = true;
                if (!cbServer.Text.Equals("localhost"))
                { MessageBox.Show("Указание путей на удалённом сервере должно проходит относительно самого сервера\nС этой целью используйте администраттивные ресурсы вида \\\\server\\disk$"); dlg.InitialDirectory = string.Format("\\\\{0}\\", cbServer.Text); }
                if (dlg.ShowDialog().Equals(DialogResult.OK))
                {
                    if (cbServer.Text.Equals("localhost")) { tbDBPath.Text = dlg.FileName; }
                    else { tbDBPath.Text = dlg.FileName.Replace(string.Format("\\\\{0}\\", cbServer.Text), "").Replace('$', ':'); }
                }

            
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            try
            {
                clDBInfo it = new clDBInfo(tbDBName.Text, cbServer.Text, tbDBPath.Text, tbUser.Text, tbPassword.Text);
                Properties.Settings.Default.Databases.Add(it.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        private void bTEST_Click(object sender, EventArgs e)
        {
            try
            {

                using (FirebirdSql.Data.FirebirdClient.FbConnection fbc = new FirebirdSql.Data.FirebirdClient.FbConnection((new clDBInfo(tbDBName.Text, cbServer.Text, tbDBPath.Text, tbUser.Text, tbPassword.Text)).GetFbConnStr()))
                {
                    fbc.Open();
                    FbServerProperties fbsp = new FbServerProperties();
                        fbsp.ConnectionString = (new clDBInfo(tbDBName.Text, cbServer.Text, tbDBPath.Text, tbUser.Text, tbPassword.Text)).GetFbConnStr();
                        string msg = "";
                        foreach (string s in fbsp.DatabasesInfo.Databases)
                        {
                            msg = msg + "\n" + s;
                        }
                        MessageBox.Show(string.Format("к серверу {0} успешно установлено соединение.\nВсего соединений к серверу БД: {1}\n {2}", cbServer.Text, fbsp.DatabasesInfo.ConnectionCount, msg));
                    
                    
                    fbc.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ServiceOutput(object sender, ServiceOutputEventArgs e)
        {

            MessageBox.Show(e.Message);
            Application.DoEvents();
        }

    }
}
