using DataMartBuilder.Interfaces;
using DataMartBuilder.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Services
{
    public class TransferData
    {
        public IDatabaseConnection CurrentDb {  get; set; }
        public List<IDatabaseConnection> TargetDb { get; set; }
        public List<SelectedTable> SelectedTables { get; set; }
        public bool IsUpdate = false;

        public TransferData(IDatabaseConnection currentDb, List<IDatabaseConnection> targetDb, List<SelectedTable> selectedTables)
        {
            CurrentDb = currentDb;
            TargetDb = targetDb;
            SelectedTables = selectedTables;
        }

        public async Task<string> Execute()
        {
            var targetDb = TargetDb.Where(x => SelectedTables.Any(y => x.Name == y.Database.Name));
            var errors = new StringBuilder();

            foreach (var db in targetDb)
            {
                var targetSelectedTables = SelectedTables.Where(x => x.Database.Name == db.Name).Select(x => x.Table);

                if (db is SqlServerConnector && CurrentDb is SqlServerConnector)
                {
                    errors.AppendLine(await new SmoTransfer(CurrentDb, db, targetSelectedTables.ToList()).TransferDate());
                }
            }

            return errors.ToString();
        }
    }
}
