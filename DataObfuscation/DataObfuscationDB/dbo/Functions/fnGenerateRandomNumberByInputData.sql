 create FUNCTION dbo.fnGenerateRandomNumberByInputData(@inputLength BIGINT)
RETURNS BIGINT
AS
BEGIN
    DECLARE @randomNumber BIGINT;

    SET @randomNumber = ABS(CHECKSUM(CAST(@inputLength  AS VARCHAR(MAX)))) % @inputLength;

    RETURN @randomNumber;
END