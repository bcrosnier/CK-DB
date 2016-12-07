﻿-- SetupConfig: {}
--
-- @GoogleAccountId can not be null.
-- When @AccessToken is null, it is replaced with an empty string.
-- When @AccessTokenExpirationTime is null, '9999-12-31T23:59:59.99' is set.
-- When @RefreshToken is null, it is left unchanged on update (an empty string is inserted on creation).
--
create procedure CK.sUserGoogleCreateOrUpdate
(
	@ActorId int,
	@UserId int,
	@GoogleAccountId varchar(36), 
	@AccessToken varchar(max),
	@AccessTokenExpirationTime datetime2(2) = null,
	@RefreshToken varchar(max) = null,
	@HasBeenCreated bit output
)
as
begin
    if @ActorId is null or @ActorId <= 0 throw 50000, 'Security.AnonymousNotAllowed', 1;
    if @UserId is null or @UserId <= 0 throw 50000, 'Argument.InvalidUserId', 1;
	if @GoogleAccountId is null throw 50000, 'Argument.NullGoogleAccountId', 1;
	if @AccessToken is null set @AccessToken = '';
	if @AccessTokenExpirationTime is null set @AccessTokenExpirationTime = '9999-12-31T23:59:59.99';

	--[beginsp]

	declare @PrevRefreshToken varchar(max);
	select @PrevRefreshToken = RefreshToken
		from CK.tUserGoogle 
		where UserId = @UserId and GoogleAccountId = @GoogleAccountId;

	--<PreCreateOrUpdate reverse /> 

	if @PrevRefreshToken is null
	begin
		if @RefreshToken is null set @RefreshToken = '';
		declare @DefaultScopeSetId int;
		select @DefaultScopeSetId = ScopeSetId from CK.tUserGoogle where UserId = 0;
		declare @NewScopeSetId int;
		exec CK.sAuthScopeSetCopy @ActorId, @DefaultScopeSetId, 'W', @NewScopeSetId output;

		--<PreCreate reverse /> 

		-- Unique constraint on GoogleAccountId will detect any existing UserId/GoogleAccountId clashes.
		insert into CK.tUserGoogle( UserId, GoogleAccountId, ScopeSetId, AccessToken, AccessTokenExpirationTime, RefreshToken, LastRefreshTokenTime ) 
			values( @UserId, @GoogleAccountId, @NewScopeSetId, @AccessToken, @AccessTokenExpirationTime, @RefreshToken, sysutcdatetime() );
		set @HasBeenCreated = 1;

		--<PostCreate reverse /> 
	end
	else
	begin
		if @RefreshToken is not null and @PrevRefreshToken <> @RefreshToken 
		begin
			update CK.tUserGoogle set 
					AccessToken = @AccessToken, 
					AccessTokenExpirationTime = @AccessTokenExpirationTime,
					RefreshToken = @RefreshToken, 
					LastRefreshTokenTime = sysutcdatetime() 
				where UserId = @UserId and GoogleAccountId = @GoogleAccountId;
		end
		else
		begin
			update CK.tUserGoogle set 
					AccessToken = @AccessToken, 
					AccessTokenExpirationTime = @AccessTokenExpirationTime
				where UserId = @UserId and GoogleAccountId = @GoogleAccountId;
		end
		set @HasBeenCreated = 0;
	end

	--<PostCreateOrUpdate /> 

	--[endsp]
end