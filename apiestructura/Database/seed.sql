-- Idempotent seed for Proyecto + Complejidades
IF NOT EXISTS (SELECT 1 FROM Proyectos.TBL_Proyecto WHERE Id = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
BEGIN
    INSERT INTO Proyectos.TBL_Proyecto (Id, Nombre, Descripcion, Fecha, HorasTotales, DiasEstimados, Riesgo)
    VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', N'Proyecto por defecto', N'Generado automáticamente', '2025-01-01', 0, 0, 0);
END

IF NOT EXISTS (SELECT 1 FROM Proyectos.TBL_Complejidad WHERE Id = '11111111-1111-1111-1111-111111111111')
    INSERT INTO Proyectos.TBL_Complejidad (Id, Nombre, Orden, Activo) VALUES ('11111111-1111-1111-1111-111111111111', N'Muy Baja', 1, 1);
IF NOT EXISTS (SELECT 1 FROM Proyectos.TBL_Complejidad WHERE Id = '22222222-2222-2222-2222-222222222222')
    INSERT INTO Proyectos.TBL_Complejidad (Id, Nombre, Orden, Activo) VALUES ('22222222-2222-2222-2222-222222222222', N'Baja', 2, 1);
IF NOT EXISTS (SELECT 1 FROM Proyectos.TBL_Complejidad WHERE Id = '33333333-3333-3333-3333-333333333333')
    INSERT INTO Proyectos.TBL_Complejidad (Id, Nombre, Orden, Activo) VALUES ('33333333-3333-3333-3333-333333333333', N'Media', 3, 1);
IF NOT EXISTS (SELECT 1 FROM Proyectos.TBL_Complejidad WHERE Id = '44444444-4444-4444-4444-444444444444')
    INSERT INTO Proyectos.TBL_Complejidad (Id, Nombre, Orden, Activo) VALUES ('44444444-4444-4444-4444-444444444444', N'Alta', 4, 1);
IF NOT EXISTS (SELECT 1 FROM Proyectos.TBL_Complejidad WHERE Id = '55555555-5555-5555-5555-555555555555')
    INSERT INTO Proyectos.TBL_Complejidad (Id, Nombre, Orden, Activo) VALUES ('55555555-5555-5555-5555-555555555555', N'Muy Alta', 5, 1);
