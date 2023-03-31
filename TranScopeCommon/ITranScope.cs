using System;
using System.Collections.Generic;
using System.Text;

namespace TranScopeCommon
{
    public interface ITranScope
    {                                                                          
        TranScopeResultDTO ProcessWithoutTransaction(int ProcessRecordCount);
        TranScopeResultDTO ProcessAllWithTransaction(int ProcessRecordCount);

    }
}
