namespace exprodtoolbox
{
    partial class frmSelectColums
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbSelect = new System.Windows.Forms.ListBox();
            this.lbSource = new System.Windows.Forms.ListBox();
            this.bOK = new System.Windows.Forms.Button();
            this.cbSETS = new System.Windows.Forms.ComboBox();
            this.cbOP = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lbSelect, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbSource, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.bOK, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbSETS, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbOP, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(404, 262);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lbSelect
            // 
            this.lbSelect.AllowDrop = true;
            this.lbSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSelect.FormattingEnabled = true;
            this.lbSelect.Location = new System.Drawing.Point(225, 3);
            this.lbSelect.Name = "lbSelect";
            this.lbSelect.Size = new System.Drawing.Size(176, 229);
            this.lbSelect.TabIndex = 0;
            this.lbSelect.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragDrop);
            this.lbSelect.DragOver += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragOver);
            this.lbSelect.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DD_MouseDown);
            // 
            // lbSource
            // 
            this.lbSource.AllowDrop = true;
            this.lbSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSource.FormattingEnabled = true;
            this.lbSource.Location = new System.Drawing.Point(3, 3);
            this.lbSource.Name = "lbSource";
            this.lbSource.Size = new System.Drawing.Size(176, 229);
            this.lbSource.TabIndex = 1;
            this.lbSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragDrop);
            this.lbSource.DragOver += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragOver);
            this.lbSource.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DD_MouseDown);
            // 
            // bOK
            // 
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(225, 238);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(176, 21);
            this.bOK.TabIndex = 2;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // cbSETS
            // 
            this.cbSETS.FormattingEnabled = true;
            this.cbSETS.Location = new System.Drawing.Point(3, 238);
            this.cbSETS.Name = "cbSETS";
            this.cbSETS.Size = new System.Drawing.Size(176, 21);
            this.cbSETS.TabIndex = 3;
            // 
            // cbOP
            // 
            this.cbOP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOP.FormattingEnabled = true;
            this.cbOP.Items.AddRange(new object[] {
            "NON",
            "SAVE",
            "LOAD",
            "DEL"});
            this.cbOP.Location = new System.Drawing.Point(185, 238);
            this.cbOP.Name = "cbOP";
            this.cbOP.Size = new System.Drawing.Size(34, 21);
            this.cbOP.TabIndex = 4;
            // 
            // frmSelectColums
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 262);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmSelectColums";
            this.Text = "Набор столбцов";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.ListBox lbSelect;
        public System.Windows.Forms.ListBox lbSource;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.ComboBox cbSETS;
        private System.Windows.Forms.ComboBox cbOP;
    }
}