﻿namespace HIS.Desktop.Plugins.TransactionDepositDetail
{
    partial class frmTransactionDepositDetail
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
            this.panelControlSereServTree = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlSereServTree)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControlSereServTree
            // 
            this.panelControlSereServTree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlSereServTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlSereServTree.Location = new System.Drawing.Point(0, 0);
            this.panelControlSereServTree.Margin = new System.Windows.Forms.Padding(0);
            this.panelControlSereServTree.Name = "panelControlSereServTree";
            this.panelControlSereServTree.Size = new System.Drawing.Size(1239, 562);
            this.panelControlSereServTree.TabIndex = 0;
            // 
            // frmTransactionDepositDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1239, 562);
            this.Controls.Add(this.panelControlSereServTree);
            this.Name = "frmTransactionDepositDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chi tiết tạm ứng";
            this.Load += new System.EventHandler(this.frmTransactionDepositDetail_Load);
            this.Controls.SetChildIndex(this.panelControlSereServTree, 0);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlSereServTree)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControlSereServTree;
    }
}