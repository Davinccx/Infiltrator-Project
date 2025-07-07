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
            treeViewArchivos = new TreeView();
            panel1 = new Panel();
            button1 = new Button();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // treeViewArchivos
            // 
            treeViewArchivos.Location = new Point(12, 163);
            treeViewArchivos.Name = "treeViewArchivos";
            treeViewArchivos.Size = new Size(456, 582);
            treeViewArchivos.TabIndex = 0;
            treeViewArchivos.AfterSelect += treeView1_AfterSelect;
            // 
            // panel1
            // 
            panel1.BackColor = Color.DodgerBlue;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(1, -1);
            panel1.Name = "panel1";
            panel1.Size = new Size(873, 138);
            panel1.TabIndex = 1;
            // 
            // button1
            // 
            button1.BackColor = Color.Red;
            button1.FlatStyle = FlatStyle.Popup;
            button1.ForeColor = SystemColors.Control;
            button1.Location = new Point(838, 6);
            button1.Name = "button1";
            button1.Size = new Size(28, 29);
            button1.TabIndex = 1;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(172, 42);
            label1.Name = "label1";
            label1.Size = new Size(519, 54);
            label1.TabIndex = 0;
            label1.Text = ">_ Infiltrator File Manager";
            // 
            // FileTree
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 42, 56);
            ClientSize = new Size(876, 757);
            Controls.Add(panel1);
            Controls.Add(treeViewArchivos);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FileTree";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FileTree";
            Load += FileTree_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeViewArchivos;
        private Panel panel1;
        private Label label1;
        private Button button1;
    }
}