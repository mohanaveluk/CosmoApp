using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public interface ISchedulerData
    {
        void InsertUpdateScheduler(SchedulerEntity scheduler);
        void InsertUpdateMultipleScheduler(SchedulerEntity scheduler);
        List<SchedulerEntity> GetSchedulerDetails(int envID, int configID);

    }

    public interface ISchedulerDataFactory
    {
        ISchedulerData Create(string dbType);
    }

    public class SchedulerDataFactory : ISchedulerDataFactory
    {
        public ISchedulerData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
               ? (ISchedulerData)new SchedulerDataOrcl()
               : new SchedulerData(); 
        }
    }
}
