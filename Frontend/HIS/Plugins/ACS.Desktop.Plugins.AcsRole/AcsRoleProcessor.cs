using Inventec.Core;
using Inventec.Desktop.Common;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Plugins.AcsRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACS.Desktop.Plugins.AcsRole;

namespace HIS.Desktop.Plugins.AcsRole
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "ACS.Desktop.Plugins.AcsRole",
       "",
       "Bussiness",
       4,
       "",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true)
    ]
    public class AcsRoleProcessor : ModuleBase, IDesktopRoot
    {
		CommonParam param;
		public AcsRoleProcessor()
        {
            param = new CommonParam();
        }
        public AcsRoleProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }        

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IAcsRole behavior = AcsRoleFactory.MakeIControl(param, args);
                result = behavior != null ? (object)(behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
		
		/// <summary>
        /// Ham tra ve trang thai cua module la enable hay disable
        /// Ghi de gia tri khac theo nghiep vu tung module
        /// </summary>
        /// <returns>true/false</returns>
		public override bool IsEnable()
        {
            bool result = false;
            try
            {
               result = true;
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
