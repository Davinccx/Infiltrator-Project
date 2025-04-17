namespace Server
{
    partial class LoadingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            label1 = new Label();
            panel1 = new Panel();
            progressBar1 = new ProgressBar();
            label2 = new Label();
            label3 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Transparent;
            label1.Location = new Point(11, 20);
            label1.Name = "label1";
            label1.Size = new Size(485, 45);
            label1.TabIndex = 0;
            label1.Text = ">_ Infiltrator Project Server 1.0";
            // 
            // panel1
            // 
            panel1.BackColor = Color.DodgerBlue;
            panel1.Controls.Add(label1);
            panel1.Location = new Point(-2, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(508, 80);
            panel1.TabIndex = 1;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(96, 169);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(315, 23);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.FromArgb(154, 169, 181);
            label2.Location = new Point(175, 205);
            label2.Name = "label2";
            label2.Size = new Size(163, 15);
            label2.TabIndex = 3;
            label2.Text = "Buscando archivo config.json";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.FromArgb(211, 220, 230);
            label3.Location = new Point(140, 304);
            label3.Name = "label3";
            label3.Size = new Size(230, 25);
            label3.TabIndex = 4;
            label3.Text = "Cargando configuración";
            // 
            // LoadingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 42, 56);
            ClientSize = new Size(509, 347);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(progressBar1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoadingForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoadingForm";
            Load += LoadingForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Panel panel1;
        private ProgressBar progressBar1;
        private Label label2;
        private Label label3;
    }
}