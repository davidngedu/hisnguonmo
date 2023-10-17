﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.Save
{
    class SaveInitBehavior : SaveAbstract, ISaveInit
    {
        HIS_IMP_MEST _ImpMestUp { get; set; }
        internal SaveInitBehavior(CommonParam param,
            List<VHisServiceADO> serviceADOs,
            UCImpMestCreate ucImpMestCreate,
            Dictionary<string, V_HIS_BID_MEDICINE_TYPE> dicbidmedicine,
            Dictionary<string, V_HIS_BID_MATERIAL_TYPE> dicbidmaterial,
            long roomId,
            ResultImpMestADO resultADO)
            : base(param,
                serviceADOs,
                ucImpMestCreate,
                dicbidmedicine,
                dicbidmaterial,
                roomId,
                resultADO)
        {
            this._ImpMestUp = ucImpMestCreate._currentImpMestUp;
        }

        object ISaveInit.Run()
        {
            ResultImpMestADO result = null;
            if (this.CheckValid())
            {
                this.InitBase();

                HisImpMestInitSDO inputImpMestSDO = new HisImpMestInitSDO();
                inputImpMestSDO.ImpMest = this._ImpMestUp != null ? this._ImpMestUp : this.ImpMest;
                inputImpMestSDO.InitMaterials = this.MaterialWithPatySDOs;
                inputImpMestSDO.InitMedicines = this.MedicineWithPatySDOs;

                if (this.ResultADO != null)
                {
                    inputImpMestSDO.ImpMest = this.ResultADO.HisInitSDO.ImpMest;
                }
                HisImpMestInitSDO rs = null;
                //if (this.ResultADO == null)

                if (this._ImpMestUp == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CALL API: api/HisImpMest/InitCreate" + Inventec.Common.Logging.LogUtil.TraceData("", inputImpMestSDO));
                    rs = new Inventec.Common.Adapter.BackendAdapter(Param).Post<HisImpMestInitSDO>("api/HisImpMest/InitCreate", ApiConsumers.MosConsumer, inputImpMestSDO, Param);
                }
                else
                {
                    inputImpMestSDO.ImpMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                    Inventec.Common.Logging.LogSystem.Debug("CALL API: api/HisImpMest/InitUpdate" + Inventec.Common.Logging.LogUtil.TraceData("", inputImpMestSDO));
                    rs = new Inventec.Common.Adapter.BackendAdapter(Param).Post<HisImpMestInitSDO>("api/HisImpMest/InitUpdate", ApiConsumers.MosConsumer, inputImpMestSDO, Param);
                }
                if (rs != null)
                    result = this.ResultADO = new ResultImpMestADO(rs);
                //else
                //{
                //    MessageManager.Show(Param, false);
                //}
            }

            return result;
        }
    }
}
