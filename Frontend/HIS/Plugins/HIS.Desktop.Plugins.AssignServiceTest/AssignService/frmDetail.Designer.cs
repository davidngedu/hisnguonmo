﻿namespace HIS.Desktop.Plugins.AssignServiceTest.AssignService
{
    partial class frmDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDetail));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.gridControlServiceReqView = new DevExpress.XtraGrid.GridControl();
            this.gridViewServiceReqView__TabService = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grcStt__TabService = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonTabServiceViewReq_Print = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.grcPrint = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ButtonEditPrintDomSoi = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.grcServiceReqCode_TabService = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grcServiceReqTypeName_TabService = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grcIntructionTime_TabService = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grcExecuteRoomName_TabService = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grcCreator_TabService = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grcTotalAmount_TabService = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grcTotalPrice_TabService = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager2 = new DevExpress.XtraBars.BarManager();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlServiceReqView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceReqView__TabService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonTabServiceViewReq_Print)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEditPrintDomSoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager2)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControlServiceReqView
            // 
            this.gridControlServiceReqView.Location = new System.Drawing.Point(2, 2);
            this.gridControlServiceReqView.MainView = this.gridViewServiceReqView__TabService;
            this.gridControlServiceReqView.Name = "gridControlServiceReqView";
            this.gridControlServiceReqView.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonTabServiceViewReq_Print,
            this.ButtonEditPrintDomSoi});
            this.gridControlServiceReqView.Size = new System.Drawing.Size(640, 322);
            this.gridControlServiceReqView.TabIndex = 29;
            this.gridControlServiceReqView.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewServiceReqView__TabService});
            // 
            // gridViewServiceReqView__TabService
            // 
            this.gridViewServiceReqView__TabService.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grcStt__TabService,
            this.gridColumn7,
            this.grcPrint,
            this.grcServiceReqCode_TabService,
            this.grcServiceReqTypeName_TabService,
            this.grcIntructionTime_TabService,
            this.grcExecuteRoomName_TabService,
            this.grcCreator_TabService,
            this.grcTotalAmount_TabService,
            this.grcTotalPrice_TabService});
            this.gridViewServiceReqView__TabService.GridControl = this.gridControlServiceReqView;
            this.gridViewServiceReqView__TabService.Name = "gridViewServiceReqView__TabService";
            this.gridViewServiceReqView__TabService.OptionsView.ShowGroupPanel = false;
            this.gridViewServiceReqView__TabService.OptionsView.ShowIndicator = false;
            this.gridViewServiceReqView__TabService.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewServiceReqView__TabService_CustomRowCellEdit);
            this.gridViewServiceReqView__TabService.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceReqView__TabService_CustomUnboundColumnData);
            // 
            // grcStt__TabService
            // 
            this.grcStt__TabService.Caption = "STT";
            this.grcStt__TabService.FieldName = "STT";
            this.grcStt__TabService.Name = "grcStt__TabService";
            this.grcStt__TabService.OptionsColumn.AllowEdit = false;
            this.grcStt__TabService.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grcStt__TabService.Visible = true;
            this.grcStt__TabService.VisibleIndex = 0;
            this.grcStt__TabService.Width = 40;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "In phiếu cầu";
            this.gridColumn7.ColumnEdit = this.repositoryItemButtonTabServiceViewReq_Print;
            this.gridColumn7.FieldName = "gridColumn40";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.FixedWidth = true;
            this.gridColumn7.OptionsColumn.ShowCaption = false;
            this.gridColumn7.ToolTip = "In phiếu yêu cầu";
            this.gridColumn7.Width = 20;
            // 
            // repositoryItemButtonTabServiceViewReq_Print
            // 
            this.repositoryItemButtonTabServiceViewReq_Print.AutoHeight = false;
            this.repositoryItemButtonTabServiceViewReq_Print.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonTabServiceViewReq_Print.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "In phiếu yêu cầu", null, null, true)});
            this.repositoryItemButtonTabServiceViewReq_Print.Name = "repositoryItemButtonTabServiceViewReq_Print";
            this.repositoryItemButtonTabServiceViewReq_Print.NullText = " ";
            this.repositoryItemButtonTabServiceViewReq_Print.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonTabServiceViewReq_Print.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonTabServiceViewReq_Print_ButtonClick);
            // 
            // grcPrint
            // 
            this.grcPrint.ColumnEdit = this.ButtonEditPrintDomSoi;
            this.grcPrint.FieldName = "print";
            this.grcPrint.Name = "grcPrint";
            this.grcPrint.OptionsColumn.ShowCaption = false;
            this.grcPrint.Visible = true;
            this.grcPrint.VisibleIndex = 1;
            this.grcPrint.Width = 20;
            // 
            // ButtonEditPrintDomSoi
            // 
            this.ButtonEditPrintDomSoi.AutoHeight = false;
            this.ButtonEditPrintDomSoi.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("ButtonEditPrintDomSoi.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "In", null, null, true)});
            this.ButtonEditPrintDomSoi.Name = "ButtonEditPrintDomSoi";
            this.ButtonEditPrintDomSoi.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.ButtonEditPrintDomSoi.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.ButtonEditPrintDomSoi_ButtonClick);
            // 
            // grcServiceReqCode_TabService
            // 
            this.grcServiceReqCode_TabService.Caption = "Mã yêu cầu";
            this.grcServiceReqCode_TabService.FieldName = "SERVICE_REQ_CODE";
            this.grcServiceReqCode_TabService.Name = "grcServiceReqCode_TabService";
            this.grcServiceReqCode_TabService.OptionsColumn.AllowEdit = false;
            this.grcServiceReqCode_TabService.Visible = true;
            this.grcServiceReqCode_TabService.VisibleIndex = 2;
            this.grcServiceReqCode_TabService.Width = 107;
            // 
            // grcServiceReqTypeName_TabService
            // 
            this.grcServiceReqTypeName_TabService.Caption = "Loại yêu cầu";
            this.grcServiceReqTypeName_TabService.FieldName = "SERVICE_REQ_TYPE_NAME";
            this.grcServiceReqTypeName_TabService.Name = "grcServiceReqTypeName_TabService";
            this.grcServiceReqTypeName_TabService.OptionsColumn.AllowEdit = false;
            this.grcServiceReqTypeName_TabService.Width = 163;
            // 
            // grcIntructionTime_TabService
            // 
            this.grcIntructionTime_TabService.Caption = "Thời gian yêu cầu";
            this.grcIntructionTime_TabService.FieldName = "INTRUCTION_TIME_DISPLAY";
            this.grcIntructionTime_TabService.Name = "grcIntructionTime_TabService";
            this.grcIntructionTime_TabService.OptionsColumn.AllowEdit = false;
            this.grcIntructionTime_TabService.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grcIntructionTime_TabService.Visible = true;
            this.grcIntructionTime_TabService.VisibleIndex = 3;
            this.grcIntructionTime_TabService.Width = 106;
            // 
            // grcExecuteRoomName_TabService
            // 
            this.grcExecuteRoomName_TabService.Caption = "Phòng xử lý";
            this.grcExecuteRoomName_TabService.FieldName = "EXECUTE_ROOM_NAME";
            this.grcExecuteRoomName_TabService.Name = "grcExecuteRoomName_TabService";
            this.grcExecuteRoomName_TabService.OptionsColumn.AllowEdit = false;
            this.grcExecuteRoomName_TabService.Visible = true;
            this.grcExecuteRoomName_TabService.VisibleIndex = 4;
            this.grcExecuteRoomName_TabService.Width = 176;
            // 
            // grcCreator_TabService
            // 
            this.grcCreator_TabService.Caption = "Người yêu cầu";
            this.grcCreator_TabService.FieldName = "REQUEST_USERNAME";
            this.grcCreator_TabService.Name = "grcCreator_TabService";
            this.grcCreator_TabService.OptionsColumn.AllowEdit = false;
            this.grcCreator_TabService.Visible = true;
            this.grcCreator_TabService.VisibleIndex = 5;
            this.grcCreator_TabService.Width = 85;
            // 
            // grcTotalAmount_TabService
            // 
            this.grcTotalAmount_TabService.Caption = "Số lượng dịch vụ";
            this.grcTotalAmount_TabService.FieldName = "TOTAL_AMOUNT";
            this.grcTotalAmount_TabService.Name = "grcTotalAmount_TabService";
            this.grcTotalAmount_TabService.OptionsColumn.AllowEdit = false;
            this.grcTotalAmount_TabService.Width = 73;
            // 
            // grcTotalPrice_TabService
            // 
            this.grcTotalPrice_TabService.Caption = "Tổng tiền";
            this.grcTotalPrice_TabService.FieldName = "TOTAL_PRICE";
            this.grcTotalPrice_TabService.Name = "grcTotalPrice_TabService";
            this.grcTotalPrice_TabService.OptionsColumn.AllowEdit = false;
            this.grcTotalPrice_TabService.Width = 98;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlServiceReqView);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(644, 326);
            this.layoutControl1.TabIndex = 30;
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
            this.layoutControlGroup1.Size = new System.Drawing.Size(644, 326);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlServiceReqView;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(644, 326);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // barManager2
            // 
            this.barManager2.DockControls.Add(this.barDockControl1);
            this.barManager2.DockControls.Add(this.barDockControl2);
            this.barManager2.DockControls.Add(this.barDockControl3);
            this.barManager2.DockControls.Add(this.barDockControl4);
            this.barManager2.Form = this;
            this.barManager2.MaxItemId = 0;
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 0);
            this.barDockControl1.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 326);
            this.barDockControl2.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 0);
            this.barDockControl3.Size = new System.Drawing.Size(0, 326);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(644, 0);
            this.barDockControl4.Size = new System.Drawing.Size(0, 326);
            // 
            // frmDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 326);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chi tiết";
            this.Load += new System.EventHandler(this.frmDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlServiceReqView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceReqView__TabService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonTabServiceViewReq_Print)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonEditPrintDomSoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControlServiceReqView;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewServiceReqView__TabService;
        private DevExpress.XtraGrid.Columns.GridColumn grcStt__TabService;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonTabServiceViewReq_Print;
        private DevExpress.XtraGrid.Columns.GridColumn grcPrint;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit ButtonEditPrintDomSoi;
        private DevExpress.XtraGrid.Columns.GridColumn grcServiceReqCode_TabService;
        private DevExpress.XtraGrid.Columns.GridColumn grcServiceReqTypeName_TabService;
        private DevExpress.XtraGrid.Columns.GridColumn grcIntructionTime_TabService;
        private DevExpress.XtraGrid.Columns.GridColumn grcExecuteRoomName_TabService;
        private DevExpress.XtraGrid.Columns.GridColumn grcCreator_TabService;
        private DevExpress.XtraGrid.Columns.GridColumn grcTotalAmount_TabService;
        private DevExpress.XtraGrid.Columns.GridColumn grcTotalPrice_TabService;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarManager barManager2;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
    }
}