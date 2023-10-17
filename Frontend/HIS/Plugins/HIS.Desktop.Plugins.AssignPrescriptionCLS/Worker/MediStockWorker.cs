﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS
{
    class MediStockWorker
    {
        /// <summary>
        /// Kiểm tra cấu hình có hiển thị thuốc hay không trong bảng MEDI_STOCK_METY, nếu IS_PREVENT_EXP = true thì không hiển thị loại thuốc ứng với kho xuất trong danh sách thuốc.
        /// </summary>
        /// <param name="mediStockIds"></param>
        internal static void FilterByMediStockMeti(List<long> mediStockIds, ref List<D_HIS_MEDI_STOCK_1> mediStockD1SDOs)
        {
            try
            {
                if (mediStockIds == null || mediStockIds.Count == 0) throw new ArgumentNullException("mediStockIds");

                var mediStockMetis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY>();
                if (mediStockMetis != null && mediStockMetis.Count > 0)
                {
                    var mediStockMetis__IdNotAllows = mediStockMetis.Where(o => mediStockIds.Contains(o.MEDI_STOCK_ID) && o.IS_PREVENT_EXP == 1).Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                    if (mediStockMetis__IdNotAllows != null && mediStockMetis__IdNotAllows.Count > 0)
                    {
                        mediStockD1SDOs = mediStockD1SDOs
                            .Where(o =>
                                o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !mediStockMetis__IdNotAllows.Contains(o.ID ?? 0))
                                ).ToList();

                        LogSystem.Debug("Load du lieu kho theo du lieu theo dieu kien MEDI_STOCK_METY.IS_PREVENT_EXP nếu = true thì không hiển thị loại thuốc ứng với kho xuất trong danh sách thuốc.____ " + "____ket qua tim thay " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thuốc trong kho cấu hình chỉ có khoa nào được phép sử dụng
        /// Sử dụng bảng HIS_MEST_METY_DEPA.
        /// Căn cứ theo khoa yêu cầu, kho được chọn --> không hiển thị các loại thuốc bị khóa.
        /// Nếu không tồn tại dữ liệu medi_stock_id & department_id nào trong bảng này tức là khoa yêu cầu không bị kho đó khóa thuốc nào cả.
        /// </summary>
        /// <param name="mediStockIds"></param>
        internal static void FilterByMestMetyDepa(List<long> mediStockIds, MOS.SDO.WorkPlaceSDO currentWorkPlace, ref List<D_HIS_MEDI_STOCK_1> mediStockD1SDOs)
        {
            try
            {
                if (mediStockIds == null || mediStockIds.Count == 0) throw new ArgumentNullException("mediStockIds");

                var mestMetyDepas = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA>();
                if (mestMetyDepas != null && mestMetyDepas.Count > 0)
                {
                    var mestMetyDepasNoMediStock__NotAllows = mestMetyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && (o.MEDI_STOCK_ID == null))
                        .Distinct().ToList();

                    var mestMetyDepasHasMediStock__NotAllows = mestMetyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && (o.MEDI_STOCK_ID != null && mediStockIds.Contains(o.MEDI_STOCK_ID.Value)))
                        .Distinct().ToList();

                    if (mestMetyDepasNoMediStock__NotAllows != null && mestMetyDepasNoMediStock__NotAllows.Count > 0)
                    {
                        var mestMetyDepas__IdNotAllows = mestMetyDepasNoMediStock__NotAllows.Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                        mediStockD1SDOs = mediStockD1SDOs
                                .Where(o =>
                                    o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                    || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !mestMetyDepas__IdNotAllows.Contains(o.ID ?? 0))
                                    ).ToList();
                    }

                    if (mestMetyDepasHasMediStock__NotAllows != null && mestMetyDepasHasMediStock__NotAllows.Count > 0)
                    {
                        var medistock__f1 = (from m in mediStockD1SDOs
                                             from n in mestMetyDepasHasMediStock__NotAllows
                                             where m.MEDI_STOCK_ID == n.MEDI_STOCK_ID
                                             && m.ID == n.MEDICINE_TYPE_ID
                                             && m.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                             select m).ToList();
                        if (medistock__f1 != null && medistock__f1.Count > 0)
                        {
                            mediStockD1SDOs = mediStockD1SDOs.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !medistock__f1.Any(t => t.ID == o.ID && t.MEDI_STOCK_ID == o.MEDI_STOCK_ID && t.SERVICE_TYPE_ID == o.SERVICE_TYPE_ID))).ToList();
                        }
                    }
                    //var mestMetyDepas__IdNotAllows = mestMetyDepas
                    //    .Where(o =>
                    //        o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                    //        && (o.MEDI_STOCK_ID == null || (o.MEDI_STOCK_ID != null && mediStockIds.Contains(o.MEDI_STOCK_ID.Value))))
                    //    .Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                    //if (mestMetyDepas__IdNotAllows != null && mestMetyDepas__IdNotAllows.Count > 0)
                    //{
                    //    mediStockD1SDOs = mediStockD1SDOs
                    //        .Where(o =>
                    //            o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                    //            || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !mestMetyDepas__IdNotAllows.Contains(o.ID ?? 0))
                    //            ).ToList();
                    //}

                    LogSystem.Debug("Load du lieu kho theo du lieu theo dieu kien Thuốc trong kho cấu hình chỉ có khoa nao được phep su dung.____ " + "____ket qua tim thay " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                }

                var mestMatyDepas = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_MATY_DEPA>();
                if (mestMatyDepas != null && mestMatyDepas.Count > 0)
                {
                    var mestMetyDepasNoMediStock__NotAllows = mestMatyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && (o.MEDI_STOCK_ID == null))
                        .Distinct().ToList();

                    var mestMetyDepasHasMediStock__NotAllows = mestMatyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && (o.MEDI_STOCK_ID != null && mediStockIds.Contains(o.MEDI_STOCK_ID.Value)))
                        .Distinct().ToList();

                    if (mestMetyDepasNoMediStock__NotAllows != null && mestMetyDepasNoMediStock__NotAllows.Count > 0)
                    {
                        var mestMetyDepas__IdNotAllows = mestMetyDepasNoMediStock__NotAllows.Select(o => o.MATERIAL_TYPE_ID).Distinct().ToList();
                        mediStockD1SDOs = mediStockD1SDOs
                                .Where(o =>
                                    o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                    || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && !mestMetyDepas__IdNotAllows.Contains(o.ID ?? 0))
                                    ).ToList();
                    }

                    if (mestMetyDepasHasMediStock__NotAllows != null && mestMetyDepasHasMediStock__NotAllows.Count > 0)
                    {
                        var medistock__f1 = (from m in mediStockD1SDOs
                                             from n in mestMetyDepasHasMediStock__NotAllows
                                             where m.MEDI_STOCK_ID == n.MEDI_STOCK_ID
                                             && m.ID == n.MATERIAL_TYPE_ID
                                             && m.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                             select m).ToList();
                        if (medistock__f1 != null && medistock__f1.Count > 0)
                        {
                            mediStockD1SDOs = mediStockD1SDOs.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && !medistock__f1.Any(t => t.ID == o.ID && t.MEDI_STOCK_ID == o.MEDI_STOCK_ID && t.SERVICE_TYPE_ID == o.SERVICE_TYPE_ID))).ToList();
                        }
                    }
                    //var mestMatyDepas__IdNotAllows = mestMatyDepas
                    //    .Where(o =>
                    //        o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                    //        && (o.MEDI_STOCK_ID == null || (o.MEDI_STOCK_ID != null && mediStockIds.Contains(o.MEDI_STOCK_ID.Value))))
                    //    .Select(o => o.MATERIAL_TYPE_ID).Distinct().ToList();
                    //if (mestMatyDepas__IdNotAllows != null && mestMatyDepas__IdNotAllows.Count > 0)
                    //{
                    //    mediStockD1SDOs = mediStockD1SDOs
                    //        .Where(o =>
                    //            o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                    //            || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && !mestMatyDepas__IdNotAllows.Contains(o.ID ?? 0))
                    //            ).ToList();
                    //}

                    LogSystem.Debug("Load du lieu kho theo du lieu theo dieu kien vật tư trong kho cấu hình chỉ có khoa nao được phep su dung.____ " + "____ket qua tim thay " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra cấu hình có hiển thị thuốc hay không trong bảng MEDI_STOCK_METY, nếu IS_PREVENT_EXP = true thì không hiển thị loại thuốc ứng với kho xuất trong danh sách thuốc.
        /// </summary>
        /// <param name="mediStockIds"></param>
        internal static void FilterByMediStockMetiD2(List<long> mediStockIds, ref List<D_HIS_MEDI_STOCK_2> mediStockDSDOs)
        {
            try
            {
                if (mediStockIds == null || mediStockIds.Count == 0) throw new ArgumentNullException("mediStockIds");

                var mediStockMetis = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK_METY>();
                if (mediStockMetis != null && mediStockMetis.Count > 0)
                {
                    var mediStockMetis__IdNotAllows = mediStockMetis.Where(o => mediStockIds.Contains(o.MEDI_STOCK_ID) && o.IS_PREVENT_EXP == 1).Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                    if (mediStockMetis__IdNotAllows != null && mediStockMetis__IdNotAllows.Count > 0)
                    {
                        mediStockDSDOs = mediStockDSDOs
                            .Where(o =>
                                o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !mediStockMetis__IdNotAllows.Contains(o.ID ?? 0))
                                ).ToList();

                        LogSystem.Debug("Load du lieu kho theo du lieu theo dieu kien MEDI_STOCK_METY.IS_PREVENT_EXP nếu = true thì không hiển thị loại thuốc ứng với kho xuất trong danh sách thuốc.____ " + "____ket qua tim thay " + (mediStockDSDOs != null ? mediStockDSDOs.Count : 0));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thuốc trong kho cấu hình chỉ có khoa nào được phép sử dụng
        /// Sử dụng bảng HIS_MEST_METY_DEPA.
        /// Căn cứ theo khoa yêu cầu, kho được chọn --> không hiển thị các loại thuốc bị khóa.
        /// Nếu không tồn tại dữ liệu medi_stock_id & department_id nào trong bảng này tức là khoa yêu cầu không bị kho đó khóa thuốc nào cả.
        /// </summary>
        /// <param name="mediStockIds"></param>
        internal static void FilterByMestMetyDepaD2(List<long> mediStockIds, MOS.SDO.WorkPlaceSDO currentWorkPlace, ref List<D_HIS_MEDI_STOCK_2> mediStockD1SDOs)
        {
            try
            {
                if (mediStockIds == null || mediStockIds.Count == 0) throw new ArgumentNullException("mediStockIds");

                var mestMetyDepas = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_METY_DEPA>();
                if (mestMetyDepas != null && mestMetyDepas.Count > 0)
                {
                    var mestMetyDepasNoMediStock__NotAllows = mestMetyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && (o.MEDI_STOCK_ID == null))
                        .Distinct().ToList();

                    var mestMetyDepasHasMediStock__NotAllows = mestMetyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && (o.MEDI_STOCK_ID != null && mediStockIds.Contains(o.MEDI_STOCK_ID.Value)))
                        .Distinct().ToList();

                    if (mestMetyDepasNoMediStock__NotAllows != null && mestMetyDepasNoMediStock__NotAllows.Count > 0)
                    {
                        var mestMetyDepas__IdNotAllows = mestMetyDepasNoMediStock__NotAllows.Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                        mediStockD1SDOs = mediStockD1SDOs
                                .Where(o =>
                                    o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                    || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !mestMetyDepas__IdNotAllows.Contains(o.ID ?? 0))
                                    ).ToList();
                    }

                    if (mestMetyDepasHasMediStock__NotAllows != null && mestMetyDepasHasMediStock__NotAllows.Count > 0)
                    {
                        var medistock__f1 = (from m in mediStockD1SDOs
                                             from n in mestMetyDepasHasMediStock__NotAllows
                                             where m.MEDI_STOCK_ID == n.MEDI_STOCK_ID
                                             && m.ID == n.MEDICINE_TYPE_ID
                                             && m.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                             select m).ToList();
                        if (medistock__f1 != null && medistock__f1.Count > 0)
                        {
                            mediStockD1SDOs = mediStockD1SDOs.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && !medistock__f1.Any(t => t.ID == o.ID && t.MEDI_STOCK_ID == o.MEDI_STOCK_ID && t.SERVICE_TYPE_ID == o.SERVICE_TYPE_ID))).ToList();
                        }
                    }

                    LogSystem.Debug("Load du lieu kho theo du lieu theo dieu kien Thuốc trong kho cấu hình chỉ có khoa nao được phep su dung.____ " + "____ket qua tim thay " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                }

                var mestMatyDepas = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEST_MATY_DEPA>();
                if (mestMatyDepas != null && mestMatyDepas.Count > 0)
                {
                    var mestMetyDepasNoMediStock__NotAllows = mestMatyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && (o.MEDI_STOCK_ID == null))
                        .Distinct().ToList();

                    var mestMetyDepasHasMediStock__NotAllows = mestMatyDepas
                        .Where(o =>
                            o.DEPARTMENT_ID == currentWorkPlace.DepartmentId
                            && (o.MEDI_STOCK_ID != null && mediStockIds.Contains(o.MEDI_STOCK_ID.Value)))
                        .Distinct().ToList();

                    if (mestMetyDepasNoMediStock__NotAllows != null && mestMetyDepasNoMediStock__NotAllows.Count > 0)
                    {
                        var mestMetyDepas__IdNotAllows = mestMetyDepasNoMediStock__NotAllows.Select(o => o.MATERIAL_TYPE_ID).Distinct().ToList();
                        mediStockD1SDOs = mediStockD1SDOs
                                .Where(o =>
                                    o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                    || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && !mestMetyDepas__IdNotAllows.Contains(o.ID ?? 0))
                                    ).ToList();
                    }

                    if (mestMetyDepasHasMediStock__NotAllows != null && mestMetyDepasHasMediStock__NotAllows.Count > 0)
                    {
                        var medistock__f1 = (from m in mediStockD1SDOs
                                             from n in mestMetyDepasHasMediStock__NotAllows
                                             where m.MEDI_STOCK_ID == n.MEDI_STOCK_ID
                                             && m.ID == n.MATERIAL_TYPE_ID
                                             && m.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                             select m).ToList();
                        if (medistock__f1 != null && medistock__f1.Count > 0)
                        {
                            mediStockD1SDOs = mediStockD1SDOs.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                || (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && !medistock__f1.Any(t => t.ID == o.ID && t.MEDI_STOCK_ID == o.MEDI_STOCK_ID && t.SERVICE_TYPE_ID == o.SERVICE_TYPE_ID))).ToList();
                        }
                    }

                    LogSystem.Debug("Load du lieu kho theo du lieu theo dieu kien vật tư trong kho cấu hình chỉ có khoa nao được phep su dung.____ " + "____ket qua tim thay " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void FilterByRankEmployee(string loginname, ref List<D_HIS_MEDI_STOCK_2> mediStockD1SDOs)
        {
            try
            {
                List<HIS_EMPLOYEE> employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>();
                HIS_EMPLOYEE employee = employees.FirstOrDefault(o => o.LOGINNAME == loginname);
                if (employee == null || !employee.MEDICINE_TYPE_RANK.HasValue)
                {
                    mediStockD1SDOs = mediStockD1SDOs.Where(o => o.RANK == null).ToList();
                    LogSystem.Debug("employee == null || !employee.MEDICINE_TYPE_RANK.HasValue " + ".____ " + "____ket qua tim thay sau khi loc " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                }
                else
                {
                    mediStockD1SDOs = mediStockD1SDOs.Where(o => o.RANK == null || o.RANK <= employee.MEDICINE_TYPE_RANK).ToList();
                    LogSystem.Debug("employee != null && employee.MEDICINE_TYPE_RANK.HasValue " + ".____ " + "____ket qua tim thay sau khi loc " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void FilterByRestrict(long roomId, ref List<D_HIS_MEDI_STOCK_2> mediStockD1SDOs)
        {
            try
            {
                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (room != null && (room.IS_RESTRICT_MEDICINE_TYPE ?? 0) == 1)
                {
                    List<HIS_MEDICINE_TYPE_ROOM> medicineTypeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE_ROOM>()
                        .Where(o => o.ROOM_ID == roomId).ToList();
                    List<long> medicineTypeIdRooms = medicineTypeRooms != null ? medicineTypeRooms.Select(o => o.MEDICINE_TYPE_ID).ToList() : new List<long>();
                    mediStockD1SDOs = mediStockD1SDOs.Where(o => medicineTypeIdRooms.Contains(o.ID ?? 0)
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                    LogSystem.Debug("room.IS_RESTRICT_MEDICINE_TYPE = " + room.IS_RESTRICT_MEDICINE_TYPE + ".____ " + "____ket qua tim thay sau khi loc " + (mediStockD1SDOs != null ? mediStockD1SDOs.Count : 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
