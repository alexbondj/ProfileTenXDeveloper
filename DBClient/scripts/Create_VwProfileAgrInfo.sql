CREATE VIEW VwProfileAgrInfo AS
 SELECT 
	CAST(FORMAT(MONTH(Date), '00') AS VARCHAR(2)) + '-' + CAST(YEAR(Date) AS VARCHAR(4)) as [Period]
	,Author
	,RepositoryName
	,SUM(1) as CsCount
	,AVG(cs.LinesAdded + cs.LinesRemoved ) as AvgTotalLines
FROM Changesets cs
GROUP BY 
	CAST(FORMAT(MONTH(Date), '00') AS VARCHAR(2)) + '-' + CAST(YEAR(Date) AS VARCHAR(4))
	,Author
	,RepositoryName

--Example how to use
select * from VwProfileAgrInfo where Author = 'm.zubok' or Author = 'k.shelest' or Author like 'P.shu%'