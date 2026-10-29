using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class SafraRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Safra> GetAll()
    {
        var lista = new List<Safra>();
        string sql = "SELECT ID, NOME, DATAINICIO, DATAFIM, DESCRICAO FROM SAFRAS";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Safra
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                DataInicio = reader.GetDateTime(2),
                DataFim = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                Descricao = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }
        return lista;
    }

    public Safra? GetById(int id)
    {
        string sql = "SELECT ID, NOME, DATAINICIO, DATAFIM, DESCRICAO FROM SAFRAS WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Safra
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                DataInicio = reader.GetDateTime(2),
                DataFim = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                Descricao = reader.IsDBNull(4) ? null : reader.GetString(4)
            };
        }
        return null;
    }

    public bool Create(Safra safra)
    {
        string sql = "INSERT INTO SAFRAS (NOME, DATAINICIO, DATAFIM, DESCRICAO) VALUES (:Nome, :DataInicio, :DataFim, :Descricao)";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Nome", safra.Nome));
        command.Parameters.Add(new OracleParameter("DataInicio", safra.DataInicio));
        command.Parameters.Add(new OracleParameter("DataFim", (object?)safra.DataFim ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Descricao", (object?)safra.Descricao ?? DBNull.Value));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Safra safra)
    {
        string sql = "UPDATE SAFRAS SET NOME = :Nome, DATAINICIO = :DataInicio, DATAFIM = :DataFim, DESCRICAO = :Descricao WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Nome", safra.Nome));
        command.Parameters.Add(new OracleParameter("DataInicio", safra.DataInicio));
        command.Parameters.Add(new OracleParameter("DataFim", (object?)safra.DataFim ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Descricao", (object?)safra.Descricao ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Id", safra.Id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        string sql = "DELETE FROM SAFRAS WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }
}