﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.UC.UCPatientRaw.ADO;
using MOS.SDO;
using Inventec.Common.QrCodeBHYT;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataPatientRawInfo(HisPatientSDO data)
        {
            try
            {
                var oldDataPatient = this.ucPatientRaw1.GetValue();
                UCPatientRawADO dataAddress = new UCPatientRawADO();
                dataAddress.CARRER_ID = data.CAREER_ID;
                dataAddress.DOB = data.DOB;
                dataAddress.GENDER_ID = data.GENDER_ID;
                dataAddress.HEIN_CARD_NUMBER = data.HeinCardNumber;
                dataAddress.IS_HAS_NOT_DAY_DOB = data.IS_HAS_NOT_DAY_DOB;
                dataAddress.PATIENT_CODE = data.PATIENT_CODE;
                dataAddress.PATIENT_NAME = data.VIR_PATIENT_NAME;
                if (oldDataPatient != null)
                    dataAddress.PATIENTTYPE_ID = oldDataPatient.PATIENTTYPE_ID;
                dataAddress.PATIENT_CLASSIFY_ID = data.PATIENT_CLASSIFY_ID;
                dataAddress.MILITARY_RANK_ID = data.MILITARY_RANK_ID;
                dataAddress.POSITION_ID = data.POSITION_ID;
                dataAddress.WORK_PLACE_ID = data.WORK_PLACE_ID;
                dataAddress.ETHNIC_CODE = data.ETHNIC_CODE;
                Inventec.Common.Logging.LogSystem.Error("FillDataPatientRawInfo");
                this.ucPatientRaw1.SetValue(dataAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPreviewForSearchByQrcodeInUCPatientRaw(HeinCardData dataCheck)
        {
            try
            {
                if (dataCheck != null)
                {
                    string heinCardNumber = dataCheck.HeinCardNumber;
                    this.FillDataAfterSaerchPatientInUCPatientRaw(dataCheck);
                    //if (!String.IsNullOrEmpty(heinCardNumber))
                    this.FillDataCareerUnder6AgeByHeinCardNumber(heinCardNumber);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowControlHrmKskCode(bool _isShow)
        {
            try
            {
                if (this.ucPlusInfo1 != null)
                {
                    this.ucPlusInfo1.ShowControlhrmKskCode(_isShow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ShowControlHrmKskCodeNotValid(bool _isShow)
        {
            try
            {
                if (this.ucPlusInfo1 != null)
                {
                    this.ucPlusInfo1.ShowControlhrmKskCodeNotValid(_isShow, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowControlGuaranteeLoginname(bool _isShow)
        {
            try
            {
                if (this.ucOtherServiceReqInfo1 != null)
                {
                    this.ucOtherServiceReqInfo1.ShowValidation(_isShow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }        
    }
}
