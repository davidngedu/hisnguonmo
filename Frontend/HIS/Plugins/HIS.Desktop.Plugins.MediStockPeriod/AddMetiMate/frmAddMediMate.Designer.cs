﻿namespace HIS.Desktop.Plugins.MediStockPeriod.AddMetiMate
{
    partial class frmAddMediMate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddMediMate));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.GridControlMediMate = new Inventec.Desktop.CustomControl.MyGridControl();
            this.gridViewMediMate = new Inventec.Desktop.CustomControl.MyGridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMETY_MATY_CODEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSERVICE_UNIT_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMETY_MATY_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ButtonEdit_add = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.SpinEdit_Amount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridControlMediMate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMediMate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEdit_add)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpinEdit_Amount)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.GridControlMediMate);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(964, 223);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // GridControlMediMate
            // 
            this.GridControlMediMate.Location = new System.Drawing.Point(4, 4);
            this.GridControlMediMate.MainView = this.gridViewMediMate;
            this.GridControlMediMate.Name = "GridControlMediMate";
            this.GridControlMediMate.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.ButtonEdit_add,
            this.SpinEdit_Amount});
            this.GridControlMediMate.Size = new System.Drawing.Size(956, 215);
            this.GridControlMediMate.TabIndex = 4;
            this.GridControlMediMate.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMediMate});
            // 
            // gridViewMediMate
            // 
            this.gridViewMediMate.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn10,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.colUnb,
            this.colMETY_MATY_CODEUnb,
            this.colSERVICE_UNIT_NAMEUnb,
            this.colMETY_MATY_NAMEUnb});
            this.gridViewMediMate.GridControl = this.GridControlMediMate;
            this.gridViewMediMate.Name = "gridViewMediMate";
            this.gridViewMediMate.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
            this.gridViewMediMate.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
            this.gridViewMediMate.OptionsView.ShowAutoFilterRow = true;
            this.gridViewMediMate.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewMediMate.OptionsView.ShowDetailButtons = false;
            this.gridViewMediMate.OptionsView.ShowGroupPanel = false;
            this.gridViewMediMate.OptionsView.ShowIndicator = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Mã";
            this.gridColumn1.FieldName = "METY_MATY_CODE";
            this.gridColumn1.FieldNameSortGroup = "METY_MATY_CODEUnb";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 87;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tên";
            this.gridColumn2.FieldName = "METY_MATY_NAME";
            this.gridColumn2.FieldNameSortGroup = "METY_MATY_NAMEUnb";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 102;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Đơn vị tính";
            this.gridColumn3.FieldName = "SERVICE_UNIT_NAME";
            this.gridColumn3.FieldNameSortGroup = "SERVICE_UNIT_NAMEUnb";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            this.gridColumn3.Width = 102;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Ngày nhập";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            this.gridColumn4.Width = 102;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Gói thầu";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 5;
            this.gridColumn5.Width = 102;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Số lô";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 6;
            this.gridColumn6.Width = 102;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Số đăng ký";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 7;
            this.gridColumn7.Width = 102;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Hạn sử dụng";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 8;
            this.gridColumn8.Width = 102;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Số lượng";
            this.gridColumn9.ColumnEdit = this.SpinEdit_Amount;
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 9;
            this.gridColumn9.Width = 123;
            // 
            // colUnb
            // 
            this.colUnb.FieldName = "Unb";
            this.colUnb.Name = "colUnb";
            this.colUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colMETY_MATY_CODEUnb
            // 
            this.colMETY_MATY_CODEUnb.FieldName = "METY_MATY_CODEUnb";
            this.colMETY_MATY_CODEUnb.Name = "colMETY_MATY_CODEUnb";
            this.colMETY_MATY_CODEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colSERVICE_UNIT_NAMEUnb
            // 
            this.colSERVICE_UNIT_NAMEUnb.FieldName = "SERVICE_UNIT_NAMEUnb";
            this.colSERVICE_UNIT_NAMEUnb.Name = "colSERVICE_UNIT_NAMEUnb";
            this.colSERVICE_UNIT_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlGroup1.Size = new System.Drawing.Size(964, 223);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.GridControlMediMate;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(960, 219);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "gridColumn10";
            this.gridColumn10.ColumnEdit = this.ButtonEdit_add;
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.OptionsColumn.ShowCaption = false;
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 0;
            this.gridColumn10.Width = 30;
            // 
            // colMETY_MATY_NAMEUnb
            // 
            this.colMETY_MATY_NAMEUnb.FieldName = "METY_MATY_NAMEUnb";
            this.colMETY_MATY_NAMEUnb.Name = "colMETY_MATY_NAMEUnb";
            this.colMETY_MATY_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // ButtonEdit_add
            // 
            this.ButtonEdit_add.AutoHeight = false;
            this.ButtonEdit_add.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("ButtonEdit_add.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.ButtonEdit_add.Name = "ButtonEdit_add";
            this.ButtonEdit_add.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.ButtonEdit_add_ButtonClick);
            // 
            // SpinEdit_Amount
            // 
            this.SpinEdit_Amount.AutoHeight = false;
            this.SpinEdit_Amount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.SpinEdit_Amount.Name = "SpinEdit_Amount";
            // 
            // frmAddMediMate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 223);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmAddMediMate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bổ sung thuốc có kì kiểm kê nhưng không có trên phần mềm";
            this.Load += new System.EventHandler(this.frmAddMetiMate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridControlMediMate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMediMate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEdit_add)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpinEdit_Amount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private Inventec.Desktop.CustomControl.MyGridControl GridControlMediMate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn colUnb;
        private Inventec.Desktop.CustomControl.MyGridView gridViewMediMate;
        private DevExpress.XtraGrid.Columns.GridColumn colMETY_MATY_CODEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colSERVICE_UNIT_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit ButtonEdit_add;
        private DevExpress.XtraGrid.Columns.GridColumn colMETY_MATY_NAMEUnb;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit SpinEdit_Amount;
    }
}