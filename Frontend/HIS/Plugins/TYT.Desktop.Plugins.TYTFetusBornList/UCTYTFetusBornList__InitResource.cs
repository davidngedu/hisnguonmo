using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;
using System.Windows.Forms;
namespace TYT.Desktop.Plugins.TYTFetusBornList
{
    public partial class UCListTYTFetusBornList : HIS.Desktop.Utility.UserControlBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("TYT.Desktop.Plugins.TYTFetusBornList.Resources.Lang", typeof(TYT.Desktop.Plugins.TYTFetusBornList.UCListTYTFetusBornList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTYTFetusExamListCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.txtTYTFetusExamListCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtMaYTeCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.txtMaYTeCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTYTFetusBornList.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}