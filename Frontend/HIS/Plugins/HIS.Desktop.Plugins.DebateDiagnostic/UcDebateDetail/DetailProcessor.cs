﻿using ACS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail
{
    class DetailProcessor
    {
        UcPttt Pttt;
        UcOther Thuoc;
        UcOther Khac;
        long TreatmentId;
        long RoomId;
        long RoomTypeId;
        HIS_SERVICE hisService;
        bool IsSetPttt;
        bool IsSetThuoc;
        bool IsSetKhac;
        public List<ACS_USER> UserList;
        public List<V_HIS_EMPLOYEE> EmployeeList;
        public List<HIS_DEPARTMENT> DepartmentList;
        public List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> ExecuteRoleList;

        public DetailProcessor(long treatmentId, long roomId, long roomTypeId)
        {
            this.TreatmentId = treatmentId;
            this.RoomId = roomId;
            this.RoomTypeId = roomTypeId;
        }
        public DetailProcessor(long treatmentId, long roomId, long roomTypeId, HIS_SERVICE hisService)
        {
            this.TreatmentId = treatmentId;
            this.RoomId = roomId;
            this.RoomTypeId = roomTypeId;
            this.hisService = hisService;
        }

        public UserControl GetControl(DetailEnum type)
        {
            UserControl result = null;
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null)
                        {
                            Thuoc = new UcOther(TreatmentId, RoomId, RoomTypeId, false);
                        }
                        result = Thuoc;
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null)
                        {
                            Pttt = new UcPttt(TreatmentId, RoomId, RoomTypeId, UserList, EmployeeList, DepartmentList, ExecuteRoleList);
                        }
                        result = Pttt;
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null)
                        {
                            if (hisService != null)
                            {
                                Khac = new UcOther(TreatmentId, RoomId, RoomTypeId, true, hisService);
                            }
                            else
                            {
                                Khac = new UcOther(TreatmentId, RoomId, RoomTypeId, true);
                            }

                        }
                        else
                        {
                            if (hisService != null)
                            {
                                Khac = new UcOther(TreatmentId, RoomId, RoomTypeId, true, hisService);
                            }
                        }
                        result = Khac;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void GetData(DetailEnum type, ref HIS_DEBATE saveData)
        {
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return;
                        Thuoc.GetData(ref saveData);
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null) return;
                        Pttt.GetData(ref saveData);
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null) return;
                        Khac.GetData(ref saveData);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetDataDiscussion(DetailEnum type, string content)
		{
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return;
                        Thuoc.SetContent(content);
                        break;              
                    case DetailEnum.Khac:
                        if (Khac == null) return;
                        Khac.SetContent(content);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ValidateControl(DetailEnum type)
        {
            bool result = false;
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return false;
                        result = Thuoc.ValidControl();
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null) return false;
                        result = Pttt.ValidControl();
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null) return false;
                        result = Khac.ValidControl();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void DisableControlItem(DetailEnum type)
        {
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return;
                        Thuoc.DisableControlItem();
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null) return;
                        Pttt.DisableControlItem();
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null) return;
                        Khac.DisableControlItem();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetData(DetailEnum type, object data)
        {
            try
            {
                switch (type)
                {
                    case DetailEnum.Thuoc:
                        if (Thuoc == null) return;
                        Thuoc.SetData(data);
                        break;
                    case DetailEnum.Pttt:
                        if (Pttt == null) return;
                        Pttt.SetData(data);
                        break;
                    case DetailEnum.Khac:
                        if (Khac == null) return;
                        Khac.SetData(data);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetDataMedicine(HIS_DEBATE data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDataMedicine.1");

                if (Thuoc == null) return;
                Thuoc.SetDataMedicine(data);
                Inventec.Common.Logging.LogSystem.Debug("SetDataMedicine.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
