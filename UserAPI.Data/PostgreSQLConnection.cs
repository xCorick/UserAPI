using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Data
{
    public class PostgreSQLConnection
    {
        private readonly string _connection;
        public PostgreSQLConnection(string connection) => _connection = connection; 
    }
}
