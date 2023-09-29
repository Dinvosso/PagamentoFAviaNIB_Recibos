
alter table historico add cdu_NrTransacao nvarchar (200)
go

alter table historico add cdu_NomeFichPgto nvarchar (200)
go

alter table cabliq add cdu_NrTransacao nvarchar (200)
go


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