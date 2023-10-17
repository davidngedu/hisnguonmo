﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.DrugInterventionInfo.ADO
{
    class PatientInfoADO
    {
        /// <summary>
        /// Mã bệnh nhân
        /// </summary>
        public string patientID { get; set; }

        /// <summary>
        /// Mã y tế
        /// </summary>
        public string maYTe { get; set; }

        /// <summary>
        /// Mã định danh 1 đơn thuốc
        /// </summary>
        public string prescriptionId { get; set; }

        /// <summary>
        /// Chiều cao (cm)
        /// </summary>
        public int? height { get; set; }

        /// <summary>
        /// Cân nặng (kg)
        /// </summary>
        public decimal? weight { get; set; }

        /// <summary>
        /// Danh sách mã ICD khi kê toa, ngăn cách bởi dấu phẩy (nếu nhiều hơn 1)
        /// Đối với bệnh nhân nữ có thai, gửi kèm ICD thai kỳ
        /// </summary>
        public string icd { get; set; }

        /// <summary>
        /// Tên bệnh nhân
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// ID và tên bác sĩ cho toa
        /// </summary>
        public ValueItemADO prescriber { get; set; }

        /// <summary>
        /// Ngày giờ toa thuốc bắt đầu
        /// </summary>
        public DateTime? prescriptionTime { get; set; }

        /// <summary>
        /// Ngày tháng năm sinh
        /// </summary>
        public DateTime? dob { get; set; }

        /// <summary>
        /// Ngày giờ kê toa
        /// </summary>
        public DateTime? examinationDate { get; set; }

        /// <summary>
        /// Mã khoa
        /// </summary>
        public string maKhoa { get; set; }

        /// <summary>
        /// Tên khoa
        /// </summary>
        public string tenKhoa { get; set; }

        /// <summary>
        /// Loại dịch vụ theo enum
        /// </summary>
        public DrugEnum.EServiceType serviceType { get; set; }

        /// <summary>
        /// Phòng khám
        /// </summary>
        public string phongKham { get; set; }

        /// <summary>
        /// Mã đối tượng (BHYT, Dịch vụ,…)
        /// </summary>
        public string maDoiTuong { get; set; }
    }
}
