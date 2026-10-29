using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class AplicacaoInsumoRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<AplicacaoInsumo> GetAll()
    {
        var aplicacoes = new List<AplicacaoInsumo>();
        string sql = "SELECT ID, TALHAOID, INSUMOID, DATAAPLICACOE, QUANTIDADE, OBSERVACOES FROM APLICACAO_INSUMOS";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            aplicacoes.Add(new AplicacaoInsumo
            {
                Id = reader.GetInt32(0),
                TalhaoId = reader.GetInt32(1),
                InsumoId = reader.GetInt32(2),
                DataAplicacao = reader.GetDateTime(3),
                Quantidade = reader.GetDecimal(4),
                Observacoes = reader.IsDBNull(5) ? null : reader.GetString(5)
            });
        }
        return aplicacoes;
    }

    public bool Create(AplicacaoInsumo aplicacao)
    {
        string sql = "INSERT INTO APLICACAO_INSUMOS (TALHAOID, INSUMOID, DATAAPLICACOE, QUANTIDADE, OBSERVACOES) VALUES (:TalhaoId, :InsumoId, :Data, :Qtd, :Obs)";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("TalhaoId", aplicacao.TalhaoId));
        command.Parameters.Add(new OracleParameter("InsumoId", aplicacao.InsumoId));
        command.Parameters.Add(new OracleParameter("Data", aplicacao.DataAplicacao));
        command.Parameters.Add(new OracleParameter("Qtd", aplicacao.Quantidade));
        command.Parameters.Add(new OracleParameter("Obs", (object?)aplicacao.Observacoes ?? DBNull.Value));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(AplicacaoInsumo aplicacao)
    {
        string sql = "UPDATE APLICACAO_INSUMOS SET TALHAOID = :TalhaoId, INSUMOID = :InsumoId, DATAAPLICACOE = :Data, QUANTIDADE = :Qtd, OBSERVACOES = :Obs WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("TalhaoId", aplicacao.TalhaoId));
        command.Parameters.Add(new OracleParameter("InsumoId", aplicacao.InsumoId));
        command.Parameters.Add(new OracleParameter("Data", aplicacao.DataAplicacao));
        command.Parameters.Add(new OracleParameter("Qtd", aplicacao.Quantidade));
        command.Parameters.Add(new OracleParameter("Obs", (object?)aplicacao.Observacoes ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Id", aplicacao.Id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        string sql = "DELETE FROM APLICACAO_INSUMOS WHERE ID = :Id";
        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }
}
