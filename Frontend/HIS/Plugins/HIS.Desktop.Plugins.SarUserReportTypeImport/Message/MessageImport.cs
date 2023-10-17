﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SarUserReportTypeImport.Message
{
    class MessageImport
    {
        internal const string Maxlength = "{0} vượt quá maxlength|";
        internal const string KhongHopLe = "{0} không hợp lệ|";
        internal const string ThieuTruongDL = "Thiếu trường {0}|";
        internal const string DaTonTai = " {0} đã tồn tại trong báo cáo|";
        internal const string DaTonTaiLoaiGiuong = " {0} đã tồn tại trong loại báo cáo|";
        internal const string TonTaiTrungNhauTrongFileImport = "Tồn tại {0} trùng nhau trong file import|";
        internal const string CoThiPhaiNhap = "Có {0} thì phải nhập {1}|";
        internal const string LoaiBaoCao = "Loại báo cáo {0} đã bị khóa| ";
        internal const string MaLoaiBaoCaoDaKhoa = "Mã báo cáo {0} đã bị khóa| ";
        internal const string FileImportDaTonTai = "File import tồn tại báo cáo giống nhau {0} | ";
    }
}
