CREATE OR REPLACE VIEW vw_rel_custos_plantio AS
SELECT
    p.user_id,
    p.id AS propriedade_id,
    p.nome AS propriedade,
    t.id AS talhao_id,
    t.nome AS talhao,
    c.id AS cultura_id,
    c.nome AS cultura,
    pl.id AS plantio_id,
    pl.data_plantio,
    TRUNC(pl.data_plantio, 'MM') AS mes_referencia,
    EXTRACT(YEAR FROM pl.data_plantio) AS ano_referencia,
    pl.status
FROM plantio pl
JOIN talhao t
    ON t.id = pl.talhao_id
JOIN propriedade p
    ON p.id = t.propriedade_id
JOIN cultura c
    ON c.id = pl.cultura_id
LEFT JOIN (
    SELECT
        plantio_id
    FROM colheita
    GROUP BY plantio_id
) h
    ON h.plantio_id = pl.id;
 