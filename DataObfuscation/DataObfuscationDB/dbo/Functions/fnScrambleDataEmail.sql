CREATE FUNCTION dbo.fnScrambleDataEmail(@p_input VARCHAR(MAX))
RETURNS VARCHAR(255)
AS
BEGIN
   DECLARE @v_output VARCHAR(255);
   DECLARE @v_alphabets VARCHAR(52) = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
   DECLARE @v_length INT;
   DECLARE @v_counter INT = 1;

   SET @v_output = '';
   SET @v_length = LEN(@p_input);

   WHILE @v_counter <= @v_length
   BEGIN
     SET @v_output = CONCAT(@v_output, SUBSTRING(@v_alphabets, 1 + ABS(CHECKSUM(@p_input, @v_counter)) % 52, 1));
     SET @v_counter = @v_counter + 1;
   END;

   SET @v_output = CONCAT(@v_output, '@example.com');

   RETURN @v_output;
END;