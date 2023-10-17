﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData.ADO
{
    public class ServiceADO : MOS.EFMODEL.DataModels.V_HIS_SERVICE
    {
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public bool? IsParentServiceId { get; set; }

        public decimal? PRICE { get; set; }
        public decimal? VAT_RATIO { get; set; }
        public decimal? PRICE_VAT { get; set; }

        public string SERVICE_CODE_FOR_SEARCH { get; set; }
        public string SERVICE_NAME_FOR_SEARCH { get; set; }

        public ServiceADO()
        {

        }
        public ServiceADO(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV data)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<ServiceADO>(this, sereServ);//Hiệu năng không tốt -> bỏ thay bằng gán trực tiếp

                this.APP_CREATOR = data.APP_CREATOR;
                this.APP_MODIFIER = data.APP_MODIFIER;
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.GROUP_CODE = data.GROUP_CODE;
                this.HEIN_LIMIT_PRICE = data.HEIN_LIMIT_PRICE;
                this.HEIN_LIMIT_RATIO = data.HEIN_LIMIT_RATIO;
                this.HEIN_SERVICE_TYPE_CODE = data.HEIN_SERVICE_TYPE_CODE;
                this.HEIN_SERVICE_TYPE_NAME = data.HEIN_SERVICE_TYPE_NAME;
                this.HEIN_SERVICE_TYPE_NUM_ORDER = data.HEIN_SERVICE_TYPE_NUM_ORDER;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.IS_OUT_PARENT_FEE = data.IS_OUT_PARENT_FEE;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
                this.PACKAGE_ID = data.PACKAGE_ID;
                this.PARENT_ID = data.PARENT_ID;
                this.SERVICE_TYPE_ID = data.TDL_SERVICE_TYPE_ID;
                this.SERVICE_TYPE_CODE = data.SERVICE_TYPE_CODE;
                this.SERVICE_TYPE_NAME = data.SERVICE_TYPE_NAME;
                this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                this.SERVICE_UNIT_ID = data.TDL_SERVICE_UNIT_ID;
                this.ID = data.SERVICE_ID;
                this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                this.SERVICE_NAME = data.TDL_SERVICE_NAME;

                //this.CONCRETE_ID__IN_SETY = (data.SERVICE_TYPE_ID + "." + data.SERVICE_ID);
                //this.PARENT_ID__IN_SETY = (data.SERVICE_TYPE_ID + "." + data.PARENT_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public ServiceADO(MOS.EFMODEL.DataModels.V_HIS_SERVICE data)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<ServiceADO>(this, service);//Hiệu năng không tốt -> bỏ thay bằng gán trực tiếp
                this.MAX_AMOUNT = data.MAX_AMOUNT;
                this.ACTIVE_INGR_BHYT_CODE = data.ACTIVE_INGR_BHYT_CODE;
                this.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                this.APP_CREATOR = data.APP_CREATOR;
                this.APP_MODIFIER = data.APP_MODIFIER;
                this.BILL_OPTION = data.BILL_OPTION;
                this.BILL_PATIENT_TYPE_ID = data.BILL_PATIENT_TYPE_ID;
                this.IS_NOT_CHANGE_BILL_PATY = data.IS_NOT_CHANGE_BILL_PATY;
                this.COGS = data.COGS;
                this.CONCENTRA = data.CONCENTRA;
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.ESTIMATE_DURATION = data.ESTIMATE_DURATION;
                this.GROUP_CODE = data.GROUP_CODE;
                this.HEIN_LIMIT_PRICE = data.HEIN_LIMIT_PRICE;
                this.HEIN_LIMIT_PRICE_IN_TIME = data.HEIN_LIMIT_PRICE_IN_TIME;
                this.HEIN_LIMIT_PRICE_INTR_TIME = data.HEIN_LIMIT_PRICE_INTR_TIME;
                this.HEIN_LIMIT_PRICE_OLD = data.HEIN_LIMIT_PRICE_OLD;
                this.HEIN_LIMIT_RATIO = data.HEIN_LIMIT_RATIO;
                this.HEIN_LIMIT_RATIO_OLD = data.HEIN_LIMIT_RATIO_OLD;
                this.HEIN_ORDER = data.HEIN_ORDER;
                this.HEIN_SERVICE_BHYT_CODE = data.HEIN_SERVICE_BHYT_CODE;
                this.HEIN_SERVICE_BHYT_NAME = data.HEIN_SERVICE_BHYT_NAME;
                this.HEIN_SERVICE_TYPE_BHYT_CODE = data.HEIN_SERVICE_TYPE_BHYT_CODE;
                this.HEIN_SERVICE_TYPE_CODE = data.HEIN_SERVICE_TYPE_CODE;
                this.HEIN_SERVICE_TYPE_ID = data.HEIN_SERVICE_TYPE_ID;
                this.HEIN_SERVICE_TYPE_NAME = data.HEIN_SERVICE_TYPE_NAME;
                this.HEIN_SERVICE_TYPE_NUM_ORDER = data.HEIN_SERVICE_TYPE_NUM_ORDER;
                this.ICD_CM_ID = data.ICD_CM_ID;
                this.ID = data.ID;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.IS_LEAF = data.IS_LEAF;
                this.IS_MULTI_REQUEST = data.IS_MULTI_REQUEST;
                this.IS_ENABLE_ASSIGN_PRICE = data.IS_ENABLE_ASSIGN_PRICE;
                this.IS_ALLOW_EXPEND = data.IS_ALLOW_EXPEND;
                this.IS_AUTO_EXPEND = data.IS_AUTO_EXPEND;
                this.IS_OUT_PARENT_FEE = data.IS_OUT_PARENT_FEE;
                this.MAX_EXPEND = data.MAX_EXPEND;
                this.MIN_DURATION = data.MIN_DURATION;
                this.AGE_FROM = data.AGE_FROM;
                this.AGE_TO = data.AGE_TO;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
                this.NUM_ORDER = data.NUM_ORDER;
                this.NUMBER_OF_FILM = data.NUMBER_OF_FILM;
                this.PACKAGE_ID = data.PACKAGE_ID;
                this.PACKAGE_PRICE = data.PACKAGE_PRICE;
                this.PACS_TYPE_CODE = data.PACS_TYPE_CODE;
                this.PARENT_ID = data.PARENT_ID;
                this.PTTT_GROUP_ID = data.PTTT_GROUP_ID;
                this.PTTT_METHOD_ID = data.PTTT_METHOD_ID;
                this.PTTT_GROUP_NAME = data.PTTT_GROUP_NAME;
                this.REVENUE_DEPARTMENT_ID = data.REVENUE_DEPARTMENT_ID;
                this.SERVICE_CODE = data.SERVICE_CODE;
                this.SERVICE_NAME = data.SERVICE_NAME;
                this.SERVICE_TYPE_CODE = data.SERVICE_TYPE_CODE;
                this.SERVICE_TYPE_ID = data.SERVICE_TYPE_ID;
                this.SERVICE_TYPE_NAME = data.SERVICE_TYPE_NAME;
                this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_ID = data.SERVICE_UNIT_ID;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                this.SPECIALITY_CODE = data.SPECIALITY_CODE;
                this.GENDER_ID = data.GENDER_ID;
                this.NOTICE = data.NOTICE;
                this.CAPACITY = data.CAPACITY;

                this.CONCRETE_ID__IN_SETY = (data.SERVICE_TYPE_ID + "." + (data.ID));
                this.PARENT_ID__IN_SETY = (data.SERVICE_TYPE_ID + "." + (data.PARENT_ID));
                this.DEFAULT_PATIENT_TYPE_ID = data.DEFAULT_PATIENT_TYPE_ID;
                this.SERVICE_CODE_FOR_SEARCH = convertToUnSign3(this.SERVICE_CODE) + this.SERVICE_CODE;
                this.SERVICE_NAME_FOR_SEARCH = convertToUnSign3(this.SERVICE_NAME) + this.SERVICE_NAME;
                this.DO_NOT_USE_BHYT = data.DO_NOT_USE_BHYT;
                this.SAMPLE_TYPE_CODE = data.SAMPLE_TYPE_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string convertToUnSign3(string s)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(s))
                    return "";

                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = s.Normalize(NormalizationForm.FormD);
                return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            catch
            {

            }
            return "";
        }
    }
}
