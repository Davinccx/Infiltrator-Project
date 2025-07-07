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
            SuspendLayout();
            // 
            // treeViewArchivos
            // 
            treeViewArchivos.Location = new Point(3, 2);
            treeViewArchivos.Name = "treeViewArchivos";
            treeViewArchivos.Size = new Size(1054, 593);
            treeViewArchivos.TabIndex = 0;
            treeViewArchivos.AfterSelect += treeView1_AfterSelect;
            // 
            // FileTree
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1060, 594);
            Controls.Add(treeViewArchivos);
            Name = "FileTree";
            Text = "FileTree";
            Load += FileTree_Load;
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeViewArchivos;
    }
}