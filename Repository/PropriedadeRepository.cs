using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class PropriedadeRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Propriedade> GetAll()
    {
        var lista = new List<Propriedade>();

        string sql = @"
            SELECT ID, NOME, AREA_TOTAL, USER_ID
            FROM PROPRIEDADE";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Propriedade
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                AreaTotal = reader.GetDecimal(2),
                UserId = reader.GetInt32(3)
            });
        }

        return lista;
    }

    public Propriedade? GetById(int id)
    {
        string sql = @"
            SELECT ID, NOME, AREA_TOTAL, USER_ID
            FROM PROPRIEDADE
            WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Propriedade
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                AreaTotal = reader.GetDecimal(2),
                UserId = reader.GetInt32(3)
            };
        }

        return null;
    }

    public bool Create(Propriedade prop)
    {
        string sql = @"
            INSERT INTO PROPRIEDADE
                (NOME, AREA_TOTAL, USER_ID)
            VALUES
                (:Nome, :AreaTotal, :UserId)";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("Nome", prop.Nome));
        command.Parameters.Add(new OracleParameter("AreaTotal", prop.AreaTotal));
        command.Parameters.Add(new OracleParameter("UserId", prop.UserId));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Propriedade prop)
    {
        string sql = @"
            UPDATE PROPRIEDADE
            SET NOME = :Nome,
                AREA_TOTAL = :AreaTotal,
                USER_ID = :UserId
            WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("Nome", prop.Nome));
        command.Parameters.Add(new OracleParameter("AreaTotal", prop.AreaTotal));
        command.Parameters.Add(new OracleParameter("UserId", prop.UserId));
        command.Parameters.Add(new OracleParameter("Id", prop.Id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        string sql = "DELETE FROM PROPRIEDADE WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}