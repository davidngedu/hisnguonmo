﻿using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApproveAggrExpMest.ADO
{
    public class ExpMestMaterialSDODetail : V_HIS_EXP_MEST_MATERIAL
    {
        public decimal? TotalAmount { get; set; }

        public ExpMestMaterialSDODetail(V_HIS_EXP_MEST_MATERIAL _data)
        {
            try
            {
                if (_data != null)
                {

                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_EXP_MEST_MATERIAL>();

                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(_data)));
                    }
                }

            }

            catch (Exception)
            {

            }
        }
    }
}
