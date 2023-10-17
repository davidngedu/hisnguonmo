﻿using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Utility;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.DelegateRegister;
namespace HIS.Desktop.Plugins.Library.CheckHeinGOV
{
    public class HeinGOVManager
    {
        const string GOV_API_RESULT_000 = "000";
        const string GOV_API_RESULT_004 = "004";
        const string GOV_API_RESULT_003 = "003";
        DelegateEnableButtonSave dlgEnableButtonSave;
        DelegateHeinEnableButtonSave dlgHeinEnableButtonSave;
        public HeinGOVManager
            (
            //Action<HeinCardData> _FillDataAfterCheckBHYT_HeinInfo,
            //Action<HeinCardData> _UpdateValueAfterCheckTT_PatientRaw,
            //Action _UpdateControlEditorTime,
            //Action<string> _SetValue_AddressCombo,
            //Action<ResultHistoryLDO> _FillDataIntoUCCheckTT_CheckTT1,
            //Action<HeinCardData> _CheckPatientOldByHeinCard_PatientRaw,
            string _GoiSangCongBHXHTraVeMaLoi
            )
        {
            //this.FillDataAfterCheckBHYT_HeinInfo = _FillDataAfterCheckBHYT_HeinInfo;
            //this.UpdateValueAfterCheckTT_PatientRaw = _UpdateValueAfterCheckTT_PatientRaw;
            //this.UpdateControlEditorTime = _UpdateControlEditorTime;
            //this.SetValue_AddressCombo = _SetValue_AddressCombo;
            //this.FillDataIntoUCCheckTT_CheckTT1 = _FillDataIntoUCCheckTT_CheckTT1;
            //this.CheckPatientOldByHeinCard_PatientRaw = _CheckPatientOldByHeinCard_PatientRaw;
            this.GoiSangCongBHXHTraVeMaLoi = _GoiSangCongBHXHTraVeMaLoi;
        }

        //Action<HeinCardData> FillDataAfterCheckBHYT_HeinInfo;
        //Action<HeinCardData> UpdateValueAfterCheckTT_PatientRaw;
        //Action UpdateControlEditorTime;
        //Action<string> SetValue_AddressCombo;
        //Action<ResultHistoryLDO> FillDataIntoUCCheckTT_CheckTT1;
        //Action<HeinCardData> CheckPatientOldByHeinCard_PatientRaw;

        string GoiSangCongBHXHTraVeMaLoi;

        /// <summary>
        /// Nếu người dùng nhập thông tin thẻ BHYT, mà có đầu mã thẻ được khai báo "Không kiểm tra thông tin trên cổng BHYT" (HIS_BHYT_WHITELIST có IS_NOT_CHECK_BHYT= 1) thì sẽ không thực hiện gọi lên cổng BHYT để lấy thông tin
        /// </summary>
        /// <param name="heinCardNumder"></param>
        /// <returns></returns>
        bool CheckBhytWhiteListAcceptNoCheckBHYT(string heinCardNumder)
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CheckBhytWhiteListAcceptNoCheckBHYT__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heinCardNumder), heinCardNumder));
                if (!String.IsNullOrEmpty(heinCardNumder))
                {
                    var bhytWhiteList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>().FirstOrDefault(o => o.BHYT_WHITELIST_CODE != null && heinCardNumder.ToUpper().Contains(o.BHYT_WHITELIST_CODE.ToUpper()) && o.IS_NOT_CHECK_BHYT == 1);
                    valid = (bhytWhiteList != null && bhytWhiteList.IS_NOT_CHECK_BHYT == 1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        public void SetDelegateEnableButtonSave(DelegateEnableButtonSave dlg)
        {
            try
            {
                if (dlg != null)
                {
                    dlgEnableButtonSave = dlg;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void SetDelegateHeinEnableButtonSave(DelegateHeinEnableButtonSave dlg)
        {
            try
            {
                if (dlg != null)
                {
                    dlgHeinEnableButtonSave = dlg;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task<ResultDataADO> Check(HeinCardData dataHein, Action focusNextControl, bool ischeckChange, string heinAddressOfPatient, DateTime dtIntructionTime, bool isReadQrCode, bool showMessage)
        {
            ResultDataADO rsData = new ResultDataADO();
            try
            {
                long keyCheck = AppConfigs.CheDoTuDongCheckThongTinTheBHYT;
                if (keyCheck > 0)
                {
                    if (String.IsNullOrEmpty(dataHein.PatientName)
                        || String.IsNullOrEmpty(dataHein.Dob)
                        || String.IsNullOrEmpty(dataHein.HeinCardNumber)
                        //|| String.IsNullOrEmpty(dataHein.Gender)
                        // || String.IsNullOrEmpty(dataHein.Address)
                        //|| String.IsNullOrEmpty(dataHein.FromDate)
                        //|| String.IsNullOrEmpty(dataHein.MediOrgCode)
                        )
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong goi cong BHXH check thong tin the do du lieu truyen vao chua du du lieu bat buoc___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein), dataHein));
                        return null;
                    }

                    if (CheckBhytWhiteListAcceptNoCheckBHYT(HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber)))
                    {
                        Inventec.Common.Logging.LogSystem.Info("Nguoi dung nhap thong tin the BHYT co dau ma the duoc khai bao 'Khong kiem tra thong tin the tren cong BHYT' (HIS_BHYT_WHITELIST có IS_NOT_CHECK_BHYT= 1) sẽ khong thuc hien goi len cong BHYT de lay thong tin____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein.HeinCardNumber), dataHein.HeinCardNumber));
                        rsData = null;
                        if (!showMessage)
                        {
                            rsData = new ResultDataADO();
                            rsData.ResultHistoryLDO = new ResultHistoryLDO();
                            rsData.ResultHistoryLDO.maKetQua = "9999";
                            rsData.ResultHistoryLDO.message = "Người dùng nhập thông tin thẻ BHYT có đầu mã thẻ được khai báo 'Không kiểm tra thông tin thẻ trên cổng BHYT'";
                        }
                        return rsData;
                    }

                    if (dlgEnableButtonSave != null)
                        dlgEnableButtonSave(false);
                    if (dlgHeinEnableButtonSave != null)
                        dlgHeinEnableButtonSave(false);

                    #region --------------------
                    bool isShowErrorMessage = true;
                    CommonParam param = new CommonParam();
                    ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                    CheckHistoryLDO checkHistoryLDO = new CheckHistoryLDO();
                    checkHistoryLDO.maThe = dataHein.HeinCardNumber.Replace("-", "").Replace("_", "");
                    checkHistoryLDO.ngaySinh = dataHein.Dob;
                    checkHistoryLDO.hoTen = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.PatientName);
                    checkHistoryLDO.hoTen = (String.IsNullOrEmpty(checkHistoryLDO.hoTen) ? dataHein.PatientName : checkHistoryLDO.hoTen);
                    Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 1");
                    if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                        || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                        || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                    {
                        rsData.ResultHistoryLDO = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                        return null;
                    }

                    if (!String.IsNullOrEmpty(rsData.ResultHistoryLDO.ketQuaDangKyTocken))
                    {
                        string message = "Thông tin tài khoản dùng đăng ký phiên làm việc tại cổng BHYT không chính xác.\nKhông thể thực hiện kiểm tra thông tin thẻ bảo hiểm.\n\nHãy liên hệ với nhà cung cấp để được giúp đỡ.";
                        if (!showMessage)
                        {
                            rsData.ResultHistoryLDO.message = message;
                            return rsData;
                        }
                        else
                            DevExpress.XtraEditors.XtraMessageBox.Show(message, "Lỗi xác thực tài khoản", MessageBoxButtons.OK, DevExpress.Utils.DefaultBoolean.True);
                    }
                    else
                    {
                        bool isHasNewCard = (!String.IsNullOrEmpty(rsData.ResultHistoryLDO.maTheMoi) && String.IsNullOrEmpty(rsData.ResultHistoryLDO.maTheCu) && (!rsData.ResultHistoryLDO.maThe.Equals(rsData.ResultHistoryLDO.maTheMoi) || rsData.ResultHistoryLDO.maKetQua.Equals(GOV_API_RESULT_004) || rsData.ResultHistoryLDO.maKetQua.Equals(GOV_API_RESULT_003)));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkHistoryLDO), checkHistoryLDO) +
                            "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsData), rsData));

                        if (ischeckChange && !CheckChangeInfo(dataHein, rsData.ResultHistoryLDO, isHasNewCard, heinAddressOfPatient, dtIntructionTime))
                        {
                            return rsData;
                        }

                        rsData.HeinCardData = dataHein;

                        if (!showMessage)
                            return rsData;
                        //Trường hợp có thông tin thẻ mới
                        //thẻ 10 số luôn update
                        //Kiểm tra ngày tiếp đón có nằm trong hạn thẻ hay không, nếu nằm ngoài thì chỉ show lên thông báo, 
                        //nếu nằm trong khoản hạn thẻ mới thì hỏi có muốn update thông tin bằng kết quả trả về không 
                        //  = > có => update 
                        //  = > Không => kiểm tra nếu các thông tin nhập vào khác thông tin trên thẻ thì hỏi có muốn cập nhật theo thông tin trên thẻ không => có => update
                        bool isShowQuestionUpdateFormData = false;
                        if (isHasNewCard)
                        {
                            if (!String.IsNullOrEmpty(rsData.ResultHistoryLDO.gtTheTuMoi))
                            {
                                DateTime dtHanTheTuMoi = DateTimeHelper.ConvertDateStringToSystemDate(rsData.ResultHistoryLDO.gtTheTuMoi).Value;
                                DateTime dtHanTheDenMoi = (DateTimeHelper.ConvertDateStringToSystemDate(rsData.ResultHistoryLDO.gtTheDenMoi) ?? DateTime.MinValue);
                                if (dtHanTheTuMoi.Date <= dtIntructionTime.Date && (dtHanTheDenMoi == DateTime.MinValue || dtIntructionTime.Date <= dtHanTheDenMoi.Date))
                                {
                                    isShowQuestionUpdateFormData = true;
                                }
                            }

                            bool isUpdateData = false;
                            if (dataHein.HeinCardNumber.Replace("-", "").Replace("_", "").Trim().Length == 10)
                            {
                                isUpdateData = true;
                            }

                            List<string> errorInfo = new List<string>();
                            var arrMessSplit = rsData.ResultHistoryLDO.ghiChu.Split(new String[] { "." }, StringSplitOptions.None);
                            string thongBaoTheMoi = (arrMessSplit != null && arrMessSplit.Length > 1 ? arrMessSplit[0] : "");
                            thongBaoTheMoi = thongBaoTheMoi + ".<color=red><b>Thẻ BHYT đã được thay đổi hạn thẻ : </color></b>Mã thẻ mới : <b>" + rsData.ResultHistoryLDO.maTheMoi + "</b>.Hạn thẻ từ <b>" + rsData.ResultHistoryLDO.gtTheTuMoi + "</b> đến <b>" + rsData.ResultHistoryLDO.gtTheDenMoi + "</b>";

                            string messageQuestion = "";
                            if (isShowQuestionUpdateFormData && !isUpdateData)
                            {
                                if (HisConfigCFG.IsRequiredToUpdateNewBhytCardInCaseOfExpiry && !rsData.ResultHistoryLDO.maKetQua.Equals(GOV_API_RESULT_004))
                                    messageQuestion = "\n\n<b>Hệ thống sẽ tự động cập nhật thông tin thẻ theo thông tin thẻ mới.</b>";
                                else
                                    messageQuestion = "\n\n<b>Bạn có muốn cập nhật thông tin thẻ theo thông tin thẻ mới không?</b>";
                            }
                            else
                            {
                                bool checkTT = true;
                                if (!(dataHein.FromDate ?? "").Equals(rsData.ResultHistoryLDO.gtTheTu))
                                {
                                    errorInfo.Add("hạn từ");
                                    checkTT = false;
                                }

                                if (!(dataHein.ToDate ?? "").Equals(rsData.ResultHistoryLDO.gtTheDen))
                                {
                                    errorInfo.Add("hạn đến");
                                    checkTT = false;
                                }

                                if (!(dataHein.Address ?? "").Equals(rsData.ResultHistoryLDO.diaChi))
                                {
                                    errorInfo.Add("địa chỉ");
                                    checkTT = false;
                                }

                                string gt = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.TextToNumber(rsData.ResultHistoryLDO.gioiTinh);
                                if (!dataHein.Gender.Equals(gt))
                                {
                                    errorInfo.Add("giới tính");
                                    checkTT = false;
                                }

                                if (!(dataHein.MediOrgCode ?? "").Equals(rsData.ResultHistoryLDO.maDKBD))
                                {
                                    errorInfo.Add("mã ĐKBĐ");
                                    checkTT = false;
                                }

                                if (!dataHein.PatientName.ToUpper().Equals(rsData.ResultHistoryLDO.hoTen.ToUpper()))
                                {
                                    errorInfo.Add("họ tên");
                                    checkTT = false;
                                }

                                if (dataHein.HeinCardNumber.Replace("-", "").Replace("_", "").Trim().Length == 10)
                                {
                                    errorInfo.Add("Mã thẻ");
                                    checkTT = false;
                                }

                                if (!checkTT)
                                {
                                    thongBaoTheMoi = thongBaoTheMoi + string.Format("\n\n<b>Thông tin \"{0}\" bạn nhập vào đã không đúng với thông tin trên cổng bảo hiểm y tế.</b>", string.Join(", ", errorInfo));
                                    messageQuestion = "\n\nBạn có muốn cập nhật thông tin bệnh nhân theo thông tin tại cổng bảo hiểm y tế trả về không?";
                                    isShowQuestionUpdateFormData = true;

                                    //Lấy thông tin của thẻ cũ để hiển thị khi ngày đăng ký nằm trong hạn thẻ cũ còn hạn
                                    rsData.ResultHistoryLDO.gtTheTuMoi = rsData.ResultHistoryLDO.gtTheTu;
                                    rsData.ResultHistoryLDO.gtTheDenMoi = rsData.ResultHistoryLDO.gtTheDen;
                                    rsData.ResultHistoryLDO.maTheMoi = rsData.ResultHistoryLDO.maThe;
                                    rsData.ResultHistoryLDO.maDKBDMoi = rsData.ResultHistoryLDO.maDKBD;
                                }
                            }
                    #endregion
                            DialogResult drReslt = DialogResult.OK;
                            if (HisConfigCFG.IsRequiredToUpdateNewBhytCardInCaseOfExpiry && !rsData.ResultHistoryLDO.maKetQua.Equals(GOV_API_RESULT_004))
                            {
                                thongBaoTheMoi = thongBaoTheMoi + messageQuestion;
                                drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBaoTheMoi, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                            }
                            else
                            {
                                if (isUpdateData)
                                {
                                    drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBaoTheMoi, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                                }
                                else if (isShowQuestionUpdateFormData)
                                {
                                    thongBaoTheMoi = thongBaoTheMoi + messageQuestion;
                                    drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBaoTheMoi, "Thông báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                                }
                                else
                                {
                                    drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBaoTheMoi, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                                }
                            }
                            isShowErrorMessage = false;
                            if ((HisConfigCFG.IsRequiredToUpdateNewBhytCardInCaseOfExpiry && !rsData.ResultHistoryLDO.maKetQua.Equals(GOV_API_RESULT_004)) || (drReslt == DialogResult.Yes || isUpdateData))
                            {
                                rsData.HeinCardData.HeinCardNumber = rsData.ResultHistoryLDO.maTheMoi;
                                rsData.HeinCardData.FromDate = rsData.ResultHistoryLDO.gtTheTu = rsData.ResultHistoryLDO.gtTheTuMoi;
                                rsData.HeinCardData.ToDate = rsData.ResultHistoryLDO.gtTheDen = rsData.ResultHistoryLDO.gtTheDenMoi;
                                rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
                                rsData.HeinCardData.MediOrgCode = rsData.ResultHistoryLDO.maDKBDMoi;
                                rsData.HeinCardData.Gender = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.TextToNumber(rsData.ResultHistoryLDO.gioiTinh);
                                rsData.HeinCardData.PatientName = rsData.ResultHistoryLDO.hoTen;
                                rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
                                rsData.HeinCardData.Dob = rsData.ResultHistoryLDO.ngaySinh;
                                rsData.HeinCardData.LiveAreaCode = rsData.ResultHistoryLDO.maKV;
                                rsData.IsShowQuestionWhileChangeHeinTime__Choose = true;
                                rsData.ResultHistoryLDO.maKetQua = "000";
                                rsData.ResultHistoryLDO.success = true;
                                rsData.ResultHistoryLDO.message = "";
                                checkHistoryLDO.maThe = rsData.ResultHistoryLDO.maTheMoi;

                                try
                                {
                                    ResultHistoryLDO rsIns2 = new ResultHistoryLDO();
                                    if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                                    {
                                        rsIns2 = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                                    }
                                    else
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                                        rsIns2 = null;
                                    }
                                    if (rsIns2 != null && rsIns2.dsLichSuKCB2018 != null && rsIns2.dsLichSuKCB2018.Count > 0 && rsIns2.success == true && String.IsNullOrEmpty(rsIns2.message))
                                    {
                                        rsData.ResultHistoryLDO.dsLichSuKCB2018 = new List<ExamHistoryLDO>();
                                        foreach (var item in rsIns2.dsLichSuKCB2018)
                                        {
                                            rsData.ResultHistoryLDO.dsLichSuKCB2018.Add(item);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn(ex);
                                }
                            }
                            else
                            {
                                //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới & người dùng chọn không lấy thông tin thẻ mới => tìm kiếm BN theo số thẻ cũ
                                //Nothing...
                                // gán message để hiển thị Sai thông tin "XXX", "YYY".
                                if (errorInfo != null && errorInfo.Count > 0)
                                {
                                    rsData.ResultHistoryLDO.message = string.Format("Sai thông tin \"{0}\"", string.Join(", ", errorInfo));
                                    rsData.ResultHistoryLDO.maKetQua = "9999";
                                }
                            }

                            try
                            {
                                if (rsData.ResultHistoryLDO.dsLichSuKCB2018 == null)
                                {
                                    ResultHistoryLDO rsIns2 = new ResultHistoryLDO();
                                    if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                                    {
                                        rsIns2 = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                                    }
                                    else
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                                        rsIns2 = null;
                                    }
                                    if (rsIns2 != null && rsIns2.dsLichSuKCB2018 != null && rsIns2.dsLichSuKCB2018.Count > 0 && rsIns2.success == true && String.IsNullOrEmpty(rsIns2.message))
                                    {
                                        rsData.ResultHistoryLDO.dsLichSuKCB2018 = new List<ExamHistoryLDO>();
                                        foreach (var item in rsIns2.dsLichSuKCB2018)
                                        {
                                            rsData.ResultHistoryLDO.dsLichSuKCB2018.Add(item);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                            }
                        }
                        else if (rsData.ResultHistoryLDO.maKetQua.Equals(GOV_API_RESULT_000))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Kiem tra du lieu nguoi dung nhap vao");
                            if (string.IsNullOrEmpty(dataHein.ToDate) || dataHein.ToDate.Replace("/", "").Length < 4)
                            {
                                rsData.HeinCardData.ToDate = rsData.ResultHistoryLDO.gtTheDen;
                                rsData.ResultHistoryLDO.success = true;
                                rsData.ResultHistoryLDO.message = "";
                                rsData.IsToDate = true;
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein.ToDate), dataHein.ToDate));
                            }
                            if (string.IsNullOrEmpty(dataHein.Address) || (!dataHein.Address.Equals(rsData.ResultHistoryLDO.diaChi) && !Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.Address).Equals(rsData.ResultHistoryLDO.diaChi)))
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein.Address), dataHein.Address) + "" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsData.ResultHistoryLDO.diaChi), rsData.ResultHistoryLDO.diaChi));
                                rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
                                rsData.IsAddress = true;
                                rsData.ResultHistoryLDO.success = true;
                            }

                            CheckChangeAndUpdateForChangeHeinData(dataHein, rsData, ref isShowErrorMessage);
                        }
                    }
                    bool successWithoutMessage = (rsData.ResultHistoryLDO != null && rsData.ResultHistoryLDO.success && String.IsNullOrEmpty(rsData.ResultHistoryLDO.message));
                    string tinNhan = (rsData.ResultHistoryLDO != null ? rsData.ResultHistoryLDO.message : "");
                    string maKQ = (rsData.ResultHistoryLDO != null ? rsData.ResultHistoryLDO.maKetQua : "");
                    Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 2");
                    if (successWithoutMessage)
                    {
                        if (HisConfigCFG.IsCheckExamHistory) rsData.SuccessWithoutMessage = true;
                        Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 3");
                    }
                    else if (isShowErrorMessage)
                    {
                        if (!String.IsNullOrEmpty(tinNhan))
                            param.Messages.Add(tinNhan);

                        if (!String.IsNullOrEmpty(maKQ))
                            param.Messages.Add(String.Format(GoiSangCongBHXHTraVeMaLoi, maKQ));

                        Inventec.Desktop.Common.Message.MessageManager.Show(param, null);
                        Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 4");
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("successWithoutMessage", successWithoutMessage) + "____" + Inventec.Common.Logging.LogUtil.TraceData("isShowErrorMessage", isShowErrorMessage) + "____" + Inventec.Common.Logging.LogUtil.TraceData("rsInsFinal", rsData.ResultHistoryLDO));
                    if (focusNextControl != null) focusNextControl();
                    if (dlgEnableButtonSave != null)
                        dlgEnableButtonSave(true);
                    if (dlgHeinEnableButtonSave != null)
                        dlgHeinEnableButtonSave(true);
                }
            }
            catch (Exception ex)
            {
                if (dlgEnableButtonSave != null)
                    dlgEnableButtonSave(true);
                if (dlgHeinEnableButtonSave != null)
                    dlgHeinEnableButtonSave(true);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rsData;
        }

        public async Task<ResultDataADO> Check(HeinCardData dataHein, Action focusNextControl, bool ischeckChange, string heinAddressOfPatient, DateTime dtIntructionTime, bool isReadQrCode)
        {
            return await Check(dataHein, focusNextControl, ischeckChange, heinAddressOfPatient, dtIntructionTime, isReadQrCode, true);
        }

        public async Task<ResultDataADO> CheckChiTietHS(string maHS, string _viTri)
        {
            ResultDataADO rsData = new ResultDataADO();
            try
            {
                if (String.IsNullOrEmpty(maHS))
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong goi cong BHXH check chi tiet ho sodieu tri truoc do du lieu truyen vao chua du du lieu bat buoc___maHS" + maHS);
                    return null;
                }

                CommonParam param = new CommonParam();
                ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                MaHoSoLDO ado = new MaHoSoLDO();
                ado.maHoSo = maHS;

                Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 1");
                //rsData.ChiTietKCBLDO = await apiInsuranceExpertise.ChiTietKCB(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, ado);
                if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                {
                    rsData.ChiTietKCBLDO = await apiInsuranceExpertise.ChiTietKCB(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, ado);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                }

                if (!String.IsNullOrEmpty(rsData.ChiTietKCBLDO.ketQuaDangKyTocken))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thông tin tài khoản dùng đăng ký phiên làm việc tại cổng BHYT không chính xác.\nKhông thể thực hiện kiểm tra thông tin thẻ bảo hiểm.\n\nHãy liên hệ với nhà cung cấp để được giúp đỡ.",
                        "Lỗi xác thực tài khoản", MessageBoxButtons.OK, DevExpress.Utils.DefaultBoolean.True);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rsData;
        }

        void CheckChangeAndUpdateForChangeHeinData(HeinCardData dataHein, ResultDataADO rsData, ref bool isShowErrorMessage)
        {
            List<string> errorInfo = new List<string>();
            string diachi = rsData.HeinCardData.Address;
            string handen = rsData.HeinCardData.ToDate;

            bool checkTT = true;// tientv issue #6745   
            //if (!String.IsNullOrEmpty(dataHein.ToDate)
            //    && !dataHein.ToDate.Equals(rsIns.gtTheDen))

            Inventec.Common.Logging.LogSystem.Debug("1." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkTT), checkTT));
            if (dataHein.FromDate != rsData.ResultHistoryLDO.gtTheTu)
            {
                checkTT = false;
                errorInfo.Add("hạn từ");
            }

            if (rsData.IsToDate || rsData.IsAddress)
            {
                if (dataHein.ToDate != rsData.ResultHistoryLDO.gtTheDen)
                {
                    errorInfo.Add("hạn đến");
                }

                if (rsData.IsAddress)
                {
                    errorInfo.Add("địa chỉ");
                }

                checkTT = false;

                rsData.HeinCardData.ToDate = rsData.ResultHistoryLDO.gtTheDen;
                rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
            }

            Inventec.Common.Logging.LogSystem.Debug("2." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkTT), checkTT));
            if (dataHein.ToDate != rsData.ResultHistoryLDO.gtTheDen)
            {
                checkTT = false;
                errorInfo.Add("hạn đến");
            }

            Inventec.Common.Logging.LogSystem.Debug("3." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkTT), checkTT));
            if (string.IsNullOrEmpty(dataHein.Address) || (!dataHein.Address.Equals(rsData.ResultHistoryLDO.diaChi) && !Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.Address).Equals(rsData.ResultHistoryLDO.diaChi)))
            {
                checkTT = false;
                errorInfo.Add("địa chỉ");
            }

            Inventec.Common.Logging.LogSystem.Debug("4." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkTT), checkTT));
            string gt = GenderConvert.TextToNumber(rsData.ResultHistoryLDO.gioiTinh);
            if (!dataHein.Gender.Equals(gt))
            {
                checkTT = false;
                errorInfo.Add("giới tính");
            }

            Inventec.Common.Logging.LogSystem.Debug("5." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkTT), checkTT));
            if (!dataHein.MediOrgCode.Equals(rsData.ResultHistoryLDO.maDKBD))
            {
                errorInfo.Add("nơi ĐKBĐ");
                checkTT = false;
            }

            Inventec.Common.Logging.LogSystem.Debug("5." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkTT), checkTT));
            if (dataHein.Dob != rsData.ResultHistoryLDO.ngaySinh)
            {
                errorInfo.Add("ngày sinh");
                checkTT = false;
            }

            bool alwaysUpdate = false;
            if (dataHein.HeinCardNumber.Replace("-", "").Replace("_", "").Trim().Length == 10)
            {
                alwaysUpdate = true;
                errorInfo.Add("Mã thẻ");
                checkTT = false;
            }

            Inventec.Common.Logging.LogSystem.Debug("6." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkTT), checkTT));
            if (!checkTT)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein), dataHein));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => errorInfo), errorInfo));
                string thongBao = string.Format("<b>Thông tin \"{0}\" bạn nhập vào đã không đúng với thông tin trên cổng bảo hiểm y tế.</b>", string.Join(", ", errorInfo));

                DialogResult drReslt;
                if (alwaysUpdate)
                {
                    isShowErrorMessage = false;
                    drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBao, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                }
                else
                {
                    thongBao += "\n\nBạn có muốn cập nhật thông tin bệnh nhân theo thông tin tại cổng bảo hiểm y tế trả về không?";
                    drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBao, "Thông báo!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                }

                if (drReslt == DialogResult.OK)
                {
                    rsData.HeinCardData.HeinCardNumber = rsData.ResultHistoryLDO.maThe;
                    rsData.HeinCardData.FromDate = rsData.ResultHistoryLDO.gtTheTu;
                    rsData.HeinCardData.ToDate = rsData.ResultHistoryLDO.gtTheDen;
                    rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
                    rsData.HeinCardData.MediOrgCode = rsData.ResultHistoryLDO.maDKBD;
                    rsData.HeinCardData.Gender = GenderConvert.TextToNumber(rsData.ResultHistoryLDO.gioiTinh);
                    rsData.HeinCardData.PatientName = rsData.ResultHistoryLDO.hoTen;
                    rsData.HeinCardData.Dob = rsData.ResultHistoryLDO.ngaySinh;

                    rsData.ResultHistoryLDO.success = true;
                    rsData.ResultHistoryLDO.message = "";

                    rsData.IsThongTinNguoiDungThayDoiSoVoiCong__Choose = true;
                }
                else if (String.IsNullOrWhiteSpace(rsData.ResultHistoryLDO.message))
                {
                    rsData.HeinCardData.ToDate = handen;
                    rsData.HeinCardData.Address = diachi;
                    // gán message để hiển thị Sai thông tin "XXX", "YYY".
                    if (errorInfo != null && errorInfo.Count > 0)
                    {
                        isShowErrorMessage = false;
                        rsData.ResultHistoryLDO.maKetQua = "9999";
                        rsData.ResultHistoryLDO.message = string.Format("Sai thông tin \"{0}\"", string.Join(", ", errorInfo));
                    }
                }
                else // không cập nhật sẽ trả về mã kết quả 9999 để chặn
                {
                    rsData.ResultHistoryLDO.maKetQua = "9999";
                }
            }
        }

        bool CheckChangeInfo(HeinCardData dataHein, ResultHistoryLDO rsIns, bool isHasNewCard, string heinAddressOfPatient, DateTime dtIntructionTime)
        {
            bool result = false;
            try
            {
                string gt = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.TextToNumber(rsIns.gioiTinh);
                bool isUsedNewCard = false;

                if (isHasNewCard)
                {
                    if (!String.IsNullOrEmpty(rsIns.gtTheTuMoi))
                    {
                        DateTime dtHanTheTuMoi = DateTimeHelper.ConvertDateStringToSystemDate(rsIns.gtTheTuMoi).Value;
                        DateTime dtHanTheDenMoi = (DateTimeHelper.ConvertDateStringToSystemDate(rsIns.gtTheDenMoi) ?? DateTime.MinValue);
                        if (dtHanTheTuMoi.Date <= dtIntructionTime.Date && (dtHanTheDenMoi == DateTime.MinValue || dtIntructionTime.Date <= dtHanTheDenMoi.Date))
                        {
                            isUsedNewCard = true;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(dataHein.Address))
                {
                    if (!String.IsNullOrEmpty(heinAddressOfPatient))
                    {
                        result = result || (dataHein.Address != heinAddressOfPatient);
                    }
                    else
                    {
                        result = result || (dataHein.Address != rsIns.diaChi);
                    }
                }
                result = result || (isUsedNewCard ? (HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber) != rsIns.maTheMoi) : (HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber) != rsIns.maThe));

                if (rsIns.ngaySinh.Length == 4)
                {
                    result = result || (dataHein.Dob.Length == 4 ? dataHein.Dob.Substring(0, 4) != rsIns.ngaySinh : dataHein.Dob.Substring(6, 4) != rsIns.ngaySinh);
                }
                else
                {
                    result = result || dataHein.Dob != rsIns.ngaySinh;
                }
                result = result || !dataHein.Gender.Equals(gt);
                result = result || (isUsedNewCard ? (dataHein.FromDate != rsIns.gtTheTuMoi) : (dataHein.FromDate != rsIns.gtTheTu));
                //result = result || (!String.IsNullOrEmpty(dataHein.ToDate) && (isUsedNewCard ? (dataHein.ToDate != rsIns.gtTheDenMoi) : (dataHein.ToDate != rsIns.gtTheDen)));
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

        public async Task<ResultDataADO> CheckCccdQrCode(HeinCardData dataHein, Action focusNextControl, DateTime dtIntructionTime)
        {
            ResultDataADO rsData = new ResultDataADO();
            try
            {
                long keyCheck = AppConfigs.CheDoTuDongCheckThongTinTheBHYT;
                if (keyCheck > 0)
                {
                    if (String.IsNullOrEmpty(dataHein.PatientName)
                        || String.IsNullOrEmpty(dataHein.Dob)
                        || String.IsNullOrEmpty(dataHein.HeinCardNumber)
                        )
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong goi cong BHXH check thong tin the do du lieu truyen vao chua du du lieu bat buoc___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein), dataHein));
                        return null;
                    }

                    #region --------------------
                    bool isShowErrorMessage = true;
                    CommonParam param = new CommonParam();
                    ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                    CheckHistoryLDO checkHistoryLDO = new CheckHistoryLDO();
                    checkHistoryLDO.maThe = dataHein.HeinCardNumber.Replace("-", "").Replace("_", "");
                    checkHistoryLDO.ngaySinh = dataHein.Dob;
                    checkHistoryLDO.hoTen = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.PatientName);
                    checkHistoryLDO.hoTen = (String.IsNullOrEmpty(checkHistoryLDO.hoTen) ? dataHein.PatientName : checkHistoryLDO.hoTen);
                    Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 1");
                    if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                        || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                        || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                    {
                        rsData.ResultHistoryLDO = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                        return null;
                    }
                    if (!String.IsNullOrEmpty(rsData.ResultHistoryLDO.ketQuaDangKyTocken))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Thông tin tài khoản dùng đăng ký phiên làm việc tại cổng BHYT không chính xác.\nKhông thể thực hiện kiểm tra thông tin thẻ bảo hiểm.\n\nHãy liên hệ với nhà cung cấp để được giúp đỡ.",
                            "Lỗi xác thực tài khoản", MessageBoxButtons.OK, DevExpress.Utils.DefaultBoolean.True);
                    }
                    else
                    {
                        bool isHasNewCard = (!String.IsNullOrEmpty(rsData.ResultHistoryLDO.maTheMoi) && String.IsNullOrEmpty(rsData.ResultHistoryLDO.maTheCu) && (!rsData.ResultHistoryLDO.maThe.Equals(rsData.ResultHistoryLDO.maTheMoi) || rsData.ResultHistoryLDO.maKetQua.Equals(GOV_API_RESULT_004)));
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkHistoryLDO), checkHistoryLDO) +
                            "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsData), rsData));

                        dataHein.HeinCardNumber = rsData.ResultHistoryLDO.maTheMoi ?? rsData.ResultHistoryLDO.maThe;
                        rsData.HeinCardData = dataHein;

                        //Trường hợp có thông tin thẻ mới
                        //thẻ 10 số luôn update
                        //Kiểm tra ngày tiếp đón có nằm trong hạn thẻ hay không, nếu nằm ngoài thì chỉ show lên thông báo, 
                        //nếu nằm trong khoản hạn thẻ mới thì hỏi có muốn update thông tin bằng kết quả trả về không 
                        //  = > có => update 
                        //  = > Không => kiểm tra nếu các thông tin nhập vào khác thông tin trên thẻ thì hỏi có muốn cập nhật theo thông tin trên thẻ không => có => update
                        bool isShowQuestionUpdateFormData = false;
                        if (isHasNewCard)
                        {
                            if (!String.IsNullOrEmpty(rsData.ResultHistoryLDO.gtTheTuMoi))
                            {
                                DateTime dtHanTheTuMoi = DateTimeHelper.ConvertDateStringToSystemDate(rsData.ResultHistoryLDO.gtTheTuMoi).Value;
                                DateTime dtHanTheDenMoi = (DateTimeHelper.ConvertDateStringToSystemDate(rsData.ResultHistoryLDO.gtTheDenMoi) ?? DateTime.MinValue);
                                if (dtHanTheTuMoi.Date <= dtIntructionTime.Date && (dtHanTheDenMoi == DateTime.MinValue || dtIntructionTime.Date <= dtHanTheDenMoi.Date))
                                {
                                    isShowQuestionUpdateFormData = true;
                                }
                            }

                            //cccd luôn update dữ liệu
                            bool isUpdateData = true;

                            List<string> errorInfo = new List<string>();
                            var arrMessSplit = rsData.ResultHistoryLDO.ghiChu.Split(new String[] { "." }, StringSplitOptions.None);
                            string thongBaoTheMoi = (arrMessSplit != null && arrMessSplit.Length > 1 ? arrMessSplit[0] : "");
                            thongBaoTheMoi = thongBaoTheMoi + ".<color=red><b>Thẻ BHYT đã được thay đổi hạn thẻ : </color></b>Mã thẻ mới : <b>" + rsData.ResultHistoryLDO.maTheMoi + "</b>.Hạn thẻ từ <b>" + rsData.ResultHistoryLDO.gtTheTuMoi + "</b> đến <b>" + rsData.ResultHistoryLDO.gtTheDenMoi + "</b>";

                            string messageQuestion = "";
                            if (isShowQuestionUpdateFormData && !isUpdateData)
                            {
                                messageQuestion = "\n\n<b>Bạn có muốn cập nhật thông tin thẻ theo thông tin thẻ mới không?</b>";
                            }
                            else
                            {
                                bool checkTT = true;

                                if (!(dataHein.Address ?? "").Equals(rsData.ResultHistoryLDO.diaChi))
                                {
                                    errorInfo.Add("địa chỉ");
                                    checkTT = false;
                                }

                                string gt = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.TextToNumber(rsData.ResultHistoryLDO.gioiTinh);
                                if (!dataHein.Gender.Equals(gt))
                                {
                                    errorInfo.Add("giới tính");
                                    checkTT = false;
                                }

                                if (!dataHein.PatientName.ToUpper().Equals(rsData.ResultHistoryLDO.hoTen.ToUpper()))
                                {
                                    errorInfo.Add("họ tên");
                                    checkTT = false;
                                }

                                if (!checkTT)
                                {
                                    thongBaoTheMoi = thongBaoTheMoi + string.Format("\n\n<b>Thông tin \"{0}\" bạn nhập vào đã không đúng với thông tin trên cổng bảo hiểm y tế.</b>", string.Join(", ", errorInfo));
                                    messageQuestion = "\n\nBạn có muốn cập nhật thông tin bệnh nhân theo thông tin tại cổng bảo hiểm y tế trả về không?";
                                    isShowQuestionUpdateFormData = true;

                                    //Lấy thông tin của thẻ cũ để hiển thị khi ngày đăng ký nằm trong hạn thẻ cũ còn hạn
                                    rsData.ResultHistoryLDO.gtTheTuMoi = rsData.ResultHistoryLDO.gtTheTu;
                                    rsData.ResultHistoryLDO.gtTheDenMoi = rsData.ResultHistoryLDO.gtTheDen;
                                    rsData.ResultHistoryLDO.maTheMoi = rsData.ResultHistoryLDO.maThe;
                                    rsData.ResultHistoryLDO.maDKBDMoi = rsData.ResultHistoryLDO.maDKBD;
                                }
                            }
                    #endregion
                            DialogResult drReslt;
                            if (isUpdateData)
                            {
                                drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBaoTheMoi, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                            }
                            else if (isShowQuestionUpdateFormData)
                            {
                                thongBaoTheMoi = thongBaoTheMoi + messageQuestion;
                                drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBaoTheMoi, "Thông báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                            }
                            else
                            {
                                drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(thongBaoTheMoi, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                            }
                            isShowErrorMessage = false;
                            if (drReslt == DialogResult.Yes || isUpdateData)
                            {
                                rsData.HeinCardData.HeinCardNumber = rsData.ResultHistoryLDO.maTheMoi;
                                rsData.HeinCardData.FromDate = rsData.ResultHistoryLDO.gtTheTu = rsData.ResultHistoryLDO.gtTheTuMoi;
                                rsData.HeinCardData.ToDate = rsData.ResultHistoryLDO.gtTheDen = rsData.ResultHistoryLDO.gtTheDenMoi;
                                rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
                                rsData.HeinCardData.MediOrgCode = rsData.ResultHistoryLDO.maDKBDMoi;
                                rsData.HeinCardData.Gender = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.TextToNumber(rsData.ResultHistoryLDO.gioiTinh);
                                rsData.HeinCardData.PatientName = rsData.ResultHistoryLDO.hoTen;
                                rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
                                rsData.HeinCardData.Dob = rsData.ResultHistoryLDO.ngaySinh;

                                rsData.IsShowQuestionWhileChangeHeinTime__Choose = true;

                                rsData.ResultHistoryLDO.success = true;
                                rsData.ResultHistoryLDO.message = "";
                                checkHistoryLDO.maThe = rsData.ResultHistoryLDO.maTheMoi;

                                try
                                {
                                    ResultHistoryLDO rsIns2 = new ResultHistoryLDO();
                                    if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                                    {
                                        rsIns2 = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                                    }
                                    else
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                                        rsIns2 = null;
                                    }
                                    if (rsIns2 != null && rsIns2.dsLichSuKCB2018 != null && rsIns2.dsLichSuKCB2018.Count > 0 && rsIns2.success == true && String.IsNullOrEmpty(rsIns2.message))
                                    {
                                        rsData.ResultHistoryLDO.dsLichSuKCB2018 = new List<ExamHistoryLDO>();
                                        foreach (var item in rsIns2.dsLichSuKCB2018)
                                        {
                                            rsData.ResultHistoryLDO.dsLichSuKCB2018.Add(item);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn(ex);
                                }
                            }
                            else
                            {
                                //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới & người dùng chọn không lấy thông tin thẻ mới => tìm kiếm BN theo số thẻ cũ
                                //Nothing...
                                // gán message để hiển thị Sai thông tin "XXX", "YYY".
                                if (errorInfo != null && errorInfo.Count > 0)
                                {
                                    rsData.ResultHistoryLDO.message = string.Format("Sai thông tin \"{0}\"", string.Join(", ", errorInfo));
                                    rsData.ResultHistoryLDO.maKetQua = "9999";
                                }
                            }

                            try
                            {
                                if (rsData.ResultHistoryLDO.dsLichSuKCB2018 == null)
                                {
                                    ResultHistoryLDO rsIns2 = new ResultHistoryLDO();
                                    if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                                        || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                                    {
                                        rsIns2 = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);
                                    }
                                    else
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                                        rsIns2 = null;
                                    }
                                    if (rsIns2 != null && rsIns2.dsLichSuKCB2018 != null && rsIns2.dsLichSuKCB2018.Count > 0 && rsIns2.success == true && String.IsNullOrEmpty(rsIns2.message))
                                    {
                                        rsData.ResultHistoryLDO.dsLichSuKCB2018 = new List<ExamHistoryLDO>();
                                        foreach (var item in rsIns2.dsLichSuKCB2018)
                                        {
                                            rsData.ResultHistoryLDO.dsLichSuKCB2018.Add(item);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                            }
                        }
                        else if (rsData.ResultHistoryLDO.maKetQua.Equals(GOV_API_RESULT_000))
                        {
                            rsData.HeinCardData.HeinCardNumber = rsData.ResultHistoryLDO.maTheMoi;
                            rsData.HeinCardData.FromDate = rsData.ResultHistoryLDO.gtTheTu = rsData.ResultHistoryLDO.gtTheTuMoi;
                            rsData.HeinCardData.ToDate = rsData.ResultHistoryLDO.gtTheDen = rsData.ResultHistoryLDO.gtTheDenMoi;
                            rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
                            rsData.HeinCardData.MediOrgCode = rsData.ResultHistoryLDO.maDKBDMoi;
                            rsData.HeinCardData.Gender = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.TextToNumber(rsData.ResultHistoryLDO.gioiTinh);
                            rsData.HeinCardData.PatientName = rsData.ResultHistoryLDO.hoTen;
                            rsData.HeinCardData.Address = rsData.ResultHistoryLDO.diaChi;
                            rsData.HeinCardData.Dob = rsData.ResultHistoryLDO.ngaySinh;

                            rsData.IsShowQuestionWhileChangeHeinTime__Choose = true;

                            rsData.ResultHistoryLDO.success = true;
                            rsData.ResultHistoryLDO.message = "";
                        }
                    }
                    bool successWithoutMessage = (rsData.ResultHistoryLDO != null && rsData.ResultHistoryLDO.success && String.IsNullOrEmpty(rsData.ResultHistoryLDO.message));
                    string tinNhan = (rsData.ResultHistoryLDO != null ? rsData.ResultHistoryLDO.message : "");
                    string maKQ = (rsData.ResultHistoryLDO != null ? rsData.ResultHistoryLDO.maKetQua : "");
                    Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 2");
                    if (successWithoutMessage)
                    {
                        if (HisConfigCFG.IsCheckExamHistory) rsData.SuccessWithoutMessage = true;
                        Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 3");
                    }
                    else if (isShowErrorMessage)
                    {
                        if (!String.IsNullOrEmpty(tinNhan))
                            param.Messages.Add(tinNhan);

                        if (!String.IsNullOrEmpty(maKQ))
                            param.Messages.Add(String.Format(GoiSangCongBHXHTraVeMaLoi, maKQ));

                        Inventec.Desktop.Common.Message.MessageManager.Show(param, null);
                        Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 4");
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("successWithoutMessage", successWithoutMessage) + "____" + Inventec.Common.Logging.LogUtil.TraceData("isShowErrorMessage", isShowErrorMessage) + "____" + Inventec.Common.Logging.LogUtil.TraceData("rsInsFinal", rsData.ResultHistoryLDO));
                    if (focusNextControl != null) focusNextControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rsData;
        }
    }
}
