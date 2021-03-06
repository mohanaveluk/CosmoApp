/****** Object:  StoredProcedure [dbo].[CWT_GetUpTime]    Script Date: 2/9/2015 6:15:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_GetUpTime](@StartDate datetime, @EndDate datetime) AS


Begin
SELECT 
      @StartDate  firstdate,@EndDate  seconddate
       ,cast(datediff(MI,@StartDate,@EndDate) as decimal(10,3)) as minutediff
      ,cast(cast(cast(datediff(MI,@StartDate,@EndDate) as decimal(10,3)) / (24*60) as int ) as varchar(10))  + 'd' + ', ' 
      + cast(cast((cast(datediff(MI,@StartDate,@EndDate) as decimal(10,3)) / (24*60) - 
        floor(cast(datediff(MI,@StartDate,@EndDate) as decimal(10,3)) / (24*60)) ) * 24 as int) as varchar(10)) + 'h' + ', ' 

     + cast( cast(((cast(datediff(MI,@StartDate,@EndDate) as decimal(10,3)) / (24*60) 
      - floor(cast(datediff(MI,@StartDate,@EndDate) as decimal(10,3)) / (24*60)))*24
        -
        cast(floor((cast(datediff(MI,@StartDate,@EndDate) as decimal(10,3)) / (24*60) 
      - floor(cast(datediff(MI,@StartDate,@EndDate) as decimal(10,3)) / (24*60)))*24) as decimal)) * 60 as int) as varchar(10)) + 'm'  as uptime

End

Go