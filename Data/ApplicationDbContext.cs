using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Data;

public class ApplicationDbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var connection = _configuration.GetConnectionString("SqlServer") ?? throw new InvalidOperationException("ConnectionString is Null");
        return new SqlConnection(connection);

    }
}