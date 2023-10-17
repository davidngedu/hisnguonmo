        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MobaCabinetCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.MobaCabinetCreate.frmMobaCabinetCreate__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboImpMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.cboImpMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Stt.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MedicineTypName.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_MedicineTypName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_MobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_Amount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_CanMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_BidNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMedicine_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMedicine_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Stt.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_MaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_MobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_MobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_Amount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_CanMobaAmount.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_CanMobaAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_ImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_BidNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_BidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_RegisterNumber.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_RegisterNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpMestMaterial_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.gridColumn_ExpMestMaterial_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalPrice.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutTotalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalFeePrice.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutTotalFeePrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTotalVatPrice.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutTotalVatPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMobaCabinetCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
