
alter table historico add cdu_NrTransacao nvarchar (200)
go
INSERT INTO [dbo].[StdCamposVar] 
	([Tabela],[Campo],[Descricao],[Texto],[Visivel],[Ordem],[Pagina],[ValorDefeito],[Query],[ExportarTTE],[DadosSensiveis])
VALUES
	('historico','cdu_NrTransacao','NrTransacao','NrTransacao',1,1,NULL,0,'',0,0)
GO

alter table historico add cdu_NomeFichPgto nvarchar (200)
go
INSERT INTO [dbo].[StdCamposVar] 
	([Tabela],[Campo],[Descricao],[Texto],[Visivel],[Ordem],[Pagina],[ValorDefeito],[Query],[ExportarTTE],[DadosSensiveis])
VALUES
	('historico','cdu_NomeFichPgto','Nome Ficheiro Pgto','Nome Ficheiro Pgto',1,2,NULL,0,'',0,0)
GO

alter table cabliq add cdu_NrTransacao nvarchar (200)
go
INSERT INTO [dbo].[StdCamposVar] 
	([Tabela],[Campo],[Descricao],[Texto],[Visivel],[Ordem],[Pagina],[ValorDefeito],[Query],[ExportarTTE],[DadosSensiveis])
VALUES
	('cabliq','cdu_NrTransacao','NrTransacao','NrTransacao',1,1,NULL,0,'',0,0)
GO

insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('RubricaTes','RubricaTes','ATM')
GO

insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('DocumentoLiq','DocumentoLiq','RE')
GO

insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('DocTesouraria','DocTesouraria','MOV')
GO
insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('DocAdiantamento','DocAdiantamento','VEC')
GO