﻿namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.MessageBoxForm
{
    partial class frmMessageBoxChooseVT
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMessageBoxChooseVT));
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.btnChonThuocNgoaiKho = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDescription.Location = new System.Drawing.Point(6, 6);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(331, 48);
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Vật tư trong kho không đủ để kê. Bạn muốn sử dụng vật tư thay thế:";
            // 
            // btnChonThuocNgoaiKho
            // 
            this.btnChonThuocNgoaiKho.Location = new System.Drawing.Point(92, 60);
            this.btnChonThuocNgoaiKho.Name = "btnChonThuocNgoaiKho";
            this.btnChonThuocNgoaiKho.Size = new System.Drawing.Size(100, 23);
            this.btnChonThuocNgoaiKho.TabIndex = 2;
            this.btnChonThuocNgoaiKho.Text = "Vật tư ngoài kho";
            this.btnChonThuocNgoaiKho.Click += new System.EventHandler(this.btnChonThuocNgoaiKho_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(198, 60);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(68, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Bỏ qua";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmMessageBoxChooseVT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 88);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnChonThuocNgoaiKho);
            this.Controls.Add(this.lblDescription);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMessageBoxChooseVT";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thông báo";
            this.Load += new System.EventHandler(this.frmMessageBoxChooseMedicineTypeAcin_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.SimpleButton btnChonThuocNgoaiKho;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}