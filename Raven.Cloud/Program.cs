using CommandLine;
using System;

namespace Raven.Cloud
{
    class Program
    {

        static readonly DateTime startedAt = DateTime.Now;


        static void Main(string[] args)
        {
            try
            {
                Bar();
                Log("Raven.Cloud App");
                Bar();

                new Parser(p => { p.EnableDashDash = false; p.HelpWriter = Console.Out; p.CaseSensitive = true; })
                    .ParseArguments<CreateOptions, DropOptions, RestoreOptions, ImportOptions>(args)
                    .MapResult(
                      (CreateOptions opts) => Run(opts),
                      (DropOptions opts) => Run(opts),
                      (RestoreOptions opts) => Run(opts),
                      (ImportOptions opts) => Run(opts),
                      errs => 1);
            }
            catch (Exception e)
            {
                Log(e);
            }
        }

        private static int Run(Options o)
        {
            var db = new DbOperation(o.Url, o.Cert);

            switch (o.Op)
            {
                case Op.Create:
                    db.CreateDb(o.Db);
                    break;

                case Op.Drop:
                    db.DropDb(o.Db);
                    break;

                case Op.Import:
                    db.Import(o.Db, ((ImportOptions)o).Dump);
                    break;

                case Op.Restore:
                    db.Restore(o.Db, ((RestoreOptions)o).Bkp);
                    break;

            }

            Log("All operations succeeded!");
            Log($"Total Time: {(DateTime.Now - startedAt).TotalSeconds} seconds");

            return 0;
        }

        private static void Log(string log)
        {
            Console.WriteLine(log);
        }

        private static void Log(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log("\n");
            Console.WriteLine(e);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void Logg(string log)
        {
            Console.Write(log);
        }

        private static void Ok()
        {
            Console.WriteLine("OK");
        }

        private static void Bar()
        {
            Console.WriteLine("----------------------------------");
        }
        
    }
}
