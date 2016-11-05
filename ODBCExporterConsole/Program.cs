using System;

namespace ODBCExporterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exported = false;
            bool forceExit = false;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Which ODBC source to export?");
                Console.WriteLine(" 1) Sage (uid=manager, no password)");
                Console.WriteLine(" 2) Custom");
                Console.WriteLine(" q) Quit");

                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                switch (input.ToLower().Trim())
                {
                    case "1":
                        ODBCExporter.ODBCExporter.Export("Dsn=SageLine50v20;uid=manager");
                        exported = true;
                        break;

                    case "2":
                        Console.WriteLine("Enter DSN:");
                        var dsn = Console.ReadLine();
                        if (!string.IsNullOrEmpty(dsn) && !string.IsNullOrEmpty(dsn.Trim()))
                        {
                            ODBCExporter.ODBCExporter.Export(dsn.Trim());
                            exported = true;
                        }
                        break;

                    case "q":
                        forceExit = true;
                        break;
                }

                if (forceExit || exported)
                {
                    break;
                }
            }

            if (!forceExit)
            {
                Console.WriteLine("Done!");
                Console.ReadLine();
            }
        }
    }
}
