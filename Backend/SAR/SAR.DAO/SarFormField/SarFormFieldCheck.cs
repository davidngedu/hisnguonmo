using SAR.DAO.Base;
using SAR.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Linq;

namespace SAR.DAO.SarFormField
{
    partial class SarFormFieldCheck : EntityBase
    {
        public SarFormFieldCheck()
            : base()
        {
            bridgeDAO = new BridgeDAO<SAR_FORM_FIELD>();
        }

        private BridgeDAO<SAR_FORM_FIELD> bridgeDAO;

        public bool IsUnLock(long id)
        {
            return bridgeDAO.IsUnLock(id);
        }
    }
}
