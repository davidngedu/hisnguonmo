﻿
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.Validate.ValidateRule
{
    public delegate string DelegateGetIcdMain();

    class BenhPhuValidationRule :
DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit maBenhPhuTxt;
        internal DevExpress.XtraEditors.TextEdit tenBenhPhuTxt;
        private string[] icdSeparators = new string[] { ";" };
        internal List<HIS_ICD> listIcd = BackendDataWorker.Get<HIS_ICD>();
        internal DelegateGetIcdMain getIcdMain;

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && ValidLength(maBenhPhuTxt);
                valid = valid && ValidIcdWrongCode(maBenhPhuTxt);
                valid = valid && ValidDuplicateWithIcdMain(getIcdMain, maBenhPhuTxt.Text.Trim());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        bool ValidLength(DevExpress.XtraEditors.TextEdit maBenhPhuTxt)
        {
            try
            {
                List<string> _mess = new List<string>();
                bool _check = false;
                if ((!String.IsNullOrEmpty(maBenhPhuTxt.Text.Trim())
                    && Inventec.Common.String.CountVi.Count(maBenhPhuTxt.Text.Trim()) > 500))
                {
                    _mess.Add("Mã bệnh phụ quá dài (>500 ký tự)");
                    _check = true;
                }
                if ((!String.IsNullOrEmpty(tenBenhPhuTxt.Text.Trim())
                    && Inventec.Common.String.CountVi.Count(tenBenhPhuTxt.Text.Trim()) > 4000))
                {
                    _mess.Add("Tên bệnh phụ quá dài (>4000 ký tự)");
                    _check = true;
                }
                if (_check)
                {
                    this.ErrorText = string.Join(";", _mess);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        bool ValidIcdWrongCode(DevExpress.XtraEditors.TextEdit maBenhPhuTxt)
        {
            try
            {
                if (!string.IsNullOrEmpty(maBenhPhuTxt.Text.Trim()))
                {
                    string icdNames = "";
                    string wrongsubCodes = "";
                    bool valid = CheckIcdWrongCode(maBenhPhuTxt.Text.Trim(), ref icdNames, ref wrongsubCodes);
                    if (!valid)
                    {
                        return false;
                    }
                }
                else 
                {
                    if (!string.IsNullOrEmpty(tenBenhPhuTxt.Text.Trim()))
                    {
                        this.ErrorText = Resources.ResourceMessage.MaICDchuaduocnhap;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        bool ValidDuplicateWithIcdMain(DelegateGetIcdMain getIcdMain, string icdSubCode)
        {
            bool valid = true;
            try
            {
                if (getIcdMain != null && !String.IsNullOrEmpty(getIcdMain()) && !String.IsNullOrEmpty(icdSubCode))
                {
                    string icdmainCode = getIcdMain();
                    Inventec.Common.Logging.LogSystem.Debug("ValidDuplicateWithIcdMain. 2__icdmainCode=" + icdmainCode + ", icdSubCode=" + icdSubCode);
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = icdSubCode.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ValidDuplicateWithIcdMain. 3__valid=" + valid);
                        if (arrIcdExtraCodes.Any(o => o.Equals(icdmainCode)))
                        {
                            this.ErrorText = String.Format(Resources.ResourceMessage.MabenhPhuDaDuocSuDungChoMaBenhChinh, icdmainCode);
                            return false;
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("getIcdMain is null hoac icdmainCode is null" + ", icdSubCode=" + icdSubCode);
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckIcdWrongCode(string icdSubCode, ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(icdSubCode))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = icdSubCode.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = listIcd.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            strWrongIcdCodes = (arrWrongCodes.Count == 1 ? strWrongIcdCodes.Replace(IcdUtil.seperator, "") : strWrongIcdCodes);
                            this.ErrorText = String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes);
                            valid = false;
                        }
                    }
                    else
                    {
                        this.ErrorText = "Mã ICD bạn nhập không hợp lệ";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}
