﻿
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MedicineTypeCreate.Popup;
using HIS.UC.SecondaryIcd.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.Validtion
{
    class BenhPhuValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit maBenhPhuTxt;
        internal DevExpress.XtraEditors.TextEdit tenBenhPhuTxt;
        private string[] icdSeparators = new string[] { ";" };
        internal List<HIS_ICD> listIcd;

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && ValidIcdWrongCode(maBenhPhuTxt);
                valid = valid && ValidLength(maBenhPhuTxt, tenBenhPhuTxt);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
        bool ValidIcdWrongCode(DevExpress.XtraEditors.TextEdit maBenhPhuTxt)
        {
            try
            {
                if (!string.IsNullOrEmpty(maBenhPhuTxt.Text.Trim()))
                {
                    if (maBenhPhuTxt.Text.Trim().Equals(";"))
                    {
                        this.ErrorText = "Mã ICD bạn nhập không hợp lệ";
                        return false;
                    }

                    if (maBenhPhuTxt.Text.Trim().Contains(";;"))
                    {
                        this.ErrorText = "Mã ICD bạn nhập không hợp lệ";
                        return false;
                    }
                    else
                    {
                        string icdNames = "";
                        string wrongsubCodes = "";
                        bool valid = CheckIcdWrongCode(maBenhPhuTxt.Text.Trim(), ref icdNames, ref wrongsubCodes);
                        if (!valid)
                        {
                            this.ErrorText = String.Format("Mã ICD sau không tồn tại hoặc sai định dạng: {0}. Phần mềm chỉ nhận danh sách các mã ICD ngăn cách nhau bởi dấu chấm phẩy. Vui lòng kiểm tra lại", wrongsubCodes);
                            return false;
                        }
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
        bool ValidLength(DevExpress.XtraEditors.TextEdit maBenhPhuTxt, DevExpress.XtraEditors.TextEdit tenBenhPhuTxt)
        {
            try
            {
                List<string> _mess = new List<string>();
                bool _check = false;
                if (String.IsNullOrEmpty(maBenhPhuTxt.Text) || String.IsNullOrEmpty(tenBenhPhuTxt.Text))
                {
                    _mess.Add("Trường dữ liệu bắt buộc");
                    _check = true;
                }
                else
                {
                    if (Inventec.Common.String.CountVi.Count(maBenhPhuTxt.Text) > 500)
                    {
                        _mess.Add("Mã bệnh phụ quá dài (>500 ký tự)");
                        _check = true;
                    }
                    if (Inventec.Common.String.CountVi.Count(tenBenhPhuTxt.Text) > 4000)
                    {
                        _mess.Add("Tên bệnh phụ quá dài (>4000 ký tự)");
                        _check = true;
                    }
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
                            valid = false;
                        }
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
