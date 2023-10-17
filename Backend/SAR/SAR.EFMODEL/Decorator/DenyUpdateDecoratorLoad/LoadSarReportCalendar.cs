using SAR.EFMODEL.DataModels;
using System.Collections.Generic;
using System.Reflection;

namespace SAR.EFMODEL.Decorator
{
    public partial class DenyUpdateDecorator
    {
        private static void LoadSarReportCalendar()
        {
            List<string> pies = new List<string>();
            pies.Add("CREATOR");
            pies.Add("APP_CREATOR");
            pies.Add("CREATE_TIME");

            properties[typeof(SAR_REPORT_CALENDAR)] = pies;
        }
    }
}
