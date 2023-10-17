﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common;

namespace LIS.Desktop.Plugins.LisSample.Validates
{
    class maxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtControl;
        internal int? Maxlangth;
        internal static System.Resources.ResourceManager langueMessage = new System.Resources.ResourceManager("LIS.Desktop.Plugins.LisSample.Resources.LangVi", System.Reflection.Assembly.GetCallingAssembly());

        public override bool Validate(Control control, object value)
        {
            bool succes = true;
            try
            {
                succes = succes && (txtControl != null);
                if (succes)
                {
                    string strError = "";
                    string lengthtxt = txtControl.Text.Trim();
                    int? CountLength = Inventec.Common.String.CountVi.Count(lengthtxt);
                    if (String.IsNullOrEmpty(lengthtxt))
                    {
                        //succes = false;
                        //strError = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    }
                    else
                    {
                        if (CountLength > Maxlangth)
                        {
                            succes = false;
                            strError += ((!string.IsNullOrEmpty(lengthtxt) ? "\r\n" : "") + String.Format(Inventec.Common.Resource.Get.Value("LisSample_TruongDuLieuVuotQuaMaxLength", langueMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), Maxlangth));
                        }
                    }
                    this.ErrorText = strError;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);

            }
            return succes;
        }
    }
}
