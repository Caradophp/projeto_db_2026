CREATE OR REPLACE VIEW vw_rel_plantios AS
SELECT
    p.user_id,
    p.id                         AS propriedade_id,
    p.nome                       AS propriedade,
    t.id                         AS talhao_id,
    t.nome                       AS talhao,
    t.area                       AS area_talhao,
    c.id                         AS cultura_id,
    c.nome                       AS cultura,
    pl.id                        AS plantio_id,
    pl.data_plantio,
    pl.data_prevista_colheita,
    pl.produtividade_estimada,
    pl.custo_producao_estimado,
    pl.status,
    -- dias até a colheita prevista (negativo = atrasado)
    (pl.data_prevista_colheita - CURRENT_DATE) AS dias_para_colheita
FROM plantio pl
JOIN talhao t       ON t.id = pl.talhao_id
JOIN propriedade p  ON p.id = t.propriedade_id
JOIN cultura c      ON c.id = pl.cultura_id;