CREATE OR REPLACE VIEW vw_rel_custos_cultura_periodo AS
SELECT
    user_id,
    propriedade_id,
    propriedade,
    cultura_id,
    cultura,
    ano_referencia,
    COUNT(*)                                AS qtd_plantios
FROM vw_rel_custos_plantio
GROUP BY user_id, propriedade_id, propriedade, cultura_id, cultura, ano_referencia;