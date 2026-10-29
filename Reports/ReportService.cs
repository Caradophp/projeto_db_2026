using Oracle.ManagedDataAccess.Client;

namespace projeto.Reports;

public sealed class ReportService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("OracleDb")
        ?? throw new InvalidOperationException(
            "A string de conexão 'OracleDb' não foi configurada.");
    private readonly ReportGenerator _reportGenerator = new(configuration);

    public ReportFilterOptions GetFilterOptions(string email)
    {
        using var connection = new OracleConnection(_connectionString);
        connection.Open();

        var properties = ReadOptions(
            connection,
            """
            SELECT p.id, p.nome
            FROM propriedade p
            WHERE p.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
            ORDER BY p.nome
            """,
            email);

        var talhoes = ReadOptions(
            connection,
            """
            SELECT t.id, t.nome
            FROM talhao t
            JOIN propriedade p ON p.id = t.propriedade_id
            WHERE p.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
            ORDER BY t.nome
            """,
            email);

        var cultures = ReadOptions(
            connection,
            """
            SELECT DISTINCT c.id, c.nome
            FROM cultura c
            JOIN plantio pl ON pl.cultura_id = c.id
            JOIN talhao t ON t.id = pl.talhao_id
            JOIN propriedade p ON p.id = t.propriedade_id
            WHERE p.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
            ORDER BY c.nome
            """,
            email);

        var reports = Templates.All.Values
            .Select(template => new ReportCatalogItem(
                template.Id,
                template.Title,
                template.Description,
                template.Filters))
            .ToList();

        return new ReportFilterOptions(reports, properties, cultures, talhoes);
    }

    public ReportResult GetReport(string reportId, string email, ReportFilters filters)
    {
        if (!Templates.All.TryGetValue(reportId, out var template))
        {
            throw new KeyNotFoundException($"Relatório não encontrado: {reportId}");
        }

        ValidateFilters(template, filters);

        return _reportGenerator.Generate(template, email, filters);
    }

    private static IReadOnlyList<ReportFilterOption> ReadOptions(
        OracleConnection connection,
        string sql,
        string email)
    {
        using var command = new OracleCommand(sql, connection)
        {
            BindByName = true
        };
        command.Parameters.Add("Email", OracleDbType.Varchar2).Value = email;

        var options = new List<ReportFilterOption>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            options.Add(new ReportFilterOption(
                Convert.ToInt32(reader.GetValue(0)),
                reader.GetString(1)));
        }

        return options;
    }

    private static void ValidateFilters(ReportTemplate template, ReportFilters filters)
    {
        if (filters.PropertyId is <= 0
            || filters.CultureId is <= 0
            || filters.TalhaoId is <= 0)
        {
            throw new ArgumentException("Os identificadores dos filtros devem ser positivos.");
        }

        if (filters.PropertyId.HasValue && !template.Filters.Contains("property"))
        {
            throw new ArgumentException("Este relatório não aceita filtro por propriedade.");
        }

        if (filters.CultureId.HasValue && !template.Filters.Contains("culture"))
        {
            throw new ArgumentException("Este relatório não aceita filtro por cultura.");
        }

        if (filters.TalhaoId.HasValue && !template.Filters.Contains("talhao"))
        {
            throw new ArgumentException("Este relatório não aceita filtro por talhão.");
        }

        if ((filters.StartDate.HasValue || filters.EndDate.HasValue)
            && !template.Filters.Contains("period"))
        {
            throw new ArgumentException("Este relatório não aceita filtro por período.");
        }

        if (filters.StartDate.HasValue
            && filters.EndDate.HasValue
            && filters.EndDate.Value.Date < filters.StartDate.Value.Date)
        {
            throw new ArgumentException(
                "A data final do período não pode ser anterior à data inicial.");
        }
    }
}

public sealed class ReportFilters
{
    public int? PropertyId { get; set; }
    public int? CultureId { get; set; }
    public int? TalhaoId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
