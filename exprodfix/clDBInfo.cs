using System;
using System.Collections.Generic;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using System.Windows.Forms;



namespace exprodtoolbox
{
     [Serializable]
    class clDBInfo
    {
        public string Name = "Database";
        public string Server = "localhost";
        public string DBpath = "";
        public string User = "SYSDBA";
        public string Password = "masterkey";
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="name">Displayed name</param>
        /// <param name="server">used server</param>
        /// <param name="dbpath">path to db</param>
        /// <param name="user">user</param>
        /// <param name="password">password</param>
        public clDBInfo(string name, string server, string dbpath, string user, string password)
        {
            this.Name = name;
            this.Server = server;
            this.DBpath = dbpath;
            this.User = user;
            this.Password = password;
        }
        public clDBInfo(string propstring)
        {
            string[] tmp = propstring.Split('|');
            this.Name = tmp[0];
            FbConnectionStringBuilder csb = new FbConnectionStringBuilder(tmp[1]);
           // csb.Dialect = 1;
                this.User = csb.UserID;
                this.Password = csb.Password;
                this.DBpath = csb.Database;
                this.Server = csb.DataSource;
        }
        public clDBInfo()
        { 
        }

        public override string ToString()
        {
            FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
            csb.UserID = this.User;
            csb.Password = this.Password;
            csb.Database = this.DBpath;
            csb.DataSource = this.Server;
            return Name + "|" + csb.ToString();
        }

        public string GetFbConnStr()
        {
            FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
            csb.UserID = this.User;
            csb.Password = this.Password;
            csb.Database = this.DBpath;
            csb.DataSource = this.Server;
            return csb.ToString();
        
        }
        /// <summary>
        /// Читаем сохранённые настройки
        /// </summary>
        /// <param name="prop">кормим параметр с настройкиами System.Collections.Specialized.StringCollection</param>
        /// <returns></returns>
        public static List<clDBInfo> ReadFromProp(System.Collections.Specialized.StringCollection prop)
        {
            List<clDBInfo> res = new List<clDBInfo>();
            if (prop != null)
            {
                foreach (string pr in prop)
                {
                    res.Add(new clDBInfo(pr));
                }
            }
            return res;
        }
        /// <summary>
        /// Читаем настройки из ПК СП
        /// </summary>
        /// <param name="filepath">путь к серверной папки ПК СП</param>
        /// <returns></returns>
        public static List<clDBInfo> ReadExProdLST(string filepath)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                List<clDBInfo> res = new List<clDBInfo>();
                using (StreamReader sr = new StreamReader(filepath + @"\ex_prod.lst", Encoding.Default))
                {
                    String l1, l2;
                    l1 = l2 = "";
                    int i = 0;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((l2 = sr.ReadLine()) != null)
                    {

                        if ((++i % 2) == 0)
                        {
                            //we have l1 - name; and l2 - data for parse
                            clDBInfo tmp = new clDBInfo(l1, null, null, null, null);
                            string[] stmp = l2.Split(':');
                            switch (stmp.Length)
                            {
                                case 1: //only alias?
                                    tmp.DBpath = l2;
                                    tmp.Server = "localhost";
                                    break;
                                case 2: //1 : - server:alias or drive:path
                                    if (stmp[1][0].Equals('\\')) //not alias - path?
                                    {
                                        tmp.DBpath = l2;
                                        tmp.Server = "localhost";
                                    }
                                    else
                                    {
                                        tmp.DBpath = stmp[1];
                                        tmp.Server = stmp[0];

                                    }
                                    break;
                                case 3: //server:drive:path
                                    {
                                        tmp.Server = stmp[0];
                                        tmp.DBpath = stmp[1] + ":" + stmp[2];
                                    }

                                    break;
                                default:
                                    break;
                            }
                            res.Add(tmp);
                        } l1 = l2;
                    }
                }

                using (StreamReader sr = new StreamReader(filepath + @"\baseparam.txt", Encoding.Default))
                {
                    String l2 = "";
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((l2 = sr.ReadLine()) != null)
                    {
                        if (l2.Contains("user_name="))
                        {
                            l2 = l2.Replace("user_name=", "");
                            foreach (clDBInfo it in res)
                            {
                                it.User = l2;
                            }
                        }
                        if (l2.Contains("password="))
                        {
                            l2 = l2.Replace("password=", "");
                            foreach (clDBInfo it in res)
                            {
                                it.Password = l2;
                            }

                        }
                    }
                }


                return res;
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                MessageBox.Show(e.Message);
                return null;
            }
        }
    }
     
    [Serializable]
    public class FieldInfo
    {
        public string Info = "Поле";
        public string Name = "unknown";

        /// <summary>
        /// Данные о поле в БД
        /// </summary>
        /// <param name="info">Описание поля</param>
        /// <param name="name">Имя поля</param>
        public FieldInfo(string info, string name)
        {
            Info = info;
            Name = name;
        }

        public FieldInfo()
        {
        }

        /// <summary>
        /// Вывод описания поля
        /// </summary>
        /// <returns>описание поля в БД</returns>
        public override string ToString()
        {
            return Info;
        }
        public string DragDrop { get {return string.Format("{0}\t{1}",Name,Info);} }
        public string dragdrop()
        {
            return string.Format("{0}\t{1}",Name,Info);
        }

        public static List<FieldInfo> ReadFromRes()
        {
            return ReadFromString(Properties.Resources.fields);
        }

        public static List<FieldInfo> ReadFromString(string source)
        {
            List<FieldInfo> res = new List<FieldInfo>();

            string[] flds = source.Split(new string[2] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in flds)
            {
                string[] tmp = s.Split('\t');
                res.Add(new FieldInfo(tmp[1], tmp[0]));
            }
            return res;

        }

        public static string List2str(List<FieldInfo> source)
        {
            string res = null;
            foreach (FieldInfo itm in source)
            {
                res+=itm.DragDrop+"\n\r";
            }
            return res.TrimEnd(new char[] { ',' });
        
        }
        public static string List2l(List<FieldInfo> source)
        {
            string res = "";
            foreach (FieldInfo itm in source)
            {
                res += itm.Name + ",";
            }
            return res.TrimEnd(new char[] { ',' });
        }

        public static string List2q(List<FieldInfo> source)
        {
            return "select "+List2l(source)+" from ip";
        }
        public static string List2qc(List<FieldInfo> source)
        {
            return "select count(" + List2l(source) + ") from ip";
        }

    
    }
  
    
}
