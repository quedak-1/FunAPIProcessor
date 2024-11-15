using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace ScaleThreadProcess
{
    class SQLConnectClass : IDisposable
    {
        //public clsUtility _util { get; set; }

        //public clsSQLConnect(clsUtility util)
        //{
        //    _util = util;
        //}

        private string MSSQL_CONNECTION = CommonHelper.connectionString;

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public SqlConnection GetOpenConnection(bool blnSetContext = false)
        {
            SqlConnection objConnection = new SqlConnection();
            try
            {
                //Set (cache) the connection string
                objConnection.ConnectionString = GetConnectionString();

                // Open connection
                objConnection.Open();

                if (blnSetContext)
                {
                    try
                    {
                        string strUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        if (strUserName.Length == 0) { strUserName = "NO_USER"; }
                        if (strUserName.IndexOf("/") > -1) { strUserName = strUserName.Split('/')[1]; }

                        using (SqlCommand objCommand = objConnection.CreateCommand())
                        {
                            objCommand.CommandText = "WJ_COMMON_PKG$set_authenticated_user";
                            objCommand.CommandType = CommandType.StoredProcedure;
                            objCommand.Parameters.Add(new SqlParameter("webuser", SqlDbType.VarChar, 100, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, strUserName));
                            objCommand.ExecuteNonQuery();
                            objCommand.Parameters.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("SetAuthenticatedUser", ex);
                    }
                }

            }
            catch (Exception ex)
            {
                //Release connection resources
                if (objConnection.State == ConnectionState.Open) { objConnection.Close(); }

                //Report application error
                throw new ApplicationException("GetOpenConnection", ex);
            }

            return objConnection;
        }

        protected string BuildConnectionString(string strEnvironment, string connectionKey = "")
        {
            // The default connection key can be overriden
            string sConnectionStringKey = connectionKey;

            //Read app configuration settings        
            if (String.IsNullOrEmpty(connectionKey))
                //Default value
                sConnectionStringKey = "CONNECTION_STRING_KEY";
            else
            {
                sConnectionStringKey = connectionKey;
                sConnectionStringKey = ConfigurationManager.AppSettings[sConnectionStringKey];
            }

            if (string.IsNullOrWhiteSpace(sConnectionStringKey)) { throw new ApplicationException("Missing Connection String Key Configuration."); }

            if (ConfigurationManager.ConnectionStrings[sConnectionStringKey] == null)
            {
                sConnectionStringKey = sConnectionStringKey + "_" + strEnvironment;//NFA.SqlServer.SqlServerConnect.getEnvironment();
                if (ConfigurationManager.ConnectionStrings[sConnectionStringKey] == null) { throw new ApplicationException("Missing connection string template."); }
            }

            //Read connection string from the <connectionStrings> section
            return ConfigurationManager.ConnectionStrings[sConnectionStringKey].ConnectionString;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public string GetConnectionString(string connectionKey = "")
        {
            try
            {
                // Profile Connection override
                if (!String.IsNullOrEmpty(CommonHelper.connectionString) & CommonHelper.connectionType == WorkerDbType.MSSQL.ToString()) return CommonHelper.connectionString;

                string strEnvironment = CommonHelper.GetEnvironment();

                if (!string.IsNullOrWhiteSpace(MSSQL_CONNECTION)) { return MSSQL_CONNECTION; }

                lock (MSSQL_CONNECTION)
                {
                    MSSQL_CONNECTION = BuildConnectionString(strEnvironment, connectionKey);
                    return MSSQL_CONNECTION;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("GetConnectionString", ex);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}
