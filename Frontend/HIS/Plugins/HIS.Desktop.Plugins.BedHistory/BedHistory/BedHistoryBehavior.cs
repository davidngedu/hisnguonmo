﻿using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedHistory.BedHistory
{
    class BedHistoryBehavior : Tool<IDesktopToolContext>, IBedHistory
    {
        object[] entity;

        Inventec.Desktop.Common.Modules.Module moduleData = null;
        MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM listBedRoom = new MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM();
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM listBedRoomView = new MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM();
        internal BedHistoryBehavior()
            : base()
        {

        }

        internal BedHistoryBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IBedHistory.Run()
        {
            try
            {
                bool isDisable = false;
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        else if (item is MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM)
                        {
                            listBedRoom = (MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM)item;
                        }
                        else if (item is MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM)
                        {
                            listBedRoomView = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM)item;
                        }
                        else if (item is bool)
                        {
                            isDisable = (bool)item;
                        }
                    }
                }
                if (moduleData != null)
                {
                    if (listBedRoomView != null && listBedRoomView.TREATMENT_ID > 0)
                    {
                        MOS.Filter.HisTreatmentBedRoomLViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        treatFilter.TREATMENT_ID = listBedRoomView.TREATMENT_ID;
                        var result = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<L_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetLView", ApiConsumers.MosConsumer, treatFilter, null);
                        if (result != null && result.Count() > 0)
                        {
                            listBedRoom = result.FirstOrDefault();
                        }
                    }

                    if (!isDisable && listBedRoom.BED_ROOM_ID == 0)
                    {
                        return null;
                    }

                    return new FormBedHistory(listBedRoom, moduleData, isDisable);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
