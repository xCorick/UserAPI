using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.DataAccess.Interface
{
    public interface IDatabaseConnectionFactory
    {
        Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
    }
}
