using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public interface IReportData
    {
        List<DailyReportSubscriptionEntity> GetDailyReportSubscription(int envId);
        int InsertUpdateEmailSubscription(EmailSubscription emailSubscription);
    }

    public interface IReportDataFactory
    {
        IReportData Create(string dbType);
    }

    public class ReportDataFactory : IReportDataFactory
    {
        public IReportData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (IReportData)new ReportDataOrcl()
                : new ReportData();
        }
    }
}
