﻿/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF NOT EXISTS(
SELECT * FROM Currency C WHERE C.CurrencyId = 1
)
BEGIN
    SET IDENTITY_INSERT Currency ON;
    INSERT INTO Currency(CurrencyId, [Description]) VALUES(1,'USD')
    SET IDENTITY_INSERT Currency OFF;
END
IF NOT EXISTS(
SELECT * FROM Currency C WHERE C.CurrencyId = 2
)
BEGIN
    SET IDENTITY_INSERT Currency ON;
    INSERT INTO Currency(CurrencyId, [Description]) VALUES(2,'CRC')
    SET IDENTITY_INSERT Currency OFF;
END