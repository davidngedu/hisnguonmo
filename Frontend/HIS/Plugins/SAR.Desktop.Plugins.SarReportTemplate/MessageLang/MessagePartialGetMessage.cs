﻿using SAR.Desktop.Plugins.SarReportTemplate.MessageLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.MessageLang
{
    internal partial class Message
    {
        private string GetMessage(Enum enumBC)
        {
            string message = "";
            if (Language == LanguageEnum.Vietnamese)
            {
                switch (enumBC)
                {
                    case Enum.HeThongTBKQXLYCCuaFrontendThanhCong: message = MessageViResource.HeThongTBKQXLYCCuaFrontendThanhCong;
                        break;
                    case Enum.HeThongTBKQXLYCCuaFrontendThatBai: message = MessageViResource.HeThongTBKQXLYCCuaFrontendThatBai;
                        break;
                    case Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat: message = MessageViResource.HeThongTBXuatHienExceptionChuaKiemDuocSoat;
                        break;
                    case Enum.TaiKhoanKhongCoQuyenThucHienChucNang: message = MessageViResource.TaiKhoanKhongCoQuyenThucHienChucNang;
                        break;
                    case Enum.TieuDeCuaSoThongBaoLaThongBao: message = MessageViResource.TieuDeCuaSoThongBaoLaThongBao;
                        break;
                    case Enum.TieuDeCuaSoThongBaoLaCanhBao: message = MessageViResource.TieuDeCuaSoThongBaoLaCanhBao;
                        break;
                    case Enum.TieuDeCuaSoThongBaoLaLoi: message = MessageViResource.TieuDeCuaSoThongBaoLaLoi;
                        break;
                    case Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong: message = MessageViResource.PhanMemKhongKetNoiDuocToiMayChuHeThong;
                        break;
                    case Enum.HeThongThongBaoMoTaChoWaitDialogForm: message = MessageViResource.HeThongThongBaoMoTaChoWaitDialogForm;
                        break;
                    case Enum.HeThongThongBaoTieuDeChoWaitDialogForm: message = MessageViResource.HeThongThongBaoTieuDeChoWaitDialogForm;
                        break;
                    case Enum.TieuDeThongTinHienThiPhanTrang: message = MessageViResource.TieuDeThongTinHienThiPhanTrang;
                        break;
                    case Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai: message = MessageViResource.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai;
                        break;
                    case Enum.TruongDuLieuBatBuoc: message = MessageViResource.TruongDuLieuBatBuoc;
                        break;
                    case Enum.ThieuTruongDuLieuBatBuoc: message = MessageViResource.ThieuTruongDuLieuBatBuoc;
                        break;
                    case Enum.HeThongTBBanQuyenKhongHopLe: message = MessageViResource.HeThongTBBanQuyenKhongHopLe;
                        break;
                    default: message = defaultViMessage;
                        break;
                }
            }
            else if (Language == LanguageEnum.English)
            {
                switch (enumBC)
                {
                    case Enum.HeThongTBKQXLYCCuaFrontendThanhCong: message = MessageEnResource.HeThongTBKQXLYCCuaFrontendThanhCong;
                        break;
                    case Enum.HeThongTBKQXLYCCuaFrontendThatBai: message = MessageEnResource.HeThongTBKQXLYCCuaFrontendThatBai;
                        break;
                    case Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat: message = MessageEnResource.HeThongTBXuatHienExceptionChuaKiemDuocSoat;
                        break;
                    case Enum.TaiKhoanKhongCoQuyenThucHienChucNang: message = MessageEnResource.TaiKhoanKhongCoQuyenThucHienChucNang;
                        break;
                    case Enum.TieuDeCuaSoThongBaoLaThongBao: message = MessageEnResource.TieuDeCuaSoThongBaoLaThongBao;
                        break;
                    case Enum.TieuDeCuaSoThongBaoLaCanhBao: message = MessageEnResource.TieuDeCuaSoThongBaoLaCanhBao;
                        break;
                    case Enum.TieuDeCuaSoThongBaoLaLoi: message = MessageEnResource.TieuDeCuaSoThongBaoLaLoi;
                        break;
                    case Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong: message = MessageEnResource.PhanMemKhongKetNoiDuocToiMayChuHeThong;
                        break;
                    case Enum.HeThongThongBaoMoTaChoWaitDialogForm: message = MessageEnResource.HeThongThongBaoMoTaChoWaitDialogForm;
                        break;
                    case Enum.HeThongThongBaoTieuDeChoWaitDialogForm: message = MessageEnResource.HeThongThongBaoTieuDeChoWaitDialogForm;
                        break;
                    case Enum.TieuDeThongTinHienThiPhanTrang: message = MessageEnResource.TieuDeThongTinHienThiPhanTrang;
                        break;
                    case Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai: message = MessageEnResource.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai;
                        break;
                    case Enum.TruongDuLieuBatBuoc: message = MessageEnResource.TruongDuLieuBatBuoc;
                        break;
                    case Enum.ThieuTruongDuLieuBatBuoc: message = MessageEnResource.ThieuTruongDuLieuBatBuoc;
                        break;
                    case Enum.HeThongTBBanQuyenKhongHopLe: message = MessageEnResource.HeThongTBBanQuyenKhongHopLe;
                        break;
                    default: message = defaultEnMessage;
                        break;
                }
            }

            return message;
        }
    }
}
