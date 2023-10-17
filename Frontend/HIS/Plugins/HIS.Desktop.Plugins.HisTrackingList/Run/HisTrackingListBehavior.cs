﻿using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HisTrackingList;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.SDO;
using HIS.Desktop.Plugins.HisTrackingList.Run;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.HisTrackingList.Run
{
    public sealed class HisTrackingListBehavior : Tool<IDesktopToolContext>, IHisTrackingList
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId = 0;
        HIS_DHST currentDhst;
        HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter;

        public HisTrackingListBehavior()
            : base()
        {
        }

        public HisTrackingListBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisTrackingList.Run()
        {
            object result = null;
            try
            {
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
                            treatmentId = (long)item;
                        }
                        else if (item is HIS_DHST)
                        {
                            currentDhst = (HIS_DHST)item;
                        }
                        else if (item is HisTreatmentBedRoomLViewFilter)
                        {
                            dataTransferTreatmentBedRoomFilter = (HisTreatmentBedRoomLViewFilter)item;
                        }
                    }
                    if (currentModule != null && treatmentId > 0 && currentDhst != null)
                    {
                        result = new frmHisTrackingList(currentModule, treatmentId, currentDhst, dataTransferTreatmentBedRoomFilter);
                    }
                    else if (currentModule != null && treatmentId > 0)
                    {
                        result = new frmHisTrackingList(currentModule, treatmentId, dataTransferTreatmentBedRoomFilter);
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
