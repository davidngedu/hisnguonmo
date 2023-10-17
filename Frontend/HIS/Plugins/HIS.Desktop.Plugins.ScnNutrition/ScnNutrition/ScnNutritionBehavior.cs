using Inventec.Core;
using Inventec.Desktop.Common;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ScnNutrition.ScnNutrition
{
    class ScnNutritionBehavior : BusinessBase, IScnNutrition
    {
        object[] entity;
        internal ScnNutritionBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IScnNutrition.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                string personCode = "";
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
                            else if (entity[i] is string)
                            {
                                personCode = (string)entity[i];
                            }
                        }
                    }
                }

                return new frmScnNutrition(moduleData, personCode);
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
