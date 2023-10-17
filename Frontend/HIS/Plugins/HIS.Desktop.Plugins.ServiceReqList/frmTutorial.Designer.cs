﻿namespace HIS.Desktop.Plugins.ServiceReqList
{
    partial class frmTutorial
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.spinCountUsedBefore = new DevExpress.XtraEditors.SpinEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.spinDayNumber = new DevExpress.XtraEditors.SpinEdit();
            this.spinSpeed = new DevExpress.XtraEditors.SpinEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtHuongDanSuDung = new DevExpress.XtraEditors.MemoEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciSpinCountUsedBefore = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinCountUsedBefore.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDayNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSpeed.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHuongDanSuDung.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSpinCountUsedBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.spinCountUsedBefore);
            this.layoutControl1.Controls.Add(this.spinDayNumber);
            this.layoutControl1.Controls.Add(this.spinSpeed);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtHuongDanSuDung);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(723, 169);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // spinCountUsedBefore
            // 
            this.spinCountUsedBefore.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinCountUsedBefore.Location = new System.Drawing.Point(417, 2);
            this.spinCountUsedBefore.MenuManager = this.barManager1;
            this.spinCountUsedBefore.Name = "spinCountUsedBefore";
            this.spinCountUsedBefore.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinCountUsedBefore.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinCountUsedBefore.Properties.IsFloatValue = false;
            this.spinCountUsedBefore.Properties.Mask.EditMask = "N00";
            this.spinCountUsedBefore.Size = new System.Drawing.Size(88, 20);
            this.spinCountUsedBefore.StyleController = this.layoutControl1;
            this.spinCountUsedBefore.TabIndex = 8;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Lưu";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(723, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 198);
            this.barDockControlBottom.Size = new System.Drawing.Size(723, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 169);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(723, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 169);
            // 
            // spinDayNumber
            // 
            this.spinDayNumber.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinDayNumber.Location = new System.Drawing.Point(97, 2);
            this.spinDayNumber.MenuManager = this.barManager1;
            this.spinDayNumber.Name = "spinDayNumber";
            this.spinDayNumber.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spinDayNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinDayNumber.Properties.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spinDayNumber.Size = new System.Drawing.Size(61, 20);
            this.spinDayNumber.StyleController = this.layoutControl1;
            this.spinDayNumber.TabIndex = 7;
            this.spinDayNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinDayNumber_PreviewKeyDown);
            // 
            // spinSpeed
            // 
            this.spinSpeed.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinSpeed.Location = new System.Drawing.Point(97, 121);
            this.spinSpeed.MenuManager = this.barManager1;
            this.spinSpeed.Name = "spinSpeed";
            this.spinSpeed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinSpeed.Properties.MaxValue = new decimal(new int[] {
            -1981284353,
            -1966660860,
            0,
            131072});
            this.spinSpeed.Size = new System.Drawing.Size(624, 20);
            this.spinSpeed.StyleController = this.layoutControl1;
            this.spinSpeed.TabIndex = 6;
            this.spinSpeed.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinSpeed_PreviewKeyDown);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(607, 145);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtHuongDanSuDung
            // 
            this.txtHuongDanSuDung.Location = new System.Drawing.Point(97, 26);
            this.txtHuongDanSuDung.Name = "txtHuongDanSuDung";
            this.txtHuongDanSuDung.Size = new System.Drawing.Size(624, 91);
            this.txtHuongDanSuDung.StyleController = this.layoutControl1;
            this.txtHuongDanSuDung.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem2,
            this.lciSpinCountUsedBefore});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(723, 169);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.layoutControlItem1.Control = this.txtHuongDanSuDung;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(723, 95);
            this.layoutControlItem1.Text = "Hướng dẫn sử dụng:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(605, 143);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(118, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 143);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(605, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.spinSpeed;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 119);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(723, 24);
            this.layoutControlItem3.Text = "Tốc độ truyền:";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem4.Control = this.spinDayNumber;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(160, 24);
            this.layoutControlItem4.Text = "Số ngày:";
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem4.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(507, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(216, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciSpinCountUsedBefore
            // 
            this.lciSpinCountUsedBefore.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSpinCountUsedBefore.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSpinCountUsedBefore.Control = this.spinCountUsedBefore;
            this.lciSpinCountUsedBefore.Location = new System.Drawing.Point(160, 0);
            this.lciSpinCountUsedBefore.Name = "lciSpinCountUsedBefore";
            this.lciSpinCountUsedBefore.OptionsToolTip.ToolTip = "Số ngày sử dụng thuốc trước đó";
            this.lciSpinCountUsedBefore.Size = new System.Drawing.Size(347, 24);
            this.lciSpinCountUsedBefore.Text = "Số lần sử dụng trước đó:";
            this.lciSpinCountUsedBefore.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSpinCountUsedBefore.TextSize = new System.Drawing.Size(250, 20);
            this.lciSpinCountUsedBefore.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmTutorial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 198);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmTutorial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sửa thông tin chung";
            this.Load += new System.EventHandler(this.frmTutorial_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinCountUsedBefore.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDayNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSpeed.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHuongDanSuDung.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSpinCountUsedBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.MemoEdit txtHuongDanSuDung;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.SpinEdit spinSpeed;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SpinEdit spinDayNumber;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SpinEdit spinCountUsedBefore;
        private DevExpress.XtraLayout.LayoutControlItem lciSpinCountUsedBefore;
    }
}