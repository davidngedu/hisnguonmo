using MOS.EFMODEL.DataModels;
using MOS.SDO;
namespace HIS.Desktop.Plugins.Register.Register
{
    interface IServiceRequestRegisterPatientProfile
    {
        HisPatientProfileSDO Run();
    }
}
