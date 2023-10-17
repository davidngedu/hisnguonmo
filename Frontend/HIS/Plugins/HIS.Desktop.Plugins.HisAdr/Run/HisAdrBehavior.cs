﻿using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HisAdr;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.HisAdr.Run
{
    public sealed class HisAdrBehavior : Tool<IDesktopToolContext>, IHisAdr
    {
        object[] entity;
        public HisAdrBehavior()
            : base()
        {
        }

        public HisAdrBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisAdr.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module currentModule = null;
                long _TreatmentId = 0;
                V_HIS_ADR _adr = null;
                RefeshReference _RefeshReference = null;
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is long)
                        {
                            _TreatmentId = (long)item;
                        }
                        else if (item is V_HIS_ADR)
                        {
                            _adr = (V_HIS_ADR)item;
                        }
                        else if (item is RefeshReference)
                        {
                            _RefeshReference = (RefeshReference)item;
                        }
                    }

                    if (currentModule != null && _TreatmentId > 0 && _RefeshReference != null)
                    {
                        result = new frmHisAdr(currentModule, _TreatmentId, _RefeshReference);
                    }
                    else if (currentModule != null && _adr != null && _RefeshReference != null)
                    {
                        result = new frmHisAdr(currentModule, _adr, _RefeshReference);
                    }
                }
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
