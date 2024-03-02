CREATE FUNCTION [dbo].[EncryptCertificate] (@ColumnName varchar(255),@CertificateName varchar(255))
RETURNS varbinary(200)
WITH EXECUTE AS CALLER
AS
BEGIN
    DECLARE @encryptedCertificate varbinary(200)

    SELECT @encryptedCertificate = ENCRYPTBYKEY(KEY_GUID(  @CertificateName ), @ColumnName)

    RETURN @encryptedCertificate
END