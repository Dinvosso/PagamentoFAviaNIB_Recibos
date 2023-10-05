
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
VALUES ('DocAdiantamento','DocAdiantamento','ADC')
GO
insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('DocValExc','DocValExc','VEC')
GO
insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('ClienteDesconhecido','ClienteDesconhecido','09999.000')
GO

--//////////////////////////////////--///////////////////////////////////--////////////////

insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('DocSerTec','DocSerTec','VHT')
insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('CodIva','CodIva','17')
insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('TaxaIva','TaxaIva','17')
insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('Armazem','Armazem','B')
insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('ArtigoSTP','ArtigoSTP','ArtSERV')
insert INTO [dbo].[TDU_Parametros]([CDU_Parametro] ,[CDU_Descricao] ,[CDU_Valor]) 
VALUES ('Unidade','Unidade','UN')

alter table cabeccompras add cdu_DocSTP varchar(25) 
go

