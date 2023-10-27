using ACS.EFMODEL.DataModels;
using ACS.SDO;
using Inventec.Core;
using System;

namespace ACS.MANAGER.Core.AcsUser.ChangePassword
{
    class AcsUserChangePasswordBehaviorFactory
    {
        internal static IAcsUserChangePassword MakeIAcsUserChangePassword(CommonParam param, object data)
        {
            IAcsUserChangePassword result = null;
            try
            {
                if (data.GetType() == typeof(AcsUserChangePasswordSDO))
                {
                    result = new AcsUserChangePasswordBehaviorEv(param, (AcsUserChangePasswordSDO)data);
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                MANAGER.Base.BugUtil.SetBugCode(param, LibraryBug.Bug.Enum.Common__FactoryKhoiTaoDoiTuongThatBai);
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
