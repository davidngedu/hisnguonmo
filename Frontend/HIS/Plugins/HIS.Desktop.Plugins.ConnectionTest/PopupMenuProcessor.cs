﻿using DevExpress.XtraBars;
using HIS.Desktop.Plugins.ConnectionTest.Config;
using HIS.Desktop.Plugins.ConnectionTest.Resources;
using Inventec.Common.LocalStorage.SdaConfig;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    delegate void MouseRightClick(object sender, ItemClickEventArgs e);
    class PopupMenuProcessor
    {
        V_LIS_SAMPLE _Sample = null;
        BarManager _BarManager = null;
        PopupMenu _PopupMenu = null;
        MouseRightClick _MouseRightClick;
        internal enum ItemType
        {
            CapNhatTinhTrangMau,
            LichSuXetNghiem,
            TaoEmr,
            PrintEmr,
            CapNhatBarcode
        }

        internal PopupMenuProcessor(V_LIS_SAMPLE sample, BarManager barmanager, MouseRightClick mouseRightClick)
        {
            this._Sample = sample;
            this._MouseRightClick = mouseRightClick;
            this._BarManager = barmanager;
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

                BarButtonItem btnTaoEmr = new BarButtonItem(this._BarManager, "Tạo hồ sơ EMR", 1);
                btnTaoEmr.Tag = ItemType.TaoEmr;
                btnTaoEmr.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                this._PopupMenu.AddItems(new BarItem[] { btnTaoEmr });
                if (LisConfigCFG.IS_AUTO_CREATE_BARCODE == "1")
                {
                    BarButtonItem btnCapNhatBarcode = new BarButtonItem(this._BarManager, "Cập nhật barcode", 1);
                    btnCapNhatBarcode.Tag = ItemType.CapNhatBarcode;
                    btnCapNhatBarcode.ItemClick += new ItemClickEventHandler(this._MouseRightClick);

                    this._PopupMenu.AddItems(new BarItem[] { btnCapNhatBarcode });
                }

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
                }

                if (!String.IsNullOrWhiteSpace(_Sample.EMR_RESULT_DOCUMENT_CODE))
                {
                    BarButtonItem btnPrintEmr = new BarButtonItem(this._BarManager, "In phiếu kết quả đã ký", 1);
                    btnPrintEmr.Tag = ItemType.PrintEmr;
                    btnPrintEmr.ItemClick += new ItemClickEventHandler(this._MouseRightClick);
                    this._PopupMenu.AddItems(new BarItem[] { btnPrintEmr });
                }

                this._PopupMenu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
