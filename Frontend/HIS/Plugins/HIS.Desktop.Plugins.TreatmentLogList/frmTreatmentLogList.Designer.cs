﻿namespace HIS.Desktop.Plugins.TreatmentLogList
{
  partial class frmTreatmentLogList
 {
  /// <summary>
  /// Required designer variable.
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  /// <summary>
  /// Clean up any resources being used.
  /// </summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
 protected   override void Dispose(bool disposing)
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
   System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTreatmentLogList));
   this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
   this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
   this.xtraUserControl1 = new DevExpress.XtraEditors.XtraUserControl();
   this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
   this.layoutControl1.SuspendLayout();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
   this.SuspendLayout();
   // 
   // layoutControl1
   // 
   this.layoutControl1.Controls.Add(this.xtraUserControl1);
   this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
   this.layoutControl1.Location = new System.Drawing.Point(0, 0);
   this.layoutControl1.Name = "layoutControl1";
   this.layoutControl1.Root = this.layoutControlGroup1;
   this.layoutControl1.Size = new System.Drawing.Size(1084, 512);
   this.layoutControl1.TabIndex = 0;
   this.layoutControl1.Text = "layoutControl1";
   // 
   // layoutControlGroup1
   // 
   this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
   this.layoutControlGroup1.GroupBordersVisible = false;
   this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
   this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
   this.layoutControlGroup1.Name = "layoutControlGroup1";
   this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
   this.layoutControlGroup1.Size = new System.Drawing.Size(1084, 512);
   this.layoutControlGroup1.TextVisible = false;
   // 
   // xtraUserControl1
   // 
   this.xtraUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
   this.xtraUserControl1.Location = new System.Drawing.Point(2, 2);
   this.xtraUserControl1.Name = "xtraUserControl1";
   this.xtraUserControl1.Size = new System.Drawing.Size(1080, 508);
   this.xtraUserControl1.TabIndex = 4;
   // 
   // layoutControlItem1
   // 
   this.layoutControlItem1.Control = this.xtraUserControl1;
   this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
   this.layoutControlItem1.Name = "layoutControlItem1";
   this.layoutControlItem1.Size = new System.Drawing.Size(1084, 512);
   this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
   this.layoutControlItem1.TextVisible = false;
   // 
   // frmTreatmentLogList
   // 
   this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
   this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
   this.ClientSize = new System.Drawing.Size(1084, 512);
   this.Controls.Add(this.layoutControl1);
   this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
   this.Name = "frmTreatmentLogList";
   this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
   this.Text = "Dòng thời gian";
   ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
   this.layoutControl1.ResumeLayout(false);
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
   this.ResumeLayout(false);

  }

  #endregion

  public DevExpress.XtraLayout.LayoutControl layoutControl1;
  public DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
  private DevExpress.XtraEditors.XtraUserControl xtraUserControl1;
  private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
 }
}