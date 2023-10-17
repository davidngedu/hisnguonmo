﻿using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using MOS.SDO;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        private void UpdateControlEditorTime(DevExpress.XtraEditors.ButtonEdit txtEditorTime, DevExpress.XtraEditors.DateEdit dtEditorTime)
        {
            try
            {
                string strtxtIntructionTime = "";
                if (txtEditorTime.Text.Length == 2 || txtEditorTime.Text.Length == 1)
                {
                    strtxtIntructionTime = "01/01/" + (DateTime.Now.Year - Inventec.Common.TypeConvert.Parse.ToInt64(txtEditorTime.Text)).ToString();
                }
                else if (txtEditorTime.Text.Length == 4)
                    strtxtIntructionTime = "01/01/" + txtEditorTime.Text;
                else if (txtEditorTime.Text.Length == 8)
                {
                    strtxtIntructionTime = txtEditorTime.Text.Substring(0, 2) + "/" + txtEditorTime.Text.Substring(2, 2) + "/" + txtEditorTime.Text.Substring(4, 4);
                }
                else
                    strtxtIntructionTime = txtEditorTime.Text;

                dtEditorTime.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strtxtIntructionTime);
                dtEditorTime.Update();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void CheckTTFull(HeinCardData heinCard)
        {
            try
            {
                if (!HisConfigCFG.IsCheckExamHistory) return;
                if (this.isNotCheckTT) { return; }
                if (heinCard == null) heinCard = new HeinCardData();

                heinCard.Dob = txtPatientDob.Text;
                heinCard.PatientName = txtPatientName.Text.Trim();
                var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboGender.EditValue ?? "0").ToString()));
                heinCard.Gender = (gender != null ? GenderConvert.HisToHein(gender.ID.ToString()) : "2");

                if (String.IsNullOrEmpty(heinCard.HeinCardNumber)
                    // || String.IsNullOrEmpty(heinCard.Address)
                        || String.IsNullOrEmpty(heinCard.FromDate)
                        || String.IsNullOrEmpty(heinCard.MediOrgCode))
                {
                    if (this.ucHeinBHYT != null && mainHeinProcessor != null)
                    {
                        HisPatientProfileSDO patientProfileSDO = new HisPatientProfileSDO();
                        mainHeinProcessor.UpdateDataFormIntoPatientTypeAlter(this.ucHeinBHYT, patientProfileSDO);
                        if (patientProfileSDO != null
                            && patientProfileSDO.HisPatientTypeAlter != null
                            && !String.IsNullOrEmpty(patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER)
                            )
                        {
                            heinCard.HeinCardNumber = patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER;
                            heinCard.Address = patientProfileSDO.HisPatientTypeAlter.ADDRESS;
                            heinCard.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_FROM_TIME.ToString());
                            heinCard.MediOrgCode = patientProfileSDO.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                            heinCard.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_TO_TIME.ToString());
                            if (patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME != null && patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME > 0)
                            {
                                heinCard.FineYearMonthDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME.ToString());
                            }
                        }
                    }
                }

                HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);

                this.ResultDataADO = await heinGOVManager.Check(heinCard, null, true, (this.currentPatientSDO != null && this.currentPatientSDO.ID > 0 ? this.currentPatientSDO.HeinAddress : ""), this.dtIntructionTime.DateTime, this.isReadQrCode);

                if (this.ResultDataADO != null)//nếu không thay đổi thông tin sẽ chỉ trả ra kết quả check thông tuyến và không thực hiện tìm kiếm
                {
                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                    if (!String.IsNullOrEmpty(heinCard.HeinCardNumber))
                    {
                        if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                        {
                            heinCard.HeinCardNumber = this.ResultDataADO.ResultHistoryLDO.maTheMoi;
                        }
                    }

                    await this.CheckTTProcessResultData(heinCard);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task CheckTTProcessResultData(HeinCardData dataHein)
        {
            try
            {
                if (this.isNotCheckTT) { return; }
                if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
                {
                    dataHein.FineYearMonthDate = this.ResultDataADO.ResultHistoryLDO.ngayDu5Nam;
                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                    if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                    {
                        var dataGenderId = GenderConvert.HeinToHisNumber(dataHein.Gender);
                        var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == dataGenderId);
                        if (gender != null && gender.ID > 0)
                        {
                            this.cboGender.EditValue = gender.ID;
                            this.txtGenderCode.Text = gender.GENDER_CODE;
                        }
                        this.txtPatientName.Text = this.ResultDataADO.ResultHistoryLDO.hoTen;
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.FillDataAfterCheckBHYT(this.ucHeinBHYT, dataHein);
                        }

                        if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1)
                        {
                            this.txtAddress.Text = this.ResultDataADO.ResultHistoryLDO.diaChi;
                        }
                    }

                    if (this.ResultDataADO.IsToDate)
                    {
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.FillDataAfterCheckBHYT(this.ucHeinBHYT, this.ResultDataADO.HeinCardData);
                        }

                        Inventec.Common.Logging.LogSystem.Debug("Ket thuc gan du lieu cho benh nhan khi doc the va khong co han den");
                    }

                    if (this.ResultDataADO.IsAddress)
                    {
                        if (AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1)
                        {
                            this.txtAddress.Text = this.ResultDataADO.ResultHistoryLDO.diaChi;
                        }
                    }

                    if (this.ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose)
                    {
                        var dataGenderId = GenderConvert.HeinToHisNumber(dataHein.Gender);
                        var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == dataGenderId);
                        if (gender != null && gender.ID > 0)
                        {
                            this.cboGender.EditValue = gender.ID;
                            this.txtGenderCode.Text = gender.GENDER_CODE;
                        }
                        this.txtPatientName.Text = this.ResultDataADO.ResultHistoryLDO.hoTen;
                        this.txtAddress.Text = this.ResultDataADO.ResultHistoryLDO.diaChi;
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.FillDataAfterCheckBHYT(this.ucHeinBHYT, dataHein);
                        }
                    }

                    if (HisConfigCFG.IsCheckExamHistory && (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose || this.ResultDataADO.SuccessWithoutMessage))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Mo form lich su voi data rsIns");
                        frmCheckHeinCardGOV frm = new frmCheckHeinCardGOV(this.ResultDataADO.ResultHistoryLDO);
                        frm.ShowDialog();
                    }

                    Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool CheckChangeInfo(HeinCardData dataHein, ResultHistoryLDO rsIns, bool isHasNewCard)
        {
            bool result = false;
            try
            {
                string gt = (rsIns.gioiTinh == "Nữ") ? "2" : "1";
                bool isUsedNewCard = false;

                if (isHasNewCard)
                {
                    if (!String.IsNullOrEmpty(rsIns.gtTheTuMoi))
                    {
                        DateTime dtHanTheTuMoi = DateTimeHelper.ConvertDateStringToSystemDate(rsIns.gtTheTuMoi).Value;
                        DateTime dtHanTheDenMoi = (DateTimeHelper.ConvertDateStringToSystemDate(rsIns.gtTheDenMoi) ?? DateTime.MinValue);
                        if (dtHanTheTuMoi.Date <= this.dtIntructionTime.DateTime.Date && (dtHanTheDenMoi == DateTime.MinValue || this.dtIntructionTime.DateTime.Date <= dtHanTheDenMoi.Date))
                        {
                            isUsedNewCard = true;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(dataHein.Address))
                {
                    result = result || (dataHein.Address != rsIns.diaChi);
                }
                result = result || (isUsedNewCard ? (HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber) != rsIns.maTheMoi) : (HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber) != rsIns.maThe));
                result = result || dataHein.Dob != rsIns.ngaySinh;
                result = result || !dataHein.Gender.Equals(gt);
                result = result || (isUsedNewCard ? (dataHein.FromDate != rsIns.gtTheTuMoi) : (dataHein.FromDate != rsIns.gtTheTu));
                // result = result || (!String.IsNullOrEmpty(dataHein.ToDate) && (isUsedNewCard ? (dataHein.ToDate != rsIns.gtTheDenMoi) : (dataHein.ToDate != rsIns.gtTheDen)));CodeCu
                result = result || (isUsedNewCard ? (dataHein.ToDate != rsIns.gtTheDenMoi) : (dataHein.ToDate != rsIns.gtTheDen));
                result = result || (!String.IsNullOrEmpty(dataHein.MediOrgCode) && (isUsedNewCard ? (dataHein.MediOrgCode != rsIns.maDKBDMoi) : (dataHein.MediOrgCode != rsIns.maDKBD)));
                result = result || dataHein.PatientName.ToUpper() != rsIns.hoTen.ToUpper();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
