using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace exprodtoolbox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ThreadExceptionHandler handler = new ThreadExceptionHandler();

            Application.ThreadException +=
                new ThreadExceptionEventHandler(handler.Application_ThreadException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
