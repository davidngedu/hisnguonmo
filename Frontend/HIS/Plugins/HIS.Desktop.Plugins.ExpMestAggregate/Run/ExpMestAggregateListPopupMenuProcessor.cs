﻿using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestAggregate
{
    delegate void ExpMestAggregateMouseRight_Click(object sender, ItemClickEventArgs e);

    internal class ExpMestAggregateListPopupMenuProcessor
    {
        MOS.EFMODEL.DataModels.V_HIS_EXP_MEST _ExpMestMouseRight;
        ExpMestAggregateMouseRight_Click expMestAggregatePrintClick;
        BarManager barManager;
        PopupMenu menu;
        List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST> CheckedExpMest;

        internal enum PrintType
        {
            InTraDoiThuoc,
            InPhieuTongHop,
            InPhieuLinhThuocGayNghienHuongTT,
            InPhieuLinhThuoc,
            InPhieuLinhThuocTheoBenhNhan,
            InPhieuLinhTongHop,
            InTraDoiThuocTongHop,
            InPhieuHuyThuocVatTu_434,
            InPhieuLinhThuocTheoBenhNhanTongHop,
            InTraPhieuDoiThuoc,
        }

        internal ExpMestAggregateListPopupMenuProcessor(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST _expMest, List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST> checkedExpMest, ExpMestAggregateMouseRight_Click aggregatePrintClick, BarManager barManager)
        {
            try
            {
                this._ExpMestMouseRight = _expMest;
                this.expMestAggregatePrintClick = aggregatePrintClick;
                this.barManager = barManager;
                this.CheckedExpMest = checkedExpMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);

                menu.ItemLinks.Clear();

                if (this.CheckedExpMest != null && this.CheckedExpMest.Count > 0 && this.CheckedExpMest.Exists(o => o.ID == this._ExpMestMouseRight.ID))
                {
                    BarButtonItem itemInPhieuTongHop = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_LINH_TONG_HOP", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    itemInPhieuTongHop.Tag = PrintType.InPhieuLinhTongHop;
                    itemInPhieuTongHop.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInPhieuTongHop);

                    BarButtonItem itemInTraDoiThuoc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_TRA_DOI_THUOC_TONG_HOP", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    itemInTraDoiThuoc.Tag = PrintType.InTraDoiThuocTongHop;
                    itemInTraDoiThuoc.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInTraDoiThuoc);

                    BarButtonItem itemInTraPhieuDoiThuoc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_TRA_DOI_THUOC", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                    itemInTraPhieuDoiThuoc.Tag = PrintType.InTraPhieuDoiThuoc;
                    itemInTraPhieuDoiThuoc.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInTraPhieuDoiThuoc);

                    BarButtonItem itemInPhieuLinhTheoBenhNhan = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_LINH_THUOC_THEO_BENH_NHAN", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    itemInPhieuLinhTheoBenhNhan.Tag = PrintType.InPhieuLinhThuocTheoBenhNhanTongHop;
                    itemInPhieuLinhTheoBenhNhan.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInPhieuLinhTheoBenhNhan);

                    menu.ShowPopup(Cursor.Position);
                }
                else if (this.CheckedExpMest == null)
                {
                    BarButtonItem itemInPhieuTongHop = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_TONG_HOP", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    itemInPhieuTongHop.Tag = PrintType.InPhieuTongHop;
                    itemInPhieuTongHop.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInPhieuTongHop);

                    BarButtonItem itemInTraDoiThuoc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_TRA_DOI_THUOC", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    itemInTraDoiThuoc.Tag = PrintType.InTraDoiThuoc;
                    itemInTraDoiThuoc.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInTraDoiThuoc);

                    BarButtonItem itemInTraPhieuDoiThuoc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_TRA_DOI_THUOC", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 3);
                    itemInTraPhieuDoiThuoc.Tag = PrintType.InTraPhieuDoiThuoc;
                    itemInTraPhieuDoiThuoc.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInTraPhieuDoiThuoc);

                    BarButtonItem itemInPhieuLinhThuoc = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_LINH_THUOC", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    itemInPhieuLinhThuoc.Tag = PrintType.InPhieuLinhThuoc;
                    itemInPhieuLinhThuoc.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInPhieuLinhThuoc);

                    BarButtonItem itemInPhieuLinhTheoBenhNhan = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_LINH_THUOC_THEO_BENH_NHAN", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                    itemInPhieuLinhTheoBenhNhan.Tag = PrintType.InPhieuLinhThuocTheoBenhNhan;
                    itemInPhieuLinhTheoBenhNhan.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                    menu.AddItem(itemInPhieuLinhTheoBenhNhan);

                    if (this._ExpMestMouseRight != null && this._ExpMestMouseRight.HAS_NOT_PRES == 1)
                    {
                        BarButtonItem itemInPhieuHuyThuocVatTu = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXP_MEST_AGGREGATE__IN_PHIEU_HUY_THUOC_VAT_TU", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 4);
                        itemInPhieuHuyThuocVatTu.Tag = PrintType.InPhieuHuyThuocVatTu_434;
                        itemInPhieuHuyThuocVatTu.ItemClick += new ItemClickEventHandler(expMestAggregatePrintClick);
                        menu.AddItem(itemInPhieuHuyThuocVatTu);
                    }

                    menu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
