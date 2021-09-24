using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseKeeper.Application
{
    public class StatementGenerator : IStatementGenerator
    {
        public string SaveStatement(int houseKeeperOid, string housekeeperName, DateTime statementDate)
        {
            var report = new HousekeeperStatementReport(houseKeeperOid, statementDate);

            if (!report.HasData)
                return string.Empty;

            report.CreateDocument();

            var filename = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            string.Format("Sandpiper Statement{0:yyy:MM} {1}.pdf",
                                            statementDate, housekeeperName)
                            );

            report.ExportToPdf(filename);
            return filename;
        }
    }
}
