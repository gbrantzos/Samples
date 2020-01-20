INSERT INTO [dbo].[tmpTargets]
           ([ItemCode]
           ,[AMonthCode]
           ,[CompanyID]
           ,[TargetQuantity]
           ,[TargetValue])
     VALUES
           (@ItemCode
           ,@AMonthCode
           ,@CompanyID
           ,@TargetQuantity
           ,@TargetValue)
