using Oracle.ManagedDataAccess.Client;
using projeto.Models;
using projeto.Util;
using System.Data;
using System.Security;

namespace projeto.Repository;

public class UtilRepository (IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException("A string de conexão 'OracleDb' não foi configurada.");

    // White-list de tabelas permitidas para consultas dinâmicas
    private static readonly HashSet<string> AllowedTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "USERS",
        "CODES_SENT",
        "COLHEITAS",
        "CULTURAS",
        "TALHOES"
    };

    // Lista de palavras-chave proibidas na cláusula WHERE para evitar ataques de modificação
    private static readonly string[] ProhibitedKeywords =
    {
        "DROP", "DELETE", "UPDATE", "INSERT", "TRUNCATE", "ALTER", "EXEC", "GRANT", "REVOKE"
    };

    /// <summary>
    /// Retorna uma lista de resultados materializada.
    /// Implementa validação de White-list para prevenir SQL Injection em nomes de tabelas.
    /// </summary>
    public List<object[]> ListBy(string tabela, string condicao, Parametro[] parametros)
    {
        // 1. Validação de Tabela (White-list)
        if (!AllowedTables.Contains(tabela))
        {
            throw new SecurityException($"Acesso não autorizado à tabela: {tabela}");
        }

        // 2. Sanitização básica da condição (Prevenção de comandos aninhados)
        if (!string.IsNullOrEmpty(condicao))
        {
            foreach (var keyword in ProhibitedKeywords)
            {
                if (condicao.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    throw new SecurityException($"A condição fornecida contém palavras-chave proibidas: {keyword}");
                }
            }
        }

        string sql = $"SELECT t.* FROM {tabela} t WHERE {condicao}";

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(sql, connection);

        if (parametros != null)
        {
            foreach (var p in parametros)
            {
                command.Parameters.Add(p.Chave, p.Valor);
            }
        }

        connection.Open();
        using var reader = command.ExecuteReader();

        var results = new List<object[]>();
        while (reader.Read())
        {
            object[] row = new object[reader.FieldCount];
            reader.GetValues(row);
            results.Add(row);
        }

        return results;
    }
}
