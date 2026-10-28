using Oracle.ManagedDataAccess.Client;

namespace projeto.Repository;

public class AuditoriaRepository(IConfiguration configuration)
{
    
     private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public void RegisterAudit(string action, int userId)
    {
        string sql = "INSERT INTO auditoria (acao, user_id) VALUES (:Action, :UserId)";

        try
        {
            using var connection = new OracleConnection(_connectionString);
            using var command = new OracleCommand(sql, connection);

            command.Parameters.Add(new OracleParameter("Action", action));
            command.Parameters.Add(new OracleParameter("UserId", userId));

            connection.Open();
            command.ExecuteNonQuery();
        } catch (OracleException)
        {
            throw;
        }
    }
}