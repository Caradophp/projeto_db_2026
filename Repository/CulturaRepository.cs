using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class CulturaRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Cultura> GetAll()
    {
        var lista = new List<Cultura>();
        string sql = "SELECT ID, NOME, DESCRICAO FROM CULTURAS";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Cultura
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.IsDBNull(2) ? null : reader.GetString(2)
            });
        }
        return lista;
    }

    public Cultura? GetById(int id)
    {
        string sql = "SELECT ID, NOME, DESCRICAO FROM CULTURAS WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Cultura
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.IsDBNull(2) ? null : reader.GetString(2)
            };
        }
        return null;
    }

    public bool Create(Cultura cultura)
    {
        string sql = "INSERT INTO CULTURAS (NOME, DESCRICAO) VALUES (:Nome, :Descricao)";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Nome", cultura.Nome));
        command.Parameters.Add(new OracleParameter("Descricao", (object?)cultura.Descricao ?? DBNull.Value));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Cultura cultura)
    {
        string sql = "UPDATE CULTURAS SET NOME = :Nome, DESCRICAO = :Descricao WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Nome", cultura.Nome));
        command.Parameters.Add(new OracleParameter("Descricao", (object?)cultura.Descricao ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Id", cultura.Id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        string sql = "DELETE FROM CULTURAS WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }
}
