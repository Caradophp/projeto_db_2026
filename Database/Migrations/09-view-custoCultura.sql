CREATE OR REPLACE VIEW vw_rel_custos_cultura_periodo AS
SELECT
    user_id,
    propriedade_id,
    propriedade,
    cultura_id,
    cultura,
    ano_referencia,
    COUNT(*)                                AS qtd_plantios,
    SUM(custo_producao_estimado)            AS custo_total,
    SUM(producao_total)                     AS producao_total,
    ROUND(
        SUM(custo_producao_estimado) FILTER (WHERE tem_colheita)
        / NULLIF(SUM(producao_total) FILTER (WHERE tem_colheita), 0), 4
    )                                       AS custo_por_unidade
FROM vw_rel_custos_plantio
GROUP BY user_id, propriedade_id, propriedade, cultura_id, cultura, ano_referencia;