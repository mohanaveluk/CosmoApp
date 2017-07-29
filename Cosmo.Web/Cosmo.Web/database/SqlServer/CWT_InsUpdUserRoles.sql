/****** Object:  StoredProcedure [dbo].[CWT_InsUpdUserRoles]    Script Date: 09/24/2015 22:52:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CWT_InsUpdUserRoles]
(
	@USERID int
	,@USERROLES varchar(MAX)
)
As
Declare
	@Character CHAR(1)
	,@StartIndex INT
	,@EndIndex INT
	,@Input varchar(max)
	,@tempRoleId int
	,@tempUserRoleId int

Begin
	set @Character = ','
	set @Input = @USERROLES
	SET @StartIndex = 1
	
	if @USERID > 0
	begin
		if @USERROLES <> ''
		Begin
			IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
			BEGIN
				SET @Input = @Input + @Character
			END				
			
			update CSM_USER_ROLE set USER_ROLE_ISACTIVE = 'false'
			where CSM_USER_ROLE.USER_ID = @USERID
			
			WHILE CHARINDEX(@Character, @Input) > 0
			Begin
				set @EndIndex = CHARINDEX(@Character, @Input)
				set @tempRoleId = SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
				
				select @tempUserRoleId = count(USER_ROLE_ID) from CSM_USER_ROLE	
				where CSM_USER_ROLE.ROLE_ID = @tempRoleId
					and CSM_USER_ROLE.USER_ID = @USERID
				
				if(@tempUserRoleId >0)
				Begin
					update CSM_USER_ROLE set USER_ROLE_ISACTIVE = 'True' 
					where CSM_USER_ROLE.ROLE_ID = @tempRoleId
						and CSM_USER_ROLE.USER_ID = @USERID
				End
				else if (@tempUserRoleId <= 0)
				Begin
					insert into CSM_USER_ROLE
					(
						USER_ID
						,ROLE_ID
						,USER_ROLE_ISACTIVE
						
					)
					values
					(
						@USERID
						,@tempRoleId
						,'True' 
					)
				End
				
				set @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
			End
		End
	End
End

--exec CWT_InsUpdUserRoles 6, '2,1,3'
GO


