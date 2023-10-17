﻿using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.PatientInfo;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.PatientInfo.Update
{
    public sealed class PatientInfoBehavior : Tool<IDesktopToolContext>, IPatientInfo
    {
        object[] entity;
        public PatientInfoBehavior()
            : base()
        {
        }

        public PatientInfoBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IPatientInfo.Run()
        {
            object result = null;
            RefeshReference refeshReference = null;
            V_HIS_PATIENT patient = null;
            try
            {
                if (entity != null && entity.Length > 0)
                {
                    for (int i = 0; i < entity.Length; i++)
                    {
                        if (entity[i] is V_HIS_PATIENT)
                        {
                            patient = (V_HIS_PATIENT)entity[i];
                        }
                        if (entity[i] is RefeshReference)
                        {
                            refeshReference = (RefeshReference)entity[i];
                        }
                    }
                    if (patient != null)
                    {
                        result = new frmPatientInfo(patient, refeshReference);
                    }
                    else
                    {
                        result = new frmPatientInfo();
                    }
                }
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity));
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
