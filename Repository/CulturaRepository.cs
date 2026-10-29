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

        const string sql = """
            SELECT ID, NOME, DESCRICAO
            FROM CULTURA
            ORDER BY NOME
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Cultura
            {
                Id = Convert.ToInt32(reader["ID"]),
                Nome = reader["NOME"].ToString() ?? string.Empty,
                Descricao = reader["DESCRICAO"] == DBNull.Value
                    ? null
                    : reader["DESCRICAO"].ToString()
            });
        }

        return lista;
    }

    public Cultura? GetById(int id)
    {
        const string sql = """
            SELECT ID, NOME, DESCRICAO
            FROM CULTURA
            WHERE ID = :pId
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pId", id));

        connection.Open();

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return new Cultura
        {
            Id = Convert.ToInt32(reader["ID"]),
            Nome = reader["NOME"].ToString() ?? string.Empty,
            Descricao = reader["DESCRICAO"] == DBNull.Value
                ? null
                : reader["DESCRICAO"].ToString()
        };
    }

    public bool Create(Cultura cultura)
    {
        const string sql = """
            INSERT INTO CULTURA (NOME, DESCRICAO)
            VALUES (:pNome, :pDescricao)
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pNome", cultura.Nome));
        command.Parameters.Add(new OracleParameter(
            "pDescricao",
            (object?)cultura.Descricao ?? DBNull.Value
        ));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Cultura cultura)
    {
        const string sql = """
            UPDATE CULTURA
            SET NOME = :pNome,
                DESCRICAO = :pDescricao
            WHERE ID = :pId
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pNome", cultura.Nome));
        command.Parameters.Add(new OracleParameter(
            "pDescricao",
            (object?)cultura.Descricao ?? DBNull.Value
        ));
        command.Parameters.Add(new OracleParameter("pId", cultura.Id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        const string sql = """
            DELETE FROM CULTURA
            WHERE ID = :pId
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pId", id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}