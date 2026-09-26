using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using projeto.Util;

namespace projeto.Repository;

public class UtilRepository (IConfiguration configuration)
{

    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public OracleDataReader ListBy(string tabela, string condicao, Parametro[] parametros)
    {
        string sql = "SELECT t.* FROM " + tabela + " t WHERE " + condicao;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        if (parametros != null)
        {
            for (int i = 0; i < parametros.Length; i++)
            {
                Parametro p = parametros[i];
                command.Parameters.Add(p.Chave, p.Valor);
            }
        }

        connection.Open();
        using var reader = command.ExecuteReader();
        return reader;
    }
}