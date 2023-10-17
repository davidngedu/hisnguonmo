﻿using HIS.Desktop.XmlCacheMonitor;
using Inventec.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Library.CacheClient
{
    public class SqliteWorker
    {
        internal SqliteDataBaseCreate SqliteDataBaseCreate { get { return (SqliteDataBaseCreate)Worker.Get<SqliteDataBaseCreate>(); } }
        internal SqliteGet SqliteDBGet { get { return (SqliteGet)Worker.Get<SqliteGet>(); } }
        internal SqliteSet SqliteSet { get { return (SqliteSet)Worker.Get<SqliteSet>(); } }

        public SqliteWorker() { }

        internal bool ValidTable<T>(string dataKey)
        {
            return SqliteDBGet.ValidTable<T>(dataKey);
        }

        internal List<T> Get<T>(string dataKey, string where)
        {
            List<T> rs = null;
            try
            {
                rs = SqliteDBGet.Get<T>(where);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return rs;
        }

        internal void Set<T>(List<T> value, string dataKey)
        {
            Set<T>(value, dataKey, true);
        }

        internal void Set<T>(List<T> value, string dataKey, bool isWaitingSync)
        {
            try
            {
                if (value == null || value.Count == 0) throw new ArgumentNullException("value is null");

                SqliteSet.Create<T>(value, isWaitingSync);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal bool Delete(string dataKey, string condition)
        {
            bool success = false;
            try
            {
                string tableName = dataKey.Substring(dataKey.LastIndexOf(".") + 1);
                success = SQLiteDatabaseWorker.SQLiteDatabase.Delete(tableName, (String.IsNullOrEmpty(condition) ? "1 = 1" : condition));
                Inventec.Common.Logging.LogSystem.Info("SqliteWorker.Delete " + success + ". table = " + tableName);
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey));
                Inventec.Common.Logging.LogSystem.Warn(ex);
                success = false;
            }

            return success;
        }

        internal void InitDataCacheLocal()
        {
            SqliteDataBaseCreate.InitDataCacheLocal();
        }
    }
}
