﻿using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestChmsCreate.ADO
{
    public class HisMaterialInStockADO : HisMaterialIn2StockSDO
    {
        public bool IsCheck { get; set; }
        public decimal? ExportedTotalAmount { get; set; }
        public decimal? ImpStockAvailableAmount { get; set; }
        public decimal? ImpStockTotalAmount { get; set; }
        public decimal? EXP_AMOUNT { get; set; }
        public string EXPIRED_DATE_STRS { get; set; }
        public virtual ICollection<HisMaterialInStockADO> MaterialInStocks { get; set; }

        public HisMaterialInStockADO()
        { }

        public HisMaterialInStockADO(HisMaterialTypeInStockSDO item)
        {
            this.AvailableAmount = item.AvailableAmount;
            this.CONCENTRA = item.Concentra;
            this.ExportedTotalAmount = item.ExportedTotalAmount;
            this.ImpStockAvailableAmount = item.ImpStockAvailableAmount;
            this.ImpStockTotalAmount = item.ImpStockTotalAmount;
            this.IS_ACTIVE = item.IsActive;
            this.IS_LEAF = item.IsLeaf;
            this.IsCheck = true;
            this.MANUFACTURER_NAME = item.ManufacturerName;
            this.MATERIAL_TYPE_CODE = item.MaterialTypeCode;
            this.MATERIAL_TYPE_ID = item.Id;
            this.MATERIAL_TYPE_NAME = item.MaterialTypeName;
            this.MEDI_STOCK_ID = item.MediStockId;
            this.NATIONAL_NAME = item.NationalName;
            this.PARENT_ID = item.ParentId;
            this.SERVICE_ID = item.ServiceId;
            this.SERVICE_UNIT_CODE = item.ServiceUnitCode;
            this.SERVICE_UNIT_NAME = item.ServiceUnitName;
            this.TotalAmount = item.TotalAmount;
            this.EXP_AMOUNT = Math.Min(item.AvailableAmount ?? 0, item.ExportedTotalAmount ?? 0);
        }

        public HisMaterialInStockADO(HisMaterialIn2StockSDO item, List<HisMaterialIn2StockSDO> lstItem)
        {
            if (item != null)
            {
                this.ALERT_EXPIRED_DATE = item.ALERT_EXPIRED_DATE;
                this.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                this.AvailableAmount = item.AvailableAmount;
                this.TotalAmount = item.TotalAmount;
                this.AvailableAmount = item.AvailableAmount;
                this.SecondTotalAmount = item.SecondTotalAmount;
                this.SecondAvailableAmount = item.SecondAvailableAmount;
                this.BID_NUMBER = item.BID_NUMBER;
                this.CONCENTRA = item.CONCENTRA;
                this.EXPIRED_DATE = item.EXPIRED_DATE;
                this.PACKAGE_NUMBER = item.PACKAGE_NUMBER;

                if (lstItem != null && lstItem.Count > 0)
                {
                    List<string> lstExpiredDate = new List<string>();
                    this.MaterialInStocks = new List<HisMaterialInStockADO>();

                    foreach (var i in lstItem)
                    {
                        if (i.EXPIRED_DATE.HasValue)
                        {
                            lstExpiredDate.Add(Inventec.Common.DateTime.Convert.TimeNumberToDateString(i.EXPIRED_DATE.ToString()));
                        }

                        HisMaterialInStockADO ado = new HisMaterialInStockADO();
                        ado.EXPIRED_DATE = i.EXPIRED_DATE;
                        ado.PACKAGE_NUMBER = i.PACKAGE_NUMBER;
                        this.MaterialInStocks.Add(ado);
                    }

                    this.EXPIRED_DATE_STRS = String.Join(", ", lstExpiredDate.Distinct());
                    this.PACKAGE_NUMBER = null;
                }

                this.ID = item.ID;
                this.IsCheck = true;
                this.IMP_PRICE = item.IMP_PRICE;
                this.IMP_TIME = item.IMP_TIME;
                this.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                this.IS_ACTIVE = item.IS_ACTIVE;
                this.IS_LEAF = item.IS_LEAF;
                this.isTypeNode = item.isTypeNode;
                this.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                this.MATERIAL_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                this.MATERIAL_TYPE_ID = item.MATERIAL_TYPE_ID;
                this.MATERIAL_TYPE_IS_ACTIVE = item.MATERIAL_TYPE_IS_ACTIVE;
                this.MATERIAL_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                this.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                this.NATIONAL_NAME = item.NATIONAL_NAME;
                this.NodeId = item.NodeId;
                this.NUM_ORDER = item.NUM_ORDER;
                this.PARENT_ID = item.PARENT_ID;
                this.ParentNodeId = item.ParentNodeId;
                this.REGISTER_NUMBER = item.REGISTER_NUMBER;
                this.SERVICE_ID = item.SERVICE_ID;
                this.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                this.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                this.SUPPLIER_CODE = item.SUPPLIER_CODE;
                this.SUPPLIER_ID = item.SUPPLIER_ID;
                this.SUPPLIER_NAME = item.SUPPLIER_NAME;
                this.TotalAmount = item.TotalAmount;

            }
        }
    }
}
