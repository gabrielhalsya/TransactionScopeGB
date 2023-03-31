using R_BackEnd;
using R_Common;
using System.Data;
using System.Data.Common;
using System.Transactions;
using TranScopeCommon;

namespace TranScopeBack
{
    public class TranScopeCls
    {
        public TranScopeDataDTO ProcessWithoutTransactionDB(int poProcessRecordCount)
        {
            R_Exception loException = new R_Exception();
            TranScopeDataDTO loRtn = new TranScopeDataDTO();
            List<CustomerDbDTO> Customers = null;

            try
            {
                Customers = GetAllCustomer(poProcessRecordCount);
                RemoveAllCustomer(Customers);
                AddAllCopyCustomer(Customers);

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();

            return loRtn;
        }
        public TranScopeDataDTO ProcessAllWithTransactionDB(int poProcessRecordCount)
        {
            R_Exception loException = new R_Exception();
            TranScopeDataDTO loRtn = new TranScopeDataDTO();
            List<CustomerDbDTO> Customers = null;

            try
            {
                Customers = GetAllCustomer(poProcessRecordCount);
                using (TransactionScope TransScope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    RemoveAllCustomer(Customers);
                    AddAllCopyCustomer(Customers);

                    TransScope.Complete();
                    loRtn.IsSuccess = true;
                }

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();

            return loRtn;
        }

        private void AddAllCopyCustomer(List<CustomerDbDTO> poListCustomers)
        {
            R_Exception loException = new R_Exception();
            DbConnection loConn = null;
            DbCommand loCommand;
            R_Db loDb = null;
            string lcCmd;

            DbParameter loDbParCustomerID;
            DbParameter loDbParCustomerName;
            DbParameter loDbParContactName;

            try
            {
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();

                loDb.R_AddCommandParameter(loCommand, "CustomerID", DbType.String, 50, "");
                loDb.R_AddCommandParameter(loCommand, "CustomerName", DbType.String, 50, "");
                loDb.R_AddCommandParameter(loCommand, "ContactName", DbType.String, 50, "");

                loDbParCustomerID = loCommand.Parameters["CustomerID"];
                loDbParCustomerName = loCommand.Parameters["CustomerName"];
                loDbParContactName = loCommand.Parameters["ContactName"];

                lcCmd = "INSERT INTO TestCopyCustomer(CustomerID, CustomerName, ContactName) VALUES (@CustomerID, @CustomerName, @ContactName)";
                loCommand.CommandText = lcCmd;

                int lnCount = 1;
                foreach (var item in poListCustomers)
                {
                    if ((lnCount % 3) == 0)
                    {
                        loException.Add("001", $"Error at {lnCount} data");
                        goto EndBlock;
                    }

                    lnCount++;

                    loDbParCustomerID.Value = item.CustomerID;
                    loDbParCustomerName.Value = item.CustomerName;
                    loDbParContactName.Value = item.ContactName;
                    loDb.SqlExecNonQuery(loConn, loCommand, false);

                    //lcCmd = "insert into TestCustomerLog(Log) Values(@CustomerID)";
                    //loCommand.CommandText = lcCmd;
                    //loDbParCustomerID.Value = $"Remove Customer {item.CustomerID}";
                    //loDb.SqlExecNonQuery(loConn, loCommand, false);
                }

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            finally
            {
                if (loConn != null)
                {
                    if (loConn.State != ConnectionState.Closed) loConn.Close();
                    loConn.Dispose();
                }
            }

        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

        private void RemoveAllCustomer1(List<CustomerDbDTO> poListCustomers)
        {
            R_Exception loException = new R_Exception();
            R_Db loDb = null;
            DbConnection loConn = null;
            DbCommand loCommand;
            string lcCmd;
            DbParameter loDbParameter;

            try
            {
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();
                loDb.R_AddCommandParameter(loCommand, "StrPar1", DbType.String, 50, "");
                loDbParameter = loCommand.Parameters[0];

                foreach (CustomerDbDTO item in poListCustomers)
                {
                    lcCmd = "DELETE FROM TestCustomer WHERE CustomerID = @StrPar1;";
                    loCommand.CommandText = lcCmd;
                    loDbParameter.Value = item.CustomerID;
                    loDb.SqlExecNonQuery(loConn, loCommand, false);

                    lcCmd = "INSERT INTO TestCustomerLog(Log) VALUES (@StrPar1);";
                    loCommand.CommandText = lcCmd;
                    loDbParameter.Value = $"Remove Customer {item.CustomerID}";
                    loDb.SqlExecNonQuery(loConn, loCommand, false);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            finally
            {
                if (loConn != null)
                {
                    if (loConn.State != ConnectionState.Closed) loConn.Close();
                    loConn.Dispose();
                }
            }

        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

        private void RemoveAllCustomer(List<CustomerDbDTO> poListCustomers)
        {
            R_Exception loException = new R_Exception();
            try
            {
                foreach (CustomerDbDTO loCustomerDTO in poListCustomers)
                {
                    RemoveEachCustomer(loCustomerDTO);
                    AddLogEachCustomer(loCustomerDTO);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            
        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

        private void AddLogEachCustomer(CustomerDbDTO poCustomer)
        {
            R_Exception loException = new R_Exception();
            R_Db loDb = null;
            DbConnection loConn = null;
            DbCommand loCommand;
            string lcCmd;
            DbParameter loDbParameter;

            try
            {
                using (TransactionScope TransScope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    loDb = new R_Db();
                    loCommand = loDb.GetCommand();
                    loConn = loDb.GetConnection();
                    loDb.R_AddCommandParameter(loCommand, "StrPar1", DbType.String, 50, "");
                    loDbParameter = loCommand.Parameters[0];

                    lcCmd = "insert into TestCustomerLog(Log) Values(@StrPar1)";
                    loCommand.CommandText = lcCmd;
                    loDbParameter.Value = $"Remove Customer {poCustomer.CustomerID}";
                    loDb.SqlExecNonQuery(loConn, loCommand, false);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            finally
            {
                if (loConn != null)
                {
                    if (loConn.State != ConnectionState.Closed)
                    {
                        loConn.Close();
                    }
                    loConn.Dispose();
                }


            }
        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

        private void RemoveEachCustomer(CustomerDbDTO poCustomer)
        {
            R_Exception loException = new R_Exception();
            R_Db loDb = null;
            DbConnection loConn = null;
            DbCommand loCommand;
            string lcCmd;
            DbParameter loDbParameter;

            try
            {
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();
                loDb.R_AddCommandParameter(loCommand, "StrPar1", DbType.String, 50, "");
                loDbParameter = loCommand.Parameters[0];

                lcCmd = "delete TestCustomer where CustomerID=@StrPar1";
                loCommand.CommandText = lcCmd;
                loDbParameter.Value = poCustomer.CustomerID;
                loDb.SqlExecNonQuery(loConn, loCommand, false);

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            finally
            {
                if (loConn != null)
                {
                    if (loConn.State != ConnectionState.Closed)
                    {
                        loConn.Close();
                    }
                    loConn.Dispose();
                }


            }
        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

        private List<CustomerDbDTO> GetAllCustomer(int pnCount)
        {
            R_Exception loException = new R_Exception();
            R_Db loDb = null;
            List<CustomerDbDTO> LoRtn = null;
            string lcCust;
            string lcCmd;

            try
            {
                lcCust = String.Format("Cust{0}", pnCount.ToString("0000"));
                lcCmd = "SELECT * FROM TestCustomer(NoLock) WHERE CustomerId <= {0};";
                loDb = new R_Db();
                LoRtn = loDb.SqlExecObjectQuery<CustomerDbDTO>(lcCmd, lcCust);

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();

            return LoRtn;

        }

        

    }
}