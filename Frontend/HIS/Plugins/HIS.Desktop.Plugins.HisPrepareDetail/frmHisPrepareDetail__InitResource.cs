using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;
namespace HIS.Desktop.Plugins.HisPrepareDetail
{
    public partial class frmHisPrepareDetail : HIS.Desktop.Utility.FormBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisPrepareDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPrepareDetail.frmHisPrepareDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnServiceUnitName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnServiceUnitName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnReqAmount.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnReqAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnReqAmount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnReqAmount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnApprovalAmount.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnApprovalAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnApprovalAmount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnApprovalAmount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnReqUserName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnReqUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnApprovalLoginname.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnApprovalLoginname.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnNationalName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnNationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnManufacturerName.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewColumnConcentra.Caption = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.gridViewColumnConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUseFrom.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.lciUseFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUseTo.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.lciUseTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciApprovalLogginName.Text = Inventec.Common.Resource.Get.Value("frmHisPrepareDetail.lciApprovalLogginName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
                this.Text = this.currentModule != null ? this.currentModule.text : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}