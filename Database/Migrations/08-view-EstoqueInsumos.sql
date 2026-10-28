CREATE OR REPLACE VIEW vw_rel_estoque_insumos AS
SELECT
    i.id                         AS insumo_id,
    i.nome                       AS insumo,
    i.tipo,
    i.unidade_medida,
    i.estoque,
    i.estoque_minimo,
    CASE
        WHEN i.estoque <= 0              THEN 'ZERADO'
        WHEN i.estoque < i.estoque_minimo THEN 'ABAIXO DO MÍNIMO'
        ELSE 'OK'
    END                          AS situacao_estoque
FROM insumo i;