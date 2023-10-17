using SAR.EFMODEL.DataModels;
using SAR.MANAGER.Base;
using Inventec.Core;
using System;
using SAR.MANAGER.Core.SarPrint;

namespace SAR.MANAGER.Core.SarPrint.Lock
{
    class SarPrintChangeLockBehaviorEv : BeanObjectBase, ISarPrintChangeLock
    {
        SAR_PRINT entity;

        internal SarPrintChangeLockBehaviorEv(CommonParam param, SAR_PRINT data)
            : base(param)
        {
            entity = data;
        }

        bool ISarPrintChangeLock.Run()
        {
            bool result = false;
            try
            {
                SAR_PRINT raw = new SarPrintBO().Get<SAR_PRINT>(entity.ID);
                if (raw != null)
                {
                    if (raw.IS_ACTIVE.HasValue && raw.IS_ACTIVE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        raw.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__FALSE;
                    }
                    else
                    {
                        raw.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                    }
                    result = DAOWorker.SarPrintDAO.Update(raw);
                    if (result) entity.IS_ACTIVE = raw.IS_ACTIVE;
                }
                else
                {
                    BugUtil.SetBugCode(param, SAR.LibraryBug.Bug.Enum.Common__KXDDDuLieuCanXuLy);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                result = false;
            }
            return result;
        }
    }
}
