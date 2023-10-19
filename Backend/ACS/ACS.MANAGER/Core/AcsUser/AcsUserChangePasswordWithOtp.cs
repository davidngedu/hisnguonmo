using ACS.MANAGER.Core.AcsUser.ChangPasswordWithOtp;
using Inventec.Core;
using System;

namespace ACS.MANAGER.Core.AcsUser
{
    partial class AcsUserChangePasswordWithOtp : BeanObjectBase, IDelegacy
    {
        object entity;

        internal AcsUserChangePasswordWithOtp(CommonParam param, object data)
            : base(param)
        {
            entity = data;
        }

        bool IDelegacy.Execute()
        {
            bool result = false;
            try
            {
                if (TypeCollection.AcsUser.Contains(entity.GetType()))
                {
                    IAcsUserChangPasswordWithOtp behavior = AcsUserChangPasswordWithOtpBehaviorFactory.MakeIAcsUserChangPasswordWithOtp(param, entity);
                    result = behavior != null ? behavior.Run() : false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
