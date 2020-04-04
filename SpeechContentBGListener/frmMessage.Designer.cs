namespace SpeechContentBGListener
{
    partial class frmMessage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMessaqe = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMessaqe
            // 
            this.lblMessaqe.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblMessaqe.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblMessaqe.Location = new System.Drawing.Point(12, 9);
            this.lblMessaqe.Name = "lblMessaqe";
            this.lblMessaqe.Size = new System.Drawing.Size(511, 76);
            this.lblMessaqe.TabIndex = 0;
            this.lblMessaqe.Text = "Добавлен текст: ГОНКИ выживание формула тра лял ля ля ля что еж делать как же быт" +
    "ь";
            // 
            // frmMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(535, 94);
            this.ControlBox = false;
            this.Controls.Add(this.lblMessaqe);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(1100, 700);
            this.Name = "frmMessage";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.frmMessage_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMessaqe;
    }
}