using Raven.Client.Documents;
using Raven.Client.Documents.Operations.Backups;
using Raven.Client.Documents.Smuggler;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Raven.Cloud
{
    public class DbOperation
    {
        private readonly IDocumentStore store;

        public DbOperation(string url, string certUri = null)
        {
            Logg($"Connecting to the Raven Cloud at \"{url}\": ");

            store = new DocumentStore
            {
                Certificate = string.IsNullOrEmpty(certUri) ? null : new X509Certificate2(certUri),
                Urls = new string[] { url }
            };

            store.Initialize();
            Ok();
        }

        /// <summary>
        /// Creates a Raven.Cloud Database
        /// Ref: https://ravendb.net/docs/article-page/4.2/csharp/client-api/operations/server-wide/create-database
        /// </summary>
        /// <param name="dbName"></param>
        public void CreateDb(string dbName)
        {
            Logg($"Creating database '{dbName}': ");
            store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(dbName)));
            Ok();
        }

        /// <summary>
        /// Restores a RavenDB Backup.
        /// Notice: Restore only works from a backup from a Raven4 database
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="folder"></param>
        public void Restore(string dbName, string folder)
        {
            Logg($"Restoring database '{dbName}': ");

            var restoreConfiguration = new RestoreBackupConfiguration
            {
                DatabaseName = dbName,
                BackupLocation = folder
            };

            var restoreBackupTask = new RestoreBackupOperation(restoreConfiguration);
            store.Maintenance.Server.Send(restoreBackupTask).WaitForCompletion();
            Ok();
        }

        /// <summary>
        /// Imports (Smugglers) Data from a dbdump
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dbDump"></param>
        public void Import(string dbName, string dbDump)
        {
            Logg($"Importing data into '{dbName}': ");

            var importOperation = store.Smuggler.ForDatabase(dbName)
                .ImportAsync(new DatabaseSmugglerImportOptions { OperateOnTypes = DatabaseItemType.Documents }, dbDump
            );

            importOperation.GetAwaiter().GetResult().WaitForCompletion();
            Ok();
        }

        public void DropDb(string dbName)
        {
            Logg($"Deleting database '{dbName}': ");
            store.Maintenance.Server.Send(new DeleteDatabasesOperation(dbName, true));
            Ok();
        }

        private void Log(string log)
        {
            Console.WriteLine(log);
        }

        private void Logg(string log)
        {
            Console.Write(log);
        }

        private void Ok()
        {
            Console.WriteLine("OK");
        }
    }
}
