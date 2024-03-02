
CREATE VIEW [dbo].[vwGetTableColumnData] AS
WITH cteMatchedColumnNames AS
(
    SELECT 
		a.ID,a.ProjectID,
		a.TableName,
		a.ColumnName,
		a.FunctionId,
		a.IsDeleted,
		b.ID KeywordID, 
		b.Keyword,
		b.FunctionId KeyWordFunctionID 
	FROM 
		[dbo].[TableColumnFunctionMapping] a WITH(NOLOCK)
	CROSS JOIN 
		[dbo].[KeywordList] b WITH(NOLOCK)
	WHERE 
		CHARINDEX(b.KeyWord,a.ColumnName,0)>0
		  --a.ColumnName LIKE '%' + b.Keyword + '%'
),
cteMappingData AS (
    SELECT 
		DISTINCT
			a.ProjectId,
			a.ID MappingID,
			a.TableName,
			a.ColumnName,
			a.FunctionId,
			a.IsDeleted,
			a.IsObfuscated,
			a.ObfuscatedDate,
			FIRST_VALUE(b.Keyword) OVER(PARTITION BY a.ID ORDER BY LEN(b.KeyWord) DESC,KeyWordFunctionID ) KeyWord,
			FIRST_VALUE(b.KeyWordFunctionID) OVER(PARTITION BY a.ID ORDER BY LEN(b.KeyWord) DESC,KeyWordFunctionID ) KeyWordFunctionID
	FROM  
		[dbo].[TableColumnFunctionMapping] a WITH(NOLOCK) 
	LEFT OUTER JOIN
		cteMatchedColumnNames b ON a.Id=b.Id and a.ProjectId=b.ProjectId
)

SELECT 
    m.ProjectID,
    m.MappingID, 
    m.TableName, 
    m.ColumnName, 
    CASE 
        WHEN m.FunctionId = 0 AND m.KeyWordFunctionID > 0 THEN m.KeyWordFunctionID
        ELSE COALESCE(m.FunctionId, m.KeyWordFunctionID)
    END AS FunctionID,
    CASE 
        WHEN m.FunctionId != 0 THEN 'Mapped'
        WHEN m.FunctionId = 0 AND m.KeyWordFunctionID > 0 THEN 'New Mapping'
        ELSE 'NotMapped'
    END AS MappingStatus,
    m.IsObfuscated,
	m.IsDeleted,
    CASE
        WHEN m.FunctionId = 0 AND m.KeyWordFunctionID > 0 THEN f_kw.DisplayName
        ELSE f.DisplayName
    END AS DisplayName -- Displaying the correct DisplayName
FROM 
    cteMappingData m
LEFT JOIN
    Functions f ON m.FunctionId = f.ID
LEFT JOIN
    Functions f_kw ON m.KeyWordFunctionID = f_kw.ID; -- Joining with Functions table for KeyWordFunctionID