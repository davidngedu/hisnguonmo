using ACS.DAO.Base;
using ACS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace ACS.DAO.StagingObject
{
    public class AcsActivityTypeSO : StagingObjectBase
    {
        public AcsActivityTypeSO()
        {
            
        }

        public List<System.Linq.Expressions.Expression<Func<ACS_ACTIVITY_TYPE, bool>>> listAcsActivityTypeExpression = new List<System.Linq.Expressions.Expression<Func<ACS_ACTIVITY_TYPE, bool>>>();
    }
}
