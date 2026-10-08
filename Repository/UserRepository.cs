using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using projeto.Models.Enums;
using projeto.Service;
using Microsoft.Extensions.Logging;

namespace projeto.Repository;

public class UserRepository(IConfiguration configuration, EncryptionService encryptionService, ILogger<UserRepository> logger)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    public bool CreateUser(User user)
    {
        string hashedPassword = encryptionService.HashPassword(user.Password);

        string sql = "INSERT INTO users (name, email, password) VALUES (:Name, :Email, :Password);";

        using (var connection = new OracleConnection(_connectionString))
        {
            using var command = new OracleCommand(sql, connection);
            command.Parameters.Add(new OracleParameter("Name", user.Name));
            command.Parameters.Add(new OracleParameter("Email", user.Email));
            command.Parameters.Add(new OracleParameter("Password", hashedPassword));

            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }

    public User? CheckUserLogin(string email, string password)
    {
        string sql = "SELECT u.id, u.name, u.email, u.password, u.status, u.created_at FROM users u WHERE u.email = :Email";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Email", email));

        connection.Open();

        using OracleDataReader reader = command.ExecuteReader();
        if (reader.Read())
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

            if (encryptionService.VerifyPassword(password, user.Password))
            {
                return user;
            }

            if (user.Password == password)
            {
                ChangePass(user.Email, password);
                return user;
            }
        }

        logger.LogWarning("Tentativa de login falhou para o usuário: {Email}", email);
        return null;
    }

    public User? FindByEmail(string email)
    {
        string sql = "SELECT id, name, email, password FROM users u WHERE u.email = :Email";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Email", email));

        connection.Open();
        using OracleDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new User
            {
                Id = (int)reader.GetInt64(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3)
            };
        }

        return null;
    }

    public bool ExistsByEmail(string email)
    {
        string sql = "SELECT 1 FROM users u WHERE u.email = :Email";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(new OracleParameter("Email", email));

        connection.Open();
        using OracleDataReader reader = command.ExecuteReader();

        return reader.Read();
    }

    public void ChangePass(string email, string newPassword)
    {
        User? userExists = this.FindByEmail(email) ?? throw new Exception("E-mail informado é inválido");

        string hashedPassword = encryptionService.HashPassword(newPassword);

        string sql = "UPDATE USERS SET PASSWORD = :Password WHERE ID = :IdUser";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        command.Parameters.Add(new OracleParameter("Password", hashedPassword));
        command.Parameters.Add(new OracleParameter("IdUser", userExists.Id));

        connection.Open();
        command.ExecuteNonQuery();
    }
}
