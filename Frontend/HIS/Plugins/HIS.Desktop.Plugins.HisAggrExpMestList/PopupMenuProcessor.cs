﻿using DevExpress.XtraBars;
using HIS.Desktop.Plugins.HisAggrExpMestList.Resources;
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAggrExpMestList
{
    delegate void AggrExpMestMouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_HIS_EXP_MEST _ExpMest = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        AggrExpMestMouseRightClick _MouseRightClick;
        List<V_HIS_EXP_MEST> CheckedExpMest;

        internal enum ItemType
        {
            PhieuCongKhaiTheoBN,
            InPhieuLinhTongHop,
            InTraDoiThuocTongHop

        }

        internal PopupMenuProcessor(V_HIS_EXP_MEST expMest, List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST> checkedExpMest, BarManager barmanager, AggrExpMestMouseRightClick mouseRightClick)
        {
            this._ExpMest = expMest;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this.CheckedExpMest = checkedExpMest;
        }

        internal void InitMenu()
        {
            try
            {
                if (this._ExpMest == null || this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();

                if (this.CheckedExpMest != null && this.CheckedExpMest.Count > 0 && this.CheckedExpMest.Exists(o => o.ID == this._ExpMest.ID))
                {
                    BarButtonItem itemInPhieuTongHop = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__IN_PHIEU_LINH_TONG_HOP", ResourceLanguageManager.LanguageUCHisAggrExpMestList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 1);
                    itemInPhieuTongHop.Tag = ItemType.InPhieuLinhTongHop;
                    itemInPhieuTongHop.ItemClick += new ItemClickEventHandler(_MouseRightClick);

                    BarButtonItem itemInTraDoiThuoc = new BarButtonItem(_BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__IN_TRA_DOI_THUOC_TONG_HOP", ResourceLanguageManager.LanguageUCHisAggrExpMestList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 2);
                    itemInTraDoiThuoc.Tag = ItemType.InTraDoiThuocTongHop;
                    itemInTraDoiThuoc.ItemClick += new ItemClickEventHandler(_MouseRightClick);
                    _PopupMenu.AddItems(new BarItem[] { itemInPhieuTongHop, itemInTraDoiThuoc });
                    _PopupMenu.ShowPopup(Cursor.Position);
                }
                else if (this.CheckedExpMest == null)
                {
                    //Phiếu thu thanh toán
                    BarButtonItem bbtnPhieuCongKhaiBN = new BarButtonItem(this._BarManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_AGGREXMEST__POPUP_MENU__ITEM_PHIEU_CONG_KHAI_THEO_BN", ResourceLanguageManager.LanguageUCHisAggrExpMestList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), 0);
                    bbtnPhieuCongKhaiBN.Tag = ItemType.PhieuCongKhaiTheoBN;
                    bbtnPhieuCongKhaiBN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuCongKhaiBN });
                    this._PopupMenu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
