using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using Microsoft.Extensions.Configuration;

namespace projeto.Repository;

public class ColheitaRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IEnumerable<Colheita> GetAll()
    {
        var colheitas = new List<Colheita>();
        string sql = "SELECT ID, PLANTIOID, DATACOLHEITA, PRODUCAOTOTAL, OBSERVACOES FROM COLHEITAS";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            colheitas.Add(new Colheita
            {
                Id = reader.GetInt32(0),
                PlantioId = reader.GetInt32(1),
                DataColheita = reader.GetDateTime(2),
                ProducaoTotal = reader.GetDecimal(3),
                Observacoes = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }
        return colheitas;
    }

    public bool Create(Colheita colheita)
    {
        string sql = "INSERT INTO COLHEITAS (PLANTIOID, DATACOLHEITA, PRODUCAOTOTAL, OBSERVACOES) VALUES (:PlantioId, :Data, :Producao, :Obs)";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("PlantioId", colheita.PlantioId));
        command.Parameters.Add(new OracleParameter("Data", colheita.DataColheita));
        command.Parameters.Add(new OracleParameter("Producao", colheita.ProducaoTotal));
        command.Parameters.Add(new OracleParameter("Obs", (object?)colheita.Observacoes ?? DBNull.Value));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Update(Colheita colheita)
    {
        string sql = "UPDATE COLHEITAS SET PLANTIOID = :PlantioId, DATACOLHEITA = :Data, PRODUCAOTOTAL = :Producao, OBSERVACOES = :Obs WHERE ID = :Id";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("PlantioId", colheita.PlantioId));
        command.Parameters.Add(new OracleParameter("Data", colheita.DataColheita));
        command.Parameters.Add(new OracleParameter("Producao", colheita.ProducaoTotal));
        command.Parameters.Add(new OracleParameter("Obs", (object?)colheita.Observacoes ?? DBNull.Value));
        command.Parameters.Add(new OracleParameter("Id", colheita.Id));

        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        string sql = "DELETE FROM COLHEITAS WHERE ID = :Id";
        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Id", id));
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }
}
