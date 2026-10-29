namespace projeto.Reports;

public sealed record ReportColumn(string Key, string Label, string Format = "text");

public sealed record ReportChart(
    string LabelKey,
    string? ValueKey,
    string? PeriodKey = null,
    string? UniqueKey = null);

public sealed record ReportTemplate(
    string Id,
    string Title,
    string Description,
    string Query,
    IReadOnlyList<ReportColumn> Columns,
    IReadOnlyList<string> Filters,
    ReportChart? Chart = null);

public sealed record ReportCatalogItem(
    string Id,
    string Title,
    string Description,
    IReadOnlyList<string> Filters);

public sealed record ReportFilterOption(int Id, string Name);

public sealed record ReportFilterOptions(
    IReadOnlyList<ReportCatalogItem> Reports,
    IReadOnlyList<ReportFilterOption> Properties,
    IReadOnlyList<ReportFilterOption> Cultures,
    IReadOnlyList<ReportFilterOption> Talhoes);

public sealed record ReportResult(
    string Id,
    string Title,
    string Description,
    IReadOnlyList<ReportColumn> Columns,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows,
    ReportChart? Chart);

public static class Templates
{
    public static IReadOnlyDictionary<string, ReportTemplate> All { get; } =
        new Dictionary<string, ReportTemplate>(StringComparer.OrdinalIgnoreCase)
        {
            ["properties"] = new(
                "properties",
                "Propriedades e talhões",
                "Distribuição das áreas das propriedades do usuário autenticado. O percentual de utilização é calculado pela área ocupada pelos talhões.",
                """
                SELECT
                    r.user_id,
                    r.propriedade_id AS property_id,
                    r.produtor AS producer_name,
                    r.propriedade AS property_name,
                    r.area_total AS total_area,
                    r.qtd_talhoes AS plot_count,
                    r.area_talhoes AS occupied_area,
                    r.area_livre AS available_area,
                    r.pct_utilizacao AS utilization_pct,
                    CASE
                        WHEN r.area_talhoes > r.area_total THEN 'Sim'
                        ELSE 'Não'
                    END AS area_overallocated
                FROM vw_rel_propriedades_talhoes r
                WHERE r.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
                  AND (:PropertyId IS NULL OR r.propriedade_id = :PropertyId)
                  AND (
                      :TalhaoId IS NULL
                      OR EXISTS (
                          SELECT 1
                          FROM talhao t
                          WHERE t.propriedade_id = r.propriedade_id
                            AND t.id = :TalhaoId
                      )
                  )
                ORDER BY r.propriedade
                """,
                [
                    new("producer_name", "Produtor"),
                    new("property_name", "Propriedade"),
                    new("total_area", "Área total (ha)", "number"),
                    new("plot_count", "Quantidade de talhões", "number"),
                    new("occupied_area", "Área ocupada (ha)", "number"),
                    new("available_area", "Área livre (ha)", "number"),
                    new("utilization_pct", "Utilização (%)", "percent"),
                    new("area_overallocated", "Área dos talhões excede a propriedade?")
                ],
                ["property", "talhao"],
                new("property_name", "utilization_pct")),

            ["plantings"] = new(
                "plantings",
                "Plantios e situação das culturas",
                "Plantios e previsões consultados na view vw_rel_plantios. Produtividade e custo são estimativas registradas no plantio.",
                """
                SELECT
                    r.propriedade_id AS property_id,
                    r.propriedade AS property_name,
                    r.talhao_id AS plot_id,
                    r.talhao AS plot_name,
                    r.cultura_id AS culture_id,
                    r.cultura AS culture_name,
                    r.plantio_id AS planting_id,
                    r.data_plantio AS planting_date,
                    r.data_prevista_colheita AS expected_harvest_date,
                    r.produtividade_estimada AS estimated_productivity,
                    r.custo_producao AS estimated_cost,
                    r.status AS planting_status
                FROM vw_rel_plantios r
                WHERE r.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
                  AND (:PropertyId IS NULL OR r.propriedade_id = :PropertyId)
                  AND (:CultureId IS NULL OR r.cultura_id = :CultureId)
                  AND (:TalhaoId IS NULL OR r.talhao_id = :TalhaoId)
                  AND (:StartDate IS NULL OR r.data_plantio >= :StartDate)
                  AND (:EndDateExclusive IS NULL OR r.data_plantio < :EndDateExclusive)
                ORDER BY r.data_plantio DESC, r.plantio_id
                """,
                [
                    new("property_name", "Propriedade"),
                    new("plot_name", "Talhão"),
                    new("culture_name", "Cultura"),
                    new("planting_date", "Data do plantio", "date"),
                    new("expected_harvest_date", "Colheita prevista", "date"),
                    new("estimated_productivity", "Produtividade estimada", "number"),
                    new("estimated_cost", "Custo estimado", "currency"),
                    new("planting_status", "Status")
                ],
                ["property", "culture", "talhao", "period"],
                new("planting_status", null)),

            ["harvests"] = new(
                "harvests",
                "Colheitas e produção agrícola",
                "Produção por colheita, com totais mensais e anuais por cultura. As unidades de produção dependem dos dados registrados no banco.",
                """
                SELECT
                    r.propriedade_id AS property_id,
                    r.propriedade AS property_name,
                    r.talhao_id AS plot_id,
                    r.talhao AS plot_name,
                    r.cultura_id AS culture_id,
                    r.cultura AS culture_name,
                    r.plantio_id AS planting_id,
                    r.colheita_id AS harvest_id,
                    r.data_colheita AS harvest_date,
                    r.mes_referencia AS month_reference,
                    r.ano_referencia AS reference_year,
                    r.producao_total AS total_production,
                    r.produtividade_estimada AS estimated_productivity,
                    SUM(r.producao_total) OVER (
                        PARTITION BY r.cultura_id, r.ano_referencia, r.mes_referencia
                    ) AS monthly_culture_production,
                    SUM(r.producao_total) OVER (
                        PARTITION BY r.cultura_id, r.ano_referencia
                    ) AS annual_culture_production
                FROM vw_rel_colheitas r
                WHERE r.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
                  AND (:PropertyId IS NULL OR r.propriedade_id = :PropertyId)
                  AND (:CultureId IS NULL OR r.cultura_id = :CultureId)
                  AND (:TalhaoId IS NULL OR r.talhao_id = :TalhaoId)
                  AND (:StartDate IS NULL OR r.data_colheita >= :StartDate)
                  AND (:EndDateExclusive IS NULL OR r.data_colheita < :EndDateExclusive)
                ORDER BY r.data_colheita DESC, r.colheita_id
                """,
                [
                    new("culture_name", "Cultura"),
                    new("plot_name", "Talhão"),
                    new("harvest_date", "Data da colheita", "date"),
                    new("month_reference", "Mês de referência", "date"),
                    new("reference_year", "Ano de referência", "number"),
                    new("total_production", "Produção da colheita", "number"),
                    new("estimated_productivity", "Produtividade estimada", "number"),
                    new("monthly_culture_production", "Produção da cultura no mês", "number"),
                    new("annual_culture_production", "Produção da cultura no ano", "number")
                ],
                ["property", "culture", "talhao", "period"],
                new(
                    "culture_name",
                    "monthly_culture_production",
                    "month_reference",
                    "monthly_culture_production")),

            ["inputs"] = new(
                "inputs",
                "Estoque e aplicação de insumos",
                "Mostra o estoque global cadastrado e as aplicações nas propriedades do usuário autenticado. A tabela INSUMO não possui vínculo com proprietário; por isso, o estoque é compartilhado e as aplicações são limitadas às propriedades do usuário.",
                """
                SELECT
                    i.insumo_id AS input_id,
                    i.insumo AS input_name,
                    i.tipo AS input_type,
                    i.unidade_medida AS unit_name,
                    i.estoque AS stock_quantity,
                    i.estoque_minimo AS minimum_stock,
                    i.situacao_estoque AS stock_status,
                    a.propriedade_id AS property_id,
                    a.propriedade AS property_name,
                    a.talhao_id AS plot_id,
                    a.talhao AS plot_name,
                    a.data_aplicacao AS application_date,
                    a.quantidade_aplicada AS applied_quantity,
                    SUM(a.quantidade_aplicada) OVER (
                        PARTITION BY i.insumo_id, a.mes_referencia
                    ) AS monthly_applied_quantity,
                    SUM(a.quantidade_aplicada) OVER (
                        PARTITION BY i.insumo_id, EXTRACT(YEAR FROM a.data_aplicacao)
                    ) AS annual_applied_quantity
                FROM vw_rel_estoque_insumos i
                LEFT JOIN vw_rel_aplicacao_insumos a
                    ON a.insumo_id = i.insumo_id
                    AND a.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
                    AND (:PropertyId IS NULL OR a.propriedade_id = :PropertyId)
                    AND (:TalhaoId IS NULL OR a.talhao_id = :TalhaoId)
                    AND (:StartDate IS NULL OR a.data_aplicacao >= :StartDate)
                    AND (:EndDateExclusive IS NULL OR a.data_aplicacao < :EndDateExclusive)
                WHERE (:PropertyId IS NULL OR EXISTS (
                    SELECT 1
                    FROM vw_rel_aplicacao_insumos scoped
                    WHERE scoped.insumo_id = i.insumo_id
                      AND scoped.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
                      AND scoped.propriedade_id = :PropertyId
                      AND (:TalhaoId IS NULL OR scoped.talhao_id = :TalhaoId)
                ))
                  AND (:TalhaoId IS NULL OR EXISTS (
                    SELECT 1
                    FROM vw_rel_aplicacao_insumos scoped
                    WHERE scoped.insumo_id = i.insumo_id
                      AND scoped.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
                      AND scoped.talhao_id = :TalhaoId
                ))
                ORDER BY a.data_aplicacao DESC NULLS LAST, i.insumo
                """,
                [
                    new("input_name", "Insumo"),
                    new("input_type", "Tipo"),
                    new("unit_name", "Unidade"),
                    new("stock_quantity", "Estoque disponível", "number"),
                    new("minimum_stock", "Estoque mínimo", "number"),
                    new("stock_status", "Situação do estoque"),
                    new("applied_quantity", "Quantidade aplicada", "number"),
                    new("monthly_applied_quantity", "Aplicado no mês", "number"),
                    new("annual_applied_quantity", "Aplicado no ano", "number"),
                    new("application_date", "Data da aplicação", "date"),
                    new("property_name", "Propriedade"),
                    new("plot_name", "Talhão")
                ],
                ["property", "talhao", "period"],
                new("input_name", "stock_quantity", null, "input_id")),

            ["costs"] = new(
                "costs",
                "Custos e eficiência da produção",
                "Compara custos estimados dos plantios e a produção colhida. O custo por unidade é omitido porque o banco não registra unidades de produtividade e produção para confirmar compatibilidade; lucro real também depende de receitas e despesas efetivas.",
                """
                SELECT
                    r.propriedade_id AS property_id,
                    r.propriedade AS property_name,
                    r.talhao_id AS plot_id,
                    r.talhao AS plot_name,
                    r.cultura_id AS culture_id,
                    r.cultura AS culture_name,
                    r.plantio_id AS planting_id,
                    r.data_plantio AS planting_date,
                    r.status AS planting_status,
                    pl.custo_producao AS estimated_cost,
                    NVL(h.harvested_production, 0) AS harvested_production,
                    CAST(NULL AS NUMBER) AS estimated_cost_per_unit,
                    SUM(NVL(pl.custo_producao, 0)) OVER (
                        PARTITION BY r.cultura_id, r.ano_referencia
                    ) AS culture_period_cost
                FROM vw_rel_custos_plantio r
                JOIN plantio pl ON pl.id = r.plantio_id
                LEFT JOIN (
                    SELECT plantio_id, SUM(producao_total) AS harvested_production
                    FROM colheita
                    GROUP BY plantio_id
                ) h ON h.plantio_id = r.plantio_id
                WHERE r.user_id = (SELECT u.id FROM users u WHERE u.email = :Email)
                  AND (:PropertyId IS NULL OR r.propriedade_id = :PropertyId)
                  AND (:CultureId IS NULL OR r.cultura_id = :CultureId)
                  AND (:TalhaoId IS NULL OR r.talhao_id = :TalhaoId)
                  AND (:StartDate IS NULL OR r.data_plantio >= :StartDate)
                  AND (:EndDateExclusive IS NULL OR r.data_plantio < :EndDateExclusive)
                ORDER BY r.data_plantio DESC, r.plantio_id
                """,
                [
                    new("property_name", "Propriedade"),
                    new("plot_name", "Talhão"),
                    new("culture_name", "Cultura"),
                    new("planting_id", "Plantio", "number"),
                    new("planting_date", "Data do plantio", "date"),
                    new("estimated_cost", "Custo estimado", "currency"),
                    new("harvested_production", "Produção colhida", "number"),
                    new("estimated_cost_per_unit", "Custo por unidade", "currency"),
                    new("culture_period_cost", "Custo da cultura no período", "currency"),
                    new("planting_status", "Status")
                ],
                ["property", "culture", "talhao", "period"],
                new("planting_id", "estimated_cost"))
        };
}
