using System;
using System.Data;
using System.Data.SqlClient;

namespace ScaleThreadProcess.Data
{
    public partial class DataClass : IDisposable
    {

        #region "SQL Methods"
        public string SQLConnectionString(string connectionKey = "")
        {
            string strConn;

            using (SQLConnectClass clsSQLConnect = new SQLConnectClass())
            {
                strConn = clsSQLConnect.GetConnectionString(connectionKey);
            }

            return strConn;
        }
        public object SQL_ExecuteScalar(string strSQL, string connectionKey = "")
        {
            object oValue;

            using (SqlConnection oConnection = new SqlConnection(SQLConnectionString(connectionKey)))
            {
                if (CommonHelper.isImporsonateAccount)
                {
                    CommonHelper.RunImpersonated(Environment.UserDomainName, CommonHelper.serviceAccountId, CommonHelper.serviceAccountKey, () =>
                    {
                        oConnection.Open();
                    });
                }
                else oConnection.Open();

                using (SqlCommand oCommand = new SqlCommand())
                {
                    oCommand.Connection = oConnection;
                    oCommand.CommandText = strSQL;

                    oValue = oCommand.ExecuteScalar();
                }
            }

            return oValue;
        }
        public void SQL_ExecuteNonQuery(string strSql, string connectionKey = "")
        {
            using (SqlConnection oConnection = new SqlConnection(SQLConnectionString(connectionKey)))
            {
                if (CommonHelper.isImporsonateAccount)
                {
                    CommonHelper.RunImpersonated(Environment.UserDomainName, CommonHelper.serviceAccountId, CommonHelper.serviceAccountKey, () =>
                    {
                        oConnection.Open();
                    });
                }
                else oConnection.Open();

                using (SqlCommand oCmd = new SqlCommand())
                {
                    oCmd.Connection = oConnection;
                    oCmd.CommandText = strSql;
                    oCmd.CommandType = CommandType.Text;
                    oCmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable SQL_GetDataTable(string strSql, string connectionKey = "", string decryptKey = null)
        {
            DataTable dt = new DataTable();
            string strExecuteSql = strSql;

            if (CommonHelper.isImporsonateAccount)
            {
                CommonHelper.RunImpersonated(Environment.UserDomainName, CommonHelper.serviceAccountId, CommonHelper.serviceAccountKey, () =>
                {

                    using (SqlConnection oConnection = new SqlConnection(SQLConnectionString(connectionKey)))
                    {
                        oConnection.Open();

                        using (SqlTransaction transaction = oConnection.BeginTransaction())
                        {

                            dt = new DataTable();
                            try
                            {
                                using (SqlCommand oCmd = new SqlCommand())
                                {
                                    oCmd.Connection = oConnection;
                                    oCmd.Transaction = transaction;

                                    if (!String.IsNullOrEmpty(decryptKey))
                                    {

                                        if (decryptKey == "BROKERAGE_ACCOUNT_NUMBER_SYM_KEY")
                                        {
                                            oCmd.CommandType = CommandType.Text;
                                            oCmd.CommandText = "OPEN SYMMETRIC KEY BROKERAGE_ACCOUNT_NUMBER_SYM_KEY DECRYPTION BY CERTIFICATE PROHIBITED_TRADE_CERT";
                                            oCmd.ExecuteNonQuery();
                                        }

                                    }

                                    oCmd.CommandText = strExecuteSql;
                                    oCmd.CommandType = CommandType.Text;

                                    using (SqlDataAdapter oDa = new SqlDataAdapter(oCmd))
                                    {
                                        oDa.Fill(dt);
                                    }

                                    if (!String.IsNullOrEmpty(decryptKey))
                                    {
                                        if (decryptKey == "BROKERAGE_ACCOUNT_NUMBER_SYM_KEY")
                                        {
                                            oCmd.CommandType = CommandType.Text;
                                            oCmd.CommandText = "CLOSE SYMMETRIC KEY BROKERAGE_ACCOUNT_NUMBER_SYM_KEY";
                                            oCmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                transaction.Commit();

                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }
                    }
                });
            }
            else
            {
                using (SqlConnection oConnection = new SqlConnection(SQLConnectionString(connectionKey)))
                {
                    oConnection.Open();

                    using (SqlTransaction transaction = oConnection.BeginTransaction())
                    {

                        dt = new DataTable();
                        try
                        {
                            using (SqlCommand oCmd = new SqlCommand())
                            {
                                oCmd.Connection = oConnection;
                                oCmd.Transaction = transaction;
                                oCmd.CommandType = CommandType.Text;

                                if (!String.IsNullOrEmpty(decryptKey))
                                {

                                    if (decryptKey == "BROKERAGE_ACCOUNT_NUMBER_SYM_KEY")
                                    {
                                        oCmd.CommandType = CommandType.Text;
                                        oCmd.CommandText = "OPEN SYMMETRIC KEY BROKERAGE_ACCOUNT_NUMBER_SYM_KEY DECRYPTION BY CERTIFICATE PROHIBITED_TRADE_CERT";
                                        oCmd.ExecuteNonQuery();
                                    }
                                }

                                oCmd.CommandText = strExecuteSql;
                                oCmd.CommandType = CommandType.Text;

                                using (SqlDataAdapter oDa = new SqlDataAdapter(oCmd))
                                {
                                    oDa.Fill(dt);
                                }

                                if (!String.IsNullOrEmpty(decryptKey))
                                {
                                    if (decryptKey == "BROKERAGE_ACCOUNT_NUMBER_SYM_KEY")
                                    {
                                        oCmd.CommandType = CommandType.Text;
                                        oCmd.CommandText = "CLOSE SYMMETRIC KEY BROKERAGE_ACCOUNT_NUMBER_SYM_KEY";
                                        oCmd.ExecuteNonQuery();
                                    }
                                }

                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }

            return dt;
        }

        public DataSet SQL_GetDataSet(string strSql, string connectionKey = "")
        {
            DataSet ds = new DataSet();

            using (SqlConnection oConnection = new SqlConnection(SQLConnectionString(connectionKey)))
            {
                // Open connection
                if (CommonHelper.isImporsonateAccount)
                {
                    CommonHelper.RunImpersonated(Environment.UserDomainName, CommonHelper.serviceAccountId, CommonHelper.serviceAccountKey, () =>
                    {
                        oConnection.Open();
                    });
                }
                else oConnection.Open();

                using (SqlCommand oCmd = new SqlCommand())
                {
                    oCmd.Connection = oConnection;
                    oCmd.CommandText = strSql;
                    oCmd.CommandType = CommandType.Text;

                    using (SqlDataAdapter oDa = new SqlDataAdapter(oCmd))
                    {
                        oDa.Fill(ds);
                    }
                }
            }

            return ds;
        }
        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region "Test Connections"
        public void TestLocalSQLConnection()
        {
            Object objCount;

            objCount = SQL_ExecuteScalar("select 1");
        }
        public int TestSQLConnection()
        {
            object intCount;
            string strSql = string.Format(@"select 1]");

            intCount = SQL_ExecuteScalar(strSql);

            return Convert.ToInt32(intCount);
        }

        #endregion

    }
}
