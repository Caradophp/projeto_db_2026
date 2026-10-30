using Oracle.ManagedDataAccess.Client;
using projeto.Models;

namespace projeto.Repository;

public class ColheitaRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException(
            "A string de conexão 'OracleDb' não foi configurada."
        );

    public IEnumerable<Colheita> GetAll()
    {
        var lista = new List<Colheita>();

        const string sql = @"
            SELECT
                ID,
                PLANTIO_ID,
                DATA_COLHEITA,
                PRODUCAO_TOTAL,
                OBSERVACOES
            FROM COLHEITA
            ORDER BY DATA_COLHEITA DESC, ID DESC";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Colheita
            {
                Id = Convert.ToInt32(reader.GetValue(0)),
                PlantioId = Convert.ToInt32(reader.GetValue(1)),
                DataColheita = reader.GetDateTime(2),
                ProducaoTotal = reader.GetDecimal(3),
                Observacoes = reader.IsDBNull(4)
                    ? null
                    : reader.GetString(4)
            });
        }

        return lista;
    }

    public Colheita? GetById(int id)
    {
        const string sql = @"
            SELECT
                ID,
                PLANTIO_ID,
                DATA_COLHEITA,
                PRODUCAO_TOTAL,
                OBSERVACOES
            FROM COLHEITA
            WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("Id", id));

        connection.Open();

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new Colheita
        {
            Id = Convert.ToInt32(reader.GetValue(0)),
            PlantioId = Convert.ToInt32(reader.GetValue(1)),
            DataColheita = reader.GetDateTime(2),
            ProducaoTotal = reader.GetDecimal(3),
            Observacoes = reader.IsDBNull(4)
                ? null
                : reader.GetString(4)
        };
    }

    public bool Create(Colheita colheita)
    {
        const string sql = @"
            INSERT INTO COLHEITA
            (
                PLANTIO_ID,
                DATA_COLHEITA,
                PRODUCAO_TOTAL,
                OBSERVACOES
            )
            VALUES
            (
                :PlantioId,
                :DataColheita,
                :ProducaoTotal,
                :Observacoes
            )";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(
            new OracleParameter("PlantioId", colheita.PlantioId)
        );

        command.Parameters.Add(
            new OracleParameter("DataColheita", colheita.DataColheita)
        );

        command.Parameters.Add(
            new OracleParameter("ProducaoTotal", colheita.ProducaoTotal)
        );

        command.Parameters.Add(
            new OracleParameter(
                "Observacoes",
                (object?)colheita.Observacoes ?? DBNull.Value
            )
        );

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Colheita colheita)
    {
        const string sql = @"
            UPDATE COLHEITA
            SET
                PLANTIO_ID = :PlantioId,
                DATA_COLHEITA = :DataColheita,
                PRODUCAO_TOTAL = :ProducaoTotal,
                OBSERVACOES = :Observacoes
            WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(
            new OracleParameter("PlantioId", colheita.PlantioId)
        );

        command.Parameters.Add(
            new OracleParameter("DataColheita", colheita.DataColheita)
        );

        command.Parameters.Add(
            new OracleParameter("ProducaoTotal", colheita.ProducaoTotal)
        );

        command.Parameters.Add(
            new OracleParameter(
                "Observacoes",
                (object?)colheita.Observacoes ?? DBNull.Value
            )
        );

        command.Parameters.Add(
            new OracleParameter("Id", colheita.Id)
        );

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        const string sql = @"
            DELETE FROM COLHEITA
            WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(
            new OracleParameter("Id", id)
        );

        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}