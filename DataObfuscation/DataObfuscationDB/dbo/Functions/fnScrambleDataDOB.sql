CREATE FUNCTION dbo.fnScrambleDataDOB (@dob VARCHAR(50))
RETURNS VARCHAR(50)
AS
BEGIN
   DECLARE @masked_dob VARCHAR(50)

   IF (@dob LIKE '__/__/____')  -- check for MM/DD/YYYY format
   BEGIN
      SET @masked_dob = CONCAT('**/', SUBSTRING(@dob, 4, 2), '/****')
   END
   ELSE IF (@dob LIKE '__-__-____')  -- check for DD-MM-YYYY format
   BEGIN
      SET @masked_dob = CONCAT('**-', SUBSTRING(@dob, 4, 2), '-****')
   END
   ELSE IF (@dob LIKE '____/__/__')  -- check for YYYY/MM/DD format
   BEGIN
      SET @masked_dob = CONCAT('****/', SUBSTRING(@dob, 6, 2), '/', '**')
   END
   ELSE IF (@dob LIKE '____-__-__')  -- check for YYYY-MM-DD format
   BEGIN
      SET @masked_dob = CONCAT('****-', SUBSTRING(@dob, 6, 2), '-', '**')
   END
   ELSE
   BEGIN
      SET @masked_dob = @dob  -- if none of the formats match, return original value
   END

   RETURN @masked_dob
END