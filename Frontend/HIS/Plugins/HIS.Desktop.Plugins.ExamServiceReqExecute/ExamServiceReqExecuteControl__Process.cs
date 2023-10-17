﻿using DevExpress.XtraTab;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Plugins.ExamServiceReqExecute.ConnectCOM;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Resources;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Sda.SdaEventLogCreate;
using HIS.Desktop.Plugins.Library.PrintPrescription;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Utility;
using HIS.UC.ExamFinish.ADO;
using HIS.UC.ExamTreatmentFinish.ADO;
using HIS.UC.HisExamServiceAdd.ADO;
using HIS.UC.Hospitalize.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void onClickSaveFormAsyncForOtherButtonClick()
        {
            try
            {
                LogTheadInSessionInfo(() => onClickSaveFormAsyncForOtherButtonClick_Action(), "Lưu xử lý khám (Tự động lưu Async)");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task onClickSaveFormAsyncForOtherButtonClick_Action()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("onClickSaveFormAsyncForOtherButtonClick.1");
                HisServiceReqExamUpdateSDO hisServiceReqSDO = new HisServiceReqExamUpdateSDO();

                ProcessExamServiceReqDTO(ref hisServiceReqSDO);
                ProcessExamSereIcdDTO(ref hisServiceReqSDO);
                ProcessExamSereNextTreatmentIntructionDTO(ref hisServiceReqSDO);
                ProcessExamSereDHST(ref hisServiceReqSDO);


                hisServiceReqSDO.RequestRoomId = moduleData.RoomId;
                HisServiceReqExamUpdateResultSDO HisServiceReqResult = await new BackendAdapter(param)
                    .PostAsync<HisServiceReqExamUpdateResultSDO>("api/HisServiceReq/ExamUpdate", ApiConsumers.MosConsumer, hisServiceReqSDO, param);

                if (HisServiceReqResult != null)
                {
                    this.HisServiceReqView = new V_HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(this.HisServiceReqView, HisServiceReqResult.ServiceReq);
                    EnableButtonByServiceReq(HisServiceReqResult.ServiceReq.SERVICE_REQ_STT_ID);
                    if (reLoadServiceReq != null)
                        reLoadServiceReq(HisServiceReqResult.ServiceReq);
                    btnPrint_ExamService.Enabled = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("onClickSaveFormAsyncForOtherButtonClick: goi api luu thong xu ly kham that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisServiceReqSDO), hisServiceReqSDO) + "____Ket qua tra ve:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisServiceReqResult), HisServiceReqResult));
                }
                Inventec.Common.Logging.LogSystem.Debug("onClickSaveFormAsyncForOtherButtonClick.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickDichVuHenKham(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AppointmentService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AppointmentService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(treatment.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickAssBlood(object sender, EventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisAssignBlood");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                    AssignBloodADO assignBlood = new AssignBloodADO(HisServiceReqView.TREATMENT_ID, intructionTime, 0);
                    assignBlood.GenderName = HisServiceReqView.TDL_PATIENT_GENDER_NAME;
                    assignBlood.PatientName = HisServiceReqView.TDL_PATIENT_NAME;
                    assignBlood.PatientDob = HisServiceReqView.TDL_PATIENT_DOB;
                    assignBlood.DgProcessRefeshIcd = RefeshIcd;
                    listArgs.Add(assignBlood);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickAssignPaan(object sender, EventArgs e)
        {
            try
            {

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(HisServiceReqView);

                    listArgs.Add(HisServiceReqView.TREATMENT_ID);
                    listArgs.Add(HisServiceReqView.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickDrugReaction(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MediReactSum").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.MediReactSum");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(HisServiceReqView.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickTruyenDich(object sender, EventArgs e)
        {
            try
            {

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfusionSumByTreatment").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.InfusionSumByTreatment");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(HisServiceReqView.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickDebateDiagnostic(object sender, EventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.HisServiceReqView);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void onClickMedicalAssessment(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMedicalAssessment").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisMedicalAssessment");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.HisServiceReqView.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickDebateDiagnosticList(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Debate").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Debate");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.HisServiceReqView.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickAccidenHurt(object sender, EventArgs e)
        {
            try
            {
                btnAccidentHurt_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickTrackingList(object sender, EventArgs e)
        {
            try
            {
                btnTrackingList_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void onClickTrackingCreate(object sender, EventArgs e)
        {
            try
            {
                btnTrackingCreate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void DataSelectReuslt(object data)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP examServiceTemp = data as MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP;
                if (examServiceTemp != null)
                {
                    LoadDataToControlFromTemp(examServiceTemp);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP SendexamServiceTemp()
        {
            MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP examServiceTemp = new HIS_EXAM_SERVICE_TEMP();
            try
            {
                examServiceTemp.HOSPITALIZATION_REASON = txtHospitalizationReason.Text.Trim();
                examServiceTemp.PATHOLOGICAL_PROCESS = txtPathologicalProcess.Text.Trim();
                examServiceTemp.PATHOLOGICAL_HISTORY = txtPathologicalHistory.Text.Trim();
                examServiceTemp.PATHOLOGICAL_HISTORY_FAMILY = txtPathologicalHistoryFamily.Text.Trim();
                examServiceTemp.FULL_EXAM = txtKhamToanThan.Text.Trim();
                examServiceTemp.PART_EXAM = txtKhamBoPhan.Text.Trim();
                examServiceTemp.PART_EXAM_CIRCULATION = txtTuanHoan.Text.Trim();
                examServiceTemp.PART_EXAM_RESPIRATORY = txtHoHap.Text.Trim();
                examServiceTemp.PART_EXAM_DIGESTION = txtTieuHoa.Text.Trim();
                examServiceTemp.PART_EXAM_KIDNEY_UROLOGY = txtThanTietNieu.Text.Trim();
                examServiceTemp.PART_EXAM_MENTAL = txtPartExamMental.Text.Trim();
                examServiceTemp.PART_EXAM_NUTRITION = txtPartExamNutrition.Text.Trim();
                examServiceTemp.PART_EXAM_MOTION = txtPartExamMotion.Text.Trim();
                examServiceTemp.PART_EXAM_OBSTETRIC = txtPartExanObstetric.Text.Trim();
                examServiceTemp.PART_EXAM_NEUROLOGICAL = txtThanKinh.Text.Trim();
                examServiceTemp.PART_EXAM_MUSCLE_BONE = txtCoXuongKhop.Text.Trim();
                examServiceTemp.PART_EXAM_STOMATOLOGY = txtRHM.Text.Trim();
                examServiceTemp.PART_EXAM_EYE = txtMat.Text.Trim();

                examServiceTemp.PART_EXAM_OEND = txtNoiTiet.Text.Trim();
                examServiceTemp.DESCRIPTION = txtSubclinical.Text.Trim();
                examServiceTemp.CONCLUDE = txtTreatmentInstruction.Text.Trim();
                examServiceTemp.NOTE = cboNextTreatmentInstructions.Text.Trim();
                examServiceTemp.PART_EXAM_EAR = txtTai.Text.Trim();
                examServiceTemp.PART_EXAM_NOSE = txtMui.Text.Trim();
                examServiceTemp.PART_EXAM_THROAT = txtHong.Text.Trim();
                examServiceTemp.PART_EXAM_EAR_RIGHT_NORMAL = txtPART_EXAM_EAR_RIGHT_NORMAL.Text.Trim();
                examServiceTemp.PART_EXAM_EAR_RIGHT_WHISPER = txtPART_EXAM_EAR_RIGHT_WHISPER.Text.Trim();
                examServiceTemp.PART_EXAM_EAR_LEFT_NORMAL = txtPART_EXAM_EAR_LEFT_NORMAL.Text.Trim();
                examServiceTemp.PART_EXAM_EAR_LEFT_WHISPER = txtPART_EXAM_EAR_LEFT_WHISPER.Text.Trim();
                examServiceTemp.PART_EXAM_UPPER_JAW = txtPART_EXAM_UPPER_JAW.Text.Trim();
                examServiceTemp.PART_EXAM_LOWER_JAW = txtPART_EXAM_LOWER_JAW.Text.Trim();

                examServiceTemp.PART_EXAM_EYE_TENSION_LEFT = txtNhanApTrai.Text.Trim();

                examServiceTemp.PART_EXAM_EYE_TENSION_RIGHT = txtNhanApPhai.Text.Trim();
                examServiceTemp.PART_EXAM_EYESIGHT_LEFT = txtThiLucKhongKinhTrai.Text.Trim();
                examServiceTemp.PART_EXAM_EYESIGHT_RIGHT = txtThiLucKhongKinhPhai.Text.Trim();
                examServiceTemp.PART_EXAM_EYESIGHT_GLASS_LEFT = txtThiLucCoKinhTrai.Text.Trim();
                examServiceTemp.PART_EXAM_EYESIGHT_GLASS_RIGHT = txtThiLucCoKinhPhai.Text.Trim();

                if (chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked == true)
                {
                    examServiceTemp.PART_EXAM_HORIZONTAL_SIGHT = 1;
                }
                if (chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked == true)
                {
                    examServiceTemp.PART_EXAM_HORIZONTAL_SIGHT = 2;
                }


                //if (examServiceTemp.PART_EXAM_HORIZONTAL_SIGHT == 1)
                //    chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked = true;
                //else if (examServiceTemp.PART_EXAM_HORIZONTAL_SIGHT == 2)
                //    chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked = true;
                //else
                //{
                //    chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked = false;
                //    chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked = false;
                //}

                if (chkPART_EXAM_VERTICAL_SIGHT__BT.Checked == true)
                {
                    examServiceTemp.PART_EXAM_VERTICAL_SIGHT = 1;
                }
                if (chkPART_EXAM_VERTICAL_SIGHT__HC.Checked == true)
                {
                    examServiceTemp.PART_EXAM_VERTICAL_SIGHT = 2;
                }
                //if (examServiceTemp.PART_EXAM_VERTICAL_SIGHT == 1)
                //    chkPART_EXAM_VERTICAL_SIGHT__BT.Checked = true;
                //else if (examServiceTemp.PART_EXAM_VERTICAL_SIGHT == 2)
                //    chkPART_EXAM_VERTICAL_SIGHT__HC.Checked = true;
                //else
                //{
                //    chkPART_EXAM_VERTICAL_SIGHT__BT.Checked = false;
                //    chkPART_EXAM_VERTICAL_SIGHT__HC.Checked = false;
                //}

                if (chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked == true)
                {
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 1;
                }
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked == true)
                {
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 2;
                }
                // chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = (examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 1);
                // chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = (examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 2);

                if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked == true)
                {
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 3;
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 6;
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 7;
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 9;
                }
                //chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = (examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 3 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 6 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 7 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 9);


                chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = (examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 4 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 6 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 8 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 9);
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked == true)
                {
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 5;
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 7;
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 8;
                    examServiceTemp.PART_EXAM_EYE_BLIND_COLOR = 9;
                }

                // chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = (examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 5 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 7 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 8 || examServiceTemp.PART_EXAM_EYE_BLIND_COLOR == 9);

                examServiceTemp.PART_EXAM_DERMATOLOGY = txtDaLieu.Text.Trim();
                return examServiceTemp;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return examServiceTemp;
        }
        private bool ValidForButtonOtherClick()
        {
            bool valid = true;
            try
            {
                long hospitalizationReasonRequired = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.HOSPITALIZATION_REASON__REQUIRED));
                if (hospitalizationReasonRequired == 1 && String.IsNullOrEmpty(txtHospitalizationReason.Text.Trim()))
                {
                    if (MessageBox.Show("Bắt buộc nhập lý do khám", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        txtHospitalizationReason.Focus();
                        txtHospitalizationReason.SelectAll();
                    }
                    return false;
                }

                if (!ValidForSave())
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ValidDhstOption()
        {
            try
            {
                if (spinHeight.EditValue == null || spinWeight.EditValue == null)
                {
                    var configCode = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY == HisConfigCFG.DHST_REQUIRED_OPTION);
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Bắt buộc nhập thông tin Chiều cao/Cân nặng (được thiết lập theo mã cấu hình hệ thống {0})", configCode.ToList()[0].CONFIG_CODE),
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.OK);
                    if (spinHeight.EditValue == null)
                        spinHeight.Focus();
                    else if (spinWeight.EditValue == null)
                        spinWeight.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return true;
        }
        private void ValiTemperatureOption()
        {
            try
            {
                if (HisConfigCFG.IsRequiredTemperatureOption && IsThan16YearOldByTreatment() && spinTemperature.EditValue == null)
                {
                    ValidateControlSpinEdit(spinTemperature);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool ValidForSave()
        {
            bool valid = IsValidForSave = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ValidForSave 1");
                if (!String.IsNullOrEmpty(this.txtIcdCode.Text.Trim()))
                {
                    Inventec.Common.Logging.LogSystem.Debug("ValidForSave 2");
                    var listData = this.currentIcds.FirstOrDefault(o => o.ICD_CODE.Contains(this.txtIcdCode.Text.Trim()));
                    var result = listData != null ? listData : null;
                    if (result == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ValidForSave 3");
                        txtIcdCode.DoValidate();
                        MessageBox.Show("Mã ICD bạn nhập không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
                        return valid;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ValidForSave 4");
                        txtIcdCode.ErrorText = "";
                        dxValidationProviderForLeftPanel.RemoveControlError(txtIcdCode);
                    }
                }

                if (!String.IsNullOrEmpty(this.txtIcdMainText.Text.Trim()))
                {
                    long AllowToEditIcdName = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.ALLOW_TOEDIT_ICD_NAME));

                    Inventec.Common.Logging.LogSystem.Warn("AllowToEditIcdName: " + AllowToEditIcdName + " _TextIcdName: " + this._TextIcdName);
                    if (AllowToEditIcdName == 1 && !string.IsNullOrEmpty(this._TextIcdName))
                    {
                        var textICD = txtIcdMainText.Text.Trim();
                        Inventec.Common.Logging.LogSystem.Warn("AllowToEditIcdName: " + AllowToEditIcdName + " _TextIcdName: " + this._TextIcdName + " textICD: " + textICD);
                        if (textICD.StartsWith(this._TextIcdName) == false)
                        {
                            txtIcdMainText.DoValidate();
                            MessageBox.Show(ResourceMessage.CanhbaoKhongChoSuaICDName, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            valid = false;
                            return valid;
                        }
                        else
                        {
                            txtIcdMainText.ErrorText = "";
                            dxValidationProviderForLeftPanel.RemoveControlError(txtIcdMainText);
                        }
                    }
                }
                isNotCheckValidateIcdUC = false;
                if ((HisConfigCFG.CheckIcdWhenSave == "1" || HisConfigCFG.CheckIcdWhenSave == "2") && isClickSaveFinish)
                {
                    string icdCode = txtIcdCode.Text.Trim();
                    string icdSubCode = txtIcdSubCode.Text.Trim();
                    if (chkTreatmentFinish.Checked)
                    {
                        var data = this.treatmentFinishProcessor.GetIcd(this.ucTreatmentFinish) as ExamTreatmentFinishResult;
                        if (icdCode.Equals(data.icdADOInTreatment.ICD_CODE))
                        {
                            isNotCheckValidateIcdUC = true;
                            if (data.TreatmentFinishSDO != null)
                            {
                                if (!string.IsNullOrEmpty(icdSubCode))
                                {
                                    var lstISC = icdSubCode.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                                    var lstISCuc = data.TreatmentFinishSDO.IcdSubCode.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                                    lstISC.AddRange(lstISCuc);
                                    icdSubCode = String.Join(";", lstISCuc.Distinct().ToList());
                                }
                                else
                                {
                                    icdSubCode = data.TreatmentFinishSDO.IcdSubCode;
                                }
                            }
                        }
                    }
                    else if (chkHospitalize.Checked)
                    {
                        var data = this.hospitalizeProcessor.GetIcd(this.ucHospitalize) as HospitalizeExamADO;
                        if (icdCode.Equals(data.icdADOInTreatment.ICD_CODE))
                        {
                            isNotCheckValidateIcdUC = true;
                        }
                    }
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(icdCode, icdSubCode, ref messErr, HisConfigCFG.CheckIcdWhenSave == "1" || HisConfigCFG.CheckIcdWhenSave == "2"))
                    {
                        if (HisConfigCFG.CheckIcdWhenSave == "1")
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(messErr + ". Bạn có muốn tiếp tục?",
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.YesNo) == DialogResult.No) valid = false;
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(messErr,
                         HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                         MessageBoxButtons.OK);
                            valid = false;
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("ValidForSave 5");
                this.dxValidationProviderForLeftPanel.RemoveControlError(txtIcdCode);
                this.positionHandleControlLeft = -1;
                valid = IsValidForSave = dxValidationProviderForLeftPanel.Validate() && valid;
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        /// <summary>
        /// Kiem tra no vien phi
        /// </summary>
        private bool CheckWarningOverTotalPatientPrice()
        {
            bool result = true;
            try
            {
                if (chkTreatmentFinish.Checked)
                {
                    if (this.treatment != null)
                    {
                        var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == this.treatment.TDL_TREATMENT_TYPE_ID);
                        if (treatmentType != null && (treatmentType.FEE_DEBT_OPTION == 1
                                                   || treatmentType.FEE_DEBT_OPTION == 2))
                        {
                            List<V_HIS_TREATMENT_FEE> treatmentFees = LoadTreatmentFee();
                            //So tien benh nhan can thu
                            if (treatmentFees == null || treatmentFees.Count == 0)
                                return result = false;

                            decimal totalPrice = 0;
                            decimal totalHeinPrice = 0;
                            decimal totalPatientPrice = 0;
                            decimal totalDeposit = 0;
                            decimal totalDebt = 0;
                            decimal totalBill = 0;
                            decimal totalBillTransferAmount = 0;
                            decimal totalRepay = 0;
                            decimal total_obtained_price = 0;
                            totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                            totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                            totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                            totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                            totalDebt = treatmentFees[0].TOTAL_DEBT_AMOUNT ?? 0;
                            totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                            totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                            totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                            total_obtained_price = (totalDeposit + totalDebt + totalBill - totalBillTransferAmount - totalRepay);//Da thu benh nhan
                            decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan

                            if (transfer > 0)
                            {
                                if (treatmentType.FEE_DEBT_OPTION == 1)
                                {
                                    DialogResult myResult;
                                    myResult = DevExpress.XtraEditors.XtraMessageBox.Show(this, String.Format("Bệnh nhân đang thiếu viện phí {0} đồng. Bạn có muốn tiếp tục?", Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator)), ResourceMessage.ThongBao, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                    if (myResult != DialogResult.OK)
                                    {
                                        result = false;
                                    }
                                }
                                else
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show(this, String.Format("Bệnh nhân đang thiếu viện phí {0} đồng.", Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator)), ResourceMessage.ThongBao, MessageBoxButtons.OK);
                                    result = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = false;
            }
            return result;
        }
        bool CheckExamFinish(HisServiceReqExamUpdateSDO hisServiceReqSDO)
        {
            bool result = true;
            try
            {
                string error = "";
                result = ValidateFinishTime(hisServiceReqSDO, ref error);
                if (result == false)
                {
                    MessageBox.Show(error, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ValidateFinishTime(HisServiceReqExamUpdateSDO hisServiceReqSDO, ref string error)
        {
            bool result = true;
            try
            {
                if (hisServiceReqSDO != null && hisServiceReqSDO.FinishTime.HasValue && this.treatment != null && hisServiceReqSDO.TreatmentFinishSDO == null)
                {
                    if (hisServiceReqSDO.FinishTime < this.treatment.IN_TIME)
                    {
                        result = false;
                        error += ResourceMessage.ThoiGianKetThucKhamPhaiLonHonThoiGianVaoVien + "\r\n";
                    }

                    if (hisServiceReqSDO.TreatmentFinishSDO == null)
                    {
                        if (this.treatment.OUT_TIME.HasValue && hisServiceReqSDO.FinishTime > this.treatment.OUT_TIME)
                        {
                            result = false;
                            error += ResourceMessage.ThoiGianKetThucKhamPhaiNhoHonThoiGianKetThucDieuTri + "\r\n";
                        }

                    }
                    if (this.HisServiceReqView.START_TIME.HasValue && hisServiceReqSDO.FinishTime < this.HisServiceReqView.START_TIME)
                    {
                        result = false;
                        error += ResourceMessage.ThoiGianKetThucKhamPhaiLonHonThoiGianBatDau + "\r\n";
                    }
                    if (this.HisServiceReqView.INTRUCTION_TIME > hisServiceReqSDO.FinishTime)
                    {
                        result = false;
                        error += ResourceMessage.ThoiGianKetThucKhamPhaiLonHonThoiGianYLenh + "\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool CheckHospitalizeTime(HisServiceReqExamUpdateSDO hisServiceReqSDO)
        {
            bool result = true;
            try
            {
                if (hisServiceReqSDO.HospitalizeSDO != null && treatment != null)
                {
                    string error = "";
                    var validateFinisTime = ValidateFinishTime(hisServiceReqSDO, ref error);
                    if (hisServiceReqSDO.HospitalizeSDO.Time > 0 && hisServiceReqSDO.HospitalizeSDO.Time < treatment.IN_TIME)
                    {
                        error += ResourceMessage.ThoiGianNhapVienNhoHonThoiGianVaoVien;

                        result = false;
                    }
                    if (result == false)
                    {
                        MessageBox.Show(error, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool CheckSunSatAppointmentTime(HisServiceReqExamUpdateSDO hisServiceReqSDO)
        {
            bool result = true;
            try
            {
                if (hisServiceReqSDO.TreatmentFinishSDO != null && hisServiceReqSDO.TreatmentFinishSDO.AppointmentTime.HasValue)
                {
                    DateTime dtAppointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisServiceReqSDO.TreatmentFinishSDO.AppointmentTime.Value) ?? DateTime.Now;
                    if (dtAppointmentTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaChuNhat,
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return false;
                    }
                    else if (dtAppointmentTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaThuBay,
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return false;
                    }

                    string error = "";
                    var validateFinisTime = ValidateFinishTime(hisServiceReqSDO, ref error);
                    if (validateFinisTime == false)
                    {
                        MessageBox.Show(error, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                if (hisServiceReqSDO.TreatmentFinishSDO != null && hisServiceReqSDO.TreatmentFinishSDO.AppointmentExamRoomIds != null && hisServiceReqSDO.TreatmentFinishSDO.AppointmentExamRoomIds.Count > 0)
                {
                    var listRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => hisServiceReqSDO.TreatmentFinishSDO.AppointmentExamRoomIds.Contains(o.ROOM_ID)).ToList();
                    var roomConfig = listRoom.Where(o => o.MAX_APPOINTMENT_BY_DAY.HasValue && o.MAX_APPOINTMENT_BY_DAY.Value > 0).ToList();
                    if (roomConfig != null && roomConfig.Count > 0)
                    {
                        HisExecuteRoomAppointedFilter exrFilter = new HisExecuteRoomAppointedFilter();
                        exrFilter.EXECUTE_ROOM_IDs = roomConfig.Select(s => s.ID).ToList();
                        if (hisServiceReqSDO.TreatmentFinishSDO.AppointmentTime.HasValue)
                        {
                            exrFilter.INTR_OR_APPOINT_DATE = long.Parse(hisServiceReqSDO.TreatmentFinishSDO.AppointmentTime.Value.ToString().Substring(0, 8) + "000000");
                        }
                        else
                        {
                            exrFilter.INTR_OR_APPOINT_DATE = HisServiceReqView.INTRUCTION_DATE;
                        }

                        var rsApi = new BackendAdapter(param).Get<List<HisExecuteRoomAppointedSDO>>("api/HisExecuteRoom/GetCountAppointed", ApiConsumers.MosConsumer, exrFilter, param);
                        if (rsApi != null && rsApi.Count > 0)
                        {
                            List<string> lstMess = new List<string>();
                            foreach (var item in rsApi)
                            {
                                var room = roomConfig.FirstOrDefault(o => o.ID == item.ExecuteRoomId);
                                if (room != null && (room.MAX_APPOINTMENT_BY_DAY ?? 0) <= (item.CurrentAmount ?? 0))
                                {
                                    lstMess.Add(string.Format("{0} ({1}/{2})", item.ExecuteRoomName, item.CurrentAmount ?? 0, item.MaxAmount ?? 0));
                                }
                            }

                            if (lstMess.Count > 0)
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Phòng khám có số lượt hẹn khám vượt số lượt cho phép: {0} Bạn có muốn tiếp tục?", string.Join(", ", lstMess)),
                                ResourceMessage.ThongBao,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool CheckTreatmentFinish(HisServiceReqExamUpdateSDO hisServiceReqSDO)
        {
            bool result = true;
            try
            {
                if (hisServiceReqSDO != null && hisServiceReqSDO.TreatmentFinishSDO != null)
                {
                    if (checkSameHeinCFG == 1)
                    {
                        bool checkSameHein = false;
                        CommonParam param = new CommonParam();

                        HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                        patientTypeAlterFilter.TREATMENT_ID = this.HisServiceReqView.TREATMENT_ID;

                        var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                        if (patientTypeAlter != null && patientTypeAlter.Count >= 2)
                        {
                            foreach (var item in patientTypeAlter)
                            {
                                var sameHein = patientTypeAlter.Where(o => o.HEIN_CARD_NUMBER == item.HEIN_CARD_NUMBER).ToList();
                                if (sameHein != null && sameHein.Count >= 2)
                                {
                                    var checkHeinOrg = sameHein.Select(o => o.HEIN_MEDI_ORG_CODE).Distinct().ToList();
                                    if (checkHeinOrg.Count > 1)
                                    {
                                        //Mã cskcb khác nhau
                                        checkSameHein = true;
                                        break;
                                    }
                                    else
                                    {
                                        var checkRightRoute = sameHein.Select(o => o.RIGHT_ROUTE_CODE).Distinct().ToList();
                                        if (checkRightRoute.Count == 1)
                                        {
                                            //Đúng tuyến và lý do đúng tuyến khác nhau
                                            if (checkRightRoute.FirstOrDefault() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                                            {
                                                var checkRightRouteType = sameHein.Select(o => o.RIGHT_ROUTE_TYPE_CODE).Distinct().ToList();
                                                if (checkRightRouteType.Count > 1)
                                                {
                                                    checkSameHein = true;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Trái tuyến
                                            checkSameHein = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (checkSameHein)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoSaiThongTinTheBHYT, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            result = false;
                        }
                    }
                    //Issue 58878: Bổ sung xử lý trong trường hợp kết thúc điều trị có check chọn tạo bệnh án ngoại trú và chọn chương trình
                    if (hisServiceReqSDO.TreatmentFinishSDO.CreateOutPatientMediRecord == true
                        && hisServiceReqSDO.TreatmentFinishSDO.ProgramId > 0)
                    {
                        var program = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PROGRAM>().Where(o => o.ID == hisServiceReqSDO.TreatmentFinishSDO.ProgramId).FirstOrDefault();
                        if (program != null && program.AUTO_CHANGE_TO_OUT_PATIENT == 1
                            && this.treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.ChuongTrinhBatBuocChonDienDieuTriNgoaiTruHoSoSeDuocTuDongCapNhatSangDienDieuTriNgoaiTru, program.PROGRAM_NAME), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool VerifyTreatmentFinish()
        {
            bool result = true;
            try
            {
                if (chkTreatmentFinish.Checked && this.ucTreatmentFinish != null)
                {
                    result = this.treatmentFinishProcessor.Validate(this.ucTreatmentFinish, isNotCheckValidateIcdUC);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        //bool IsValiICDSub()
        //{
        //    bool result = true;
        //    try
        //    {
        //        result = (bool)subIcdProcessor.GetValidate(ucSecondaryIcd);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        //bool IsValiICDCause()
        //{
        //    bool result = true;
        //    try
        //    {
        //        result = (bool)this.IcdCauseProcessor.ValidationIcd(this.ucIcdCause);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        //bool IsValiNextTreatmentIntruction()
        //{
        //    bool result = true;
        //    try
        //    {
        //        result = (bool)this.nextTreatmentIntructionProcessor.ValidationNextTreatmentInstruction(this.ucNextTreatmentIntruction);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        //bool IsValidateDHST()
        //{
        //    bool result = true;
        //    try
        //    {
        //        DHSTADO dhstADO = UcDHSTGetValue() as DHSTADO;
        //        if (dhstADO != null && dhstADO.IsVali == false)
        //        {
        //            result = false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        bool CheckExecuteExt(HisServiceReqExamUpdateSDO hisServiceReqSDO)
        {
            bool result = true;
            try
            {
                if (hisServiceReqSDO != null)
                {
                    if (chkHospitalize.Checked && hisServiceReqSDO.HospitalizeSDO == null)
                        return false;
                    if (chkExamServiceAdd.Checked && hisServiceReqSDO.ExamAdditionSDO == null)
                        return false;
                    if (chkTreatmentFinish.Checked && hisServiceReqSDO.TreatmentFinishSDO == null)
                        return false;

                    var treatment = this.treatment;

                    //Không check với bệnh nhân VP và BN điều trị nội trú và ĐT ngoại trú
                    if (hisServiceReqSDO.ExamAdditionSDO != null
                        && treatment != null
                        && treatment.TDL_HEIN_CARD_NUMBER != null
                        && treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                        && treatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    {
                        V_HIS_EXECUTE_ROOM executeRoom = new V_HIS_EXECUTE_ROOM();
                        if (HisConfigs.Get<string>(SdaConfigKeys.WARNING_OVER_EXAM_BHYT) == "1")
                        {
                            if (CheckOverExamBhyt(hisServiceReqSDO.ExamAdditionSDO, ref executeRoom) && executeRoom.MAX_REQ_BHYT_BY_DAY.HasValue)
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(executeRoom.EXECUTE_ROOM_NAME + " đã vượt quá " + executeRoom.MAX_REQ_BHYT_BY_DAY + " lượt khám BHYT trong ngày. Bạn có muốn thực hiện không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    result = true;
                                else
                                    result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool CheckOverExamBhyt(HisServiceReqExamAdditionSDO input, ref V_HIS_EXECUTE_ROOM data)
        {
            bool rs = false;
            try
            {
                if (input.AdditionRoomId > 0)
                {
                    var executeRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == input.AdditionRoomId);
                    if (executeRoom != null)
                    {
                        data = executeRoom;
                        CommonParam param = new CommonParam();
                        HisSereServBhytOutpatientExamFilter filter = new HisSereServBhytOutpatientExamFilter();
                        long now = Inventec.Common.DateTime.Get.Now() ?? 0;
                        if (now > 0 && now.ToString().Length > 8)
                            filter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64((now.ToString().Substring(0, 8) + "000000"));
                        filter.ROOM_IDs = new List<long>();
                        filter.ROOM_IDs.Add(executeRoom.ROOM_ID);

                        var rsApi = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetSereServBhytOutpatientExam", ApiConsumers.MosConsumer, filter, param);
                        if (rsApi != null && rsApi.Count >= (executeRoom.MAX_REQ_BHYT_BY_DAY ?? 0))
                        {
                            rs = true;
                        }
                    }
                    else
                    {
                        data = new V_HIS_EXECUTE_ROOM();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return rs;
        }

        bool CheckFinishTime(HisServiceReqExamUpdateSDO data)
        {
            bool rs = false;
            try
            {
                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.TREATMENT_ID = data.TreatmentFinishSDO.TreatmentId;
                ssFilter.HAS_EXECUTE = true;
                List<HIS_SERE_SERV> hisSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                if (hisSereServs != null || hisSereServs.Count > 0)
                {
                    var listData = hisSereServs.Where(o => o.AMOUNT > 0 && o.TDL_INTRUCTION_TIME > data.TreatmentFinishSDO.TreatmentFinishTime).ToList();
                    if (listData != null && listData.Count > 0)
                    {
                        var listCode = listData.Select(s => s.TDL_SERVICE_REQ_CODE).Distinct().ToList();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Thời gian kết thúc điều trị nhỏ hơn thời gian y lệnh của các mã yêu cầu sau: {0}", String.Join(",", listCode)), ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        rs = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return rs;
        }

        void TreatmentFinishSuccess()
        {
            try
            {
                CommonParam param = new CommonParam();
                var finishExam = new BackendAdapter(param)
                    .Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_FINISH, ApiConsumers.MosConsumer, HisServiceReqView.ID, param);
                if (finishExam != null)
                {
                    HIS.Desktop.ModuleExt.TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void WhileAutoTreatmentEnd()
        {
            try
            {
                //Close tab
                HIS.Desktop.ModuleExt.TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessExamServiceReqExecute(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ HisServiceReqWithOrderSDO, HisServiceReqExamUpdateSDO examServiceReqUpdateSDO)
        {
            try
            {
                bool success = true;
                if (chkHospitalize.Checked == true)
                {

                    var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.moduleData.RoomId).DepartmentId;
                    MOS.Filter.HisServiceReqFilter _reqFilter = new HisServiceReqFilter();
                    _reqFilter.TREATMENT_ID = this.treatmentId;
                    _reqFilter.REQUEST_DEPARTMENT_ID = DepartmentID;
                    var dataReqs = new BackendAdapter(null).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, _reqFilter, null);
                    List<HIS_SERVICE_REQ> hisser_ = new List<HIS_SERVICE_REQ>();

                    //dataReqs.Select(o => o.SERVICE_REQ_CODE);
                    if (dataReqs != null && dataReqs.Count > 0)
                    {

                        foreach (var item in dataReqs.Select(o => o.INTRUCTION_TIME))
                        {
                            //long dateString = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(item.ToString()).ToString("yyyy/MM/dd HH:mm:ss"));

                            DateTime t1 = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item) ?? DateTime.Now;
                            DateTime t2 = DateTime.Now;

                            if (t1 > t2)
                            {
                                var data = dataReqs.Where(o => o.INTRUCTION_TIME == item);
                                hisser_.AddRange(data);
                            }
                        }
                    }
                    if (hisser_ != null && hisser_.Count > 0)
                    {
                        success = false;
                        string TB1 = string.Join(",", hisser_.Select(o => o.SERVICE_REQ_CODE).ToList().Distinct()).ToString();
                        string Khoa1 = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == DepartmentID).DEPARTMENT_NAME;

                        HospitalizeExamADO hisDepartmentTranHospitalizeSDO = this.hospitalizeProcessor.GetValue(this.ucHospitalize) as HospitalizeExamADO;
                        if (hisDepartmentTranHospitalizeSDO != null)
                        {
                            string Khoa2 = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.DepartmentId).DEPARTMENT_NAME;
                            string TB2 = "Các y lệnh " + TB1 + " " + Khoa1 + " có thời gian y lệnh lớn hơn thời gian hiện tại nhập viện của " + Khoa2 + ". Bạn có muốn tiếp tục?";
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(TB2, "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                success = true;
                            }
                            else
                            {
                                success = false;
                            }

                        }
                    }
                }
                if (success)
                {
                    if (HisServiceReqWithOrderSDO != null && examServiceReqUpdateSDO != null)
                    {
                        ProcessExamServiceReqDTO(ref examServiceReqUpdateSDO);
                        ProcessExamAddition(ref examServiceReqUpdateSDO, HisServiceReqWithOrderSDO);
                        ProcessHospitalize(ref examServiceReqUpdateSDO);
                        ProcessTreatmentFinish(ref examServiceReqUpdateSDO);
                        ProcessExamFinish(ref examServiceReqUpdateSDO);
                        ProcessExamSereIcdDTO(ref examServiceReqUpdateSDO);
                        ProcessExamSereNextTreatmentIntructionDTO(ref examServiceReqUpdateSDO);
                        ProcessExamSereDHST(ref examServiceReqUpdateSDO);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessExamServiceReqDTO(ref HisServiceReqExamUpdateSDO examServiceReqUpdateSDO)
        {
            try
            {
                examServiceReqUpdateSDO.Id = this.HisServiceReqView.ID;
                examServiceReqUpdateSDO.Advise = this.HisServiceReqView.ADVISE;
                examServiceReqUpdateSDO.Conclusion = this.HisServiceReqView.CONCLUSION;
                examServiceReqUpdateSDO.SickDay = Inventec.Common.TypeConvert.Parse.ToInt64(spinNgayThuCuaBenh.Text ?? "0");
                examServiceReqUpdateSDO.HospitalizationReason = !string.IsNullOrEmpty(txtHospitalizationReason.Text.Trim()) ? txtHospitalizationReason.Text.Trim() : null;
                examServiceReqUpdateSDO.FullExam = !string.IsNullOrEmpty(txtKhamToanThan.Text.Trim()) ? txtKhamToanThan.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExam = !string.IsNullOrEmpty(txtKhamBoPhan.Text.Trim()) ? txtKhamBoPhan.Text.Trim() : null;
                examServiceReqUpdateSDO.ProvisionalDiagnosis = !string.IsNullOrEmpty(txtProvisionalDianosis.Text.Trim()) ? txtProvisionalDianosis.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamCirculation = !string.IsNullOrEmpty(txtTuanHoan.Text.Trim()) ? txtTuanHoan.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamDigestion = !string.IsNullOrEmpty(txtTieuHoa.Text.Trim()) ? txtTieuHoa.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEar = !string.IsNullOrEmpty(txtTai.Text.Trim()) ? txtTai.Text.Trim() : null;//TODO
                                                                                                                            //TODO
                examServiceReqUpdateSDO.PartExamEarRightNormal = !string.IsNullOrEmpty(txtPART_EXAM_EAR_RIGHT_NORMAL.Text.Trim()) ? txtPART_EXAM_EAR_RIGHT_NORMAL.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEarRightWhisper = !string.IsNullOrEmpty(txtPART_EXAM_EAR_RIGHT_WHISPER.Text.Trim()) ? txtPART_EXAM_EAR_RIGHT_WHISPER.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEarLeftNormal = !string.IsNullOrEmpty(txtPART_EXAM_EAR_LEFT_NORMAL.Text.Trim()) ? txtPART_EXAM_EAR_LEFT_NORMAL.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEarLeftWhisper = !string.IsNullOrEmpty(txtPART_EXAM_EAR_LEFT_WHISPER.Text.Trim()) ? txtPART_EXAM_EAR_LEFT_WHISPER.Text.Trim() : null;

                if (chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked)
                    examServiceReqUpdateSDO.PartExamHorizontalSight = 1;
                else if (chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked)
                    examServiceReqUpdateSDO.PartExamHorizontalSight = 2;
                else
                {
                    examServiceReqUpdateSDO.PartExamHorizontalSight = null;
                }

                if (chkPART_EXAM_VERTICAL_SIGHT__BT.Checked)
                    examServiceReqUpdateSDO.PartExamVerticalSight = 1;
                else if (chkPART_EXAM_VERTICAL_SIGHT__HC.Checked)
                    examServiceReqUpdateSDO.PartExamVerticalSight = 2;
                else
                {
                    examServiceReqUpdateSDO.PartExamVerticalSight = null;
                }


                //- Sắc giác: PART_EXAM_EYE_BLIND_COLOR lưu vào HIS_SERVICE_REQ: 1-bình thường, 2- mù màu tòan bộ , 3- mù màu đỏ, 4- mù màu xanh lá, 5- mù màu vàng, 6- mù màu đỏ +xanh lá, 7 - mù màu đỏ + vàng, 8- mù màu xanh lá + vàng, 9- mù màu đỏ + xanh lá + vàng
                //* nếu tích bình thường --->không được tính mù màu + mù màu 1 phần.
                //* nếu tích mù toàn bộ--->cũng không được tính mù 1 phần + bình thường
                //* nếu k tích 2 TH bình thường và mù màu toàn bộ - >thì 3 cái mù ở dưới cho phép chọn nhiều
                if (chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked)
                    examServiceReqUpdateSDO.PartExamEyeBlindColor = 1;
                else if (chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked)
                    examServiceReqUpdateSDO.PartExamEyeBlindColor = 2;
                else
                {
                    if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        examServiceReqUpdateSDO.PartExamEyeBlindColor = 9;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        examServiceReqUpdateSDO.PartExamEyeBlindColor = 8;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        examServiceReqUpdateSDO.PartExamEyeBlindColor = 7;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                        examServiceReqUpdateSDO.PartExamEyeBlindColor = 6;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        examServiceReqUpdateSDO.PartExamEyeBlindColor = 5;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                        examServiceReqUpdateSDO.PartExamEyeBlindColor = 4;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked)
                        examServiceReqUpdateSDO.PartExamEyeBlindColor = 3;
                    else
                        examServiceReqUpdateSDO.PartExamEyeBlindColor = null;
                }

                examServiceReqUpdateSDO.PartExamEye = !string.IsNullOrEmpty(txtMat.Text.Trim()) ? txtMat.Text.Trim() : null;

                examServiceReqUpdateSDO.PartExamUpperJaw = !string.IsNullOrEmpty(txtPART_EXAM_UPPER_JAW.Text.Trim()) ? txtPART_EXAM_UPPER_JAW.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamLowerJaw = !string.IsNullOrEmpty(txtPART_EXAM_LOWER_JAW.Text.Trim()) ? txtPART_EXAM_LOWER_JAW.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamStomatology = !string.IsNullOrEmpty(txtRHM.Text.Trim()) ? txtRHM.Text.Trim() : null;

                examServiceReqUpdateSDO.PartExamNose = !string.IsNullOrEmpty(txtMui.Text.Trim()) ? txtMui.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamThroat = !string.IsNullOrEmpty(txtHong.Text.Trim()) ? txtHong.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEyeTensionRight = !string.IsNullOrEmpty(txtNhanApPhai.Text.Trim()) ? txtNhanApPhai.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEyeTensionLeft = !string.IsNullOrEmpty(txtNhanApTrai.Text.Trim()) ? txtNhanApTrai.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEyeSightRight = !string.IsNullOrEmpty(txtThiLucKhongKinhPhai.Text.Trim()) ? txtThiLucKhongKinhPhai.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEyeSightLeft = !string.IsNullOrEmpty(txtThiLucKhongKinhTrai.Text.Trim()) ? txtThiLucKhongKinhTrai.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEyeSightGlassRight = !string.IsNullOrEmpty(txtThiLucCoKinhPhai.Text.Trim()) ? txtThiLucCoKinhPhai.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamEyeSightGlassLeft = !string.IsNullOrEmpty(txtThiLucCoKinhTrai.Text.Trim()) ? txtThiLucCoKinhTrai.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamKidneyUrology = !string.IsNullOrEmpty(txtThanTietNieu.Text.Trim()) ? txtThanTietNieu.Text.Trim() : null;

                examServiceReqUpdateSDO.PartExamDermatology = !string.IsNullOrEmpty(txtDaLieu.Text.Trim()) ? txtDaLieu.Text.Trim() : null;

                examServiceReqUpdateSDO.PartExamMental = !string.IsNullOrEmpty(txtPartExamMental.Text.Trim()) ? txtPartExamMental.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamNutrition = !string.IsNullOrEmpty(txtPartExamNutrition.Text.Trim()) ? txtPartExamNutrition.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamMotion = !string.IsNullOrEmpty(txtPartExamMotion.Text.Trim()) ? txtPartExamMotion.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamObstetric = !string.IsNullOrEmpty(txtPartExanObstetric.Text.Trim()) ? txtPartExanObstetric.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamMuscleBone = !string.IsNullOrEmpty(txtCoXuongKhop.Text.Trim()) ? txtCoXuongKhop.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamNeurological = !string.IsNullOrEmpty(txtThanKinh.Text.Trim()) ? txtThanKinh.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamOend = !string.IsNullOrEmpty(txtNoiTiet.Text.Trim()) ? txtNoiTiet.Text.Trim() : null;
                examServiceReqUpdateSDO.PartExamRespiratory = !string.IsNullOrEmpty(txtHoHap.Text.Trim()) ? txtHoHap.Text.Trim() : null;
                examServiceReqUpdateSDO.PathologicalHistory = !string.IsNullOrEmpty(txtPathologicalHistory.Text.Trim()) ? txtPathologicalHistory.Text.Trim() : null; // tien su cua BN
                examServiceReqUpdateSDO.PathologicalHistoryFamily = !string.IsNullOrEmpty(txtPathologicalHistoryFamily.Text.Trim()) ? txtPathologicalHistoryFamily.Text.Trim() : null; // tien su gia dinh
                examServiceReqUpdateSDO.PathologicalProcess = !string.IsNullOrEmpty(txtPathologicalProcess.Text.Trim()) ? txtPathologicalProcess.Text.Trim() : null; // kham toan than
                examServiceReqUpdateSDO.Note = !string.IsNullOrEmpty(txtResultNote.Text.Trim()) ? txtResultNote.Text.Trim() : null;
                examServiceReqUpdateSDO.Subclinical = !string.IsNullOrEmpty(txtSubclinical.Text.Trim()) ? txtSubclinical.Text.Trim() : null;
                examServiceReqUpdateSDO.TreatmentInstruction = !string.IsNullOrEmpty(txtTreatmentInstruction.Text.Trim()) ? txtTreatmentInstruction.Text.Trim() : null;
                if (cboKskCode.EditValue != null)
                    examServiceReqUpdateSDO.HealthExamRankId = Inventec.Common.TypeConvert.Parse.ToInt64(cboKskCode.EditValue.ToString());
                else
                    examServiceReqUpdateSDO.HealthExamRankId = null;

                if (cboPatientCase.EditValue != null)
                    examServiceReqUpdateSDO.PatientCaseId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientCase.EditValue.ToString());
                else
                    examServiceReqUpdateSDO.PatientCaseId = null;
                var lstContraindications = contraindicationSelecteds.Select(o => o.ID).ToList();
                if (lstContraindications != null && lstContraindications.Count > 0)
                {
                    examServiceReqUpdateSDO.ContraindicationIds = lstContraindications;
                }
                examServiceReqUpdateSDO.NotePatient = CurrentPatient.NOTE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessExamAddition(ref HisServiceReqExamUpdateSDO serviceReqUpdateSDO, MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ HisServiceReqWithOrderSDO)
        {
            try
            {
                if (chkExamServiceAdd.Checked && this.ucExamAddition != null)
                {
                    ExamServiceAddADO hisServiceReqExamAdditionSDO = this.examServiceAddProcessor.GetValueV2(this.ucExamAddition) as ExamServiceAddADO;
                    if (hisServiceReqExamAdditionSDO != null)
                    {
                        List<long> serviceIds = new List<long>();
                        if (hisServiceReqExamAdditionSDO.AdditionServiceId.HasValue)
                        {
                            serviceIds.Add(hisServiceReqExamAdditionSDO.AdditionServiceId.Value);
                        }

                        List<HIS_SERE_SERV> sereServWithMinDurations = this.GetSereServWithMinDuration(treatment.PATIENT_ID, serviceIds);
                        if (sereServWithMinDurations != null && sereServWithMinDurations.Count > 0)
                        {
                            string sereServMinDurationStr = "";
                            foreach (var item in sereServWithMinDurations)
                            {
                                sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                            }

                            if (MessageBox.Show(string.Format("Các dịch vụ sau có thời gian chỉ định nằm trong khoảng thời gian không cho phép: {0} .Bạn có muốn tiếp tục?", sereServMinDurationStr), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                serviceReqUpdateSDO.ExamAdditionSDO = null;
                                return;
                            }
                        }

                        serviceReqUpdateSDO.ExamAdditionSDO = new HisServiceReqExamAdditionSDO();
                        serviceReqUpdateSDO.ExamAdditionSDO.AdditionRoomId = hisServiceReqExamAdditionSDO.AdditionRoomId;
                        serviceReqUpdateSDO.ExamAdditionSDO.AdditionServiceId = hisServiceReqExamAdditionSDO.AdditionServiceId;
                        serviceReqUpdateSDO.ExamAdditionSDO.CurrentSereServId = hisServiceReqExamAdditionSDO.CurrentSereServId;
                        serviceReqUpdateSDO.ExamAdditionSDO.IsChangeDepartment = hisServiceReqExamAdditionSDO.IsChangeDepartment;
                        serviceReqUpdateSDO.ExamAdditionSDO.IsPrimary = hisServiceReqExamAdditionSDO.IsPrimary;
                        serviceReqUpdateSDO.ExamAdditionSDO.IsFinishCurrent = hisServiceReqExamAdditionSDO.IsFinishCurrent;
                        serviceReqUpdateSDO.ExamAdditionSDO.RequestRoomId = hisServiceReqExamAdditionSDO.RequestRoomId;
                        serviceReqUpdateSDO.ExamAdditionSDO.InstructionTime = hisServiceReqExamAdditionSDO.InstructionTime;
                        serviceReqUpdateSDO.ExamAdditionSDO.PatientTypeId = hisServiceReqExamAdditionSDO.PatientTypeId;
                        serviceReqUpdateSDO.ExamAdditionSDO.PrimaryPatientTypeId = hisServiceReqExamAdditionSDO.PrimaryPatientTypeId;
                        serviceReqUpdateSDO.ExamAdditionSDO.IsNotRequireFee = hisServiceReqExamAdditionSDO.IsNotRequireFee;
                        serviceReqUpdateSDO.AppointmentExamRoomId = hisServiceReqExamAdditionSDO.RoomApointmentId;
                        serviceReqUpdateSDO.AppointmentExamServiceId = hisServiceReqExamAdditionSDO.ServiceApointmentId;
                        serviceReqUpdateSDO.NotePatient = hisServiceReqExamAdditionSDO.Note;
                        //serviceReqUpdateSDO.ExamAdditionSDO.xxx = hisServiceReqExamAdditionSDO.NumOrderBlockId;
                        serviceReqUpdateSDO.FinishTime = hisServiceReqExamAdditionSDO.FinishTime;

                        serviceReqUpdateSDO.ExamAdditionSDO.IsNotUseBhyt = hisServiceReqExamAdditionSDO.IsNotUseBhyt;

                        long finishExamAdd = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.IS_FINISH_EXAM_ADD));
                        if (finishExamAdd == 1)
                        {
                            serviceReqUpdateSDO.IsFinish = true;
                        }

                        //Neu kham hien tai la kham chinh, neu kham them thi tu dong ket thuc kham hien tai
                        if (this.HisServiceReqView != null && (this.HisServiceReqView.IS_MAIN_EXAM ?? 0) == 1 && hisServiceReqExamAdditionSDO.IsPrimary)
                        {
                            serviceReqUpdateSDO.IsFinish = true;
                        }

                        isPrintExamServiceAdd = hisServiceReqExamAdditionSDO.IsPrintExamAdd;
                        isSignExamServiceAdd = hisServiceReqExamAdditionSDO.IsSignExamAdd;
                        serviceReqUpdateSDO.AppointmentTime = hisServiceReqExamAdditionSDO.AppointmentTime;
                        serviceReqUpdateSDO.AppointmentDesc = hisServiceReqExamAdditionSDO.Advise;
                        //serviceReqUpdateSDO.APPOINTMENT_CODE = treatment.TREATMENT_CODE;

                        IsAppointment_ExamServiceAdd = hisServiceReqExamAdditionSDO.IsAppointment;
                        IsPrintAppointment_ExamServiceAdd = hisServiceReqExamAdditionSDO.IsPrintAppointment;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessHospitalize(ref HisServiceReqExamUpdateSDO serviceReqUpdateSDO)
        {
            try
            {
                if (chkHospitalize.Checked && this.ucHospitalize != null)
                {
                    HospitalizeExamADO hisDepartmentTranHospitalizeSDO = this.hospitalizeProcessor.GetValue(this.ucHospitalize) as HospitalizeExamADO;
                    if (hisDepartmentTranHospitalizeSDO != null)
                    {
                        serviceReqUpdateSDO.HospitalizeSDO = new HisDepartmentTranHospitalizeSDO();
                        if (hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO != null)
                        {
                            serviceReqUpdateSDO.HospitalizeSDO.BedRoomId = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.BedRoomId;
                            serviceReqUpdateSDO.HospitalizeSDO.DepartmentId = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.DepartmentId;
                            serviceReqUpdateSDO.HospitalizeSDO.RequestRoomId = moduleData.RoomId;
                            serviceReqUpdateSDO.HospitalizeSDO.TreatmentId = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.TreatmentId;
                            serviceReqUpdateSDO.HospitalizeSDO.TreatmentTypeId = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.TreatmentTypeId;
                            serviceReqUpdateSDO.HospitalizeSDO.Time = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.Time;
                            serviceReqUpdateSDO.HospitalizeSDO.IsEmergency = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.IsEmergency;
                            serviceReqUpdateSDO.FinishTime = hisDepartmentTranHospitalizeSDO.FinishTime;
                            serviceReqUpdateSDO.HospitalizeSDO.RelativeName = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.RelativeName;
                            serviceReqUpdateSDO.HospitalizeSDO.RelativePhone = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.RelativePhone;
                            serviceReqUpdateSDO.HospitalizeSDO.RelativeAddress = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.RelativeAddress;
                            serviceReqUpdateSDO.HospitalizeSDO.CareerId = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.CareerId;
                            serviceReqUpdateSDO.HospitalizeSDO.InHospitalizationReasonCode = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.InHospitalizationReasonCode;
                            serviceReqUpdateSDO.HospitalizeSDO.InHospitalizationReasonName = hisDepartmentTranHospitalizeSDO.HisDepartmentTranHospitalizeSDO.InHospitalizationReasonName;
                        }
                        serviceReqUpdateSDO.NotePatient = hisDepartmentTranHospitalizeSDO.Note;
                        this.isPrintHospitalizeExam = hisDepartmentTranHospitalizeSDO.IsPrintHospitalizeExam;
                        this.isSign = hisDepartmentTranHospitalizeSDO.IsSign;
                        this.isPrintSign = hisDepartmentTranHospitalizeSDO.IsPrintSign;



                        if (hisDepartmentTranHospitalizeSDO.icdADOInTreatment != null && !String.IsNullOrWhiteSpace(hisDepartmentTranHospitalizeSDO.icdADOInTreatment.ICD_CODE))
                        {
                            serviceReqUpdateSDO.HospitalizeSDO.IcdCode = hisDepartmentTranHospitalizeSDO.icdADOInTreatment.ICD_CODE;
                            serviceReqUpdateSDO.HospitalizeSDO.IcdName = hisDepartmentTranHospitalizeSDO.icdADOInTreatment.ICD_NAME;
                        }
                        else
                        {
                            var icdValue = UcIcdGetValue() as UC.Icd.ADO.IcdInputADO;
                            if (icdValue != null)
                            {
                                serviceReqUpdateSDO.HospitalizeSDO.IcdCode = icdValue.ICD_CODE;
                                serviceReqUpdateSDO.HospitalizeSDO.IcdName = icdValue.ICD_NAME;
                            }
                        }

                        if (hisDepartmentTranHospitalizeSDO.TraditionalIcdADOInTreatment != null && !String.IsNullOrWhiteSpace(hisDepartmentTranHospitalizeSDO.TraditionalIcdADOInTreatment.ICD_CODE))
                        {
                            serviceReqUpdateSDO.HospitalizeSDO.TraditionalIcdCode = hisDepartmentTranHospitalizeSDO.TraditionalIcdADOInTreatment.ICD_CODE;
                            serviceReqUpdateSDO.HospitalizeSDO.TraditionalIcdName = hisDepartmentTranHospitalizeSDO.TraditionalIcdADOInTreatment.ICD_NAME;
                        }

                        if (hisDepartmentTranHospitalizeSDO.tradtionalIcdSub != null)
                        {
                            serviceReqUpdateSDO.HospitalizeSDO.TraditionalIcdSubCode = hisDepartmentTranHospitalizeSDO.tradtionalIcdSub.ICD_SUB_CODE;
                            serviceReqUpdateSDO.HospitalizeSDO.TraditionalIcdText = hisDepartmentTranHospitalizeSDO.tradtionalIcdSub.ICD_TEXT;
                        }

                        SecondaryIcdDataADO icdSub = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                        serviceReqUpdateSDO.HospitalizeSDO.IcdSubCode = icdSub != null ? icdSub.ICD_SUB_CODE : "";
                        serviceReqUpdateSDO.HospitalizeSDO.IcdText = icdSub != null ? icdSub.ICD_TEXT : "";


                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessTreatmentFinish(ref HisServiceReqExamUpdateSDO serviceReqUpdateSDO)
        {
            try
            {

                if (chkTreatmentFinish.Checked && this.ucTreatmentFinish != null)
                {
                    string str1 = null;
                    string str2 = null;
                    ExamTreatmentFinishResult treatmentFinish = this.treatmentFinishProcessor.GetValue(this.ucTreatmentFinish) as ExamTreatmentFinishResult;

                    if (treatmentFinish != null && treatmentFinish.TreatmentFinishSDO != null)
                    {
                        var cboThongTinBoSung = treatmentFinish.TreatmentFinishSDO.TreatmentEndTypeExtId;
                        if (cboThongTinBoSung == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                        {
                            if (treatmentFinish.TreatmentFinishSDO.SickLeaveDay == null || treatmentFinish.TreatmentFinishSDO.SickLeaveDay <= 0)
                            {
                                MessageBox.Show("Thông tin nghỉ hưởng BHXH thiếu số ngày nghỉ. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            //HIS_PATIENT patient = GetPatientByID(treatment.PATIENT_ID);
                            if (treatment != null)
                            {
                                var age = Inventec.Common.DateTime.Calculation.Age(treatment.TDL_PATIENT_DOB);
                                if (age < 7 && (String.IsNullOrWhiteSpace(treatmentFinish.TreatmentFinishSDO.PatientRelativeName) || String.IsNullOrWhiteSpace(treatmentFinish.TreatmentFinishSDO.PatientRelativeType)))
                                {
                                    MessageBox.Show("Thông tin nghỉ hưởng BHXH thiếu thông tin bố mẹ. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                        else if (cboThongTinBoSung == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                        {
                            if (treatmentFinish.TreatmentFinishSDO.SickLeaveDay == null || treatmentFinish.TreatmentFinishSDO.SickLeaveDay <= 0)
                            {
                                MessageBox.Show("Thông tin nghỉ dưỡng thai thiếu số ngày nghỉ. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        serviceReqUpdateSDO.TreatmentFinishSDO = new HisTreatmentFinishSDO();
                        serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentEndTypeExtId = treatmentFinish.TreatmentFinishSDO.TreatmentEndTypeExtId;
                        if (this.treatment != null
                            && this.treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                            && treatmentFinish.TreatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                        {
                            var SickLeaveDay = treatmentFinish.TreatmentFinishSDO.SickLeaveDay;
                            var SickLeaveFrom = treatmentFinish.TreatmentFinishSDO.SickLeaveFrom;
                            var SickLeaveTo = treatmentFinish.TreatmentFinishSDO.SickLeaveTo;
                            long SickLeaveFrom_Date = 0;
                            long SickLeaveTo_Date = 0;
                            var dtSickLeaveFrom = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(SickLeaveFrom ?? 0);
                            if (dtSickLeaveFrom != null && dtSickLeaveFrom != DateTime.MinValue)
                            {
                                SickLeaveFrom_Date = Int64.Parse((dtSickLeaveFrom ?? new DateTime()).ToString("yyyyMMdd") + "000000");
                            }
                            var dtSickLeaveTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(SickLeaveTo ?? 0);
                            if (dtSickLeaveTo != null && dtSickLeaveTo != DateTime.MinValue)
                            {
                                SickLeaveTo_Date = Int64.Parse((dtSickLeaveTo ?? new DateTime()).ToString("yyyyMMdd") + "000000");
                            }

                            if (SickLeaveDay != null && SickLeaveDay > 30)
                            {
                                MessageBox.Show("Số ngày nghỉ không được vượt quá 30 ngày", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                treatmentFinishProcessor.FocusControl(ucTreatmentFinish);
                                IsReturn = true;
                                return;
                            }

                            CommonParam param = new CommonParam();
                            HisTreatmentFilter filter = new HisTreatmentFilter();
                            filter.PATIENT_ID = treatment.PATIENT_ID;
                            filter.TREATMENT_END_TYPE_EXT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM;
                            filter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                            var dataCheck = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

                            if (dataCheck != null && dataCheck.Count > 0)
                            {
                                var dt = dataCheck.Where(o => o.ID != this.treatment.ID
                                                    && o.SICK_LEAVE_FROM != null && SickLeaveFrom_Date >= o.SICK_LEAVE_FROM
                                                    && o.SICK_LEAVE_TO != null && SickLeaveFrom_Date <= o.SICK_LEAVE_TO).ToList();
                                if (dt != null && dt.Count > 0)
                                {
                                    var treatmentCheck = dt.OrderByDescending(o => o.OUT_TIME).ToList()[0];
                                    MessageBox.Show(String.Format("Ngày nghỉ ốm giao với ngày nghỉ ốm được cấp của đợt khám trước đó: {0} (nghỉ từ {1} - {2})", treatmentCheck.TREATMENT_CODE,
                                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentCheck.SICK_LEAVE_FROM ?? 0),
                                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentCheck.SICK_LEAVE_TO ?? 0)), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    treatmentFinishProcessor.FocusControl(ucTreatmentFinish);
                                    IsReturn = true;
                                    return;
                                }
                            }

                        }
                        serviceReqUpdateSDO.NotePatient = treatmentFinish.Note;
                        serviceReqUpdateSDO.TreatmentFinishSDO.Advise = treatmentFinish.TreatmentFinishSDO.Advise;
                        serviceReqUpdateSDO.TreatmentFinishSDO.AppointmentExamRoomIds = treatmentFinish.TreatmentFinishSDO.AppointmentExamRoomIds;
                        serviceReqUpdateSDO.TreatmentFinishSDO.AppointmentTime = treatmentFinish.TreatmentFinishSDO.AppointmentTime;
                        serviceReqUpdateSDO.TreatmentFinishSDO.ClinicalNote = treatmentFinish.TreatmentFinishSDO.ClinicalNote;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathCauseId = treatmentFinish.TreatmentFinishSDO.DeathCauseId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathTime = treatmentFinish.TreatmentFinishSDO.DeathTime;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathWithinId = treatmentFinish.TreatmentFinishSDO.DeathWithinId;

                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathDocumentDate = treatmentFinish.TreatmentFinishSDO.DeathDocumentDate;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathDocumentNumber = treatmentFinish.TreatmentFinishSDO.DeathDocumentNumber;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathDocumentPlace = treatmentFinish.TreatmentFinishSDO.DeathDocumentPlace;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathDocumentType = treatmentFinish.TreatmentFinishSDO.DeathDocumentType;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathPlace = treatmentFinish.TreatmentFinishSDO.DeathPlace;

                        serviceReqUpdateSDO.TreatmentFinishSDO.DoctorLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        serviceReqUpdateSDO.TreatmentFinishSDO.DoctorUsernname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();


                        //serviceReqUpdateSDO.TreatmentFinishSDO.EndOrderRequest = treatmentFinish.TreatmentFinishSDO.EndOrderRequest;
                        serviceReqUpdateSDO.TreatmentFinishSDO.EndRoomId = moduleData.RoomId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.IsChronic = treatmentFinish.TreatmentFinishSDO.IsChronic;
                        serviceReqUpdateSDO.TreatmentFinishSDO.IsHasAupopsy = treatmentFinish.TreatmentFinishSDO.IsHasAupopsy;
                        serviceReqUpdateSDO.TreatmentFinishSDO.IsTemporary = treatmentFinish.TreatmentFinishSDO.IsTemporary;
                        serviceReqUpdateSDO.TreatmentFinishSDO.MainCause = treatmentFinish.TreatmentFinishSDO.MainCause;
                        serviceReqUpdateSDO.TreatmentFinishSDO.PatientCondition = treatmentFinish.TreatmentFinishSDO.PatientCondition;
                        serviceReqUpdateSDO.TreatmentFinishSDO.ServiceReqId = treatmentFinish.TreatmentFinishSDO.ServiceReqId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.SubclinicalResult = treatmentFinish.TreatmentFinishSDO.SubclinicalResult;
                        serviceReqUpdateSDO.TreatmentFinishSDO.Surgery = treatmentFinish.TreatmentFinishSDO.Surgery;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TranPatiFormId = treatmentFinish.TreatmentFinishSDO.TranPatiFormId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TranPatiReasonId = treatmentFinish.TreatmentFinishSDO.TranPatiReasonId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TranPatiTechId = treatmentFinish.TreatmentFinishSDO.TranPatiTechId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TransferOutMediOrgCode = treatmentFinish.TreatmentFinishSDO.TransferOutMediOrgCode;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TransferOutMediOrgName = treatmentFinish.TreatmentFinishSDO.TransferOutMediOrgName;
                        serviceReqUpdateSDO.TreatmentFinishSDO.Transporter = treatmentFinish.TreatmentFinishSDO.Transporter;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TransportVehicle = treatmentFinish.TreatmentFinishSDO.TransportVehicle;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentDirection = treatmentFinish.TreatmentFinishSDO.TreatmentDirection;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentEndTypeId = treatmentFinish.TreatmentFinishSDO.TreatmentEndTypeId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentFinishTime = treatmentFinish.TreatmentFinishSDO.TreatmentFinishTime;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentId = treatmentFinish.TreatmentFinishSDO.TreatmentId;
                        if (!string.IsNullOrEmpty(treatmentFinish.TreatmentFinishSDO.TreatmentMethod))
                        {
                            serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentMethod = treatmentFinish.TreatmentFinishSDO.TreatmentMethod;
                        }
                        else
                        {
                            serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentMethod = txtTreatmentInstruction.Text.Trim();
                        }
                        serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentResultId = treatmentFinish.TreatmentFinishSDO.TreatmentResultId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.UsedMedicine = treatmentFinish.TreatmentFinishSDO.UsedMedicine;
                        //xuandv
                        serviceReqUpdateSDO.TreatmentFinishSDO.SickLeaveDay = treatmentFinish.TreatmentFinishSDO.SickLeaveDay;
                        serviceReqUpdateSDO.TreatmentFinishSDO.SickLeaveFrom = treatmentFinish.TreatmentFinishSDO.SickLeaveFrom;
                        serviceReqUpdateSDO.TreatmentFinishSDO.SickLeaveTo = treatmentFinish.TreatmentFinishSDO.SickLeaveTo;
                        if (!String.IsNullOrEmpty(treatmentFinish.TreatmentFinishSDO.SickLoginname))
                        {
                            serviceReqUpdateSDO.TreatmentFinishSDO.SickLoginname = treatmentFinish.TreatmentFinishSDO.SickLoginname;
                            serviceReqUpdateSDO.TreatmentFinishSDO.SickUsername = treatmentFinish.TreatmentFinishSDO.SickUsername;
                        }
                        serviceReqUpdateSDO.TreatmentFinishSDO.PatientRelativeName = treatmentFinish.TreatmentFinishSDO.PatientRelativeName;
                        serviceReqUpdateSDO.TreatmentFinishSDO.PatientRelativeType = treatmentFinish.TreatmentFinishSDO.PatientRelativeType;
                        serviceReqUpdateSDO.TreatmentFinishSDO.Babies = treatmentFinish.TreatmentFinishSDO.Babies;
                        serviceReqUpdateSDO.TreatmentFinishSDO.PatientWorkPlace = treatmentFinish.TreatmentFinishSDO.PatientWorkPlace;
                        serviceReqUpdateSDO.TreatmentFinishSDO.WorkPlaceId = treatmentFinish.TreatmentFinishSDO.WorkPlaceId;
                        serviceReqUpdateSDO.FinishTime = treatmentFinish.TreatmentFinishSDO.TreatmentFinishTime;
                        serviceReqUpdateSDO.TreatmentFinishSDO.ProgramId = treatmentFinish.TreatmentFinishSDO.ProgramId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.CreateOutPatientMediRecord = treatmentFinish.TreatmentFinishSDO.CreateOutPatientMediRecord;
                        serviceReqUpdateSDO.TreatmentFinishSDO.SickHeinCardNumber = treatmentFinish.TreatmentFinishSDO.SickHeinCardNumber;
                        serviceReqUpdateSDO.TreatmentFinishSDO.AppointmentSurgery = treatmentFinish.TreatmentFinishSDO.AppointmentSurgery;
                        serviceReqUpdateSDO.TreatmentFinishSDO.SurgeryAppointmentTime = treatmentFinish.TreatmentFinishSDO.SurgeryAppointmentTime;
                        serviceReqUpdateSDO.TreatmentFinishSDO.AppointmentPeriodId = treatmentFinish.TreatmentFinishSDO.AppointmentPeriodId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DocumentBookId = treatmentFinish.TreatmentFinishSDO.DocumentBookId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.SocialInsuranceNumber = treatmentFinish.TreatmentFinishSDO.SocialInsuranceNumber;
                        serviceReqUpdateSDO.TreatmentFinishSDO.NumOrderBlockId = treatmentFinish.TreatmentFinishSDO.NumOrderBlockId;
                        //serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentMethod = txtTreatmentInstruction.Text.Trim();
                        str1 = treatmentFinish.TreatmentFinishSDO.TreatmentMethod;
                        str2 = treatmentFinish.TreatmentFinishSDO.SubclinicalResult;
                        if (!string.IsNullOrEmpty(str1)) serviceReqUpdateSDO.TreatmentInstruction = str1;
                        if (!string.IsNullOrEmpty(str2)) serviceReqUpdateSDO.Subclinical = str2;
                        if (treatmentFinish != null && treatmentFinish.icdADOInTreatment != null && !String.IsNullOrWhiteSpace(treatmentFinish.icdADOInTreatment.ICD_CODE))
                        {
                            serviceReqUpdateSDO.TreatmentFinishSDO.IcdCode = treatmentFinish.icdADOInTreatment.ICD_CODE;
                            serviceReqUpdateSDO.TreatmentFinishSDO.IcdName = treatmentFinish.icdADOInTreatment.ICD_NAME;
                        }
                        else
                        {
                            var icdValue = this.UcIcdGetValue() as UC.Icd.ADO.IcdInputADO;
                            if (icdValue != null)
                            {
                                serviceReqUpdateSDO.TreatmentFinishSDO.IcdCode = icdValue.ICD_CODE;
                                serviceReqUpdateSDO.TreatmentFinishSDO.IcdName = icdValue.ICD_NAME;
                            }
                        }

                        serviceReqUpdateSDO.TreatmentFinishSDO.TraditionalIcdSubCode = treatmentFinish.traditionInIcdSub.ICD_SUB_CODE;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TraditionalIcdText = treatmentFinish.traditionInIcdSub.ICD_TEXT;

                        if (treatmentFinish != null && treatmentFinish.traditionalIcdTreatment != null)
                        {
                            serviceReqUpdateSDO.TreatmentFinishSDO.TraditionalIcdCode = treatmentFinish.traditionalIcdTreatment.ICD_CODE;
                            serviceReqUpdateSDO.TreatmentFinishSDO.TraditionalIcdName = treatmentFinish.traditionalIcdTreatment.ICD_NAME;
                        }

                        //SecondaryIcdDataADO icdSub = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                        serviceReqUpdateSDO.TreatmentFinishSDO.IcdSubCode = treatmentFinish.TreatmentFinishSDO.IcdSubCode;
                        serviceReqUpdateSDO.TreatmentFinishSDO.IcdText = treatmentFinish.TreatmentFinishSDO.IcdText;

                        serviceReqUpdateSDO.TreatmentFinishSDO.CareerId = treatmentFinish.TreatmentFinishSDO.CareerId;

                        serviceReqUpdateSDO.TreatmentFinishSDO.ShowIcdCode = treatmentFinish.TreatmentFinishSDO.ShowIcdCode;
                        serviceReqUpdateSDO.TreatmentFinishSDO.ShowIcdName = treatmentFinish.TreatmentFinishSDO.ShowIcdName;
                        serviceReqUpdateSDO.TreatmentFinishSDO.ShowIcdSubCode = treatmentFinish.TreatmentFinishSDO.ShowIcdSubCode;
                        serviceReqUpdateSDO.TreatmentFinishSDO.ShowIcdText = treatmentFinish.TreatmentFinishSDO.ShowIcdText;
                        // minhnq
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathCertBookId = treatmentFinish.TreatmentFinishSDO.DeathCertBookId;
                        //
                        serviceReqUpdateSDO.TreatmentFinishSDO.IsExpXml4210Collinear = treatmentFinish.TreatmentFinishSDO.IsExpXml4210Collinear;
                        serviceReqUpdateSDO.Advise = treatmentFinish.Advise;
                        serviceReqUpdateSDO.Conclusion = treatmentFinish.Conclusion;
                        if (treatmentFinish.SevereIllNessInfo != null)
                            treatmentFinish.SevereIllNessInfo.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.moduleData.RoomId).DepartmentId;
                        SevereIllnessInfo = treatmentFinish.SevereIllNessInfo;
                        EventsCausesDeaths = treatmentFinish.ListEventsCausesDeath;

                        serviceReqUpdateSDO.TreatmentFinishSDO.EndDeptSubsHeadLoginname = treatmentFinish.TreatmentFinishSDO.EndDeptSubsHeadLoginname;
                        serviceReqUpdateSDO.TreatmentFinishSDO.EndDeptSubsHeadUsername = treatmentFinish.TreatmentFinishSDO.EndDeptSubsHeadUsername;
                        serviceReqUpdateSDO.TreatmentFinishSDO.HospSubsDirectorLoginname = treatmentFinish.TreatmentFinishSDO.HospSubsDirectorLoginname;
                        serviceReqUpdateSDO.TreatmentFinishSDO.HospSubsDirectorUsername = treatmentFinish.TreatmentFinishSDO.HospSubsDirectorUsername;

                        serviceReqUpdateSDO.TreatmentFinishSDO.TranPatiHospitalLoginname = treatmentFinish.TreatmentFinishSDO.TranPatiHospitalLoginname;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TranPatiHospitalUsername = treatmentFinish.TreatmentFinishSDO.TranPatiHospitalUsername;
                        serviceReqUpdateSDO.TreatmentFinishSDO.EndTypeExtNote = treatmentFinish.TreatmentFinishSDO.EndTypeExtNote;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathCertBookFirstId = treatmentFinish.TreatmentFinishSDO.DeathCertBookFirstId;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathCertNumFirst = treatmentFinish.TreatmentFinishSDO.DeathCertNumFirst;
                        serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentMethod = treatmentFinish.TreatmentFinishSDO.TreatmentMethod;
                        serviceReqUpdateSDO.TreatmentFinishSDO.PregnancyTerminationReason = treatmentFinish.TreatmentFinishSDO.PregnancyTerminationReason;
                        serviceReqUpdateSDO.TreatmentFinishSDO.IsPregnancyTermination = treatmentFinish.TreatmentFinishSDO.IsPregnancyTermination;
                        serviceReqUpdateSDO.TreatmentFinishSDO.GestationalAge = treatmentFinish.TreatmentFinishSDO.GestationalAge;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathCertIssuerLoginname = treatmentFinish.TreatmentFinishSDO.DeathCertIssuerLoginname;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathCertIssuerUsername = treatmentFinish.TreatmentFinishSDO.DeathCertIssuerUsername;
                        serviceReqUpdateSDO.TreatmentFinishSDO.PregnancyTerminationTime = treatmentFinish.TreatmentFinishSDO.PregnancyTerminationTime;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathDocumentTypeCode = treatmentFinish.TreatmentFinishSDO.DeathDocumentTypeCode;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathDocumentType = treatmentFinish.TreatmentFinishSDO.DeathDocumentType;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathStatus = treatmentFinish.TreatmentFinishSDO.DeathStatus;
                        serviceReqUpdateSDO.TreatmentFinishSDO.PatientRelativeName = treatmentFinish.TreatmentFinishSDO.PatientRelativeName;
                        serviceReqUpdateSDO.TreatmentFinishSDO.DeathIssuedDate = treatmentFinish.TreatmentFinishSDO.DeathIssuedDate;
                        isPrintAppoinment = treatmentFinish.IsPrintAppoinment;
                        isPrintBordereau = treatmentFinish.IsPrintBordereau;
                        isSignAppoinment = treatmentFinish.IsSignAppoinment;
                        isSignBordereau = treatmentFinish.IsSignBordereau;
                        isPrintBANT = treatmentFinish.IsPrintBANT;
                        isPrintSurgAppoint = treatmentFinish.IsPrintSurgAppoint;
                        IsPrintBHXH = treatmentFinish.IsPrintBHXH;
                        IsSignBHXH = treatmentFinish.IsSignBHXH;
                        isInPhieuPhuLuc = treatmentFinish.IsPrintTrichPhuLuc;
                        isKyPhieuPhuLuc = treatmentFinish.IsSignTrichPhuLuc;
                        isPrintPrescription = treatmentFinish.IsPrintPrescription;
                        isPrintHosTransfer = treatmentFinish.IsPrintHosTransfer;
                        IsSignExam = treatmentFinish.IsSignExam;
                        IsPrintExam = treatmentFinish.IsPrintExam;
                    }
                }
                //else
                //{
                //    Inventec.Common.Logging.LogSystem.Warn("____________TreatmentFinishSDO NULL");
                //    serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentId = treatment.ID;
                //    //serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentEndTypeId = treatment.TREATMENT_END_TYPE_ID ?? 0 ;
                //    //serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentDayCount = treatment.TREATMENT_DAY_COUNT ?? 0;
                //    //serviceReqUpdateSDO.TreatmentFinishSDO.EndRoomId = treatment.END_ROOM_ID ?? 0;
                //    serviceReqUpdateSDO.TreatmentFinishSDO.SubclinicalResult = txtSubclinical.Text.Trim();
                //    serviceReqUpdateSDO.TreatmentFinishSDO.TreatmentMethod = txtTreatmentInstruction.Text.Trim();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallApiSevereIllnessInfo()
        {
            try
            {
                CommonParam param = new CommonParam();
                SevereIllnessInfoSDO sdo = new SevereIllnessInfoSDO();
                sdo.SevereIllnessInfo = SevereIllnessInfo;
                sdo.EventsCausesDeaths = EventsCausesDeaths;
                var dt = new BackendAdapter(param)
                   .Post<HisServiceReqExamUpdateResultSDO>("api/HisSevereIllnessInfo/CreateOrUpdate", ApiConsumers.MosConsumer, sdo, param);
                string message = string.Format("Lưu thông tin tử vong. TREATMENT_CODE: {0}. SERVICE_REQ_CODE: {1}", treatment.TREATMENT_CODE, HisServiceReqView.SERVICE_REQ_CODE);
                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                eventlog.Create(login, null, true, message);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private HIS_PATIENT GetPatientByID(long id)
        {
            HIS_PATIENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        void ProcessExamFinish(ref HisServiceReqExamUpdateSDO serviceReqUpdateSDO)
        {
            try
            {
                if (chkExamFinish.Checked)
                {
                    serviceReqUpdateSDO.IsFinish = true;
                    ExamFinishADO examFinishADO = this.examFinishProcessor.GetValue(this.ucExamFinish) as ExamFinishADO;
                    if (examFinishADO != null)
                    {
                        serviceReqUpdateSDO.FinishTime = examFinishADO.FinishTime;

                        serviceReqUpdateSDO.AppointmentTime = examFinishADO.AppointmentTime;
                        serviceReqUpdateSDO.AppointmentDesc = examFinishADO.Advise;
                        //serviceReqUpdateSDO.APPOINTMENT_CODE = treatment.TREATMENT_CODE;
                        IsAppointment_ExamFinish = examFinishADO.IsAppointment;
                        IsPrintAppointment_ExamFinish = examFinishADO.IsPrintAppointment;
                        serviceReqUpdateSDO.NotePatient = examFinishADO.Note;
                    }
                    else
                    {
                        IsReturn = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessExamSereIcdDTO(ref HisServiceReqExamUpdateSDO serviceReqUpdateSDO)
        {
            try
            {
                var icdValue = UcIcdGetValue();
                if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                {
                    serviceReqUpdateSDO.IcdCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    serviceReqUpdateSDO.IcdName = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                }

                var icdValueCause = UcIcdCauseGetValue();
                if (icdValueCause != null && icdValueCause is UC.Icd.ADO.IcdInputADO)
                {
                    serviceReqUpdateSDO.IcdCauseCode = ((UC.Icd.ADO.IcdInputADO)icdValueCause).ICD_CODE;
                    serviceReqUpdateSDO.IcdCauseName = ((UC.Icd.ADO.IcdInputADO)icdValueCause).ICD_NAME;
                }

                SecondaryIcdDataADO icdSub = (SecondaryIcdDataADO)this.UcSecondaryIcdGetValue();
                serviceReqUpdateSDO.IcdSubCode = icdSub != null ? icdSub.ICD_SUB_CODE : "";
                serviceReqUpdateSDO.IcdText = icdSub != null ? icdSub.ICD_TEXT : "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessExamSereNextTreatmentIntructionDTO(ref HisServiceReqExamUpdateSDO serviceReqUpdateSDO)
        {
            try
            {
                UC.NextTreatmentInstruction.ADO.NextTreatmentInstructionInputADO value = UcNextTreatmentInstGetValue() as UC.NextTreatmentInstruction.ADO.NextTreatmentInstructionInputADO;
                serviceReqUpdateSDO.NextTreaIntrCode = value != null ? value.NEXT_TREA_INTR_CODE : "";
                serviceReqUpdateSDO.NextTreaIntrName = value != null ? value.NEXT_TREA_INTR_NAME : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessExamSereDHST(ref HisServiceReqExamUpdateSDO serviceReqUpdateSDO)
        {
            try
            {

                DHSTADO dhstADO = UcDHSTGetValue() as DHSTADO;
                if (dhstADO != null)
                {
                    valiDHST = dhstADO.IsVali;
                    if (dhstADO.IsVali)
                    {
                        serviceReqUpdateSDO.HisDhst = new HIS_DHST();
                        if (this.HisServiceReqView.DHST_ID.HasValue)
                            serviceReqUpdateSDO.HisDhst.ID = this.HisServiceReqView.DHST_ID.Value;
                        serviceReqUpdateSDO.HisDhst.BELLY = dhstADO.BELLY;
                        serviceReqUpdateSDO.HisDhst.BLOOD_PRESSURE_MAX = dhstADO.BLOOD_PRESSURE_MAX;
                        serviceReqUpdateSDO.HisDhst.BLOOD_PRESSURE_MIN = dhstADO.BLOOD_PRESSURE_MIN;
                        serviceReqUpdateSDO.HisDhst.WEIGHT = dhstADO.WEIGHT;
                        serviceReqUpdateSDO.HisDhst.HEIGHT = dhstADO.HEIGHT;
                        serviceReqUpdateSDO.HisDhst.PULSE = dhstADO.PULSE;
                        serviceReqUpdateSDO.HisDhst.CHEST = dhstADO.CHEST;
                        serviceReqUpdateSDO.HisDhst.TEMPERATURE = dhstADO.TEMPERATURE;
                        serviceReqUpdateSDO.HisDhst.BREATH_RATE = dhstADO.BREATH_RATE;
                        serviceReqUpdateSDO.HisDhst.BELLY = dhstADO.BELLY;
                        serviceReqUpdateSDO.HisDhst.SPO2 = dhstADO.SPO2;
                        serviceReqUpdateSDO.HisDhst.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        serviceReqUpdateSDO.HisDhst.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        serviceReqUpdateSDO.HisDhst.EXECUTE_TIME = dhstADO.EXECUTE_TIME;
                        serviceReqUpdateSDO.HisDhst.NOTE = dhstADO.NOTE;
                        serviceReqUpdateSDO.HisDhst.CAPILLARY_BLOOD_GLUCOSE = dhstADO.CAPILLARY_BLOOD_GLUCOSE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadPostApi()
        {
            Thread threadSevereIllnessInfo = new System.Threading.Thread(new System.Threading.ThreadStart(CallS));

            try
            {
                threadSevereIllnessInfo.Start();
                threadSevereIllnessInfo.Join();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadSevereIllnessInfo.Abort();
            }
        }

        private void CallS()
        {
            try
            {
                if (SevereIllnessInfo != null || (EventsCausesDeaths != null && EventsCausesDeaths.Count > 0))
                    CallApiSevereIllnessInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallSda()
        {
            try
            {
                string message = string.Format("Lưu xử lý khám: SERVICE_REQ_CODE {0}", HisServiceReqView.SERVICE_REQ_CODE);
                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                eventlog.Create(login, null, true, message);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SaveExamServiceReq(HisServiceReqExamUpdateSDO serviceReqExamUpdateSDO)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                serviceReqExamUpdateSDO.RequestRoomId = moduleData.RoomId;
                HisServiceReqResult = new BackendAdapter(param)
                    .Post<HisServiceReqExamUpdateResultSDO>("api/HisServiceReq/ExamUpdate", ApiConsumers.MosConsumer, serviceReqExamUpdateSDO, param);


                if (HisServiceReqResult != null)
                {

                    CreateThreadPostApi();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisServiceReqResult", HisServiceReqResult));

                    success = true;
                    this.HisServiceReqView = new V_HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(this.HisServiceReqView, HisServiceReqResult.ServiceReq);
                    EnableButtonByServiceReq(HisServiceReqResult.ServiceReq.SERVICE_REQ_STT_ID);
                    btnSaveFinish.Enabled = false;
                    this.icdDefaultFinish.ICD_CODE = HisServiceReqResult.ServiceReq.ICD_CODE;
                    this.icdDefaultFinish.ICD_NAME = HisServiceReqResult.ServiceReq.ICD_NAME;
                    //Lấy lại thông tin lưu vào thông tin bệnh nhân để thực hiện cho thao tác tiếp theo
                    LoadCurrentPatient();

                    if (HisServiceReqResult.TreatmentFinishResult != null)
                    {

                        this.treatment = HisServiceReqResult.TreatmentFinishResult;

                        if (this.treatment != null)
                        {
                            UpdateNeedSickLeaveCertControl(this.treatment.NEED_SICK_LEAVE_CERT);
                            this.icdDefaultFinish.ICD_CODE = this.treatment.ICD_CODE;
                            this.icdDefaultFinish.ICD_NAME = this.treatment.ICD_NAME;
                        }

                        if (HisServiceReqResult.TreatmentFinishResult.IS_PAUSE == 1)
                        {

                            btnAggrExam.Enabled = true;
                        }
                        else
                        {
                            btnAggrExam.Enabled = false;
                        }
                    }

                    //Reload executeRoom
                    if (reLoadServiceReq != null)
                    {

                        reLoadServiceReq(HisServiceReqResult.ServiceReq);

                    }

                    btnPrint_ExamService.Enabled = true;

                    if (HisServiceReqResult.HospitalizeResult != null && HisServiceReqResult.HospitalizeResult.Treatment != null)
                    {

                        this.icdDefaultFinish.ICD_CODE = HisServiceReqResult.HospitalizeResult.Treatment.ICD_CODE;
                        this.icdDefaultFinish.ICD_NAME = HisServiceReqResult.HospitalizeResult.Treatment.ICD_NAME;

                        HospitalizeInitADO hospitalizeInitData = new HospitalizeInitADO();
                        hospitalizeInitData.InCode = HisServiceReqResult.HospitalizeResult.Treatment.IN_CODE;
                        hospitalizeInitData.isAutoCheckChkHospitalizeExam = HisConfigCFG.IsAutoCheckPrintHospitalizeExam;
                        hospitalizeInitData.IcdCode = HisServiceReqResult.HospitalizeResult.Treatment.ICD_CODE;
                        hospitalizeInitData.IcdName = HisServiceReqResult.HospitalizeResult.Treatment.ICD_NAME;
                        hospitalizeInitData.TraditionalIcdCode = HisServiceReqResult.HospitalizeResult.Treatment.TRADITIONAL_ICD_CODE;
                        hospitalizeInitData.TraditionalIcdName = HisServiceReqResult.HospitalizeResult.Treatment.TRADITIONAL_ICD_NAME;
                        if (this.ucHospitalize != null)
                        {
                            this.hospitalizeProcessor.ReLoad(this.ucHospitalize, hospitalizeInitData);
                        }
                    }


                    if (HisServiceReqResult.AdditionExamResult != null)
                    {
                        //chkExamServiceAdd.CheckState = CheckState.Unchecked;
                        HisServiceReqResult.AdditionExamResult.ICD_CODE = this.HisServiceReqView.ICD_CODE;
                        HisServiceReqResult.AdditionExamResult.ICD_NAME = this.HisServiceReqView.ICD_NAME;
                        HisServiceReqResult.AdditionExamResult.ICD_CAUSE_CODE = this.HisServiceReqView.ICD_CAUSE_CODE;
                        HisServiceReqResult.AdditionExamResult.ICD_CAUSE_NAME = this.HisServiceReqView.ICD_CAUSE_NAME;
                        HisServiceReqResult.AdditionExamResult.ICD_SUB_CODE = this.HisServiceReqView.ICD_SUB_CODE;
                        HisServiceReqResult.AdditionExamResult.ICD_TEXT = this.HisServiceReqView.ICD_TEXT;
                        ReLoadPrintExamAddition();
                        if (this.isPrintExamServiceAdd || this.isSignExamServiceAdd)
                        {
                            PrintProcess(PrintType.YEU_CAU_KHAM_THEM);
                        }
                    }

                    if (HisServiceReqResult.TreatmentFinishResult != null)
                    {
                        if (HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                        {
                            PrintMps = false;
                            ThreadCustomManager.ThreadResultCallBack(LoadSereServ8, ReLoadPrintHasAppoinment);
                        }
                        else if (HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                        {
                            ThreadCustomManager.ThreadResultCallBack(LoadSereServ8, ReLoadPrintTranPati);
                        }
                        else if ((HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                            || HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN
                            || HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON)
                            && HisServiceReqResult.TreatmentFinishResult.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            ThreadCustomManager.ThreadResultCallBack(LoadSereServ8, ReLoadPrintOutHospital);
                        }
                        else if (HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                        {
                            ThreadCustomManager.ThreadResultCallBack(LoadSereServ8, ReLoadPrintGiayBaoTu);
                        }
                        else
                        {
                            ThreadCustomManager.ThreadResultCallBack(LoadSereServ8, ReLoadPrint);
                        }

                        if (HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                        {
                            ReLoadPrintTreatmentEndTypeExt(PrintType.IN_GIAY_NGHI_OM);
                        }
                        else if (HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                        {
                            ReLoadPrintTreatmentEndTypeExt(PrintType.IN_GIAY_NGHI_DUONG_THAI);
                        }
                        else if (HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                        {
                            ReLoadPrintTreatmentEndTypeExt(PrintType.IN_PHIEU_HEN_MO, this.isPrintSurgAppoint);
                        }

                        if (this.ucTreatmentFinish != null)
                        {
                            if (HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                                groupControlTreatmentFinish.Enabled = true;
                            this.icdDefaultFinish.ICD_CODE = HisServiceReqResult.TreatmentFinishResult.ICD_CODE;
                            this.icdDefaultFinish.ICD_NAME = HisServiceReqResult.TreatmentFinishResult.ICD_NAME;

                            this.treatmentFinishProcessor.SetValueV2(this.ucTreatmentFinish, this.moduleData, HisServiceReqResult);

                            if (HisServiceReqResult.TreatmentFinishResult.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                            {
                                this.LoadProcessAndPrintBHXH();
                            }
                        }

                        if (this.isPrintBANT)
                        {
                            PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(this.treatment, currentModuleBase != null ? currentModuleBase.RoomId : 0);
                            printTreatmentFinishProcessor.Print(HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintEnum.IN_BANT__MPS000174);
                        }

                        Inventec.Common.Logging.LogSystem.Fatal("Phiếu Trích Phụ Lục MPS000316 ( UC_TREATMENT_FINISH) _____ 1");
                        //Phiếu Trích Phụ Lục MPS000316 ( UC_TREATMENT_FINISH)
                        HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(req, HisServiceReqView);
                        var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(req);
                        Inventec.Common.Logging.LogSystem.Fatal("Phiếu Trích Phụ Lục MPS000316 ( UC_TREATMENT_FINISH) _____ 2");
                        if (this.isInPhieuPhuLuc && !this.isKyPhieuPhuLuc)
                        {
                            printTest.Print("Mps000316", MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow);
                        }
                        else if (this.isInPhieuPhuLuc && this.isKyPhieuPhuLuc)
                        {
                            printTest.Print("Mps000316", MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow);

                        }
                        else if (!this.isInPhieuPhuLuc && this.isKyPhieuPhuLuc)
                        {
                            printTest.Print("Mps000316", MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow);
                        }
                        Inventec.Common.Logging.LogSystem.Fatal("Phiếu Trích Phụ Lục MPS000316 ( UC_TREATMENT_FINISH) _____ 3");

                        if (this.IsSignExam || this.IsPrintExam)
                        {
                            IsActionButtonSave = true;
                            PrintProcess(PrintType.KHAM_BENH_VAO_VIEN);
                        }
                        //if (this.isInPhieuPhuLuc && !this.isKyPhieuPhuLuc)
                        //{
                        //    if (printTest != null)
                        //    {
                        //        printTest.Print("Mps000316");
                        //    }
                        //}

                        if (this.isPrintPrescription)
                        {
                            bool printNow = true;
                            InDonPhongKhamTongHop(printNow, false);
                        }
                    }
                    if (this.isPrintHosTransfer)
                    {
                        PrintProcess(PrintType.IN_GIAY_CHUYEN_VIEN);
                    }
                    //if ((IsAppointment_ExamServiceAdd && IsPrintAppointment_ExamServiceAdd) || (IsAppointment_ExamFinish && IsPrintAppointment_ExamFinish))
                    if (IsAppointment_ExamServiceAdd || IsAppointment_ExamFinish)
                    {
                        PrintMps = false;
                        ThreadCustomManager.ThreadResultCallBack(LoadSereServ8, ReLoadPrintHasAppoinment);
                    }

                    if (this.isPrintHospitalizeExam)
                    {
                        PrintProcess(PrintType.KHAM_BENH_VAO_VIEN);
                    }
                    this.BtnRefreshForFormOther();
                    FillDataToButtonPrintAndAutoPrint();

                    //Inventec.Common.Logging.LogSystem.Warn("ProgramId: " + serviceReqExamUpdateSDO.TreatmentFinishSDO.ProgramId.Value + " EMR_COVER_TYPE_ID: "+this.treatment.EMR_COVER_TYPE_ID.Value);

                    if (serviceReqExamUpdateSDO.TreatmentFinishSDO != null && serviceReqExamUpdateSDO.TreatmentFinishSDO.ProgramId != null && this.treatment.EMR_COVER_TYPE_ID == null)
                    {
                        btnVoBenhAn.Enabled = true;
                    }
                    else
                    {
                        btnVoBenhAn.Enabled = false;
                    }

                    // đóng tab sau khi lưu
                    if (success
                        && HisConfigCFG.IsAutoExitAfterFinish
                        && ((chkExamServiceAdd.Checked && this.ucExamAddition != null)
                        || (chkHospitalize.Checked && this.ucHospitalize != null)
                        || (chkTreatmentFinish.Checked && this.ucTreatmentFinish != null)
                        || chkExamFinish.Checked))
                    {
                        timeClose.Start();
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                if (success)
                {
                    long WarningOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(SdaConfigKeys.WARNING_OPTION);

                    if (WarningOption == 1 && this.treatment != null && this.treatment.PROGRAM_ID != null && this.treatment.EMR_COVER_TYPE_ID == null)
                    {
                        if (MessageBox.Show(ResourceMessage.ChuaDuocTaoVoBenhAn, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            VoBenhAn();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        bool SaveExamExecute()
        {
            bool result = true;
            try
            {
                this.positionHandleControlLeft = -1;
                if (!dxValidationProviderForLeftPanel.Validate())
                    return false;
                HisServiceReqExamUpdateSDO hisServiceReqSDO = new HisServiceReqExamUpdateSDO();

                ProcessExamServiceReqDTO(ref hisServiceReqSDO);
                ProcessExamSereIcdDTO(ref hisServiceReqSDO);
                ProcessExamSereNextTreatmentIntructionDTO(ref hisServiceReqSDO);
                ProcessExamSereDHST(ref hisServiceReqSDO);

                if (valiDHST)
                    SaveExamServiceReq(hisServiceReqSDO);
                else
                    return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        void SuccessLog(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ result)
        {
            try
            {
                if (result != null)
                {
                    //string message = String.Format(EventLogUtil.SetLog(His.EventLog.Message.Enum.TraKetQuaDichVuKhamBenh), result.ID, result.SERVICE_REQ_CODE, result.FINISH_TIME, result.EXECUTE_LOGINNAME, result.EXECUTE_USERNAME, result.TREATMENT_CODE, result.PATIENT_CODE, result.VIR_PATIENT_NAME);
                    //His.EventLog.Logger.Log(LOGIC.LocalStore.GlobalStore.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnClickInCongKhaiDichVu(object sender, EventArgs e)
        {
            try
            {
                var treatment = new V_HIS_TREATMENT_4();
                treatment.ID = HisServiceReqView.TREATMENT_ID;
                List<object> listArgs = new List<object>();
                listArgs.Add(treatment);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PublicServices_NT", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void OnclickOpenMuduleRequestForAdvance(object sender, EventArgs e)
        {
            try
            {
                List<object> ListArgs = new List<object>();
                ListArgs.Add(this.HisServiceReqView.TREATMENT_ID);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.RequestDeposit", this.moduleData.RoomId, this.moduleData.RoomTypeId, ListArgs);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnClickOpenXemGiayChuyenTuyen(object sender, EventArgs e)
        {
            try
            {
                var treatment = GetTreatmentById(this.HisServiceReqView.TREATMENT_ID);
                if (treatment != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTreatmentFile").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTreatmentFile");
                    Inventec.Common.Logging.LogSystem.Error("CHECK HIS.Desktop.Plugins.HisTreatmentFile");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("OPEN HIS.Desktop.Plugins.HisTreatmentFile");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(treatment.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    Inventec.Common.Logging.LogSystem.Error("CLOSE HIS.Desktop.Plugins.HisTreatmentFile");

                    //if (treatment.TRANSFER_IN_URL != null)
                    //{
                    //    System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(treatment.TRANSFER_IN_URL);
                    //    frmXemGiayChuyenTuyen frm = new frmXemGiayChuyenTuyen(stream);
                    //    frm.ShowDialog();
                    //}
                    //else
                    //{
                    //    DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân không có giấy chuyển tuyến", ResourceMessage.ThongBao, MessageBoxButtons.OK);
                    //}
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnclickInBenhAnNgoaiChan(object sender, EventArgs e)
        {
            try
            {
                List<object> ListArgs = new List<object>();
                ListArgs.Add(this.HisServiceReqView.TREATMENT_ID);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.RequestDeposit", this.moduleData.RoomId, this.moduleData.RoomTypeId, ListArgs);

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BenhAnNgoaiTruDayMatClick(object sender, EventArgs e)
        {
            try
            {
                if (this.treatment != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.treatment.ID;
                    ado.PatientId = this.treatment.PATIENT_ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000418_BENH_AN_NGOAI_TRU_DAY_MAT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThanhToanChiPhiVienPhiClick(object sender, EventArgs e)
        {
            try
            {
                ProcessPayment(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CauHinhKetNoiDauDocTheClick(object sender, EventArgs e)
        {
            try
            {
                frmConnectCOM connectCom = new frmConnectCOM();
                connectCom.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BenhAnNgoaiTruGlaucomaClick(object sender, EventArgs e)
        {
            try
            {
                if (this.treatment != null)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = this.treatment.ID;
                    ado.PatientId = this.treatment.PATIENT_ID;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000419_BENH_AN_NGOAI_TRU_GLAUCOMA);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuVoBenhAn(object sender, EventArgs e)
        {
            try
            {
                long roomId = this.moduleData.RoomId;
                HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();

                emrInputAdo.TreatmentId = this.treatment.ID;
                emrInputAdo.PatientId = this.treatment.PATIENT_ID;
                var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.ROOM_ID == roomId && o.TREATMENT_TYPE_ID == this.treatment.TDL_TREATMENT_TYPE_ID).ToList();
                if (this.treatment.EMR_COVER_TYPE_ID != null)
                {
                    emrInputAdo.EmrCoverTypeId = this.treatment.EMR_COVER_TYPE_ID.Value;
                }
                else
                {
                    if (data != null && data.Count > 0)
                    {
                        if (data.Count == 1)
                        {
                            emrInputAdo.EmrCoverTypeId = data.FirstOrDefault().EMR_COVER_TYPE_ID;
                        }
                        else
                        {
                            emrInputAdo.lstEmrCoverTypeId = new List<long>();
                            emrInputAdo.lstEmrCoverTypeId = data.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                        }
                    }
                    else
                    {
                        var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;

                        var DataConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == this.treatment.TDL_TREATMENT_TYPE_ID).ToList();

                        if (DataConfig != null && DataConfig.Count > 0)
                        {
                            if (DataConfig.Count == 1)
                            {
                                emrInputAdo.EmrCoverTypeId = DataConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                            }
                            else
                            {
                                emrInputAdo.lstEmrCoverTypeId = new List<long>();
                                emrInputAdo.lstEmrCoverTypeId = DataConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                            }
                        }
                    }
                }

                emrInputAdo.roomId = roomId;


                HIS.Desktop.Plugins.Library.FormMedicalRecord.FromConfig.frmPhieu frm = new Library.FormMedicalRecord.FromConfig.frmPhieu(emrInputAdo);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DisposeData()
        {
            try
            {
                PatyAlterBhyt = null;
                currentExpMestMedicines = null;
                HisServiceReqView = null;
                serviceReq = null;
                treatmentByPatients = null;
                currentPatient = null;
                treatment = null;
                treatmentView = null;
                sereServ = null;
                ltreatment2 = null;
                SereServsCurrentTreatment = null;
                ClsSereServ = null;
                SereServ8s = null;
                TreatmentHistorys = null;
                dataNextTreatmentInstructions = null;
                datas = null;
                currentIcds = null;
                icdPopupSelect = null;
                icdSubcodeAdoChecks = null;
                subIcdPopupSelect = null;
                ServiceReqExamAddition = null;
                HisServiceReqResult = null;
                reLoadServiceReq = null;
                refresh = null;
                treatmentFinishProcessor = null;
                examServiceAddProcessor = null;
                hospitalizeProcessor = null;
                examFinishProcessor = null;
                PatientProgramList = null;
                DataStoreList = null;
                MediRecode = null;
                icdDefaultFinish = null;
                LstEmrCoverConfig = null;
                LstEmrCoverConfigDepartment = null;
                contraindicationSelecteds = null;
                IcdSubChoose = null;
                icdCodeList = null;
                lstHealthExamRank = null;
                CurrentPatient = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}