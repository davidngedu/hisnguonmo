﻿using LIS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ADO
{
    public class TestLisResultADO : MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX
    {
        public TestLisResultADO()
        {

        }

        public TestLisResultADO(MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<TestLisResultADO>(this, data);
        }
        public short? BACTERIAL_CULTIVATION_RESULT { get; set; }
        public string MIC { get; set; }
        public string SRI_CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string SUMMARY_CODE { get; set; }

        public long? ANTIBIOTIC_ID { get; set; }
        public string ANTIBIOTIC_CODE { get; set; }
        public string ANTIBIOTIC_NAME { get; set; }
        public long? BACTERIUM_ID { get; set; }
        public long? BACTERIUM_FAMILY_ID { get; set; }
        public string BACTERIUM_CODE { get; set; }
        public string BACTERIUM_FAMILY_CODE { get; set; }
        public string BACTERIUM_FAMILY_NAME { get; set; }
        public string BACTERIUM_NAME { get; set; }

        public long IS_PARENT { get; set; }
        public long HAS_ONE_CHILD { get; set; }
        public string VALUE_RANGE { get; set; }
        public long? LEVEL { get; set; }
        public string SAMPLE_SERVICE_STT_CODE { get; set; }
        public long? SAMPLE_SERVICE_STT_ID { get; set; }
        public string SAMPLE_SERVICE_STT_NAME { get; set; }
        public long? SAMPLE_ID { get; set; }
        public long? SAMPLE_STT_ID { get; set; }
        public long? LIS_RESULT_ID { get; set; }
        public long? SAMPLE_SERVICE_ID { get; set; }
        public short? Item_Edit_Value { get; set; }
        public long? MACHINE_ID { get; set; }
        public long? MACHINE_ID_OLD { get; set; }
        public string PARENT_ID { get; set; }
        public string CHILD_ID { get; set; }
        public decimal? MIN_VALUE { get; set; }
        public decimal? MAX_VALUE { get; set; }
        public string NOTE { get; set; }
        public short? IS_RETURN_RESULT { get; set; }
        public decimal? VALUE { get; set; }
        public bool? IS_NORMAL { get; set; }
        public bool? IS_LOWER { get; set; }
        public bool? IS_HIGHER { get; set; }
        public short? IS_NO_EXECUTE { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }
        public decimal? APPOINTMENT_TIME { get; set; }
        public string OLD_VALUE { get; set; }
        public short? IS_RUN_AGAIN { get; set; }
        public short? IS_RUNNING { get; set; }
        public bool? RERUN { get; set; }
        public string MICROBIOLOGICAL_RESULT { get; set; }
        public long? ANTIBIOTIC_ORDER { get; set; }
        public string LABORATORY_CODE { get; set; }
        public long? TECHNIQUE_ID { get; set; }
        public string ErrorMessageLabCode { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeLabCode{ get; set; }


        public List<LisResultADO> LResultDetails { get; set; }

    }
}
