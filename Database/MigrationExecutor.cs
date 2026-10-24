using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using projeto.Repository;
using projeto.Util;

namespace projeto.Database;

public class MigrationExecutor(
    IConfiguration configuration,
    IWebHostEnvironment environment,
    ILogger<MigrationExecutor> logger,
    UtilRepository utilRepository)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException(
            "Connection string 'OracleDb' was not configured.");

    private readonly string _migrationPath =
        Path.Combine(
            environment.ContentRootPath,
            "Database",
            "Migrations");

    private readonly UtilRepository _utilRepository = utilRepository;

    public void Init()
    {
        logger.LogInformation("MigrationExecutor.Init started.");
        logger.LogInformation("Migration directory: {Path}", _migrationPath);

        if (!Directory.Exists(_migrationPath))
        {
            throw new DirectoryNotFoundException(
                $"Migration directory not found: {_migrationPath}");
        }

        var migrations = Directory
            .GetFiles(_migrationPath, "*.sql")
            .OrderBy(path => path)
            .ToList();

        logger.LogInformation(
            "Found {Count} migration files.", migrations.Count);

        using var connection = new OracleConnection(_connectionString);
        connection.Open();

        logger.LogInformation("Oracle connection opened.");

        using var commandInsert = new OracleCommand("INSERT INTO migrations (script_name, done) VALUES (:ScriptName, :Done)", connection);

        foreach (var migration in migrations)
        {
            try {
                logger.LogInformation(
                    "Executing migration: {Migration}", migration);

                Parametro parametro = new()
                {
                    Chave = "ScriptName",
                    Valor = migration
                };
                List<object[]> migrate =_utilRepository.ListBy("migrations", "t.script_name = :ScriptName AND t.done = 1", [parametro]);

                if (migrate.Count == 0)
                {
                    var sql = File.ReadAllText(migration);

                    if (sql.Contains("FUNCTION") || sql.Contains("function"))
                    {
                        sql = sql.Trim();

                        using var command = new OracleCommand(sql, connection);
                        command.ExecuteNonQuery();

                    } else
                    {
                        foreach (var statement in sql.Split(';'))
                        {
                            var trimmedSql = statement.Trim();

                            if (string.IsNullOrWhiteSpace(trimmedSql))
                                continue;

                                using var command = new OracleCommand(
                                    trimmedSql, connection);

                                command.ExecuteNonQuery();
                        }
                    }

                    // Resolve erro ao rodar projeto
                    // TODO: Encontrar forma de remover concatenação
                    string insertSql = "INSERT INTO migrations (script_name, done) VALUES ('" + migration + "', " + 1 + ")";
                    
                    using var customCommand = new OracleCommand(insertSql, connection);

                    customCommand.ExecuteNonQuery();

                    logger.LogInformation(
                        "Migration completed: {Migration}", migration);
                }
            } catch (OracleException)
            {
                Parametro parametro = new()
                {
                    Chave = "ScriptName",
                    Valor = migration
                };
                List<object[]> migrate =_utilRepository.ListBy("migrations", "t.script_name = :ScriptName AND t.done = 0", [parametro]);

                if (migrate.Count == 0)
                {
                    commandInsert.Parameters.Add(new OracleParameter("ScriptName", migration));
                    commandInsert.Parameters.Add(new OracleParameter("Done", OracleDbType.Int32)
                    {
                        Value = 0
                    });

                    // connection.Open();
                    commandInsert.ExecuteNonQuery();
                }
            }
        }

        logger.LogInformation("MigrationExecutor.Init completed.");
    }
}