﻿using DevExpress.XtraBars;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Resources;
using Inventec.Common.LocalStorage.SdaConfig;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults
{
    delegate void MouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_LIS_SAMPLE _Sample = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        MouseRightClick _MouseRightClick;
        Point _pointPopup;
        internal enum ItemType
        {
            CapNhatTinhTrangMau,
            LichSuXetNghiem,
            AmTinh,
            DuongTinh
        }

        internal PopupMenuProcessor(V_LIS_SAMPLE sample, BarManager barmanager, MouseRightClick mouseRightClick)
        {
            this._Sample = sample;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
        }

        internal PopupMenuProcessor(BarManager barmanager, MouseRightClick mouseRightClick)
        {
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
        }
        internal PopupMenuProcessor(BarManager barmanager, MouseRightClick mouseRightClick, Point pointPopup)
        {
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
            this._pointPopup = pointPopup;
        }

        internal void InitMenu()
        {
            try
            {
                if (this._Sample == null || this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();

                if (_Sample.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                    && _Sample.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                {
                    BarButtonItem bbtnPhieuCongKhaiBN = new BarButtonItem(this._BarManager, "Cập nhật tình trạng mẫu", 0);
                    bbtnPhieuCongKhaiBN.Tag = ItemType.CapNhatTinhTrangMau;
                    bbtnPhieuCongKhaiBN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { bbtnPhieuCongKhaiBN });

                    BarButtonItem btnLichSuXN = new BarButtonItem(this._BarManager, "Lịch sử xét nghiệm của bệnh nhân", 1);
                    btnLichSuXN.Tag = ItemType.LichSuXetNghiem;
                    btnLichSuXN.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { btnLichSuXN });

                    this._PopupMenu.ShowPopup(Cursor.Position);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void InitMenuResult()
        {
            try
            {
                if (this._BarManager == null || this._MouseRightClick == null)
                    return;
                if (this._PopupMenu == null)
                    this._PopupMenu = new PopupMenu(this._BarManager);
                this._PopupMenu.ItemLinks.Clear();


                BarButtonItem bbtnAmTinh = new BarButtonItem(this._BarManager, "Âm tính", 0);
                bbtnAmTinh.Tag = ItemType.AmTinh;
                bbtnAmTinh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                this._PopupMenu.AddItems(new BarItem[] { bbtnAmTinh });

                BarButtonItem bbtnDuongTinh = new BarButtonItem(this._BarManager, "Dương tính", 1);
                bbtnDuongTinh.Tag = ItemType.DuongTinh;
                bbtnDuongTinh.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                this._PopupMenu.AddItems(new BarItem[] { bbtnDuongTinh });

                this._PopupMenu.ShowPopup(_pointPopup);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
