using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class PlantioRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Plantio> GetAll()
    {
        var lista = new List<Plantio>();
        string sql = "SELECT ID, TALHAOID, CULTURAID, DATAPLANTIO, DATAPREVISTACOLHEITA, PRODUTIVIDADEESTIMADA, CUSTOPRODUCAO, STATUS, OBSERVACOES FROM PLANTIOS";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Plantio
            {
                Id = reader.GetInt32(0),
                TalhaoId = reader.GetInt32(1),
                CulturaId = reader.GetInt32(2),
                DataPlantio = reader.GetDateTime(3),
                DataPrevistaColheita = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                ProdutividadeEstimada = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                CustoProducao = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                Status = reader.GetString(7),
                Observacoes = reader.IsDBNull(8) ? null : reader.GetString(8)
            });
        }
        return lista;
    }

    public Plantio? GetById(int id)
    {
        string sql = "SELECT ID, TALHAOID, CULTURAID, DATAPLANTIO, DATAPREVISTACOLHEITA, PRODUTIVIDADEESTIMADA, CUSTOPRODUCAO, STATUS, OBSERVACOES FROM PLANTIOS WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Plantio
            {
                Id = reader.GetInt32(0),
                TalhaoId = reader.GetInt32(1),
                CulturaId = reader.GetInt32(2),
                DataPlantio = reader.GetDateTime(3),
                DataPrevistaColheita = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                ProdutividadeEstimada = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                CustoProducao = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                Status = reader.GetString(7),
                Observacoes = reader.IsDBNull(8) ? null : reader.GetString(8)
            };
        }
        return null;
    }

    public bool Create(Plantio plantio)
    {
        string sql = "INSERT INTO PLANTIOS (TALHAOID, CULTURAID, DATAPLANTIO, DATAPREVISTACOLHEITA, PRODUTIVIDADEESTIMADA, CUSTOPRODUCAO, STATUS, OBSERVACOES) VALUES (:TalhaoId, :CulturaId, :DataPlantio, :DataPrevistaColheita, :ProdutividadeEstimada, :CustoProducao, :Status, :Observacoes)";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("TalhaoId", plantio.TalhaoId));
        command.Parameters.Add(new OracleParameter("CulturaId", plantio.CulturaId));
        command.Parameters.Add(new OracleParameter("DataPlantio", plantio.DataPlantio));
        command.Parameters.Add(new OracleParameter("DataPrevistaColheita", (object?)plantio.DataPrevistaColheita ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("ProdutividadeEstimada", (object?)plantio.ProdutividadeEstimada ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("CustoProducao", (object?)plantio.CustoProducao ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Status", plantio.Status));
        command.Parameters.Add(new OracleParameter("Observacoes", (object?)plantio.Observacoes ?? DBNull.Value));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Plantio plantio)
    {
        string sql = "UPDATE PLANTIOS SET TALHAOID = :TalhaoId, CULTURAID = :CulturaId, DATAPLANTIO = :DataPlantio, DATAPREVISTACOLHEITA = :DataPrevistaColheita, PRODUTIVIDADEESTIMADA = :ProdutividadeEstimada, CUSTOPRODUCAO = :CustoProducao, STATUS = :Status, OBSERVACOES = :Observacoes WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("TalhaoId", plantio.TalhaoId));
        command.Parameters.Add(new OracleParameter("CulturaId", plantio.CulturaId));
        command.Parameters.Add(new OracleParameter("DataPlantio", plantio.DataPlantio));
        command.Parameters.Add(new OracleParameter("DataPrevistaColheita", (object?)plantio.DataPrevistaColheita ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("ProdutividadeEstimada", (object?)plantio.ProdutividadeEstimada ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("CustoProducao", (object?)plantio.CustoProducao ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Status", plantio.Status));
        command.Parameters.Add(new OracleParameter("Observacoes", (object?)plantio.Observacoes ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Id", plantio.Id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        string sql = "DELETE FROM PLANTIOS WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }
}