CREATE OR REPLACE VIEW vw_rel_propriedades_talhoes AS
SELECT
    u.id                                   AS user_id,
    u.nome                                 AS produtor,
    p.id                                   AS propriedade_id,
    p.nome                                 AS propriedade,
    p.area_total,
    COUNT(t.id)                            AS qtd_talhoes,
    COALESCE(SUM(t.area), 0)               AS area_talhoes,
    p.area_total - COALESCE(SUM(t.area), 0) AS area_livre,
    ROUND(COALESCE(SUM(t.area), 0) / NULLIF(p.area_total, 0) * 100, 2) AS pct_utilizacao,
    
    (COALESCE(SUM(t.area), 0) > p.area_total) AS area_inconsistente
FROM propriedade p
JOIN users u         ON u.id = p.user_id
LEFT JOIN talhao t   ON t.propriedade_id = p.id
GROUP BY u.id, u.nome, p.id, p.nome, p.area_total;
 