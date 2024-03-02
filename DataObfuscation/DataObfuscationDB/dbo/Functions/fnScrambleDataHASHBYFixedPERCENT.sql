CREATE FUNCTION [dbo].[fnScrambleDataHASHBYFixedPERCENT] (@input VARCHAR(max))
RETURNS VARCHAR(max)
AS
BEGIN
    DECLARE @masked VARCHAR(max);
    DECLARE @maskStartIndex INT;
    DECLARE @maskLength INT;
    DECLARE @inputLength INT = LEN(@input);

    SET @maskLength = ROUND(@inputLength * 0.8, 0); -- Set the percentage to 80%
    SET @maskStartIndex = ROUND((@inputLength - @maskLength) / 2.0, 0) + 1;

    IF @maskLength >= @inputLength
        SET @masked = REPLICATE('*', @inputLength);
    ELSE IF @maskLength <= 0
        SET @masked = @input;
    ELSE
        SET @masked = LEFT(@input, @maskStartIndex - 1)
                     + REPLICATE('*', @maskLength)
                     + RIGHT(@input, @inputLength - (@maskStartIndex + @maskLength - 1));

    RETURN @masked;
END