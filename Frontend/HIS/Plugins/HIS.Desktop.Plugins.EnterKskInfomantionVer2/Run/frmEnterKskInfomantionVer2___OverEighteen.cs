﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using DevExpress.XtraEditors.Controls;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private HIS_DHST dhstOverEighteen { get; set; }

        private void ResetControlOverEighteen()
        {
            try
            {
                spnHeight2.EditValue = null;
                spnPulse2.EditValue = null;
                spnWeight2.EditValue = null;
                spnBloodPressureMax2.EditValue = null;
                spnBloodPressureMin2.EditValue = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPageOverEighteen()
        {
            try
            {
                ResetControlOverEighteen();
                SetDataCboRank(cboDhstRank2);
                SetDataCboRank(cboExamCirculationRank2);
                SetDataCboRank(cboExamRespiratoryRank2);
                SetDataCboRank(cboExamDigestionRank2);
                SetDataCboRank(cboExamKidneyUrologyRank2);
                SetDataCboRank(cboExamNeurologicalRank2);
                SetDataCboRank(cboExamMuscleBoneRank2);
                SetDataCboRank(cboExamMentalRank2);
                SetDataCboRank(cboExamSurgeryRank2);
                SetDataCboRank(cboExamObstetricRank2);
                SetDataCboRank(cboExamEyeRank2);
                SetDataCboRank(cboExamEntDiseaseRank2);
                SetDataCboRank(cboExamStomatologyRank2);
                SetDataCboRank(cboHealthExamRank2);
                SetDataCboRank(cboExamDernatologyRank2);
                FillDataOverEighteen();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataOverEighteen()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    CommonParam param = new CommonParam();
                    HisKskOverEighteenFilter filter = new HisKskOverEighteenFilter();
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_OVER_EIGHTEEN>>("api/HisKskOverEighteen/Get", ApiConsumers.MosConsumer, filter, param);
                    if (data != null && data.Count > 0)
                    {
                        currentKskOverEight = data.First();
                        txtPathologicalHistoryFamily.Text = currentKskOverEight.PATHOLOGICAL_HISTORY_FAMILY;
                        txtPathologicalHistory2.Text = currentKskOverEight.PATHOLOGICAL_HISTORY;
                        txtMedicineUsing.Text = currentKskOverEight.MEDICINE_USING;
                        txtMaternityHistory.Text = currentKskOverEight.MATERNITY_HISTORY;
                        cboDhstRank2.EditValue = currentKskOverEight.DHST_RANK;
                        txtExamCirculation2.Text = currentKskOverEight.EXAM_CIRCULATION;
                        cboExamCirculationRank2.EditValue = currentKskOverEight.EXAM_CIRCULATION_RANK;
                        txtExamRespiratory2.Text = currentKskOverEight.EXAM_RESPIRATORY;
                        cboExamRespiratoryRank2.EditValue = currentKskOverEight.EXAM_RESPIRATORY_RANK;
                        txtExamDigestion2.Text = currentKskOverEight.EXAM_DIGESTION;
                        cboExamDigestionRank2.EditValue = currentKskOverEight.EXAM_DIGESTION_RANK;
                        txtExamKidneyUrology2.Text = currentKskOverEight.EXAM_KIDNEY_UROLOGY;
                        cboExamKidneyUrologyRank2.EditValue = currentKskOverEight.EXAM_KIDNEY_UROLOGY_RANK;
                        txtExamNeurological2.Text = currentKskOverEight.EXAM_NEUROLOGICAL;
                        cboExamNeurologicalRank2.EditValue = currentKskOverEight.EXAM_NEUROLOGICAL_RANK;
                        txtExamMuscleBone2.Text = currentKskOverEight.EXAM_MUSCLE_BONE;
                        cboExamMuscleBoneRank2.EditValue = currentKskOverEight.EXAM_MUSCLE_BONE_RANK;
                        txtExamMental2.Text = currentKskOverEight.EXAM_MENTAL;
                        cboExamMentalRank2.EditValue = currentKskOverEight.EXAM_MENTAL_RANK;
                        txtExamSurgery2.Text = currentKskOverEight.EXAM_SURGERY;
                        cboExamSurgeryRank2.EditValue = currentKskOverEight.EXAM_SURGERY_RANK;
                        txtExamDernatology2.Text = currentKskOverEight.EXAM_DERMATOLOGY;
                        cboExamDernatologyRank2.EditValue = currentKskOverEight.EXAM_DERMATOLOGY_RANK;
                        txtExamObstetric2.Text = currentKskOverEight.EXAM_OBSTETRIC;
                        cboExamObstetricRank2.EditValue = currentKskOverEight.EXAM_OBSTETRIC_RANK;

                        txtExamEyeSightRight2.Text = currentKskOverEight.EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft2.Text = currentKskOverEight.EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight2.Text = currentKskOverEight.EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft2.Text = currentKskOverEight.EXAM_EYESIGHT_GLASS_LEFT;
                        txtExamEyeDisease2.Text = currentKskOverEight.EXAM_EYE_DISEASE;
                        cboExamEyeRank2.EditValue = currentKskOverEight.EXAM_EYE_RANK;
                        txtExamEntLeftNormal2.Text = currentKskOverEight.EXAM_ENT_LEFT_NORMAL;
                        txtExamEntLeftWhisper2.Text = currentKskOverEight.EXAM_ENT_LEFT_WHISPER;
                        txtExamEntRightNomal2.Text = currentKskOverEight.EXAM_ENT_RIGHT_NORMAL;
                        txtExamEntRightWhisper2.Text = currentKskOverEight.EXAM_ENT_RIGHT_WHISPER;
                        txtExamEntDisease2.Text = currentKskOverEight.EXAM_ENT_DISEASE;
                        cboExamEntDiseaseRank2.EditValue = currentKskOverEight.EXAM_ENT_RANK;
                        txtExamStomatologyUpper2.Text = currentKskOverEight.EXAM_STOMATOLOGY_UPPER;
                        txtExamStomatologyLower2.Text = currentKskOverEight.EXAM_STOMATOLOGY_LOWER;
                        txtExamStomatologyDisease2.Text = currentKskOverEight.EXAM_STOMATOLOGY_DISEASE;
                        cboExamStomatologyRank2.EditValue = currentKskOverEight.EXAM_STOMATOLOGY_RANK;

                        txtTestBloodHc2.Text = currentKskOverEight.TEST_BLOOD_HC;
                        txtTestBloodTc2.Text = currentKskOverEight.TEST_BLOOD_TC;
                        txtTestBloodBc2.Text = currentKskOverEight.TEST_BLOOD_BC;
                        txtTestBloodGluco2.Text = currentKskOverEight.TEST_BLOOD_GLUCO;
                        txtTestBloodUre2.Text = currentKskOverEight.TEST_BLOOD_URE;
                        txtTestBloodCreatinin2.Text = currentKskOverEight.TEST_BLOOD_CREATININ;
                        txtTestBloodAsat2.Text = currentKskOverEight.TEST_BLOOD_ASAT;
                        txtTestBloodAlat2.Text = currentKskOverEight.TEST_BLOOD_ALAT;
                        txtTestBloodOther2.Text = currentKskOverEight.TEST_BLOOD_OTHER;
                        txtTestUrineGluco2.Text = currentKskOverEight.TEST_URINE_GLUCO;
                        txtTestUrineProtein2.Text = currentKskOverEight.TEST_URINE_PROTEIN;
                        txtTestUrineOther2.Text = currentKskOverEight.TEST_URINE_OTHER;

                        txtResultDiim2.Text = currentKskOverEight.RESULT_DIIM;
                        cboHealthExamRank2.EditValue = currentKskOverEight.HEALTH_EXAM_RANK_ID;
                        txtDiseases2.Text = currentKskOverEight.DISEASES;
                        if (currentKskOverEight.DHST_ID != null && currentKskOverEight.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentKskOverEight.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                dhstOverEighteen = dataDhst.First();
                                spnHeight2.EditValue = dhstOverEighteen.HEIGHT;
                                spnPulse2.EditValue = dhstOverEighteen.PULSE;
                                spnWeight2.EditValue = dhstOverEighteen.WEIGHT;
                                spnBloodPressureMax2.EditValue = dhstOverEighteen.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin2.EditValue = dhstOverEighteen.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight2, spnWeight2, txtVirBmi2);
                            }
                        }
                    }
                    else
                    {
                        txtPathologicalHistoryFamily.Text = currentServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                        txtPathologicalHistory2.Text = currentServiceReq.PATHOLOGICAL_HISTORY;
                        txtExamCirculation2.Text = currentServiceReq.PART_EXAM_CIRCULATION;
                        txtExamRespiratory2.Text = currentServiceReq.PART_EXAM_RESPIRATORY;
                        txtExamDigestion2.Text = currentServiceReq.PART_EXAM_DIGESTION;
                        txtExamKidneyUrology2.Text = currentServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                        txtExamMuscleBone2.Text = currentServiceReq.PART_EXAM_MUSCLE_BONE;
                        txtExamNeurological2.Text = currentServiceReq.PART_EXAM_NEUROLOGICAL;
                        txtExamMental2.Text = currentServiceReq.PART_EXAM_MENTAL;
                        txtExamObstetric2.Text = currentServiceReq.PART_EXAM_OBSTETRIC;

                        txtExamEyeSightRight2.Text = currentServiceReq.PART_EXAM_EYESIGHT_RIGHT;
                        txtExamEyeSightLeft2.Text = currentServiceReq.PART_EXAM_EYESIGHT_LEFT;
                        txtExamEyeSightGlassRight2.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_RIGHT;
                        txtExamEyeSightGlassLeft2.Text = currentServiceReq.PART_EXAM_EYESIGHT_GLASS_LEFT;

                        txtExamEntLeftNormal2.Text = currentServiceReq.PART_EXAM_EAR_LEFT_NORMAL;
                        txtExamEntLeftWhisper2.Text = currentServiceReq.PART_EXAM_EAR_LEFT_WHISPER;
                        txtExamEntRightNomal2.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_NORMAL;
                        txtExamEntRightWhisper2.Text = currentServiceReq.PART_EXAM_EAR_RIGHT_WHISPER;

                        txtExamStomatologyUpper2.Text = currentServiceReq.PART_EXAM_UPPER_JAW;
                        txtExamStomatologyLower2.Text = currentServiceReq.PART_EXAM_LOWER_JAW;
                        txtExamDernatology2.Text = currentServiceReq.PART_EXAM_DERMATOLOGY;
                        txtExamSurgery2.Text = currentServiceReq.SUBCLINICAL;
                        if (currentServiceReq.DHST_ID != null && currentServiceReq.DHST_ID > 0)
                        {
                            HisDhstFilter dhstFilter = new HisDhstFilter();
                            dhstFilter.ID = currentServiceReq.DHST_ID;
                            var dataDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                            if (dataDhst != null && dataDhst.Count > 0)
                            {
                                var currentDhst = dataDhst.First();
                                spnHeight2.EditValue = currentDhst.HEIGHT;
                                spnPulse2.EditValue = currentDhst.PULSE;
                                spnWeight2.EditValue = currentDhst.WEIGHT;
                                spnBloodPressureMax2.EditValue = currentDhst.BLOOD_PRESSURE_MAX;
                                spnBloodPressureMin2.EditValue = currentDhst.BLOOD_PRESSURE_MIN;
                                //txtVirBmi.Text = currentDhst.VIR_BMI!=null ? currentDhst.VIR_BMI.ToString() : "";
                                FillNoteBMI(spnHeight2, spnWeight2, txtVirBmi2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight2_EditValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight2, spnWeight2, txtVirBmi2);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight2_EditValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                FillNoteBMI(spnHeight2, spnWeight2, txtVirBmi2);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_KSK_OVER_EIGHTEEN GetValueOverEighteen()
        {
            HIS_KSK_OVER_EIGHTEEN obj = new HIS_KSK_OVER_EIGHTEEN();
            try
            {
                if (currentKskOverEight != null)
                    obj.ID = currentKskOverEight.ID;
                obj.PATHOLOGICAL_HISTORY_FAMILY = txtPathologicalHistoryFamily.Text;
                obj.PATHOLOGICAL_HISTORY = txtPathologicalHistory2.Text;
                obj.MEDICINE_USING = txtMedicineUsing.Text;
                obj.MATERNITY_HISTORY = txtMaternityHistory.Text;
                //DHST
                obj.DHST_RANK = cboDhstRank2.EditValue != null ? Int64.Parse(cboDhstRank2.EditValue.ToString()) : 0;
                obj.EXAM_CIRCULATION = txtExamCirculation2.Text;
                obj.EXAM_CIRCULATION_RANK = cboExamCirculationRank2.EditValue != null ? Int64.Parse(cboExamCirculationRank2.EditValue.ToString()) : 0;
                obj.EXAM_RESPIRATORY = txtExamRespiratory2.Text;
                obj.EXAM_RESPIRATORY_RANK = cboExamRespiratoryRank2.EditValue != null ? Int64.Parse(cboExamRespiratoryRank2.EditValue.ToString()) : 0;
                obj.EXAM_DIGESTION = txtExamDigestion2.Text;
                obj.EXAM_DIGESTION_RANK = cboExamDigestionRank2.EditValue != null ? Int64.Parse(cboExamDigestionRank2.EditValue.ToString()) : 0;
                obj.EXAM_KIDNEY_UROLOGY = txtExamKidneyUrology2.Text;
                obj.EXAM_KIDNEY_UROLOGY_RANK = cboExamKidneyUrologyRank2.EditValue != null ? Int64.Parse(cboExamKidneyUrologyRank2.EditValue.ToString()) : 0;
                obj.EXAM_NEUROLOGICAL = txtExamNeurological2.Text;
                obj.EXAM_NEUROLOGICAL_RANK = cboExamNeurologicalRank2.EditValue != null ? Int64.Parse(cboExamNeurologicalRank2.EditValue.ToString()) : 0;
                obj.EXAM_MUSCLE_BONE = txtExamMuscleBone2.Text;
                obj.EXAM_MUSCLE_BONE_RANK = cboExamMuscleBoneRank2.EditValue != null ? Int64.Parse(cboExamMuscleBoneRank2.EditValue.ToString()) : 0;
                obj.EXAM_MENTAL = txtExamMental2.Text;
                obj.EXAM_MENTAL_RANK = cboExamMentalRank2.EditValue != null ? Int64.Parse(cboExamMentalRank2.EditValue.ToString()) : 0;
                obj.EXAM_SURGERY = txtExamSurgery2.Text;
                obj.EXAM_SURGERY_RANK = cboExamSurgeryRank2.EditValue != null ? Int64.Parse(cboExamSurgeryRank2.EditValue.ToString()) : 0;
                obj.EXAM_DERMATOLOGY = txtExamDernatology2.Text;
                obj.EXAM_DERMATOLOGY_RANK = cboExamDernatologyRank2.EditValue != null ? Int64.Parse(cboExamDernatologyRank2.EditValue.ToString()) : 0;
                obj.EXAM_OBSTETRIC = txtExamObstetric2.Text;
                obj.EXAM_OBSTETRIC_RANK = cboExamObstetricRank2.EditValue != null ? Int64.Parse(cboExamObstetricRank2.EditValue.ToString()) : 0;

                obj.EXAM_EYESIGHT_RIGHT = txtExamEyeSightRight2.Text;
                obj.EXAM_EYESIGHT_LEFT = txtExamEyeSightLeft2.Text;
                obj.EXAM_EYESIGHT_GLASS_RIGHT = txtExamEyeSightGlassRight2.Text;
                obj.EXAM_EYESIGHT_GLASS_LEFT = txtExamEyeSightGlassLeft2.Text;
                obj.EXAM_EYE_DISEASE = txtExamEyeDisease2.Text;
                obj.EXAM_EYE_RANK = cboExamEyeRank2.EditValue != null ? Int64.Parse(cboExamEyeRank2.EditValue.ToString()) : 0;
                obj.EXAM_ENT_LEFT_NORMAL = txtExamEntLeftNormal2.Text;
                obj.EXAM_ENT_LEFT_WHISPER = txtExamEntLeftWhisper2.Text;
                obj.EXAM_ENT_RIGHT_NORMAL = txtExamEntRightNomal2.Text;
                obj.EXAM_ENT_RIGHT_WHISPER = txtExamEntRightWhisper2.Text;
                obj.EXAM_ENT_DISEASE = txtExamEntDisease2.Text;
                obj.EXAM_ENT_RANK = cboExamEntDiseaseRank2.EditValue != null ? Int64.Parse(cboExamEntDiseaseRank2.EditValue.ToString()) : 0;
                obj.EXAM_STOMATOLOGY_UPPER = txtExamStomatologyUpper2.Text;
                obj.EXAM_STOMATOLOGY_LOWER = txtExamStomatologyLower2.Text;
                obj.EXAM_STOMATOLOGY_DISEASE = txtExamStomatologyDisease2.Text;
                obj.EXAM_STOMATOLOGY_RANK = cboExamStomatologyRank2.EditValue != null ? Int64.Parse(cboExamStomatologyRank2.EditValue.ToString()) : 0;
                obj.TEST_BLOOD_HC = txtTestBloodHc2.Text;
                obj.TEST_BLOOD_BC = txtTestBloodTc2.Text;
                obj.TEST_BLOOD_TC = txtTestBloodBc2.Text;
                obj.TEST_BLOOD_GLUCO = txtTestBloodGluco2.Text;
                obj.TEST_BLOOD_URE = txtTestBloodUre2.Text;
                obj.TEST_BLOOD_CREATININ = txtTestBloodCreatinin2.Text;
                obj.TEST_BLOOD_ASAT = txtTestBloodAsat2.Text;
                obj.TEST_BLOOD_ALAT = txtTestBloodAlat2.Text;
                obj.TEST_BLOOD_OTHER = txtTestBloodOther2.Text;
                obj.TEST_URINE_GLUCO = txtTestUrineGluco2.Text;
                obj.TEST_URINE_PROTEIN = txtTestUrineProtein2.Text;
                obj.TEST_URINE_OTHER = txtTestUrineOther2.Text;
                obj.RESULT_DIIM = txtResultDiim2.Text;
                obj.HEALTH_EXAM_RANK_ID = cboHealthExamRank2.EditValue != null ? Int64.Parse(cboHealthExamRank2.EditValue.ToString()) : 0;
                obj.DISEASES = txtDiseases2.Text;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private HIS_DHST GetDhstOverighteen()
        {
            HIS_DHST obj = new HIS_DHST();
            try
            {
                if (dhstOverEighteen != null)
                    obj.ID = dhstOverEighteen.ID;
                if (spnBloodPressureMax2.EditValue != null)
                    obj.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMax2.Value.ToString());
                if (spnBloodPressureMin2.EditValue != null)
                    obj.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spnBloodPressureMin2.Value.ToString());
                if (spnHeight2.EditValue != null)
                    obj.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnHeight2.Value, 2);
                if (spnPulse2.EditValue != null)
                    obj.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spnPulse2.Value.ToString());
                if (spnWeight2.EditValue != null)
                    obj.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spnWeight2.Value, 2);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        #region ---PREVIEWKEYDOWN---
        private void txtPathologicalHistoryFamily_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalHistory2.Focus();
                    txtPathologicalHistory2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPathologicalHistory2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineUsing.Focus();
                    txtMedicineUsing.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineUsing_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaternityHistory.Focus();
                    txtMaternityHistory.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaternityHistory_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnHeight2.Focus();
                    spnHeight2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnHeight2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnWeight2.Focus();
                    spnWeight2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnWeight2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnPulse2.Focus();
                    spnPulse2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnPulse2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMax2.Focus();
                    spnBloodPressureMax2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMax2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnBloodPressureMin2.Focus();
                    spnBloodPressureMin2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnBloodPressureMin2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDhstRank2.Focus();
                    cboDhstRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDhstRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamCirculation2.Focus();
                    txtExamCirculation2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamCirculation2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamCirculationRank2.Focus();
                    cboExamCirculationRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamCirculationRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamRespiratory2.Focus();
                    txtExamRespiratory2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamRespiratory2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamRespiratoryRank2.Focus();
                    cboExamRespiratoryRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamRespiratoryRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDigestion2.Focus();
                    txtExamDigestion2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamDigestion2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDigestionRank2.Focus();
                    cboExamDigestionRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamDigestionRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamKidneyUrology2.Focus();
                    txtExamKidneyUrology2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamKidneyUrology2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamKidneyUrologyRank2.Focus();
                    cboExamKidneyUrologyRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamKidneyUrologyRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMuscleBone2.Focus();
                    txtExamMuscleBone2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMuscleBone2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMuscleBoneRank2.Focus();
                    cboExamMuscleBoneRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMuscleBoneRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamNeurological2.Focus();
                    txtExamNeurological2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamNeurological2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamNeurologicalRank2.Focus();
                    cboExamNeurologicalRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamNeurologicalRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamMental2.Focus();
                    txtExamMental2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamMental2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamMentalRank2.Focus();
                    cboExamMentalRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamMentalRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamSurgery2.Focus();
                    txtExamSurgery2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamSurgery2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamSurgeryRank2.Focus();
                    cboExamSurgeryRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamSurgeryRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamObstetric2.Focus();
                    txtExamObstetric2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamObstetric2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamObstetricRank2.Focus();
                    cboExamObstetricRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamObstetricRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEyeSightRight2.Focus();
                    txtExamEyeSightRight2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightRight2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightLeft2.Focus();
                    txtExamEyeSightLeft2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightLeft2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassRight2.Focus();
                    txtExamEyeSightGlassRight2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassRight2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeSightGlassLeft2.Focus();
                    txtExamEyeSightGlassLeft2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeSightGlassLeft2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEyeDisease2.Focus();
                    txtExamEyeDisease2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEyeDisease2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEyeRank2.Focus();
                    cboExamEyeRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEyeRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamEntLeftNormal2.Focus();
                    txtExamEntLeftNormal2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftNormal2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntLeftWhisper2.Focus();
                    txtExamEntLeftWhisper2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntLeftWhisper2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightNomal2.Focus();
                    txtExamEntRightNomal2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightNomal2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntRightWhisper2.Focus();
                    txtExamEntRightWhisper2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntRightWhisper2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamEntDisease2.Focus();
                    txtExamEntDisease2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamEntDisease2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamEntDiseaseRank2.Focus();
                    cboExamEntDiseaseRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamEntDiseaseRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamStomatologyUpper2.Focus();
                    txtExamStomatologyUpper2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyUpper2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyLower2.Focus();
                    txtExamStomatologyLower2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyLower2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExamStomatologyDisease2.Focus();
                    txtExamStomatologyDisease2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamStomatologyDisease2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamStomatologyRank2.Focus();
                    cboExamStomatologyRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamStomatologyRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExamDernatology2.Focus();
                    txtExamDernatology2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamDernatology2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExamDernatologyRank2.Focus();
                    cboExamDernatologyRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamDernatologyRank2_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtTestBloodHc2.Focus();
                    txtTestBloodHc2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodHc2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodBc2.Focus();
                    txtTestBloodBc2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodBc2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodTc2.Focus();
                    txtTestBloodTc2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodTc2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodGluco2.Focus();
                    txtTestBloodGluco2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodGluco2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodUre2.Focus();
                    txtTestBloodUre2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodUre2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodCreatinin2.Focus();
                    txtTestBloodCreatinin2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodCreatinin2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodAsat2.Focus();
                    txtTestBloodAsat2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodAsat2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodAlat2.Focus();
                    txtTestBloodAlat2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodAlat2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestBloodOther2.Focus();
                    txtTestBloodOther2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestBloodOther2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestUrineGluco2.Focus();
                    txtTestUrineGluco2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestUrineGluco2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestUrineProtein2.Focus();
                    txtTestUrineProtein2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestUrineProtein2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestUrineOther2.Focus();
                    txtTestUrineOther2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestUrineOther2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtResultDiim2.Focus();
                    txtResultDiim2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultDiim2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHealthExamRank2.Focus();
                    cboHealthExamRank2.ShowPopup();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHealthExamRank2_Closed(object sender, ClosedEventArgs e)
        {

            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDiseases2.Focus();
                    txtDiseases2.SelectAll();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiseases2_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        #endregion
    }
}
