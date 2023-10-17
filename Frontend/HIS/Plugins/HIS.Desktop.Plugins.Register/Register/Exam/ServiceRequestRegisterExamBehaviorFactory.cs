using HIS.Desktop.Plugins.Register.Run;
using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.Register.Register
{
    class ServiceRequestRegisterExamBehaviorFactory
    {
        internal static IServiceRequestRegisterExam MakeIServiceRequestRegister(CommonParam param, object data, object patient)
        {
            IServiceRequestRegisterExam result = null;
            try
            {
                result = new ServiceRequestRegisterExamBehavior(param, (UCRegister)data, (MOS.SDO.HisPatientSDO)patient);

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
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
