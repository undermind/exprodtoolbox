using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace exprodtoolbox
{
    //delegate byte[] DownloadData(string uri);
    public partial class DlForm : Form
    {
        //DownloadData download;
        WebClient wcl;
        byte[] result;
        public DlForm(string Source)
        {
            result = null; 
            InitializeComponent();
            llSource.Text = Source;
            llSource.Links.Add(0, Source.Length, Source);
            wcl = new WebClient();
            wcl.DownloadDataCompleted += new DownloadDataCompletedEventHandler(wcl_DownloadDataCompleted);
            wcl.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wcl_DownloadProgressChanged);
            wcl.DownloadDataAsync(new Uri(Source));
            FormBorderStyle = Properties.Settings.Default.DetailedLog ? FormBorderStyle.FixedToolWindow : FormBorderStyle.None;
        }

        void wcl_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbDownload.Value = e.ProgressPercentage;
            Text=string.Format("{0} {1} из {2} б. {3} % скачано...", e.UserState, e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
            
        }

        void wcl_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs args)
        {
            pbDownload.Value = 100;
            Text = string.Format("Загрузка завершена");
            if (args.Cancelled)
                MessageBox.Show("Отменено");
            else if (args.Error != null)
                throw args.Error;
            else
            {
                result=args.Result;
            }
            Close();
        }

        public byte[] Res()
        {
            while (wcl.IsBusy) ;
            return result;
        }

        private void DlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason==CloseReason.UserClosing)
            e.Cancel = wcl.IsBusy;
        }

        private void llSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = e.Link.LinkData as string;
            prc.Start();

        }

    }
}
