using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace SpeechContentBGListener
{
    public partial class frmMessage : Form
    {
        System.Timers.Timer tmrTimeOut;
        
        public frmMessage(string msg, bool isError = false)
        {
            InitializeComponent();
            lblMessaqe.Text = msg;
            if (isError)
            {
                this.BackColor = Color.Red;
                lblMessaqe.ForeColor = Color.White;
            }

            tmrTimeOut = new System.Timers.Timer(5000);
            tmrTimeOut.Elapsed += TmrTimeOut_Elapsed;
            tmrTimeOut.Start();

        }

        private void TmrTimeOut_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // close the form on the forms thread
                this.Close();
                this.Dispose();
            });

        }

        private void frmMessage_Shown(object sender, EventArgs e)
        {
            
        }
    }
}
