using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Raven.Cloud
{

    enum Op
    {
        Create,
        Drop,
        Import,
        Restore
    }

    abstract class Options
    {
        public abstract Op Op { get; }

        [Option(HelpText = "Specify the database name", Required = true)]
        public string Db { get; set; }

        [Option(HelpText = "Specify the Url of your RavenDb cluster", Required = true)]
        public string Url { get; set; }


        [Option(HelpText = "Specify the path to the certificat file (optional)")]
        public string Cert { get; set; }

    }

    [Verb("create", HelpText = "Create a new database")]
    class CreateOptions : Options
    {
        public override Op Op { get => Op.Create; }

        [Usage(ApplicationAlias = "ravendb")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Create database", new CreateOptions { Db = "ATISDB", Url = "http://127.0.0.1:8082/" });
            }
        }
    }

    [Verb("drop", HelpText = "Drop an existing database")]
    class DropOptions : Options
    {
        public override Op Op { get => Op.Drop; }

        [Usage(ApplicationAlias = "ravendb")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Drop database", new DropOptions { Db = "ATISDB", Url = "http://127.0.0.1:8082/" });
            }
        }
    }

    [Verb("import", HelpText = "Restores a dbbump from file")]
    class ImportOptions : Options
    {
        public override Op Op { get => Op.Import; }

        [Option(HelpText = "The location for the source folder")]
        public string Dump { get; set; }

        [Usage(ApplicationAlias = "ravendb")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Import database", new ImportOptions { Db = "ATISDB", Url = "http://127.0.0.1:8082/", Dump = @"c:\bkp\raven.dbdump" });
            }
        }
    }


    [Verb("restore", HelpText = "Restores a database from backup")]
    class RestoreOptions : Options
    {
        public override Op Op { get => Op.Restore; }

        [Option(HelpText = "The location for the source folder", Required = true)]
        public string Bkp { get; set; }

        [Usage(ApplicationAlias = "ravendb")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Restore database", new RestoreOptions { Db = "ATISDB", Url = "http://127.0.0.1:8082/", Bkp = @"c:\bkp\folder" });
            }
        }
    }
}
