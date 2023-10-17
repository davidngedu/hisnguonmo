﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportPatient.Message
{
    class MessageImport
    {
        internal const string Maxlength = "{0} vượt quá độ dài cho phép| ";
        internal const string KhongHopLe = "{0} không hợp lệ| ";
        internal const string KhongTimThayThongTin = "Không tìm thấy thông tin {0}| ";
        internal const string ThieuTruongDL = "Thiếu trường {0}| ";


        internal const string ChiChoPhepNhapVoiLoaiDichVuLaPTTT = "Chỉ cho phép nhập với loại dịch vụ là PTTT|";
        internal const string ChuaCoDichVu = "{0} chưa có dịch vụ khám|";
        internal const string ChuaCoPhongKham= "{0} chưa có phòng khám|";
        internal const string ChiDuocNhaoVoiLoaiDichVuLaKham = "{0} chỉ được nhập với loại DV là Khám|";
        internal const string ChiDuocNhapGiaHoacTiLeTran = "Chỉ được nhập giá trần hoặc tỉ lệ trần|";
        internal const string ChiDuocNhapTGVaoVienHoacTGChiDinh = "Chỉ được nhập TG vào viện hoặc TG chỉ định|";
        internal const string ChiDuocNhapGiaGoiKhiCoGoi = "Chỉ được nhập giá gói khi có gói|";
        internal const string TonTaiTrungNhauTrongFileImport = "Tồn tại {0} trùng nhau trong file import|";
        internal const string CoThiPhaiNhap = "Có {0} thì phải nhập {1}|";
        internal const string KhongTonTai = "{0} không tồn tại|";
        internal const string DichVuBoSungKhongtrongNhom = "dịch vụ bổ sung {0} không thuộc nhóm dịch vụ bổ sung|";
        
    }
}
