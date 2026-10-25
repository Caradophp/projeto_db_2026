CREATE OR REPLACE VIEW vw_rel_aplicacao_insumos AS
SELECT
    p.user_id,
    p.id                         AS propriedade_id,
    p.nome                       AS propriedade,
    t.id                         AS talhao_id,
    t.nome                       AS talhao,
    i.id                         AS insumo_id,
    i.nome                       AS insumo,
    i.tipo,
    i.unidade_medida,
    a.data_aplicacao,
    TRUNC(a.data_aplicacao, 'MM') AS mes_referencia,
    a.quantidade                 AS quantidade_aplicada 
FROM aplicacao_insumo a
JOIN insumo i       ON i.id = a.insumo_id
JOIN talhao t       ON t.id = a.talhao_id
JOIN propriedade p  ON p.id = t.propriedade_id;
 