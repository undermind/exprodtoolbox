using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
//using System.Web.Mail;

namespace exprodtoolbox
{
    internal class ThreadExceptionHandler
    {
        /// 
        /// Handles the thread exception.
        /// 
        public void Application_ThreadException(
            object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                string fn = "";
                //write exception2 log
                try
                {

                    try
                    {
                        string capt;

                        if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                        {
                            System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                            capt = ad.CurrentVersion.ToString();
                        }
                        else capt = Assembly.GetExecutingAssembly().GetName().Version.ToString() + " (local)";

                            string err = string.Format("{0};{2}; {1}", DateTime.Now, e.Exception.ToString(),capt);
                            if (ThreadExceptionHandler.SendErrorMessage(err)) fn = "::WWW::"; else throw new Exception(err);
                        
                    }
                    catch (Exception excp)
                    {
                        MessageBox.Show(string.Format("error sending data:{0}\n Saving to file...", excp.Message));

                        using (FileStream fs = new FileStream(Properties.Settings.Default.ErrorLogName, FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            StreamWriter wr = new StreamWriter(fs);
                            wr.WriteLine(DateTime.Now);
                            wr.WriteLine(e.Exception.ToString());
                            wr.WriteLine("-".PadLeft(128, '-'));
                            wr.Flush();
                            wr.Close();
                            fn = fs.Name;
                        }
                    }

                }
                catch { MessageBox.Show("Exception log saving denied!"); }
                // Exit the program if the user clicks Abort.

                DialogResult result = ShowThreadExceptionDialog(e.Exception, (!string.IsNullOrEmpty(fn) ? (fn.Equals("::WWW::")?"Отчёт отправлен разработчику.":string.Format("Отчёт сохранён в {0}", fn)) : "Отчёт не сохранён."));
                if (result == DialogResult.Retry)
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Application.Exit();
                }
                if (result == DialogResult.Abort)
                    Application.Exit();
                
            }
            catch
            {
                // Fatal error, terminate program

                try
                {
                    MessageBox.Show("Epic fail...",
                        "Epic fail...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        /// 
        /// Creates and displays the error message.
        /// 
        private static DialogResult ShowThreadExceptionDialog(Exception ex, string message)
        {
            string errorMessage =
                "Исключительная ситуация:\n" + ex.Message + "\n" + message + "\n";

            return MessageBox.Show(errorMessage,
                "Epic fail...",
                MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop,MessageBoxDefaultButton.Button3);
        }

        public static bool SendErrorMessage(string message)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                UriBuilder ub = new UriBuilder(string.Format(Properties.Resources.ErrorReportURL, message));
                string fr =wc.DownloadString(ub.Uri);
                //MessageBox.Show("Error report sent: OK");
                return fr.StartsWith("OK.");
            }

        }

    } // End ThreadExceptionHandler
}