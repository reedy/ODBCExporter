using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;

namespace ODBCExporter
{
    public class ODBCExporter
    {
        private const string QUOTE = "\"";
        private const string ESCAPED_QUOTE = "\"\"";
        private static readonly char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };

        public static void Export(string dsn)
        {
            OdbcConnection conn = new OdbcConnection(dsn);
            conn.Open();

            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }

            using (DataTable tableschema = conn.GetSchema("TABLES"))
            {
                foreach (DataRow row in tableschema.Rows)
                {
                    var tableName = row["TABLE_NAME"];
                    var command = conn.CreateCommand();
                    command.CommandText = string.Format("SELECT * FROM {0}", tableName);
                    var reader = command.ExecuteReader();

                    StringBuilder builder = new StringBuilder();
                    List<string> cols = new List<string>();
                    var fieldCount = reader.FieldCount;
                    for (int col = 0; col < fieldCount; col++)
                    {
                        cols.Add(reader.GetName(col));
                    }
                    builder.AppendLine(string.Join(",", cols));
                    while (reader.Read())
                    {
                        List<string> rowVals = new List<string>();
                        for (int col = 0; col < fieldCount; col++)
                        {
                            string val = reader[col].ToString();
                            if (val.Contains(QUOTE))
                            {
                                val = val.Replace(QUOTE, ESCAPED_QUOTE);
                            }

                            if (val.IndexOfAny(CHARACTERS_THAT_MUST_BE_QUOTED) > -1)
                            {
                                val = QUOTE + val + QUOTE;
                            }

                            rowVals.Add(val);
                        }
                        builder.AppendLine(string.Join(",", rowVals));
                    }

                    using (TextWriter writer = new StreamWriter(string.Format("output/{0}.csv", tableName)))
                    {
                        writer.Write(builder.ToString());
                    }
                }
            }

            conn.Close();
        }
    }
}
