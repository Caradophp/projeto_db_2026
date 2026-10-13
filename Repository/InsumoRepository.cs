using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class InsumoRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Insumo> GetAll()
    {
        var insumos = new List<Insumo>();
        string sql = "SELECT ID, NOME, TIPO, UNIDADEMEDIDA, ESTOQUE FROM INSUMOS";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            insumos.Add(new Insumo
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Tipo = reader.IsDBNull(2) ? null : reader.GetString(2),
                UnidadeMedida = reader.GetString(3),
                Estoque = reader.GetDecimal(4)
            });
        }
        return insumos;
    }

    public Insumo? GetById(int id)
    {
        string sql = "SELECT ID, NOME, TIPO, UNIDADEMEDIDA, ESTOQUE FROM INSUMOS WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Insumo
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Tipo = reader.IsDBNull(2) ? null : reader.GetString(2),
                UnidadeMedida = reader.GetString(3),
                Estoque = reader.GetDecimal(4)
            };
        }
        return null;
    }

    public bool Create(Insumo insumo)
    {
        string sql = "INSERT INTO INSUMOS (NOME, TIPO, UNIDADEMEDIDA, ESTOQUE) VALUES (:Nome, :Tipo, :Unidade, :Estoque)";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Nome", insumo.Nome));
        command.Parameters.Add(new OracleParameter("Tipo", (object?)insumo.Tipo ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Unidade", insumo.UnidadeMedida));
        command.Parameters.Add(new OracleParameter("Estoque", insumo.Estoque));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Insumo insumo)
    {
        string sql = "UPDATE INSUMOS SET NOME = :Nome, TIPO = :Tipo, UNIDADEMEDIDA = :Unidade, ESTOQUE = :Estoque WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Nome", insumo.Nome));
        command.Parameters.Add(new OracleParameter("Tipo", (object?)insumo.Tipo ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Unidade", insumo.UnidadeMedida));
        command.Parameters.Add(new OracleParameter("Estoque", insumo.Estoque));
        command.Parameters.Add(new OracleParameter("Id", insumo.Id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        string sql = "DELETE FROM INSUMOS WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }
}
