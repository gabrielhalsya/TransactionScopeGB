using Microsoft.AspNetCore.Mvc;
using R_Common;
using TranScopeBack;
using TranScopeCommon;

namespace TranScopeService
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class TranScopeController : ITranScope
    {
        [HttpPost]
        public TranScopeResultDTO ProcessAllWithTransaction(int ProcessRecordCount)
        {
            R_Exception loException = new R_Exception();
            TranScopeResultDTO loRtn = null;
            TranScopeCls loCls = null;
            try
            {
                loRtn = new TranScopeResultDTO() { data = new TranScopeDataDTO() };
                loCls = new TranScopeCls();
                loRtn.data = loCls.ProcessAllWithTransactionDB(ProcessRecordCount);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();
            return loRtn;
        }

        [HttpPost] public TranScopeResultDTO ProcessWithoutTransaction(int ProcessRecordCount)
        {
            R_Exception loException = new R_Exception();
            TranScopeResultDTO loRtn = null;
            TranScopeCls loCls = null;
            try
            {
                loRtn = new TranScopeResultDTO() { data = new TranScopeDataDTO() };
                loCls = new TranScopeCls();
                loRtn.data = loCls.ProcessWithoutTransactionDB(ProcessRecordCount);
            }
            catch (Exception ex)
            { 
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();
            return loRtn;
        }
    }
}
