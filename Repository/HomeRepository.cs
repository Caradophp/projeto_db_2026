using Oracle.ManagedDataAccess.Client;

namespace projeto.Repository;

public class HomeRepository(IConfiguration configuration)
{
     private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    
    public Dictionary<string, int> GetRegisterNumbersPerTable()
    {
        try
        {
            Dictionary<string, int> dictionaryToReturn = [];

            string sql = "SELECT * FROM TABLE(GET_TABLE_COUNTS());";

            using var connection = new OracleConnection(_connectionString);
            using var command = new OracleCommand(sql, connection);

            connection.Open();
            OracleDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string tableName = reader.GetString(0);
                int rowCount = reader.GetInt16(1);
                dictionaryToReturn.Add(tableName, rowCount);
            }

            return dictionaryToReturn;
            
        } catch (Exception)
        {
            throw;
        }
    } 

}