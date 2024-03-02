Create FUNCTION dbo.fnScrambleDataSsn (@ssn VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS
BEGIN
  DECLARE @output VARCHAR(MAX)
  
  SET @output = 'XXX-XX-X' + RIGHT(@ssn, 3)
  
  RETURN @output
END;