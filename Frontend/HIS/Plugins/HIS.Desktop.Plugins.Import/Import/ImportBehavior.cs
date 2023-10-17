using Inventec.Core;
using Inventec.Desktop.Common;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.Import.FormLoad;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.Import
{
    class ImportBehavior : BusinessBase, IImport
    {
        object[] entity;
        //DelegateRefreshData delegateRefresh = null;

        internal ImportBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IImport.Run()
        {
            object frm = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                ImportADO ado;
                RefeshReference refeshReference = null;
                List<ServiceImportADO> service = null;
                List<MaterialTypeImportADO> material = null;
                List<MedicinePatyImportADO> medicinePaty = null;
                List<MaterialPatyImportADO> materialPaty = null;
                List<MedicineTypeImportADO> medicine = null;
                List<ServicePatyImportADO> servicePaty = null;
                //List<ServiceImportADO> service = null;
                //List<ServiceImportADO> service = null;


                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is RefeshReference)
                            {
                                refeshReference = (RefeshReference)entity[i];
                            }
                            if (entity[i] is ImportADO)
                            {
                                ado = (ImportADO)entity[i];
                                if (ado != null)
                                {
                                    service = ado.serviceAdos;
                                    material = ado.materialAdos;
                                    medicine = ado.medicineAdos;
                                    materialPaty = ado.materialPatyAdos;
                                    medicinePaty = ado.medicinePatyAdos;
                                    servicePaty = ado.servicePatyAdos;
                                }
                            }
                        }
                    }
                }

                if (service != null)
                {
                    frm = new frmService(service, refeshReference);
                }
                else if (material != null)
                {
                    frm = new frmMaterialType(material, refeshReference);
                }
                else if (medicine != null)
                {
                    frm = new frmMedicineType(medicine, refeshReference);
                }
                else if (materialPaty != null)
                {
                    frm = new frmMaterialPaty(materialPaty, refeshReference);
                }
                else if (medicinePaty != null)
                {
                    frm = new frmMedicinePaty(medicinePaty, refeshReference);
                }
                else if (servicePaty != null)
                {
                    frm = new frmServicePaty(servicePaty, refeshReference);
                }
                return frm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
