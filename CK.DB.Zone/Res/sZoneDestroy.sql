-- Version = 15.12.5, Requires = { CK.sGroupRemoveAllUsers, CK.sGroupDestroy }
create procedure CK.sZoneDestroy
(
	@ActorId int,
	@ZoneId int,
	@ForceDestroy bit = 0
)
as
begin
    if @ActorId <= 0 throw 50000, 'Security.AnonymousNotAllowed', 1;
	if @ZoneId <= 1 throw 50000, 'Zone.UndestroyableZone', 1;

	--[beginsp]

	declare @AdministratorsGroupId int;
	select @AdministratorsGroupId = AdministratorsGroupId 
		from CK.tZone 
		where ZoneId = @ZoneId;
	
	if @AdministratorsGroupId is not null 
	begin

		--<Extension Name="Zone.PreZoneDestroy" />

		if @AdministratorsGroupId <> 0
		begin
			exec CK.sGroupRemoveAllUsers @ActorId, @AdministratorsGroupId;
			update CK.tZone set AdministratorsGroupId = 0 where ZoneId = @ZoneId;
			exec CK.sGroupDestroy @ActorId, @AdministratorsGroupId, @ForceDestroy;
		end
		if @ForceDestroy = 1
		begin
			-- Removes all groups owned by the zone
			declare @GroupId int;
			declare @CGroup cursor;
			set @CGroup = cursor local fast_forward for select GroupId from CK.tGroup p where p.ZoneId = @ZoneId and p.GroupId <> @ZoneId;
			open @CGroup;
			fetch from @CGroup into @GroupId;
			while @@FETCH_STATUS = 0
			begin
				exec CK.sGroupDestroy @ActorId, @GroupId, @ForceDestroy;
				fetch next from @CGroup into @GroupId;
			end
			deallocate @CGroup;
		end

		update CK.tGroup set ZoneId = 0 where GroupId = @ZoneId;
		delete from CK.tZone where ZoneId = @ZoneId;

		exec CK.sGroupDestroy @ActorId, @ZoneId, @ForceDestroy;

		--<Extension Name="Zone.PostZoneDestroy" />
	
	end

	--[endsp]
end
