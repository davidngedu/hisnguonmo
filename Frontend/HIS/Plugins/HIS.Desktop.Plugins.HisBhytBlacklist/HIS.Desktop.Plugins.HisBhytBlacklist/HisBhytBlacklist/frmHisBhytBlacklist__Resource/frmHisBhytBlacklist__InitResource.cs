        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBhytBlacklist.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBhytBlacklist.HIS.Desktop.Plugins.HisBhytBlacklist.HisBhytBlacklist.frmHisBhytBlacklist__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.grdColHeinCardNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardNumber.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.lciHeinCardNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisBhytBlacklist.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
