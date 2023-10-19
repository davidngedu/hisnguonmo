using ACS.MANAGER.Core.AcsOtpType.Lock;
using Inventec.Core;
using System;

namespace ACS.MANAGER.Core.AcsOtpType
{
    partial class AcsOtpTypeChangeLock : BeanObjectBase, IDelegacy
    {
        object entity;

        internal AcsOtpTypeChangeLock(CommonParam param, object data)
            : base(param)
        {
            entity = data;
        }

        bool IDelegacy.Execute()
        {
            bool result = false;
            try
            {
                if (TypeCollection.AcsOtpType.Contains(entity.GetType()))
                {
                    IAcsOtpTypeChangeLock behavior = AcsOtpTypeChangeLockBehaviorFactory.MakeIAcsOtpTypeChangeLock(param, entity);
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
