using Oracle.ManagedDataAccess.Client;
using projeto.Models;

namespace projeto.Repository;

public class AplicacaoInsumoRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException(
            "A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<AplicacaoInsumo> GetAll()
    {
        var aplicacoes = new List<AplicacaoInsumo>();

        const string sql = @"
            SELECT
                ID,
                TALHAO_ID,
                INSUMO_ID,
                DATA_APLICACAO,
                QUANTIDADE,
                OBSERVACOES
            FROM APLICACAO_INSUMO
            ORDER BY DATA_APLICACAO DESC, ID DESC";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            aplicacoes.Add(new AplicacaoInsumo
            {
                Id = Convert.ToInt32(reader["ID"]),
                TalhaoId = Convert.ToInt32(reader["TALHAO_ID"]),
                InsumoId = Convert.ToInt32(reader["INSUMO_ID"]),
                DataAplicacao = Convert.ToDateTime(reader["DATA_APLICACAO"]),
                Quantidade = Convert.ToDecimal(reader["QUANTIDADE"]),
                Observacoes = reader["OBSERVACOES"] == DBNull.Value
                    ? null
                    : reader["OBSERVACOES"].ToString()
            });
        }

        return aplicacoes;
    }

    public bool Create(AplicacaoInsumo aplicacao)
    {
        const string sql = @"
            INSERT INTO APLICACAO_INSUMO
                (TALHAO_ID, INSUMO_ID, DATA_APLICACAO, QUANTIDADE, OBSERVACOES)
            VALUES
                (:pTalhaoId, :pInsumoId, :pDataAplicacao, :pQuantidade, :pObservacoes)";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(
            new OracleParameter("pTalhaoId", aplicacao.TalhaoId));

        command.Parameters.Add(
            new OracleParameter("pInsumoId", aplicacao.InsumoId));

        command.Parameters.Add(
            new OracleParameter("pDataAplicacao", aplicacao.DataAplicacao));

        command.Parameters.Add(
            new OracleParameter("pQuantidade", aplicacao.Quantidade));

        command.Parameters.Add(
            new OracleParameter(
                "pObservacoes",
                (object?)aplicacao.Observacoes ?? DBNull.Value));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(AplicacaoInsumo aplicacao)
    {
        const string sql = @"
            UPDATE APLICACAO_INSUMO
            SET
                TALHAO_ID = :pTalhaoId,
                INSUMO_ID = :pInsumoId,
                DATA_APLICACAO = :pDataAplicacao,
                QUANTIDADE = :pQuantidade,
                OBSERVACOES = :pObservacoes
            WHERE ID = :pId";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(
            new OracleParameter("pTalhaoId", aplicacao.TalhaoId));

        command.Parameters.Add(
            new OracleParameter("pInsumoId", aplicacao.InsumoId));

        command.Parameters.Add(
            new OracleParameter("pDataAplicacao", aplicacao.DataAplicacao));

        command.Parameters.Add(
            new OracleParameter("pQuantidade", aplicacao.Quantidade));

        command.Parameters.Add(
            new OracleParameter(
                "pObservacoes",
                (object?)aplicacao.Observacoes ?? DBNull.Value));

        command.Parameters.Add(
            new OracleParameter("pId", aplicacao.Id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        const string sql = @"
            DELETE FROM APLICACAO_INSUMO
            WHERE ID = :pId";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(
            new OracleParameter("pId", id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}