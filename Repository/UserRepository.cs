using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using projeto.Models.Enums;

namespace projeto.Repository;

public class UserRepository(IConfiguration configuration)
{
    
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public IActionResult CreateUser(User user)
    {

        string sql = "INSERT INTO users (name, email, password) VALUES (:Name, :Email, :Password);";

        using (var connection = new OracleConnection(_connectionString))
        {
            using var command = new OracleCommand(sql, connection);
            command.Parameters.Add(new OracleParameter("Name", user.Name));
            command.Parameters.Add(new OracleParameter("Email", user.Email));
            command.Parameters.Add(new OracleParameter("Password", user.Password));

            connection.Open();
            command.ExecuteNonQuery();
        }

        return new RedirectToActionResult("Index", "User", null);
    }

    public User? CheckUserLogin(string email, string password)
    {
        string sql = "SELECT u.id, u.name, u.email, u.password, u.status, u.created_at FROM users u WHERE u.email = :Email";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Email", email));

        connection.Open();

        using OracleDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            User user = new()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3),
                Status = Enum.Parse<UserStatusEnum>(reader.GetString(4), ignoreCase: true),
                CreatedAt = reader.GetDateTime(5)
            };

            return user;
            
        }

        return null;
    }

    public bool ExistsByEmail(string email)
    {
        string sql = "SELECT 1 FROM users u WHERE u.email = :Email";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        
        command.Parameters.Add("Email", email);

        connection.Open();
        OracleDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return true;
        }

        return false;
    }
}