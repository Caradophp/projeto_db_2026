using Oracle.ManagedDataAccess.Client;

namespace projeto.Repository;

public class CodeRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public void SaveCode(int code, int idUser)
    {
        string sql = "INSERT INTO CODES_SENT (CODE, ID_USER) VALUES(:Code, :IdUser);";
        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add("Code", code);
        command.Parameters.Add("IdUser", idUser);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public bool CheckIfCodeIsValid(int code, int idUser)
    {
        string sql = "SELECT 1 FROM CODES_SENT WHERE CODE = :Code AND ID_USER = :IdUser";
        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add("Code", code);
        command.Parameters.Add("IdUser", idUser);

        connection.Open();
        using var reader = command.ExecuteReader();

        return reader.Read();
    }
}
