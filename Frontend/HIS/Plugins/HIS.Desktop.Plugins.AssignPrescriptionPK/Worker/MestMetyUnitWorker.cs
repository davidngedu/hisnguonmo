﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Worker
{
    class MestMetyUnitWorker
    {
        internal static bool UpdateUnit(MediMatyTypeADO mediMatyTypeADO, List<HIS_MEST_METY_UNIT> mestMetyUnitData = null)
        {
            bool success = false;
            try
            {
                if (mediMatyTypeADO == null || (mediMatyTypeADO.MEDI_STOCK_ID ?? 0) == 0 || mediMatyTypeADO.ID == 0)
                    throw new ArgumentNullException("data");
                if (mestMetyUnitData != null && mestMetyUnitData.Count > 0 && mediMatyTypeADO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    var oneCheck = mestMetyUnitData.Where(o => o.MEDI_STOCK_ID == mediMatyTypeADO.MEDI_STOCK_ID && o.MEDICINE_TYPE_ID == mediMatyTypeADO.ID).FirstOrDefault();
                    success = (oneCheck != null && oneCheck.USE_ORIGINAL_UNIT_FOR_PRES == 1);
                    mediMatyTypeADO.IsUseOrginalUnitForPres = success ? (bool?)true : null;

                }
                else
                {
                    mediMatyTypeADO.IsUseOrginalUnitForPres = null;
                    success = true;
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        internal static bool UpdateUnit(DMediStock1ADO mediMatyTypeADO, List<HIS_MEST_METY_UNIT> mestMetyUnitData = null)
        {
            bool success = false;
            try
            {
                if (mediMatyTypeADO == null || (mediMatyTypeADO.MEDI_STOCK_ID ?? 0) == 0 || mediMatyTypeADO.ID == 0)
                    throw new ArgumentNullException("data");
                if (mestMetyUnitData != null && mestMetyUnitData.Count > 0 && mediMatyTypeADO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    var oneCheck = mestMetyUnitData.Where(o => o.MEDI_STOCK_ID == mediMatyTypeADO.MEDI_STOCK_ID && o.MEDICINE_TYPE_ID == mediMatyTypeADO.ID).FirstOrDefault();
                    success = (oneCheck != null && oneCheck.USE_ORIGINAL_UNIT_FOR_PRES == 1);
                    mediMatyTypeADO.IsUseOrginalUnitForPres = success ? (bool?)true : null;                    
                }
                else
                {
                    mediMatyTypeADO.IsUseOrginalUnitForPres = null;
                    success = true;
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
    }
}
