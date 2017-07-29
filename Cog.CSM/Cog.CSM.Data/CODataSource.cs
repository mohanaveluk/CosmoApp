using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRW;

namespace Cog.CSM.Data
{
    public enum DataSourceType
    {
        OracleDatabase,
        SqlServerDatabase,
        WebService,
        XmlFile
    }
    public class CODataSource
    {
        #region constants

        private const string DataSourceConfigSetting = "DataSource";
        protected const string DataSourceFilenameSetting = "FilePath";
        protected const string DataSourceConnectionSetting = "ConnectionString";

        #endregion

        #region instance variables

        protected DataSourceType _dataSourceType = DataSourceType.XmlFile;

        #endregion

        #region properties

        /// <summary>
        /// Get or sets the type of data source this is
        /// </summary>
        public DataSourceType DAOType
        {
            get { return _dataSourceType; }
            set { _dataSourceType = value; }
        }

        #endregion

        /// <summary>
        /// The base constructor reads the data source type from the 
        /// configuration file
        /// </summary>
        public CODataSource()
        {
            string configDataSource = ConfigurationUtility.GetConfigValue(this.GetType(), DataSourceConfigSetting, false);

            if (!string.IsNullOrEmpty(configDataSource))
            {
                DAOType = (DataSourceType)Enum.Parse(typeof(DataSourceType), configDataSource);
            }
        }
    }
}
