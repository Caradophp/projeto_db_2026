using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class PlantioRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException(
            "A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Plantio> GetAll()
    {
        var lista = new List<Plantio>();

        const string sql = """
            SELECT ID,
                   TALHAO_ID,
                   CULTURA_ID,
                   DATA_PLANTIO,
                   DATA_PREVISTA_COLHEITA,
                   PRODUTIVIDADE_ESTIMADA,
                   CUSTO_PRODUCAO,
                   STATUS,
                   OBSERVACOES
            FROM PLANTIO
            ORDER BY DATA_PLANTIO DESC, ID DESC
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(MapPlantio(reader));
        }

        return lista;
    }

    public Plantio? GetById(int id)
    {
        const string sql = """
            SELECT ID,
                   TALHAO_ID,
                   CULTURA_ID,
                   DATA_PLANTIO,
                   DATA_PREVISTA_COLHEITA,
                   PRODUTIVIDADE_ESTIMADA,
                   CUSTO_PRODUCAO,
                   STATUS,
                   OBSERVACOES
            FROM PLANTIO
            WHERE ID = :pId
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pId", id));

        connection.Open();

        using var reader = command.ExecuteReader();

        return reader.Read()
            ? MapPlantio(reader)
            : null;
    }

    public bool Create(Plantio plantio)
    {
        const string sql = """
            INSERT INTO PLANTIO (
                TALHAO_ID,
                CULTURA_ID,
                DATA_PLANTIO,
                DATA_PREVISTA_COLHEITA,
                PRODUTIVIDADE_ESTIMADA,
                CUSTO_PRODUCAO,
                STATUS,
                OBSERVACOES
            )
            VALUES (
                :pTalhaoId,
                :pCulturaId,
                :pDataPlantio,
                :pDataPrevistaColheita,
                :pProdutividadeEstimada,
                :pCustoProducao,
                :pStatus,
                :pObservacoes
            )
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        AddParameters(command, plantio, includeId: false);

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Plantio plantio)
    {
        const string sql = """
            UPDATE PLANTIO
               SET TALHAO_ID = :pTalhaoId,
                   CULTURA_ID = :pCulturaId,
                   DATA_PLANTIO = :pDataPlantio,
                   DATA_PREVISTA_COLHEITA = :pDataPrevistaColheita,
                   PRODUTIVIDADE_ESTIMADA = :pProdutividadeEstimada,
                   CUSTO_PRODUCAO = :pCustoProducao,
                   STATUS = :pStatus,
                   OBSERVACOES = :pObservacoes
             WHERE ID = :pId
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        AddParameters(command, plantio, includeId: true);

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        const string sql = """
            DELETE FROM PLANTIO
            WHERE ID = :pId
            """;

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("pId", id));

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    private static Plantio MapPlantio(OracleDataReader reader)
    {
        return new Plantio
        {
            Id = Convert.ToInt32(reader["ID"]),
            TalhaoId = Convert.ToInt32(reader["TALHAO_ID"]),
            CulturaId = Convert.ToInt32(reader["CULTURA_ID"]),
            DataPlantio = Convert.ToDateTime(reader["DATA_PLANTIO"]),

            DataPrevistaColheita =
                reader["DATA_PREVISTA_COLHEITA"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["DATA_PREVISTA_COLHEITA"]),

            ProdutividadeEstimada =
                reader["PRODUTIVIDADE_ESTIMADA"] == DBNull.Value
                    ? null
                    : Convert.ToDecimal(reader["PRODUTIVIDADE_ESTIMADA"]),

            CustoProducao =
                reader["CUSTO_PRODUCAO"] == DBNull.Value
                    ? null
                    : Convert.ToDecimal(reader["CUSTO_PRODUCAO"]),

            Status =
                reader["STATUS"] == DBNull.Value
                    ? "Em andamento"
                    : reader["STATUS"].ToString() ?? "Em andamento",

            Observacoes =
                reader["OBSERVACOES"] == DBNull.Value
                    ? null
                    : reader["OBSERVACOES"].ToString()
        };
    }

    private static void AddParameters(
        OracleCommand command,
        Plantio plantio,
        bool includeId)
    {
        command.Parameters.Add(
            new OracleParameter("pTalhaoId", plantio.TalhaoId));

        command.Parameters.Add(
            new OracleParameter("pCulturaId", plantio.CulturaId));

        command.Parameters.Add(
            new OracleParameter("pDataPlantio", plantio.DataPlantio));

        command.Parameters.Add(
            new OracleParameter(
                "pDataPrevistaColheita",
                (object?)plantio.DataPrevistaColheita ?? DBNull.Value));

        command.Parameters.Add(
            new OracleParameter(
                "pProdutividadeEstimada",
                (object?)plantio.ProdutividadeEstimada ?? DBNull.Value));

        command.Parameters.Add(
            new OracleParameter(
                "pCustoProducao",
                (object?)plantio.CustoProducao ?? DBNull.Value));

        command.Parameters.Add(
            new OracleParameter(
                "pStatus",
                string.IsNullOrWhiteSpace(plantio.Status)
                    ? "Em andamento"
                    : plantio.Status));

        command.Parameters.Add(
            new OracleParameter(
                "pObservacoes",
                (object?)plantio.Observacoes ?? DBNull.Value));

        if (includeId)
        {
            command.Parameters.Add(
                new OracleParameter("pId", plantio.Id));
        }
    }
}