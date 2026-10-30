using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class TalhaoRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Talhao> GetAll()
    {
        var lista = new List<Talhao>();

        string sql = @"
            SELECT ID, NOME, AREA, PROPRIEDADE_ID, DESCRICAO
            FROM TALHAO";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Talhao
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Area = reader.GetDecimal(2),
                PropriedadeId = reader.GetInt32(3),
                Descricao = reader.IsDBNull(4)
                    ? null
                    : reader.GetString(4)
            });
        }

        return lista;
    }

    public Talhao? GetById(int id)
    {
        string sql = @"
            SELECT ID, NOME, AREA, PROPRIEDADE_ID, DESCRICAO
            FROM TALHAO
            WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Talhao
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Area = reader.GetDecimal(2),
                PropriedadeId = reader.GetInt32(3),
                Descricao = reader.IsDBNull(4)
                    ? null
                    : reader.GetString(4)
            };
        }

        return null;
    }

    public bool Create(Talhao talhao)
{
    string sql = @"
        INSERT INTO TALHAO
            (NOME, AREA, PROPRIEDADE_ID, DESCRICAO)
        VALUES
            (:pNome, :pArea, :pPropriedadeId, :pDescricao)";

    using var connection = new OracleConnection(_connectionString);
    using var command = new OracleCommand(sql, connection);

    command.Parameters.Add(new OracleParameter("pNome", talhao.Nome));
    command.Parameters.Add(new OracleParameter("pArea", talhao.Area));
    command.Parameters.Add(new OracleParameter("pPropriedadeId", talhao.PropriedadeId));
    command.Parameters.Add(
        new OracleParameter(
            "pDescricao",
            (object?)talhao.Descricao ?? DBNull.Value
        )
    );

    connection.Open();

    return command.ExecuteNonQuery() > 0;
}

           public bool Update(Talhao talhao)
{
    string sql = @"
        UPDATE TALHAO
        SET NOME = :pNome,
            AREA = :pArea,
            PROPRIEDADE_ID = :pPropriedadeId,
            DESCRICAO = :pDescricao
        WHERE ID = :pId";

    using var connection = new OracleConnection(_connectionString);
    using var command = new OracleCommand(sql, connection);

    command.Parameters.Add(new OracleParameter("pNome", talhao.Nome));
    command.Parameters.Add(new OracleParameter("pArea", talhao.Area));
    command.Parameters.Add(new OracleParameter("pPropriedadeId", talhao.PropriedadeId));
    command.Parameters.Add(
        new OracleParameter(
            "pDescricao",
            (object?)talhao.Descricao ?? DBNull.Value
        )
    );
    command.Parameters.Add(new OracleParameter("pId", talhao.Id));

    connection.Open();

    return command.ExecuteNonQuery() > 0;
}

    public bool Delete(int id)
    {
        string sql = "DELETE FROM TALHAO WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}