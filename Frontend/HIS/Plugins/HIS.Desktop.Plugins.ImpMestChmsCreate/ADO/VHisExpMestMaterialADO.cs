﻿using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestChmsCreate.ADO
{
    public class VHisExpMestMaterialADO : V_HIS_EXP_MEST_MATERIAL
    {
        public decimal MOBA_AMOUNT { get; set; }
        public decimal CAN_MOBA_AMOUNT { get; set; }
        public bool IsMoba { get; set; }
        public decimal? VIR_TOTAL_IMP_PRICE { get; set; }

        public VHisExpMestMaterialADO()
        {
        }

        public VHisExpMestMaterialADO(V_HIS_EXP_MEST_MATERIAL data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_EXP_MEST_MATERIAL>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(data));
                    }
                    this.VIR_TOTAL_IMP_PRICE = this.AMOUNT * (this.PRICE ?? 0) * (1 + (this.VAT_RATIO ?? 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
