﻿using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ContentSubclinical.Print
{
    class Print
    {
        internal static void PrintData(string printTypeCode, string fileName, object data, bool printNow,
            ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType,
            int count, Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Method PrintData: " + printTypeCode);
                string printerName = "";
                WaitingManager.Hide();
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.Logging.LogSystem.Debug("PrintData____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PreviewType), PreviewType)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printNow), printNow)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isView), isView));

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(EmrDataStore.treatmentCode, printTypeCode, roomId);
                Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo ado = null;
                if (isView)
                {
                    if (Config.Config.IsmergePrint)
                    {
                        MPS.ProcessorBase.Core.PrintData printD = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                        using (printD.saveMemoryStream = new System.IO.MemoryStream())
                        {
                            printD.EmrInputADO = inputADO;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printD), printD));
                            result = MPS.MpsPrinter.Run(printD);
                            if (result)
                            {
                                ado = new Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo();
                                ado.data = printD.data;
                                ado.EmrInputADO = printD.EmrInputADO;
                                ado.eventLog = printD.eventLog;
                                ado.eventPrint = printD.eventPrint;
                                ado.fileName = printD.fileName;
                                ado.isAllowExport = printD.isAllowExport;
                                ado.numCopy = printD.numCopy;
                                ado.printerName = printD.printerName;
                                ado.printTypeCode = printD.printTypeCode;
                                ado.saveFilePath = printD.saveFilePath;
                                ado.ShowTutorialModule = printD.ShowTutorialModule;
                                ado.IsAllowEditTemplateFile = true;// printD.IsAllowEditTemplateFile;
                                                                   //ado.TemplateKey = printD.TemplateKey;
                                                                   //ado.ActShowPrintLog = printD.ActShowPrintLog;
                                                                   //ado.PrintLog = printD.PrintLog;
                                                                   //ado.IsSingleCopy = printD.IsSingleCopy;
                                                                   //ado.ShowPrintLog = printD.ShowPrintLog;
                                if (printD.saveMemoryStream != null)
                                {
                                    printD.saveMemoryStream.Position = 0;
                                    ado.saveMemoryStream = new System.IO.MemoryStream();
                                    printD.saveMemoryStream.CopyTo(ado.saveMemoryStream);
                                }
                            }
                        }

                    }
                    else if (PreviewType != null)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, PreviewType ?? MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    if (PreviewType == MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow
                        || PreviewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow
                        || PreviewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, PreviewType ?? MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName) { EmrInputADO = inputADO });
                    }
                    else if (printNow || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else if (Config.Config.IsmergePrint)
                    {
                        MPS.ProcessorBase.Core.PrintData printD = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                        using (printD.saveMemoryStream = new System.IO.MemoryStream())
                        {
                            printD.EmrInputADO = inputADO;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printD), printD));
                            result = MPS.MpsPrinter.Run(printD);
                            if (result)
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("print out data EmrInputADO: ", printD.EmrInputADO));
                                ado = new Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo();
                                ado.data = printD.data;
                                ado.EmrInputADO = printD.EmrInputADO;
                                ado.eventLog = printD.eventLog;
                                ado.eventPrint = printD.eventPrint;
                                ado.fileName = printD.fileName;
                                ado.isAllowExport = printD.isAllowExport;
                                ado.numCopy = printD.numCopy;
                                ado.printerName = printD.printerName;
                                ado.printTypeCode = printD.printTypeCode;
                                ado.saveFilePath = printD.saveFilePath;
                                ado.ShowTutorialModule = printD.ShowTutorialModule;
                                ado.IsAllowEditTemplateFile = true;// printD.IsAllowEditTemplateFile;
                                                                   //ado.TemplateKey = printD.TemplateKey;
                                                                   //ado.ActShowPrintLog = printD.ActShowPrintLog;
                                                                   //ado.PrintLog = printD.PrintLog;
                                                                   //ado.IsSingleCopy = printD.IsSingleCopy;
                                                                   //ado.ShowPrintLog = printD.ShowPrintLog;
                                if (printD.saveMemoryStream != null)
                                {
                                    printD.saveMemoryStream.Position = 0;
                                    ado.saveMemoryStream = new System.IO.MemoryStream();
                                    printD.saveMemoryStream.CopyTo(ado.saveMemoryStream);
                                }
                            }
                        }
                    }
                    else if (PreviewType != null)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, PreviewType ?? MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }

                savedData(count, ado);
            }
            catch (Exception ex)
            {
                savedData(count, null);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
