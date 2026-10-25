using System.Data;

namespace UserAPI.UnitTests.Helpers
{
    public static class Rows
    {
        // Columnas con los mismos nombres y tipos .NET que devuelve Npgsql para cada función
        public static DataTable Table(params (string Name, Type Type)[] columns)
        {
            var table = new DataTable();
            foreach (var (name, type) in columns)
                table.Columns.Add(name, type);
            return table;
        }

        // Para las funciones que devuelven un único valor jsonb con alias ("response", "result")
        public static DataTable Json(string column, string json)
        {
            var table = Table((column, typeof(string)));
            table.Rows.Add(json);
            return table;
        }
    }
}
