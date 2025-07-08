namespace Server
{
    partial class FileTree
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
            components = new System.ComponentModel.Container();
            treeViewArchivos = new TreeView();
            panel1 = new Panel();
            button1 = new Button();
            label1 = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            descargarToolStripMenuItem = new ToolStripMenuItem();
            label2 = new Label();
            button2 = new Button();
            textBox1 = new TextBox();
            label4 = new Label();
            label5 = new Label();
            openFileDialog1 = new OpenFileDialog();
            label6 = new Label();
            textBox2 = new TextBox();
            groupBox1 = new GroupBox();
            button3 = new Button();
            panel1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // treeViewArchivos
            // 
            treeViewArchivos.Location = new Point(29, 148);
            treeViewArchivos.Name = "treeViewArchivos";
            treeViewArchivos.Size = new Size(425, 492);
            treeViewArchivos.TabIndex = 0;
            treeViewArchivos.AfterSelect += treeView1_AfterSelect;
            // 
            // panel1
            // 
            panel1.BackColor = Color.DodgerBlue;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(-2, -1);
            panel1.Name = "panel1";
            panel1.Size = new Size(933, 77);
            panel1.TabIndex = 1;
            // 
            // button1
            // 
            button1.BackColor = Color.Red;
            button1.FlatAppearance.BorderColor = Color.Red;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = SystemColors.Control;
            button1.Location = new Point(886, 10);
            button1.Name = "button1";
            button1.Size = new Size(35, 29);
            button1.TabIndex = 1;
            button1.Text = "X";
            button1.TextAlign = ContentAlignment.TopCenter;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(3, 10);
            label1.Name = "label1";
            label1.Size = new Size(519, 54);
            label1.TabIndex = 0;
            label1.Text = ">_ Infiltrator File Manager";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { descargarToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(146, 28);
            // 
            // descargarToolStripMenuItem
            // 
            descargarToolStripMenuItem.Name = "descargarToolStripMenuItem";
            descargarToolStripMenuItem.Size = new Size(145, 24);
            descargarToolStripMenuItem.Text = "Descargar";
            descargarToolStripMenuItem.Click += descargarToolStripMenuItem_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(29, 100);
            label2.Name = "label2";
            label2.Size = new Size(79, 31);
            label2.TabIndex = 2;
            label2.Text = "label2";
            // 
            // button2
            // 
            button2.ForeColor = SystemColors.ActiveCaptionText;
            button2.Location = new Point(402, 93);
            button2.Name = "button2";
            button2.Size = new Size(29, 29);
            button2.TabIndex = 4;
            button2.Text = "...";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox1
            // 
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(54, 97);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(342, 25);
            textBox1.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.White;
            label4.Location = new Point(130, 40);
            label4.Name = "label4";
            label4.Size = new Size(187, 31);
            label4.TabIndex = 6;
            label4.Text = "Archivo a enviar";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.Control;
            label5.Location = new Point(6, 97);
            label5.Name = "label5";
            label5.Size = new Size(42, 20);
            label5.TabIndex = 7;
            label5.Text = "Ruta";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.Control;
            label6.Location = new Point(6, 154);
            label6.Name = "label6";
            label6.Size = new Size(94, 20);
            label6.TabIndex = 8;
            label6.Text = "Ruta Cliente";
            // 
            // textBox2
            // 
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.Location = new Point(106, 152);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(325, 25);
            textBox2.TabIndex = 9;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label5);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = SystemColors.Control;
            groupBox1.Location = new Point(482, 204);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(437, 285);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "Enviar Archivos";
            // 
            // button3
            // 
            button3.ForeColor = SystemColors.ActiveCaptionText;
            button3.Location = new Point(167, 219);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 10;
            button3.Text = "Send";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // FileTree
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 42, 56);
            ClientSize = new Size(928, 662);
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(treeViewArchivos);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FileTree";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FileTree";
            Load += FileTree_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView treeViewArchivos;
        private Panel panel1;
        private Label label1;
        private Button button1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem descargarToolStripMenuItem;
        private Label label2;
        private Button button2;
        private TextBox textBox1;
        private Label label4;
        private Label label5;
        private OpenFileDialog openFileDialog1;
        private Label label6;
        private TextBox textBox2;
        private GroupBox groupBox1;
        private Button button3;
    }
}