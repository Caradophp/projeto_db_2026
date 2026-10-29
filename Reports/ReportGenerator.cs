using Oracle.ManagedDataAccess.Client;

namespace projeto.Reports;

public sealed class ReportGenerator(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException(
            "A string de conexão 'OracleDb' não foi configurada.");

    public ReportResult Generate(
        ReportTemplate template,
        string email,
        ReportFilters filters)
    {
        var rows = new List<IReadOnlyDictionary<string, object?>>();

        using var connection = new OracleConnection(_connectionString);
        using var command = new OracleCommand(template.Query, connection)
        {
            BindByName = true
        };

        command.Parameters.Add("Email", OracleDbType.Varchar2).Value = email;
        AddParameterIfUsed(command, "PropertyId", OracleDbType.Int32, filters.PropertyId);
        AddParameterIfUsed(command, "CultureId", OracleDbType.Int32, filters.CultureId);
        AddParameterIfUsed(command, "TalhaoId", OracleDbType.Int32, filters.TalhaoId);
        AddParameterIfUsed(command, "StartDate", OracleDbType.Date, filters.StartDate?.Date);
        AddParameterIfUsed(
            command,
            "EndDateExclusive",
            OracleDbType.Date,
            filters.EndDate?.Date.AddDays(1));

        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < reader.FieldCount; index++)
            {
                var value = reader.GetValue(index);
                row[reader.GetName(index).ToLowerInvariant()] =
                    value == DBNull.Value ? null : value;
            }

            rows.Add(row);
        }

        return new ReportResult(
            template.Id,
            template.Title,
            template.Description,
            template.Columns,
            rows,
            template.Chart);
    }

    private static void AddParameterIfUsed(
        OracleCommand command,
        string name,
        OracleDbType type,
        object? value)
    {
        if (command.CommandText.Contains($":{name}", StringComparison.Ordinal))
        {
            command.Parameters.Add(name, type).Value = value ?? DBNull.Value;
        }
    }
}
