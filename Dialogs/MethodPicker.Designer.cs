namespace Interfaciator.Dialogs
{
    partial class MethodPicker
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.methodList = new System.Windows.Forms.CheckedListBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDeselect = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.savePath = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtPackage = new System.Windows.Forms.TextBox();
            this.grpName = new System.Windows.Forms.GroupBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.savePath.SuspendLayout();
            this.grpName.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.methodList);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.groupBox1.Size = new System.Drawing.Size(384, 175);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Methods";
            // 
            // methodList
            // 
            this.methodList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.methodList.CheckOnClick = true;
            this.methodList.FormattingEnabled = true;
            this.methodList.Location = new System.Drawing.Point(6, 15);
            this.methodList.Name = "methodList";
            this.methodList.Size = new System.Drawing.Size(372, 154);
            this.methodList.Sorted = true;
            this.methodList.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(316, 328);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "Generate";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(235, 328);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnDeselect
            // 
            this.btnDeselect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeselect.Location = new System.Drawing.Point(316, 193);
            this.btnDeselect.Name = "btnDeselect";
            this.btnDeselect.Size = new System.Drawing.Size(75, 23);
            this.btnDeselect.TabIndex = 2;
            this.btnDeselect.Text = "Deselect all";
            this.btnDeselect.UseVisualStyleBackColor = true;
            this.btnDeselect.Click += new System.EventHandler(this.btnDeselect_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(235, 193);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select all";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // savePath
            // 
            this.savePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.savePath.Controls.Add(this.btnSearch);
            this.savePath.Controls.Add(this.txtPackage);
            this.savePath.Location = new System.Drawing.Point(13, 222);
            this.savePath.Name = "savePath";
            this.savePath.Size = new System.Drawing.Size(384, 47);
            this.savePath.TabIndex = 5;
            this.savePath.TabStop = false;
            this.savePath.Text = "Package";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(303, 18);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search...";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtPackage
            // 
            this.txtPackage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPackage.Location = new System.Drawing.Point(6, 20);
            this.txtPackage.Name = "txtPackage";
            this.txtPackage.ReadOnly = true;
            this.txtPackage.Size = new System.Drawing.Size(291, 20);
            this.txtPackage.TabIndex = 0;
            // 
            // grpName
            // 
            this.grpName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpName.Controls.Add(this.txtName);
            this.grpName.Location = new System.Drawing.Point(13, 275);
            this.grpName.Name = "grpName";
            this.grpName.Size = new System.Drawing.Size(384, 47);
            this.grpName.TabIndex = 6;
            this.grpName.TabStop = false;
            this.grpName.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Location = new System.Drawing.Point(6, 20);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(372, 20);
            this.txtName.TabIndex = 0;
            // 
            // MethodPicker
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(409, 363);
            this.Controls.Add(this.grpName);
            this.Controls.Add(this.savePath);
            this.Controls.Add(this.btnDeselect);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Name = "MethodPicker";
            this.Text = "MethodPicker";
            this.Load += new System.EventHandler(this.MethodPicker_Load);
            this.groupBox1.ResumeLayout(false);
            this.savePath.ResumeLayout(false);
            this.savePath.PerformLayout();
            this.grpName.ResumeLayout(false);
            this.grpName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.CheckedListBox methodList;
        private System.Windows.Forms.Button btnDeselect;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.GroupBox savePath;
        private System.Windows.Forms.TextBox txtPackage;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grpName;
        private System.Windows.Forms.TextBox txtName;
    }
}