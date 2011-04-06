using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using FirebirdSql.Data.FirebirdClient;
using System.Threading;
using FirebirdSql.Data.Isql;
using System.IO;
using FirebirdSql.Data.Services;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Net;

namespace exprodtoolbox
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Вычисляем md5 hash параметра.
        /// </summary>
        /// <param name="input">UTF8 строка для которой считаем хэш</param>
        /// <returns>md5 hash in HEX</returns>
        public static string GetMD5Hash(string input)
        {
            return GetMD5Hash(System.Text.Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Вычисляем md5 hash параметра.
        /// </summary>
        /// <param name="input">Массив байтов для которого считаем хэш</param>
        /// <returns>md5 hash in HEX</returns>
        public static string GetMD5Hash(byte[] input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = input;
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        /// <summary>
        /// Вычисляем md5 hash параметра.
        /// </summary>
        /// <param name="input">Поток для которого считаем хэш</param>
        /// <returns>md5 hash in HEX</returns>
        public static string GetMD5Hash(Stream input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs;
            bs = x.ComputeHash(input);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }        
        [DllImport("shell32.dll")]

        static extern int ShellAbout(IntPtr hWnd, string szApp, string szOtherStuff, IntPtr hIcon);


        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string capt;

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                capt = ad.CurrentVersion.ToString();
            }
            else capt = Assembly.GetExecutingAssembly().GetName().Version.ToString() + " (local)";
            capt +=
#if (DEBUG)
 " DEBUG";
            ShellAbout(this.Handle, "ПК СП Toolbox " + capt, string.Format("Волшебное число сегодня = {0}.", GetMD5Hash(DateTime.Now.ToString()),"Программа распространяется \"as is\", т.е. Вы используете ее на свой страх и риск."), IntPtr.Zero);
#else
 " RELEASE";
            ShellAbout(this.Handle, "ПК СП Toolbox " + capt, "Программа распространяется \"as is\", т.е. Вы используете ее на свой страх и риск. Архивируете базы перед применением!", IntPtr.Zero);
#endif
            //string text1="Программа распространяется \"as is\", т.е. Вы используете ее на свой страх и риск. Разработчик не несет ответственности за возможные последствия пользования программой, в том числе - потерю данных по неосторожности пользователя или вследствие несовершенства реализации программы. Разработчик, в свою очередь, гарантирует, что никакие Ваши личные данные не передаются и не будут переданы третьему лицу, украдены или другим образом использованы не по назначению.";
            //ShellAbout(this.Handle, "ПК СП Toolbox " + capt, string.Format("{1}\tВолшебное число сегодня нет.", GetMD5Hash(DateTime.Now.ToString()),"Программа распространяется \"as is\", т.е. Вы используете ее на свой страх и риск."), IntPtr.Zero);
            //Clipboard.SetText(AddCSVScriptMaker(@"c:\!\Шаблоны Арест.csv", "S_TEMPLATES"));
            //Clipboard.SetText(AddCSVScriptMaker(@"c:\!\Метки.csv", "PRN_METKI"));
        }

        private void автоматическиИзExprodlstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Укажите папку с установленным ПК \"СП\"\n(требуются ex_prod.lst и baseparam.txt)";
            dlg.ShowNewFolderButton = false;
            dlg.SelectedPath = Properties.Settings.Default.LastPath;
            if (dlg.ShowDialog().Equals(DialogResult.Cancel)) return;
            Properties.Settings.Default.LastPath = dlg.SelectedPath;
            Properties.Settings.Default.Save();
            if (!File.Exists(dlg.SelectedPath + @"\ex_prod.lst")) { MessageBox.Show("ex_prod.lst не найден в выбранной папке!"); return; }
            if (!File.Exists(dlg.SelectedPath + @"\baseparam.txt")) { MessageBox.Show("baseparam.txt не найден в выбранной папке!"); return; }
            List<clDBInfo> tmp = clDBInfo.ReadExProdLST(dlg.SelectedPath);
            if (tmp == null) return;
            if (Properties.Settings.Default.Databases.Count > 0)
            {
                if (MessageBox.Show("Старый список чистим?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
                {
                    Properties.Settings.Default.Databases.Clear();
                }
            }
            foreach (clDBInfo it in tmp)
            {
                Properties.Settings.Default.Databases.Add(it.ToString());
            }
            Properties.Settings.Default.Save();
            UpdateMenu();
            MessageBox.Show(string.Format("Загружено {0} баз(а)(ы)", tmp.Count));
        }

        private void DBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem tmp = sender as ToolStripMenuItem;
                toolStripStatusLabel1.Text = tmp.ToolTipText;
            }
            catch (Exception)
            {

            }

        }

        private void DBKillStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem tmp = sender as ToolStripMenuItem;
                string deadmen = tmp.Text + "|" + tmp.ToolTipText;
                Properties.Settings.Default.Databases.Remove(deadmen);
                UpdateMenu();
                Properties.Settings.Default.Save();

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }



        private void UpdateMenu()
        {
            List<clDBInfo> tmp = clDBInfo.ReadFromProp(Properties.Settings.Default.Databases);
            ToolStripMenuItem tsmi;
            dbTSMI.DropDownItems.Clear();
            dbKillTSMI.DropDownItems.Clear();
            foreach (clDBInfo it in tmp)
            {
                tsmi = new ToolStripMenuItem(it.Name);
                tsmi.Click += DBToolStripMenuItem_Click;
                tsmi.CheckOnClick = true;
                tsmi.ToolTipText = it.GetFbConnStr();
                dbTSMI.DropDownItems.Add(tsmi);

                tsmi = new ToolStripMenuItem(it.Name);
                tsmi.Click += DBKillStripMenuItem_Click;
                tsmi.ToolTipText = it.GetFbConnStr();
                dbKillTSMI.DropDownItems.Add(tsmi);

            }

        }


        public void Application_CheckErrorReport()
        {
            if ((File.Exists(Properties.Settings.Default.ErrorLogName)) && (MessageBox.Show("Send error report?", "error report found!", MessageBoxButtons.YesNo).Equals(DialogResult.Yes)))
            {
                try
                {
                    if (ThreadExceptionHandler.SendErrorMessage(File.ReadAllText(Properties.Settings.Default.ErrorLogName)))
                    {
                        MessageBox.Show("OK");
                        File.Delete(Properties.Settings.Default.ErrorLogName);
                    }
                    else MessageBox.Show("Отправка отложена");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Exception caught.");
                }
            }
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
            //error reporting!
            if (Properties.Settings.Default.FirstRun)
            {
                Properties.Settings.Default.FirstRun = !ThreadExceptionHandler.SendErrorMessage("First run <NOT A ERROR>");
                Properties.Settings.Default.Save();

            }
            Application_CheckErrorReport();

            UpdateMenu();
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите очистить список баз?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2).Equals(DialogResult.Yes))
            {
                Properties.Settings.Default.Databases.Clear();
                Properties.Settings.Default.Save();
                UpdateMenu();
            }
        }


        private void PerformOperations(string resourcename, string resourceif = null, bool format = false)
        {
            ResourceManager rm = Properties.Resources.ResourceManager;
            string script = rm.GetString(resourcename);
            string ifscript = !string.IsNullOrEmpty(resourceif) ? rm.GetString(resourceif) : null;
            //0 - password
            //1 - docs
            //2 - templates
            //3 - 
            if (format)
            {
                script = string.Format(script, Properties.Settings.Default.DefaultPassword, Properties.Settings.Default.DefaultDocs, Properties.Settings.Default.DefaultTemplates);
                ifscript = string.IsNullOrEmpty(ifscript) ? null : string.Format(ifscript, Properties.Settings.Default.DefaultPassword, Properties.Settings.Default.DefaultDocs, Properties.Settings.Default.DefaultTemplates);
            }
            if (string.IsNullOrEmpty(script)) return;
            MainMenu.Enabled = false;
            //Определяемя с базами данных
            int i = 0;
            pbDB.Maximum = CheckedCount();
            pbDB.Value = 0;
            pbDB.Step = 1;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {

                if (it.Checked)
                {
                    lCurrentDB.Text = string.Format("{0}. {1}", (++i), it.Text); pbDB.PerformStep();
                    //////
                    PutLog(string.Format("Строчка соединения {0}", it.ToolTipText));
                    pbDB.Update();

                    DoDirtyWork(it.ToolTipText, script, ifscript);
                    toolStripStatusLabel1.Text = it.Text + " OK";
                    Application.DoEvents();
                }
            }


            pbDB.Maximum = CheckedCount();
            MainMenu.Enabled = true;
        }

        private void PerformScript(string script, bool format = false)
        {
            //0 - password
            //1 - docs
            //2 - templates
            //3 - 
            if (format)
            {
                script = string.Format(script, Properties.Settings.Default.DefaultPassword, Properties.Settings.Default.DefaultDocs, Properties.Settings.Default.DefaultTemplates);
            }
            if (string.IsNullOrEmpty(script)) return;
            MainMenu.Enabled = false;
            //Определяемя с базами данных
            int i = 0;
            pbDB.Maximum = CheckedCount();
            pbDB.Value = 0;
            pbDB.Step = 1;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {

                if (it.Checked)
                {
                    lCurrentDB.Text = string.Format("{0}. {1}", (++i), it.Text); pbDB.PerformStep();
                    //////
                    PutLog(string.Format("Строчка соединения {0}", it.ToolTipText));
                    pbDB.Update();

                    DoDirtyWork(it.ToolTipText, script);
                    toolStripStatusLabel1.Text = it.Text + " OK";
                    Application.DoEvents();
                }
            }


            pbDB.Maximum = CheckedCount();
            MainMenu.Enabled = true;
        }


        void fbe_CommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            if (Properties.Settings.Default.DetailedLog) PutLog(string.Format("{0} {1}", e.StatementType.ToString(), e.CommandText));
        }

        private bool DoDirtyWork(string db, StreamReader sr)
        {
            FbScript script = new FbScript(sr);
            script.Parse();
            FbConnectionStringBuilder fbcs = new FbConnectionStringBuilder(db);
            fbcs.Dialect = 1;
            // execute the SQL script
            using (FbConnection c = new FbConnection(fbcs.ConnectionString))
            {
                c.Open();
                FbBatchExecution fbe = new FbBatchExecution(c);
                fbe.CommandExecuted += new CommandExecutedEventHandler(fbe_CommandExecuted);
                foreach (string cmd in script.Results)
                {
                    if (!cmd.Contains("commit work"))
                        fbe.SqlStatements.Add(cmd);
                }
                try
                {
                    fbe.Execute();
                }
                catch (Exception ex)
                {
                    PutLog(string.Format("Work Error: {0}", ex.Message));
                    ThreadExceptionHandler.SendErrorMessage(string.Format("Work Error: {0}", ex.Message));
                    c.Close(); 
                    return false;
                }
                c.Close(); while (c.State != ConnectionState.Closed) ;
                FbConnection.ClearAllPools();

            }

            return true;
        }


        private bool DoDirtyWork(string db, string script, string ifscript = null)
        {

            try
            {


                using (FbConnection c = new FbConnection(db))
                {
                    c.Open();
                    bool doit = string.IsNullOrEmpty(ifscript);
                    int cnterr = 0;
                    if (!doit)
                    {
                        FbCommand test = new FbCommand(ifscript, c);

                        doit = !(((cnterr = (int)test.ExecuteScalar())).Equals(0));
                    }
                    if (doit)
                    {
                        if (cnterr != 0) PutLog(string.Format("Выявлено ошибок: {0}", cnterr));
                        pbWork.Value = 0;
                        pbWork.Step = 1;
                        script = script.Replace('\n', ' ');
                        script = script.Replace('\r', ' ');
                        pbWork.Maximum = script.Split(new Char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        ///*
                        FbTransaction trans = c.BeginTransaction();
                        FbCommand fbcmd = new FbCommand("", c, trans);
                        try
                        {

                            foreach (string cmd in script.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (!string.IsNullOrEmpty(cmd))
                                {
                                    fbcmd.CommandText = cmd;
                                    int result = fbcmd.ExecuteNonQuery();
                                    pbWork.PerformStep();
                                    pbWork.Update();
                                    PutLog(string.Format("\"{0} result\"={1}", cmd, result));

                                }
                            }
                            trans.Commit();
                        }
                        catch (Exception e)
                        {

                            if (MessageBox.Show(string.Format("Ошибка:{0}\nОткатиться на начало?", e.Message), "fatality", MessageBoxButtons.YesNo).Equals(DialogResult.Yes)) trans.Rollback();
                            ThreadExceptionHandler.SendErrorMessage(string.Format("{0}:{1}", db, e.Message));
                            return false;
                        }
                    }
                    else { PutLog("Исправление не требуется!"); }


                    c.Close();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
                ThreadExceptionHandler.SendErrorMessage(string.Format("Dirty work:{0}", e.ToString()));
                return false;

            }

            return true;

        }

        private string CSVScriptMaker(string sourcefile, string desttable)
        {
            string res = null;
            using (StreamReader sr = new StreamReader(sourcefile, Encoding.Default))
            {
                string capt = string.Format("UPDATE OR INSERT INTO {1} ({0}) VALUES", sr.ReadLine().Replace(';', ','), desttable);
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] stmp = line.Split(';');
                    line = "("; int sss = 0;
                    for (int i = 0; i < stmp.Length; i++)
                    {
                        string xxx = stmp[i].Replace(" ", "");
                        if (int.TryParse(xxx, out sss)) stmp[i] = xxx; //workaround
                        line += string.Format("\"{0}\"{1}", stmp[i], i == stmp.Length - 1 ? ");\n" : ",");
                    }
                    res += capt + line;
                }
            }
            return res;

        }

        private void CommandExecutingEventHandler(object sender, CommandExecutingEventArgs e) { PutLog(e.SqlCommand != null ? e.SqlCommand.CommandText : "ГОТОВО"); }

        private void toolStripMenuItem_fixoo_Click(object sender, EventArgs e) { PerformOperations("fixoo"); }
        private void toolStripMenuItem_fixpassword_Click(object sender, EventArgs e) { PerformOperations("fixpassword", null, true); }
        private void toolStripMenuItem_fixnewuser_Click(object sender, EventArgs e) { PerformOperations("fixnewuser"); }
        private void toolStripMenuItem_fixdox_Click(object sender, EventArgs e) { PerformOperations("fixdox", null, true); }
        private void toolStripMenuItem_fix18_Click(object sender, EventArgs e) { PerformOperations("fix18", "fix18if"); }
        private void toolStripMenuItem_fix20_Click(object sender, EventArgs e) { PerformOperations("fix20", "fix20if"); }
        private void toolStripMenuItem_fix22_Click(object sender, EventArgs e) { PerformOperations("fix22", "fix22if"); }

        private void выделитьВсёToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbTSMI.HasDropDownItems) return;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {
                it.Checked = true;
            }
        }

        private void снятьВыделениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbTSMI.HasDropDownItems) return;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {
                it.Checked = false;
            }
        }

        private void инвертироватьВыделениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbTSMI.HasDropDownItems) return;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {
                it.Checked = !it.Checked;
            }
        }


        private void правитьToolStripMenuItem_Click(object sender, EventArgs e) { SettingsForm frm = new SettingsForm(); frm.ShowDialog(); }

        private int CheckedCount() { int res = 0; foreach (ToolStripMenuItem it in dbTSMI.DropDownItems) if (it.Checked) res++; return res; }

        private void DBbackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainMenu.Enabled = false;
            //Определяемя с базами данных
            int i = 0;
            pbDB.Maximum = CheckedCount();
            pbDB.Value = 0;
            pbDB.Step = 1;
            pbWork.Style = ProgressBarStyle.Marquee;
            pbWork.MarqueeAnimationSpeed = 0;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {

                if (it.Checked)
                {
                    try
                    {
                        string bakname = string.Format(Properties.Settings.Default.DefaultArchives, ".GBK", it.Text, DateTime.Today);
                        lCurrentDB.Text = string.Format("{0}. {1}", (++i), it.Text) + "=>" + bakname;
                        pbDB.PerformStep();
                        PutLog(string.Format("Строчка соединения {0}", it.ToolTipText));
                        pbDB.Update();
                        FbBackup fbb = new FbBackup();
                        fbb.ConnectionString = it.ToolTipText;
                        fbb.BackupFiles.Add(new FbBackupFile(bakname, 2048));
                        fbb.Verbose = true;
                        fbb.Options = FbBackupFlags.IgnoreLimbo; //FbBackupFlags.
                        fbb.ServiceOutput += new ServiceOutputEventHandler(ServiceOutput);
                        pbWork.MarqueeAnimationSpeed = 50;
                        Application.DoEvents();
                        fbb.Execute();
                        pbWork.MarqueeAnimationSpeed = 0;
                        toolStripStatusLabel1.Text = it.Text + " OK";
                        Application.DoEvents();
                    }
                    catch (Exception er)
                    {
                        pbWork.ForeColor = Color.Red;
                        PutLog(string.Format("Упс... ошибочка:{0}", er.Message));
                        MessageBox.Show(er.Message, "Ошибочка вышла...");
                        pbWork.ForeColor = pbDB.ForeColor;
                        ThreadExceptionHandler.SendErrorMessage(string.Format("{0}:{1}", it.ToolTipText, er.Message));

                    }
                    pbWork.Value = 0;

                }
            }


            pbDB.Maximum = CheckedCount();
            MainMenu.Enabled = true; pbWork.Style = ProgressBarStyle.Blocks;
            pbWork.MarqueeAnimationSpeed = 0;
            PutLog("Архивация завершена");

        }

        private void ServiceOutput(object sender, ServiceOutputEventArgs e)
        {
            if (Properties.Settings.Default.DetailedLog) PutLog(e.Message);
            lCurrentWork.Text = e.Message; lCurrentWork.Update();
            pbWork.Value = (pbWork.Value + 1) % pbWork.Maximum;
            pbWork.Update();
            //Application.DoEvents();
        }


        private void проверкаБДToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainMenu.Enabled = false;
            pbWork.Style = ProgressBarStyle.Marquee;
            pbWork.MarqueeAnimationSpeed = 0;
            int cnt = 0, i = 0;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {
                if (it.Checked) cnt++; i++;
            }
            pbDB.Maximum = cnt; pbDB.Value = 0;
            pbDB.Step = 1;
            PutLog(string.Format("Начинаем проверку {0} баз из {1} доступных", cnt, i));
            i = 0;
            //string[] itms = new string[cnt];
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {

                if (it.Checked)
                {
                    ++i;
                    lCurrentDB.Text = string.Format("{0}. {1}", (i), it.Text); pbDB.PerformStep();
                    PutLog(string.Format("{0}. {1}", (i), it.Text));
                    PutLog(string.Format("Строчка соединения {0}", it.ToolTipText));
                    lCurrentDB.Update(); pbDB.Update();
                    bool dobackup = true;
                    try
                    {
                        using (FbConnection fc = new FbConnection(it.ToolTipText))
                        {
                            fc.Open();
                            FbCommand fcmd = new FbCommand(Properties.Resources.FBMonCnt, fc);
                            int MonCnt = (int)fcmd.ExecuteScalar();
                            if (MonCnt > 1)
                            {
                                dobackup = false; PutLog(string.Format("Невозможно исправить базу данных при множественных подключениях. Сейчас подключены следующие {0} пользоватеей:", MonCnt));
                                fcmd.CommandText = Properties.Resources.FBMon;
                                using (FbDataReader fbdr = fcmd.ExecuteReader())
                                {
                                    while (fbdr.Read())
                                    {
                                        string s1 = "";
                                        for (int z = 0; z < fbdr.FieldCount; z++)
                                        {
                                            s1 += (string.IsNullOrEmpty(s1) ? "" : ":") + fbdr.GetValue(z).ToString();
                                        }
                                        PutLog(s1);
                                    }



                                }
                            }

                            fc.Close();
                            while (fc.State == ConnectionState.Open) ;
                        }
                        if (dobackup)
                        {
                            FbConnection.ClearAllPools();
                            FbValidation validationSvc = new FbValidation();
                            //FbConnectionStringBuilder fbcsb = new FbConnectionStringBuilder();
                            validationSvc.ConnectionString = it.ToolTipText;
                            validationSvc.Options = FbValidationFlags.ValidateDatabase;
                            validationSvc.ServiceOutput += new ServiceOutputEventHandler(ServiceOutput);
                            pbWork.MarqueeAnimationSpeed = 50;
                            validationSvc.Execute();
                            pbWork.MarqueeAnimationSpeed = 0;
                            toolStripStatusLabel1.Text = it.Text + " [ПРОВЕРЕНО]";
                            PutLog(it.Text + " [ПРОВЕРЕНО]");
                            Application.DoEvents();
                        }

                    }
                    catch (Exception er)
                    {
                        pbWork.ForeColor = Color.Red;
                        MessageBox.Show(er.Message, "Ошибочка вышла...");
                        PutLog(string.Format("Упс... ошибочка:{0}", er.Message));
                        pbWork.ForeColor = pbDB.ForeColor;

                    }

                }
            }


            pbDB.Maximum = cnt;
            MainMenu.Enabled = true; pbWork.Style = ProgressBarStyle.Blocks;
            pbWork.MarqueeAnimationSpeed = 0;
            PutLog("Проверка завершена");
        }

        public void PutLog(string message) { lbLog.BeginUpdate(); lbLog.Items.Add(message); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; lbLog.EndUpdate(); lbLog.Update(); }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditDBInfo frm = new EditDBInfo();
            if (frm.ShowDialog().Equals(DialogResult.OK))
            {
                UpdateMenu();
                Properties.Settings.Default.Save();
            }
        }

        public bool TestWeCanWrite(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    string testfile = string.Format("{0}{2}{1}.tmp", path, GetMD5Hash(DateTime.Now.ToString()), path.EndsWith("\\") ? "" : "\\");
                    PutLog(testfile);
                    using (TextWriter streamWriter =
                        new StreamWriter(testfile))
                    {
                        streamWriter.WriteLine(testfile);
                    }
                    File.Delete(testfile);
                    return true;
                }
                else throw (new DirectoryNotFoundException(string.Format("Указанная папка не найдена!\n{0}", path)));
            }
            catch (Exception ex)
            {
                PutLog(string.Format("TestWeCanWrite: {0}", ex.Message));
                return false;
            }
        }

        private void проверкаДоступностиПутейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainMenu.Enabled = false;
            int cnt = 0, i = 0;
            string errorreport = "Выявленные ошибки:\n";
            int errhash = errorreport.GetHashCode();
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {
                if (it.Checked) cnt++; i++;
            }
            pbDB.Maximum = cnt; pbDB.Value = 0;
            pbDB.Step = 1;
            PutLog(string.Format("Начинаем проверку {0} баз из {1} доступных", cnt, i));
            i = 0;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {

                if (it.Checked)
                {
                    ++i;
                    lCurrentDB.Text = string.Format("{0}. {1}", (i), it.Text);
                    PutLog(string.Format("{0}. {1}", (i), it.Text));
                    PutLog(string.Format("Строчка соединения {0}", it.ToolTipText));
                    try
                    {
                        //ConnectionString = it.ToolTipText;
                        using (FbConnection fbc = new FbConnection(it.ToolTipText))
                        {

                            fbc.Open();
                            FbCommand cmd = new FbCommand("", fbc);
                            string path = "";

                            cmd.CommandText = "select path_doc from s_subdividings";
                            path = cmd.ExecuteScalar().ToString();
                            PutLog(path);
                            try
                            {
                                if (Directory.Exists(path))
                                {
                                    string testfile = string.Format("{0}{2}{1}.tmp", path, GetMD5Hash(DateTime.Now.ToString()), path.EndsWith("\\") ? "" : "\\");
                                    PutLog(testfile);
                                    using (TextWriter streamWriter =
                                        new StreamWriter(testfile))
                                    {
                                        streamWriter.WriteLine(testfile);
                                    }

                                    File.Delete(testfile);
                                    PutLog("OK. testfile deleted");
                                }
                                else throw (new DirectoryNotFoundException(string.Format("Папка для документов не найдена!\n{0}", path)));
                            }
                            catch (Exception er)
                            {
                                PutLog(string.Format("Упс... ошибочка:{0}", er.Message));
                                errorreport += string.Format("Ошибка в {0} c документами:{1}\n", it.Text, er.Message);


                            }

                            cmd.CommandText = "select templates_path from s_subdividings";
                            path = cmd.ExecuteScalar().ToString();
                            PutLog(path);
                            try
                            {
                                if (Directory.Exists(path))
                                {

                                    string testfile = string.Format("{0}{2}{1}.tmp", path, GetMD5Hash(DateTime.Now.ToString()), path.EndsWith("\\") ? "" : "\\");
                                    PutLog(testfile);
                                    using (TextWriter streamWriter =
                                        new StreamWriter(testfile))
                                    {
                                        streamWriter.WriteLine(testfile);
                                    }

                                    File.Delete(testfile);
                                    PutLog("OK. testfile deleted");
                                    PutLog(string.Format("Проверяем доступность прописанных в БД {0} шаблонов", it.Text));

                                    cmd.CommandText = "select doc_name, filename, doc_group  from s_templates";
                                    using (FbDataReader fbdr = cmd.ExecuteReader())
                                    {
                                        while (fbdr.Read())
                                        {
                                            if (File.Exists(string.Format("{0}{3}\\{2}{1}", path, fbdr.GetString(1), path.EndsWith("\\") ? "" : "\\", fbdr.GetString(2))))
                                            {

                                                if (Properties.Settings.Default.DetailedLog)
                                                    PutLog(string.Format("{4} @ {0}{3}\\{2}{1} OK", path, fbdr.GetString(1), path.EndsWith("\\") ? "" : "\\", fbdr.GetString(2), fbdr.GetString(0)));
                                            }
                                            else
                                            {
                                                PutLog(string.Format("{4} @ {0}{3}\\{2}{1} FAIL", path, fbdr.GetString(1), path.EndsWith("\\") ? "" : "\\", fbdr.GetString(2), fbdr.GetString(0)));
                                                errorreport += string.Format("{4} @ {0}{3}\\{2}{1} FAIL\n", path, fbdr.GetString(1), path.EndsWith("\\") ? "" : "\\", fbdr.GetString(2), fbdr.GetString(0));
                                            }
                                        }
                                    }

                                }
                                else throw (new DirectoryNotFoundException(string.Format("Папка шаблонов документов не найдена!\n{0}", path)));
                            }
                            catch (Exception er)
                            {
                                PutLog(string.Format("Упс... ошибочка:{0}", er.Message));
                                errorreport += string.Format("Ошибка в {0} c шаблонами:{1}\n", it.Text, er.Message);

                            }

                            toolStripStatusLabel1.Text = it.Text + " [ПРОВЕРЕНО]";
                            PutLog(it.Text + " [ПРОВЕРЕНО]");
                            Application.DoEvents();
                        }

                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.Message, "Ошибочка вышла...");
                        PutLog(string.Format("Упс... ошибочка:{0}", er.Message));


                    }

                    if (errorreport.GetHashCode() != errhash)
                    {
                        MessageBox.Show(errorreport); errorreport = "Выявленные ошибки:\n";
                    }

                    pbDB.PerformStep();
                    lCurrentDB.Update(); pbDB.Update();

                }
            }

            MainMenu.Enabled = true;
            PutLog("Проверка завершена");
        }

        private string ExtractResource2File(string ResourceName, string outputname = null, string ResourceURL = null, string ResourceFile = null,string md5hash=null)
        {
            ResourceManager rm = Properties.Resources.ResourceManager;
            
           string outputfile = string.IsNullOrEmpty(outputname) ? Path.GetTempFileName() : outputname;
            if (rm.GetObject(ResourceName) != null)
            {
                try
                {
                    byte[] res = (byte[])rm.GetObject(ResourceName);
                    if (!string.IsNullOrEmpty(md5hash))
                    {
                        string resmd5 = GetMD5Hash(res);
                        if (resmd5 != md5hash)
                        {
                            throw new IOException(string.Format("Ошибка выгрузки {0}. md5 hash = {1}. Ожидаемый md5 hash = {2}.", ResourceName, resmd5, md5hash));
                        }
                    }

                    {
                        using (FileStream dest = File.Open(outputfile, FileMode.Create))
                        {

                            dest.Write(res, 0, res.Length);
                            dest.Flush();
                            return dest.Name;
                        }
                    }
                }
                catch (Exception ex)
                {
                    PutLog(ex.Message);
                }
                return null;
            }
            else
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.CheckFileExists = true;
                switch (MessageBox.Show(string.Format("Ресурс {1} больше не входит в состав основного пакета.\nПри обновлении необходимо выбрать источник пакета. Пакет можно скачать из {0}\nНажмите Да - для автоматического скачивания пакета \nНет - для ручного указания положения пакета обновления \nОтмена - для отмены обновления", ResourceURL,ResourceName), "Порядок действий по выполнению обновлений", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                {
                    case DialogResult.No:
                        dlg.Title = string.Format("Укажите расположение ресурса {1}",ResourceName);
                        dlg.DefaultExt = dlg.FileName = ResourceFile;
                        dlg.Filter = string.Format("{0}|{0}",ResourceFile);
                        if (!dlg.ShowDialog().Equals(DialogResult.OK)) return null;
                        if (!string.IsNullOrEmpty(md5hash))
                        {
                            string resmd5 = GetMD5Hash(File.OpenRead(dlg.FileName));
                            if (resmd5 != md5hash)
                            {
                                PutLog(string.Format("Ошибка проверки {0}. md5 hash = {1}. Ожидаемый md5 hash = {2}.", dlg.FileName, resmd5, md5hash));
                                if (MessageBox.Show(string.Format("Ошибка проверки {0}. md5 hash = {1}. Ожидаемый md5 hash = {2}.", dlg.FileName, resmd5, md5hash),"Epic fail",MessageBoxButtons.YesNo,MessageBoxIcon.Hand,MessageBoxDefaultButton.Button2).Equals(DialogResult.No)) return null;
                                string.Format("Используем {0} на зло кондуктору ;)", dlg.FileName);
                            }
                        }
                        return dlg.FileName;
                    case DialogResult.Yes:
                        PutLog("Скачиваем пакет во временный файл " + Path.GetTempPath());
                        return DownloadFromNet2File(ResourceURL,outputname,md5hash);
                    default: return null;
                }
            }

        }
        private string DownloadFromNet2File(string resourcename, string outputname = null, string md5hash=null)
        {

            string path=string.IsNullOrEmpty(outputname) ? Path.GetTempFileName() : outputname;
            PutLog(string.Format("Скачиваем {0} в {1}", resourcename, path));
            try
            {
                //WebClient Client = new WebClient();
                //byte[] res = Client.DownloadData(resourcename);
                DlForm df = new DlForm(resourcename);
                df.ShowDialog();
                byte[] res = df.Res();
                if (res == null)
                {
                    throw new IOException(string.Format("Ошибка скачивания {0}. Null result", resourcename));
                }
                if (!string.IsNullOrEmpty(md5hash))
                {
                    string resmd5=GetMD5Hash(res);
                    if (resmd5 != md5hash)
                    {
                        throw new IOException(string.Format("Ошибка скачивания {0}. md5 hash = {1}. Ожидаемый md5 hash = {2}.", resourcename, resmd5, md5hash));
                    }
                }

                using (FileStream dest = File.Open(path, FileMode.Create))
                {

                    dest.Write(res, 0, res.Length);
                    dest.Flush();

                    return dest.Name;
                }

            }
            catch (Exception ex)
            {
                PutLog(ex.Message);
            }
            return null;
        }


        static void Unzip(string source, string dest, DisplayMessage output)
        {

            if (!File.Exists(source))
            {
                throw new FileNotFoundException();
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(source)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    try
                    {

                        //Console.WriteLine(theEntry.Name);
                        string fname = dest + (dest.EndsWith("\\") ? "" : "\\") + theEntry.Name;
                        string directoryName = Path.GetDirectoryName(fname);
                        string fileName = Path.GetFileName(fname);
                        output(fname);

                        // create directory
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(fname))
                            {

                                int size = 2048;
                                byte[] data = new byte[size];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0) streamWriter.Write(data, 0, size); else break;
                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        output(ex.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Возвращает количество пользователей подключенных к БД db
        /// </summary>
        /// <param name="db">строка подключения к БД</param>
        /// <param name="output">Делегат вывода сообщений - если указан - будет выведен список подключенных пользователей к БД</param>
        /// <returns>Количество подключенных пользователей к БД +1 (наше соедиенение до БД)</returns>
        public int ConnectedUsersCount(string db, DisplayMessage output = null)
        {
            using (FbConnection fc = new FbConnection(db))
            {
                fc.Open();
                FbCommand fcmd = new FbCommand(Properties.Resources.FBMonCnt, fc);
                int MonCnt = (int)fcmd.ExecuteScalar();
                if (output != null)
                {
                    if (MonCnt > 1)
                    {
                        output(string.Format("К БД подключены следующие пользователи ({0}, не считая нас):", MonCnt - 1));
                        fcmd.CommandText = Properties.Resources.FBMon;
                        using (FbDataReader fbdr = fcmd.ExecuteReader())
                        {
                            while (fbdr.Read())
                            {
                                string s1 = "";
                                for (int z = 0; z < fbdr.FieldCount; z++)
                                {
                                    s1 += (string.IsNullOrEmpty(s1) ? "" : ":") + fbdr.GetValue(z).ToString();
                                }
                                if (!s1.Contains(Application.ExecutablePath)) output(s1);
                            }
                        }
                    }
                    else output("К БД подключены только мы.");
                }

                fc.Close();
                return MonCnt;
            }
        }

        private void релизПКСП307Cб9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainMenu.Enabled = false; PutLog("Проверка наличия незакрытых подключений");
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
                if (it.Checked)
                {
                    if ((ConnectedUsersCount(it.ToolTipText, (Properties.Settings.Default.DetailedLog ? PutLog : (DisplayMessage)null)) > 1) && (MessageBox.Show(string.Format("Обновить БД {0} при подключенных пользователях!?", it.Text), "Подтверждение уничтожения БД", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2).Equals(DialogResult.No))) it.Checked = false;
                    PutLog(string.Format("Флаг обновления с базы {0} {1}.", it.Text, it.Checked ? "не снят" : "снят"));
                }
            if (MessageBox.Show("Обновление выполнять при отключении всех пользователей от БД ПК «СП». \nВыполнить обновление отмеченных БД?", "Порядок действий по выполнению обновлений", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
            {
                /////////////////////////////
                int cnt = 0, i = 0; foreach (ToolStripMenuItem it in dbTSMI.DropDownItems) { if (it.Checked) cnt++; i++; }
                pbDB.Maximum = cnt; pbDB.Value = 0; pbDB.Step = 1; PutLog(string.Format("Начинаем обновление {0} баз из {1} доступных", cnt, i));
                /////////////////////////////
                if (MessageBox.Show("1. Сделать копию БД \n Создать архив БД в соответствии с настройками?", "Порядок действий по выполнению обновлений", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
                {
                    DBbackupToolStripMenuItem_Click(sender, e);
                    PutLog("Обновление. Архивация проведена.");
                }
                else MessageBox.Show("Архивация отменена - делаем всё на свой страх и риск!");
                PutLog("Получаем и распаковываем пакеты...");
                string serverzip = ExtractResource2File("Ex_prod_server3_07_9", null, Properties.Resources.UpdEx_prod_server3_07_9url, "Ex_prod_server3_07_9.zip");
                if (string.IsNullOrEmpty(serverzip)) { PutLog("Aborted!"); return; }//throw new IOException("При получении архива возникли трудности");
                string templates_path = ExtractResource2File("Ex_Prod_templates3_07_9", null, Properties.Resources.UpdTemplates3_7_9url, "Templates3_7_9.zip");
                if (string.IsNullOrEmpty(templates_path)) { PutLog("Невозможно получить пакет шаблонов!"); return; }
                PutLog("Пакеты получены");
                MessageBox.Show("2.Заменить в каталоге Ex_prod_server на сервере подразделения файл Ex_prod.exe", "Порядок действий по выполнению обновлений");
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.CheckFileExists = true;
                dlg.Title = "2.Заменить в каталоге Ex_prod_server на сервере подразделения файл Ex_prod.exe";
                dlg.FileName = "ex_prod.exe";
                dlg.DefaultExt = "ex_prod.exe";
                dlg.Filter = "ex_prod.exe|ex_prod.exe";
                PutLog("Используем временную папку " + Path.GetTempPath());
                if (!dlg.ShowDialog().Equals(DialogResult.OK)) return;
                if (Directory.Exists(Path.GetDirectoryName(dlg.FileName) + "\\sql")) Directory.Delete(Path.GetDirectoryName(dlg.FileName) + "\\sql", true);//выпиливаем каталог sql
                Unzip(serverzip, Path.GetDirectoryName(dlg.FileName), PutLog);
                //MessageBox.Show("3. Обновить структуру базы данных ПК «СП», запустив Ex_prod.exe.", "Порядок действий по выполнению обновлений");
                /////////////////////////////////////////////////////////////////
                i = 0;
                foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
                {

                    if (it.Checked)
                    {
                        ++i;
                        lCurrentDB.Text = string.Format("{0}. {1}", (i), it.Text);
                        PutLog(string.Format("{0}. {1}", (i), it.Text));
                        MessageBox.Show(string.Format("3. Обновить структуру базы данных ПК «СП», запустив Ex_prod.exe.\nСейчас будет запущен обновленный ПК СП. Выполните вход в БД {0},\nПосле этого закройте программу.", it.Text), "Порядок действий по выполнению обновлений");
                        this.Hide();
                        System.Diagnostics.Process pr = new System.Diagnostics.Process();
                        pr.StartInfo.FileName = dlg.FileName;
                        pr.Start();
                        pr.WaitForExit();
                        this.Show();
                        bool res = (MessageBox.Show(string.Format("Вы успешно зашли в БД {0}?", it.Text), "Порядок действий по выполнению обновлений", MessageBoxButtons.YesNo).Equals(DialogResult.Yes));
                        if (res)
                        {
                            using (FbConnection fbc = new FbConnection(it.ToolTipText))
                            {

                                fbc.Open();
                                FbCommand cmd = new FbCommand("select templates_path from s_subdividings", fbc);
                                string templates=cmd.ExecuteScalar().ToString();
                                if (!string.IsNullOrEmpty(templates))
                                {
                                    if (TestWeCanWrite(templates))
                                    {
                                         Unzip(templates_path, templates, PutLog);
                                         PutLog("Шаблоны выложены");
                                    }
                                    else
                                    { PutLog("С шаблонами ничего не выйдет - почините их ручками!"); MessageBox.Show("С шаблонами ничего не выйдет - почините их ручками!"); }
                                }
                                else { PutLog("С шаблонами вообще грабли!"); MessageBox.Show("С шаблонами вообще грабли! Кто-то облажался!"); }
                                foreach (string sqls in Directory.GetFiles(Path.GetDirectoryName(dlg.FileName) + "\\sql"))
                                {
                                    if (res)
                                    {
                                        //res = DoDirtyWork(it.ToolTipText, File.ReadAllText(sqls, Encoding.Default) /*new StreamReader(sqls, Encoding.Default)*/);
                                        res = DoDirtyWork(it.ToolTipText, new StreamReader(sqls, Encoding.Default));
                                        PutLog(string.Format("{0} {1}", sqls, res ? "OK" : "FAILED"));
                                    }
                                }
                                //if (res) trans.Commit(); else trans.Rollback();
                                fbc.Close();
                            }
                        }
                        PutLog(string.Format("{0}. {1} ОБНОВЛЕНО {2}.", (i), it.Text, (res ? "УСПЕШНО" : "НЕУДАЧНО")));
                        pbDB.PerformStep();
                        lCurrentDB.Update(); pbDB.Update();
                    }
                }
                ///////////////////////////////////////////////////////////////////////////
                MessageBox.Show("Обновление завершено!\nНе забудьте обновить АРМ Депозит и обучить пользователей!");
            }
            else PutLog("Обновление отменено");
            MainMenu.Enabled = true;
        }

        private void DoExport(string query=null,string coutquery=null,string caption=null)
        {
            {
                string q = string.IsNullOrEmpty(query) ? "select * from ip" : query;
                string cq = string.IsNullOrEmpty(coutquery) ? "select count(*) from ip" : coutquery;
                bool ask4file = (MessageBox.Show("Свалить все базы в один файл?", "Вопросик", MessageBoxButtons.YesNo).Equals(DialogResult.No));
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = "csv";
                dlg.Filter = "Comma separated values (*.csv)|*.csv|ALL (*.*)|*.*";
                dlg.Title = "файлик для экспорта. Существующий будет дописан!";
                if (!ask4file) if (!dlg.ShowDialog().Equals(DialogResult.OK)) return;
                MainMenu.Enabled = false;
                int cnt = 0, i = 0;
                foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
                {
                    if (it.Checked) cnt++; i++;
                }
                pbDB.Maximum = cnt; pbDB.Value = 0;
                pbDB.Step = 1;
                PutLog(string.Format("Начинаем экспорт. Выбрано {0} баз из {1}.", cnt, i));
                i = 0;
                foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
                {

                    if (it.Checked)
                    {
                        ++i;
                        lCurrentDB.Text = string.Format("{0}. {1}", (i), it.Text);
                        PutLog(string.Format("{0}. {1}", (i), it.Text));
                        PutLog(string.Format("Строчка соединения {0}", it.ToolTipText));
                        lCurrentDB.Update(); pbDB.Update();
                        if (ask4file)
                        {
                            dlg.Title = string.Format("файлик для {0}. Существующий будет изничтожен!", it.Text);
                            dlg.ShowDialog();
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////
                        try
                        {
                            //ConnectionString = it.ToolTipText;
                            using (FbConnection fbc = new FbConnection(it.ToolTipText))
                            {

                                fbc.Open();
                                FbCommand cmd = new FbCommand(cq, fbc);
                                //int wcnt =
                                pbWork.Maximum = (int)(cmd.ExecuteScalar()); pbWork.Value = 0; pbWork.Step = 1;
                                cmd.CommandText = q;
                                PutLog(string.Format("Экспортирую {1} в файл {0} режим:{2}", dlg.FileName, it.Text, ask4file ? "rw" : "add"));
                                using (FbDataReader fbdr = cmd.ExecuteReader())
                                {
                                    using (TextWriter tw = new StreamWriter(dlg.FileName, !ask4file))
                                    {
                                        if (!string.IsNullOrEmpty(caption))
                                        {
                                            tw.WriteLine(caption);
                                            if (!ask4file) caption = null;
                                        }
                                        while (fbdr.Read())
                                        {
                                            for (int z = 0; z < fbdr.FieldCount; z++)
                                            {
                                                tw.Write(fbdr.GetValue(z)); tw.Write('\t');
                                            }
                                            pbWork.PerformStep(); pbWork.Update();
                                            tw.WriteLine(); Application.DoEvents();
                                        }


                                    }
                                }

                                PutLog(it.Text + " [Экспортировано]");
                                Application.DoEvents();
                            }

                        }
                        catch (Exception er)
                        {
                            MessageBox.Show(er.Message, "Ошибочка вышла...");
                            PutLog(string.Format("Упс... ошибочка:{0}", er.Message));


                        }
                        /////////////////////////////////////////////////////////////////////////////////////////
                        pbDB.PerformStep();
                    }
                }


                pbDB.Maximum = cnt;
                MainMenu.Enabled = true;
                PutLog("Экспорт запрещён");


            }
        }


        private void вCSVToolStripMenuItem_Click(object sender, EventArgs e)

        {
            DoExport();
        }


        private void релизПКСП307Cб9Этап10ДепозитToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string serverzip = ExtractResource2File("deposit3_07_9",null,Properties.Resources.UpdDeposit3_7_9url,"deposit379.zip");
            if (string.IsNullOrEmpty(serverzip)) { PutLog("Aborted!"); return; }
            PutLog("Используем пакет " + serverzip);
            MessageBox.Show("10.На рабочей станции специалиста по учету денежных средств в каталоге Deposit заменить файлы deposit.exe, deposit.pdb.", "Порядок действий по выполнению обновлений");
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Title = "10.На рабочей станции специалиста по учету денежных средств в каталоге Deposit заменить файлы deposit.exe, deposit.pdb.";
            dlg.FileName = "deposit.exe";
            dlg.DefaultExt = "deposit.exe";
            dlg.Filter = "deposit.exe|deposit.exe";
            if (!dlg.ShowDialog().Equals(DialogResult.OK)) return;
            Unzip(serverzip, Path.GetDirectoryName(dlg.FileName), PutLog);//заменяем!

        }



        private void дляСайтаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = "txt";
            dlg.Filter = "Comma separated values (*.txt)|*.txt|ALL (*.*)|*.*";
            dlg.Title = "файлик для экспорта. Существующий будет дописан!";
            if (!dlg.ShowDialog().Equals(DialogResult.OK)) return;
            MainMenu.Enabled = false;
            int cnt = 0, i = 0;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {
                if (it.Checked) cnt++; i++;
            }
            pbDB.Maximum = cnt; pbDB.Value = 0;
            pbDB.Step = 1;
            PutLog(string.Format("Начинаем экспорт {0} баз из {1} выбранных", cnt, i));
            i = 0;
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
            {

                if (it.Checked)
                {
                    ++i;
                    lCurrentDB.Text = string.Format("{0}. {1}", (i), it.Text);
                    PutLog(string.Format("{0}. {1}", (i), it.Text));
                    PutLog(string.Format("Строчка соединения {0}", it.ToolTipText));
                    lCurrentDB.Update(); pbDB.Update();

                    /////////////////////////////////////////////////////////////////////////////////////////
                    try
                    {
                        //ConnectionString = it.ToolTipText;
                        using (FbConnection fbc = new FbConnection(it.ToolTipText))
                        {

                            fbc.Open();
                            FbCommand cmd = new FbCommand(Properties.Resources.toSiteCnt, fbc);
                            //int wcnt =
                            pbWork.Maximum = (int)(cmd.ExecuteScalar()); pbWork.Value = 0; pbWork.Step = 1;
                            cmd.CommandText = Properties.Resources.toSiteQuery;
                            PutLog(string.Format("Экспортирую {1} в файл {0} количество:{2}", dlg.FileName, it.Text, pbWork.Maximum));
                            using (FbDataReader fbdr = cmd.ExecuteReader())
                            {
                                using (TextWriter tw = new StreamWriter(dlg.FileName, true, Encoding.Default))
                                {

                                    while (fbdr.Read())
                                    {
                                        for (int z = 0; z < fbdr.FieldCount; z++)
                                        {
                                            if (!fbdr.IsDBNull(z))
                                            {
                                                switch (z)
                                                {
                                                    case 1: string tmporg = fbdr.GetString(z);
                                                        tmporg = tmporg.Remove(0, tmporg.LastIndexOf('.') + 1);
                                                        tw.Write(tmporg);
                                                        break;
                                                    case 4:
                                                        tw.Write(fbdr.GetDateTime(z).ToShortDateString());
                                                        break;
                                                    default: tw.Write(fbdr.GetValue(z));
                                                        break;
                                                }
                                            }
                                            else { if (Properties.Settings.Default.DetailedLog) PutLog(string.Format("Null value detected! z={0}", z)); }



                                            if (z != fbdr.FieldCount - 1) tw.Write('\t');
                                        }
                                        pbWork.PerformStep(); pbWork.Update();
                                        //tw.WriteLine(); 
                                        tw.Write('\n');
                                        Application.DoEvents();
                                    }


                                }
                            }

                            PutLog(it.Text + " [Экспортировано]");
                            Application.DoEvents();
                        }

                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.Message, "Ошибочка вышла...");
                        PutLog(string.Format("Упс... ошибочка:{0}", er.Message));


                    }
                    /////////////////////////////////////////////////////////////////////////////////////////
                    pbDB.PerformStep();
                }
            }


            pbDB.Maximum = cnt;
            MainMenu.Enabled = true;
            PutLog("Экспорт запрещён");



        }

        private void направитьОшибкуToolStripMenuItem_Click(object sender, EventArgs e) { if (MessageBox.Show("Создать пробное исключение?", "errorreporting test", MessageBoxButtons.YesNo).Equals(DialogResult.Yes)) throw new Exception("ТЕСТОВАЯ ОШИБКА!"); }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:apps@ufssp44.org.ru");
        }

        private void письмоРазработчикамToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:apps@ufssp44.org.ru");
        }

        private void шаблоновToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "csv";
            dlg.Filter = "Comma separated values (*.csv)|*.csv|ALL (*.*)|*.*";
            dlg.Title = "файлик шаблонов.";
            if (!dlg.ShowDialog().Equals(DialogResult.OK)) return;
            Clipboard.SetText(CSVScriptMaker(dlg.FileName, "s_templates"));
            PerformScript(CSVScriptMaker(dlg.FileName, "s_templates"));
        }

        private void метокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "csv";
            dlg.Filter = "Comma separated values (*.csv)|*.csv|ALL (*.*)|*.*";
            dlg.Title = "файлик меток.";
            if (!dlg.ShowDialog().Equals(DialogResult.OK)) return;
            Clipboard.SetText(CSVScriptMaker(dlg.FileName, "PRN_METKI"));
            PerformScript(CSVScriptMaker(dlg.FileName, "PRN_METKI"));

        }

        private void cSV2SQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Будет создан соответствующий sql-скрипт для внесения содержимого .csv файлов ПК СП в БД.\nДля работоспособности скрипта необходимо ВРУЧНУЮ заменить !TABLE! на имя таблицы в которую вносятся данные (S_TEMPLATES, PRN_METKI или другую)!!!");
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "csv";
            dlg.Filter = "Comma separated values (*.csv)|*.csv|ALL (*.*)|*.*";
            dlg.Title = "sql скрипт.";
            if (!dlg.ShowDialog().Equals(DialogResult.OK)) return;
            File.AppendAllText(Path.ChangeExtension(dlg.FileName, "sql"), CSVScriptMaker(dlg.FileName, "!TABLE!"), Encoding.Default);

        }

        private void lbLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lbLog.SelectedItem != null)
                MessageBox.Show(lbLog.SelectedItem.ToString(), "Строка отчёта");
        }

        private void списокПодключенныхПользователейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem it in dbTSMI.DropDownItems)
                if (it.Checked)
                {
                    PutLog(string.Format("Подключения к {0}:", it.Text));
                    ConnectedUsersCount(it.ToolTipText, PutLog);
                }
        }

        private void вCSVAdvancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSelectColums f = new frmSelectColums();
            if (f.ShowDialog().Equals(DialogResult.OK))
            {
                DoExport(FieldInfo.List2q(f.dest),null,FieldInfo.List2l(f.dest).Replace(',','\t'));
            }
            
        }

        private void наборыСтолбцовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSelectColums f = new frmSelectColums();
            if (f.ShowDialog().Equals(DialogResult.OK))
            { }

        }

 
    }
    public delegate void DisplayMessage(string message);
}
