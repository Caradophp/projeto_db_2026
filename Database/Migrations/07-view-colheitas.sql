CREATE OR REPLACE VIEW vw_rel_colheitas AS
SELECT
    p.user_id,
    p.id                         AS propriedade_id,
    p.nome                       AS propriedade,
    t.id                         AS talhao_id,
    t.nome                       AS talhao,
    c.id                         AS cultura_id,
    c.nome                       AS cultura,
    pl.id                        AS plantio_id,
    pl.produtividade_estimada,
    h.id                         AS colheita_id,
    h.data_colheita,
    TRUNC(h.data_colheita, 'MM') AS mes_referencia,
    EXTRACT(YEAR FROM h.data_colheita)    AS ano_referencia,
    h.producao_total 
FROM colheita h
JOIN plantio pl     ON pl.id = h.plantio_id
JOIN talhao t       ON t.id = pl.talhao_id
JOIN propriedade p  ON p.id = t.propriedade_id
JOIN cultura c      ON c.id = pl.cultura_id;
 
CREATE OR REPLACE VIEW vw_rel_producao_cultura_periodo AS
SELECT
    user_id,
    propriedade_id,
    propriedade,
    cultura_id,
    cultura,
    ano_referencia,
    mes_referencia,
    COUNT(*)                     AS qtd_colheitas
FROM vw_rel_colheitas
GROUP BY user_id, propriedade_id, propriedade, cultura_id, cultura,
         ano_referencia, mes_referencia;