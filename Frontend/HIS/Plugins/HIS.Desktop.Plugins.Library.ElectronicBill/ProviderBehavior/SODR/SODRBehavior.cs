﻿using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using Inventec.Common.EBillSoftDreams.Model;
using Inventec.Common.EBillSoftDreams.ModelXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SODR
{
    class SODRBehavior : IRun
    {
        string serviceConfig { get; set; }
        string accountConfig { get; set; }
        ElectronicBillDataInput ElectronicBillDataInput { get; set; }
        TemplateEnum.TYPE TempType { get; set; }

        public SODRBehavior(ElectronicBillDataInput _electronicBillDataInput, string _serviceConfig, string _accountConfig)
            : base()
        {
            this.serviceConfig = _serviceConfig;
            this.ElectronicBillDataInput = _electronicBillDataInput;
            this.accountConfig = _accountConfig;
        }

        ElectronicBillResult IRun.Run(ElectronicBillType.ENUM _electronicBillTypeEnum, TemplateEnum.TYPE _tempType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (this.Check(ref result))
                {
                    this.TempType = _tempType;

                    string[] configArr = serviceConfig.Split('|');

                    string serviceUrl = configArr[1]; //ConfigurationManager.AppSettings[AppConfigKey.WEBSERVICE_URL];
                    if (String.IsNullOrEmpty(serviceUrl))
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay dia chi Webservice URL");
                        ElectronicBillResultUtil.Set(ref result, false, "Không tìm thấy địa chỉ Webservice URL");
                        return result;
                    }

                    string[] accountConfigArr = accountConfig.Split('|');

                    Inventec.Common.EBillSoftDreams.DataInitApi login = new Inventec.Common.EBillSoftDreams.DataInitApi();
                    login.Address = serviceUrl;
                    login.User = accountConfigArr[0].Trim();
                    login.Pass = accountConfigArr[1].Trim();

                    switch (_electronicBillTypeEnum)
                    {
                        case ElectronicBillType.ENUM.CREATE_INVOICE:
                            ProcessCreateInvoice(login, ref result);
                            break;
                        case ElectronicBillType.ENUM.GET_INVOICE_LINK:
                            //ProcessGetInvoice(login, ref result);
                            break;
                        case ElectronicBillType.ENUM.DELETE_INVOICE:
                        case ElectronicBillType.ENUM.CANCEL_INVOICE:
                            //ProcessCancelInvoice(login, ref result);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool Check(ref ElectronicBillResult electronicBillResult)
        {
            bool result = true;
            try
            {
                string[] configArr = serviceConfig.Split('|');
                if (configArr.Length != 2)
                    throw new Exception("Sai định dạng cấu hình hệ thống.");

                string[] accountArr = accountConfig.Split('|');
                if (accountArr.Length != 2)
                    throw new Exception("Sai định dạng cấu hình tài khoản.");
            }
            catch (Exception ex)
            {
                result = false;
                ElectronicBillResultUtil.Set(ref electronicBillResult, false, "");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessCreateInvoice(Inventec.Common.EBillSoftDreams.DataInitApi login, ref ElectronicBillResult result)
        {
            try
            {
                InvCreate invoices = this.GetInvoice(this.ElectronicBillDataInput);
                Inventec.Common.EBillSoftDreams.Response response = null;
                Inventec.Common.Logging.LogSystem.Info("SendData" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => invoices), invoices));

                var eMoit = new Inventec.Common.EBillSoftDreams.EBillSoftDreamsManager(login);
                if (eMoit != null)
                {
                    response = eMoit.Run(invoices);
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => response), response));

                if (response != null && response.Success)
                {
                    //Thanh cong
                    result.InvoiceCode = response.Ikey;
                    result.InvoiceNumOrder = response.invoiceNo;
                    result.InvoiceTime = Inventec.Common.DateTime.Get.Now();
                    result.InvoiceLoginname = login.User;
                }

                result.InvoiceSys = ProviderType.SoftDream;
                ElectronicBillResultUtil.Set(ref result, response.Success, response.Messages);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private InvCreate GetInvoice(Base.ElectronicBillDataInput electronicBillDataInput)
        {
            InvCreate result = new InvCreate();
            try
            {
                if (electronicBillDataInput != null)
                {
                    InvCreate invoice = new InvCreate();

                    invoice.Pattern = electronicBillDataInput.TemplateCode;
                    invoice.Serial = electronicBillDataInput.SymbolCode;
                    invoice.Inv = GetDataInv(electronicBillDataInput);

                    result = invoice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private Inventec.Common.EBillSoftDreams.ModelXml.Inv GetDataInv(Base.ElectronicBillDataInput electronicBillDataInput)
        {
            Inventec.Common.EBillSoftDreams.ModelXml.Inv result = new Inventec.Common.EBillSoftDreams.ModelXml.Inv();
            try
            {
                Invoice invoice = new Invoice();
                if (electronicBillDataInput != null)
                {
                    string key = "";
                    if (ElectronicBillDataInput.Transaction != null)
                    {
                        key = ElectronicBillDataInput.Transaction.TRANSACTION_CODE;
                    }
                    else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                    {
                        key = ElectronicBillDataInput.ListTransaction.Select(s => s.TRANSACTION_CODE).OrderBy(o => o).FirstOrDefault();
                    }
                    else
                    {
                        string time = electronicBillDataInput.TransactionTime > 0 ? electronicBillDataInput.TransactionTime.ToString() : Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now).ToString();
                        key = String.Format("{0}-{1}", time, Guid.NewGuid().ToString("N"));
                        if (key.Length > 20) key = key.Substring(0, 20);
                    }

                    invoice.Ikey = key;

                    invoice.ArisingDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(electronicBillDataInput.TransactionTime);

                    string paymentMethod = "T/M";
                    if (electronicBillDataInput.Transaction != null)
                    {
                        if (electronicBillDataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK)
                        {
                            paymentMethod = "C/K";
                        }
                        else if (electronicBillDataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                        {
                            paymentMethod = "TT/D";
                        }
                        else if (electronicBillDataInput.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                        {
                            paymentMethod = "TM/CK";
                        }
                    }
                    else if (electronicBillDataInput.ListTransaction != null && electronicBillDataInput.ListTransaction.Count > 0)
                    {
                        if (electronicBillDataInput.ListTransaction.First().PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK)
                        {
                            paymentMethod = "C/K";
                        }
                        else if (electronicBillDataInput.ListTransaction.First().PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                        {
                            paymentMethod = "TT/D";
                        }
                        else if (electronicBillDataInput.ListTransaction.First().PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                        {
                            paymentMethod = "TM/CK";
                        }
                    }

                    invoice.PaymentMethod = paymentMethod;

                    InvoiceInfo.InvoiceInfoADO adoInfo = InvoiceInfo.InvoiceInfoProcessor.GetData(electronicBillDataInput);

                    invoice.CusCode = adoInfo.BuyerCode;
                    invoice.CusAddress = adoInfo.BuyerAddress ?? " ";
                    invoice.CusName = adoInfo.BuyerName;
                    invoice.CusTaxCode = adoInfo.BuyerTaxCode;
                    invoice.CusPhone = adoInfo.BuyerPhone;

                    string cusName = adoInfo.BuyerName;

                    if (Config.HisConfigCFG.IsSwapNameOption)
                    {
                        invoice.Buyer = cusName;
                        invoice.CusName = "";
                    }
                    else
                    {
                        invoice.Buyer = "";
                        invoice.CusName = cusName;
                    }

                    invoice.Total = Math.Round(electronicBillDataInput.Amount ?? 0, 0);
                    invoice.VATRate = -1;
                    invoice.VATAmount = 0;
                    invoice.Amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(electronicBillDataInput.Amount ?? 0);
                    invoice.AmountInWords = Inventec.Common.String.Convert.CurrencyToVneseString(String.Format("{0:0.##}", invoice.Total)) + "đồng";

                    invoice.Products = this.GetProductElectronicBill();

                    //•	Chiết khấu được hiển thị như 1 sản phẩm riêng biệt kế tiếp sản phẩm tương ứng với nó, khi đó ProdName thể hiện là dòng chiết khấu. Các trường Total, VATAmount, Amount mang giá trị âm.
                    if (electronicBillDataInput.Discount.HasValue)
                    {
                        Product discount = new Product();
                        discount.ProdName = "Chiết khấu";
                        discount.ProdPrice -= electronicBillDataInput.Discount.Value;
                        discount.Amount -= electronicBillDataInput.Discount.Value;
                        discount.Total -= electronicBillDataInput.Discount.Value;
                        discount.Extra = "{\"Pos\":\"\"}";
                        invoice.Products.Add(discount);
                    }
                }

                result.Invoice = invoice;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<Product> GetProductElectronicBill()
        {
            List<Product> result = new List<Product>();
            try
            {
                IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(this.TempType, ElectronicBillDataInput);
                var listProduct = iRunTemplate.Run();
                if (listProduct == null)
                {
                    throw new Exception("Loi phan tich listProductBase");
                }

                if (this.TempType != TemplateEnum.TYPE.TemplateNhaThuoc)
                {
                    List<ProductBase> listProductBase = (List<ProductBase>)listProduct;
                    if (listProductBase == null || listProductBase.Count == 0)
                    {
                        throw new Exception("Loi phan tich listProductBase");
                    }

                    foreach (var item in listProductBase)
                    {
                        Product product = new Product();
                        product.ProdName = item.ProdName;
                        product.ProdUnit = item.ProdUnit;
                        product.ProdQuantity = item.ProdQuantity;
                        product.Amount = item.Amount;//Tổng tiền
                        product.Total = item.Amount;//Tổng tiền trước thuế
                        product.ProdPrice = item.ProdPrice;

                        result.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
