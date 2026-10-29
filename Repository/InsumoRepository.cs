using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class InsumoRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException(
            "A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Insumo> GetAll()
    {
        var insumos = new List<Insumo>();

        const string sql = """
            SELECT ID, NOME, TIPO, UNIDADE_MEDIDA, ESTOQUE
            FROM INSUMO
            ORDER BY NOME
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            insumos.Add(new Insumo
            {
                Id = Convert.ToInt32(reader["ID"]),
                Nome = reader["NOME"].ToString() ?? string.Empty,
                Tipo = reader["TIPO"] == DBNull.Value
                    ? null
                    : reader["TIPO"].ToString(),
                UnidadeMedida = reader["UNIDADE_MEDIDA"].ToString()
                    ?? string.Empty,
                Estoque = reader["ESTOQUE"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(reader["ESTOQUE"])
            });
        }

        return insumos;
    }

    public Insumo? GetById(int id)
    {
        const string sql = """
            SELECT ID, NOME, TIPO, UNIDADE_MEDIDA, ESTOQUE
            FROM INSUMO
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

        return new Insumo
        {
            Id = Convert.ToInt32(reader["ID"]),
            Nome = reader["NOME"].ToString() ?? string.Empty,
            Tipo = reader["TIPO"] == DBNull.Value
                ? null
                : reader["TIPO"].ToString(),
            UnidadeMedida = reader["UNIDADE_MEDIDA"].ToString()
                ?? string.Empty,
            Estoque = reader["ESTOQUE"] == DBNull.Value
                ? 0
                : Convert.ToDecimal(reader["ESTOQUE"])
        };
    }

    public bool Create(Insumo insumo)
    {
        const string sql = """
            INSERT INTO INSUMO
                (NOME, TIPO, UNIDADE_MEDIDA, ESTOQUE)
            VALUES
                (:pNome, :pTipo, :pUnidadeMedida, :pEstoque)
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pNome", insumo.Nome));
        command.Parameters.Add(new OracleParameter(
            "pTipo",
            (object?)insumo.Tipo ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter(
            "pUnidadeMedida",
            insumo.UnidadeMedida));
        command.Parameters.Add(new OracleParameter(
            "pEstoque",
            insumo.Estoque));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Insumo insumo)
    {
        const string sql = """
            UPDATE INSUMO
            SET NOME = :pNome,
                TIPO = :pTipo,
                UNIDADE_MEDIDA = :pUnidadeMedida,
                ESTOQUE = :pEstoque
            WHERE ID = :pId
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pNome", insumo.Nome));
        command.Parameters.Add(new OracleParameter(
            "pTipo",
            (object?)insumo.Tipo ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter(
            "pUnidadeMedida",
            insumo.UnidadeMedida));
        command.Parameters.Add(new OracleParameter(
            "pEstoque",
            insumo.Estoque));
        command.Parameters.Add(new OracleParameter("pId", insumo.Id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        const string sql = """
            DELETE FROM INSUMO
            WHERE ID = :pId
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pId", id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}