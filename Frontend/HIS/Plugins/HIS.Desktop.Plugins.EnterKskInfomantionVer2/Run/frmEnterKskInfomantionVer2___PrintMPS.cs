﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{

    public partial class frmEnterKskInfomantionVer2
    {
        private enum PRINT_TYPE
        {
            MPS000315,
            MPS000452,
            MPS000453,
            MPS000454,
            MPS000455,
            MPS000464
        }
        private void PrintProcess(PRINT_TYPE printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                switch (printType)
                {
                    case PRINT_TYPE.MPS000315:
                        richEditorMain.RunPrintTemplate("Mps000315", DelegateRunPrinter);
                        break;
                    case PRINT_TYPE.MPS000452:
                        richEditorMain.RunPrintTemplate("Mps000452", DelegateRunPrinter);
                        break;
                    case PRINT_TYPE.MPS000453:
                        richEditorMain.RunPrintTemplate("Mps000453", DelegateRunPrinter);
                        break;
                    case PRINT_TYPE.MPS000454:
                        richEditorMain.RunPrintTemplate("Mps000454", DelegateRunPrinter);
                        break;
                    case PRINT_TYPE.MPS000455:
                        richEditorMain.RunPrintTemplate("Mps000455", DelegateRunPrinter);
                        break;
                    case PRINT_TYPE.MPS000464:
                        richEditorMain.RunPrintTemplate("Mps000464", DelegateRunPrinter);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {

                switch (printTypeCode)
                {
                    case "Mps000315":
                        LoadBieuMauPhieuMps000315(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000452":
                        LoadBieuMauPhieuMps000452(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000453":
                        LoadBieuMauPhieuMps000453(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000454":
                        LoadBieuMauPhieuMps000454(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000455":
                        LoadBieuMauPhieuMps000455(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000464":
                        LoadBieuMauPhieuMps000464(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadBieuMauPhieuMps000315(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                List<V_HIS_DHST> currentDhst = new List<V_HIS_DHST>();
                if (currentKskGeneral.DHST_ID != null && currentKskGeneral.DHST_ID > 0)
                {
                    MOS.Filter.HisDhstFilter filter = new MOS.Filter.HisDhstFilter();
                    filter.ID = currentKskGeneral.DHST_ID;
                    currentDhst = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_DHST>>("api/HisDhst/GetView", ApiConsumers.MosConsumer, filter, param);

                }
                WaitingManager.Hide();
                MPS.Processor.Mps000315.PDO.Mps000315PDO rdo = new MPS.Processor.Mps000315.PDO.Mps000315PDO(
                    new List<HIS_KSK_GENERAL>() { currentKskGeneral },
                      new List<V_HIS_SERVICE_REQ>() { currentServiceReq },
                    currentDhst,
                    HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList()

                    );

                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBieuMauPhieuMps000452(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                HIS_DHST currentDhst = new HIS_DHST();
                if (currentKskOverEight.DHST_ID != null && currentKskOverEight.DHST_ID > 0)
                {
                    MOS.Filter.HisDhstFilter filter = new MOS.Filter.HisDhstFilter();
                    filter.ID = currentKskOverEight.DHST_ID;
                    var dt = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dt != null && dt.Count > 0) currentDhst = dt.FirstOrDefault();
                }
                WaitingManager.Hide();
                MPS.Processor.Mps000452.PDO.Mps000452PDO rdo = new MPS.Processor.Mps000452.PDO.Mps000452PDO(
                    currentKskOverEight,
                    currentServiceReq,
                    currentDhst,
                    HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList()

                    );

                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBieuMauPhieuMps000453(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                try
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    HIS_DHST currentDhst = new HIS_DHST();
                    if (currentKskUnderEight.DHST_ID != null && currentKskUnderEight.DHST_ID > 0)
                    {
                        MOS.Filter.HisDhstFilter filter = new MOS.Filter.HisDhstFilter();
                        filter.ID = currentKskUnderEight.DHST_ID;
                        var dt = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                        if (dt != null && dt.Count > 0) currentDhst = dt.FirstOrDefault();
                    }
                    List<HIS_KSK_UNEI_VATY> lstUneiVaty = new List<HIS_KSK_UNEI_VATY>();
                    List<HIS_VACCINE_TYPE> lstVaccinType = new List<HIS_VACCINE_TYPE>();
                    HisKskUneiVatyFilter vatyfilter = new HisKskUneiVatyFilter();
                    vatyfilter.KSK_UNDER_EIGHTEEN_ID = currentKskUnderEight.ID;
                    lstUneiVaty = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_UNEI_VATY>>("api/HisKskUneiVaty/Get", ApiConsumers.MosConsumer, vatyfilter, param);


                    if (lstUneiVaty != null && lstUneiVaty.Count > 0)
                    {
                        HisVaccineTypeFilter filter = new HisVaccineTypeFilter();
                        filter.IDs = lstUneiVaty.Select(o => o.VACCINE_TYPE_ID).ToList();
                        lstVaccinType = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_VACCINE_TYPE>>("api/HisVaccineType/Get", ApiConsumers.MosConsumer, filter, param);


                    }
                    WaitingManager.Hide();
                    MPS.Processor.Mps000453.PDO.Mps000453PDO rdo = new MPS.Processor.Mps000453.PDO.Mps000453PDO(
                        currentKskUnderEight,
                        currentServiceReq,
                        currentDhst,
                        lstVaccinType,
                        lstUneiVaty,
                        HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList()

                        );

                    PrintData(printTypeCode, fileName, rdo, ref result);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBieuMauPhieuMps000454(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                CommonParam param = new CommonParam();
                WaitingManager.Show();
                HIS_DHST currentDhst = new HIS_DHST();
                List<HIS_PERIOD_DRIVER_DITY> lstDriverDity = new List<HIS_PERIOD_DRIVER_DITY>();
                List<HIS_DISEASE_TYPE> lstDiseaseType = new List<HIS_DISEASE_TYPE>();
                HisPeriodDriverDityFilter dityFilter = new HisPeriodDriverDityFilter();
                dityFilter.KSK_PERIOD_DRIVER_ID = currentKskPeriodDriver.ID;
                lstDriverDity = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PERIOD_DRIVER_DITY>>("api/HisPeriodDriverDity/Get", ApiConsumers.MosConsumer, dityFilter, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstDriverDity), lstDriverDity));
                if (lstDriverDity != null && lstDriverDity.Count > 0)
                {
                    HisDiseaseTypeFilter Disfilter = new HisDiseaseTypeFilter();
                    Disfilter.IS_ACTIVE = 1;
                    Disfilter.IDs = lstDriverDity.Select(o => o.DISEASE_TYPE_ID).ToList();
                    lstDiseaseType = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DISEASE_TYPE>>("api/HisDiseaseType/Get", ApiConsumers.MosConsumer, Disfilter, param);

                }

                WaitingManager.Hide();
                MPS.Processor.Mps000454.PDO.Mps000454PDO rdo = new MPS.Processor.Mps000454.PDO.Mps000454PDO(
                    currentKskPeriodDriver,
                    currentServiceReq,
                    lstDiseaseType,
                    lstDriverDity,
                    HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList()

                    );

                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBieuMauPhieuMps000455(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                MPS.Processor.Mps000455.PDO.Mps000455PDO rdo = new MPS.Processor.Mps000455.PDO.Mps000455PDO(
                    currentKskDriverCar,
                    currentServiceReq,
                    HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList()

                    );
                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBieuMauPhieuMps000464(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                MPS.Processor.Mps000464.PDO.Mps000464PDO rdo = new MPS.Processor.Mps000464.PDO.Mps000464PDO(
                    this.currentServiceReq,
                    this.currentKskOther
                    );
                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintData(string printTypeCode, string fileName, object data, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(currentServiceReq != null ? currentServiceReq.TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
