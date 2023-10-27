using ACS.DAO.Base;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACS.DAO.AcsAppOtpType
{
    partial class AcsAppOtpTypeUpdate : EntityBase
    {
        public AcsAppOtpTypeUpdate()
            : base()
        {
            bridgeDAO = new BridgeDAO<ACS_APP_OTP_TYPE>();
        }

        private BridgeDAO<ACS_APP_OTP_TYPE> bridgeDAO;

        public bool Update(ACS_APP_OTP_TYPE data)
        {
            return IsNotNull(data) && IsGreaterThanZero(data.ID) && bridgeDAO.Update(data);
        }

        public bool UpdateList(List<ACS_APP_OTP_TYPE> listData)
        {
            return IsNotNull(listData) && bridgeDAO.UpdateList(listData);
        }
    }
}
