using ACS.EFMODEL.DataModels;
using ACS.MANAGER.Core.AcsUser;
using Inventec.Core;
using System;

namespace ACS.MANAGER.Core.Check
{
    class AcsUserCheckVerifyId
    {
        internal static bool Verify(CommonParam param, long id)
        {
            bool result = true;
            try
            {
                ACS_USER raw = new AcsUserBO().Get<ACS_USER>(id);
                if (raw == null)
                {
                    result = false;
                    ACS.MANAGER.Base.BugUtil.SetBugCode(param, LibraryBug.Bug.Enum.Common__KXDDDuLieuCanXuLy);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
                param.HasException = true;
            }
            return result;
        }

        internal static bool Verify(CommonParam param, long id, ref ACS_USER raw)
        {
            bool result = true;
            try
            {
                raw = new AcsUserBO().Get<ACS_USER>(id);
                if (raw == null)
                {
                    result = false;
                    ACS.MANAGER.Base.BugUtil.SetBugCode(param, LibraryBug.Bug.Enum.Common__KXDDDuLieuCanXuLy);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
                param.HasException = true;
            }
            return result;
        }
    }
}
