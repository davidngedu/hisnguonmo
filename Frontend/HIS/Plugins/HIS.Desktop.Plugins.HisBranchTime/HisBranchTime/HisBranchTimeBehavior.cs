using Inventec.Core;
using Inventec.Desktop.Common;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisBranchTime
{
    class HisBranchTimeBehavior : BusinessBase, IHisBranchTime
    {
        object[] entity;
        internal HisBranchTimeBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisBranchTime.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                HIS_BRANCH executeRoom = null;

                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            if (entity[i] is HIS_BRANCH)
                            {
                                executeRoom = (HIS_BRANCH)entity[i];
                            }
                        }
                    }
                }

                if (executeRoom != null)
                {
                    return new frmHisBranchTime(moduleData, executeRoom);
                }
                else
                {
                    return new frmHisBranchTime(moduleData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
