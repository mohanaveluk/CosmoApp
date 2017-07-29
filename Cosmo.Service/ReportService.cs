using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Cosmo.Data;
using Cosmo.Entity;

namespace Cosmo.Service
{
    public class ReportService
    {
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];
        private readonly IReportData _reportData;

        public ReportService()
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            _reportData = new ReportDataFactory().Create(iDbType);
        }

        public List<DailyReportSubscriptionEntity> GetDailyReportSubscription(int envId)
        {
            return _reportData.GetDailyReportSubscription(envId);
        }

        public int InsertUpdateEmailSubscription(EmailSubscription emailSubscription)
        {
            return _reportData.InsertUpdateEmailSubscription(emailSubscription);
        }
    }
}
