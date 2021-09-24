using System;

namespace HouseKeeper.Application
{
    public interface IStatementGenerator
    {
        string SaveStatement(int houseKeeperOid, string housekeeperName, DateTime statementDate);
    }
}