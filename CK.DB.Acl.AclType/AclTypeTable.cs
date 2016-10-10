﻿using CK.Setup;
using CK.SqlServer;
using CK.SqlServer.Setup;
using System.Threading.Tasks;

namespace CK.DB.Acl.AclType
{
    [SqlTable( "tAclType", Package = typeof( Package ) )]
    [Versions( "1.0.0" )]
    [SqlObjectItem( "transform:sAclGrantSet" )]
    public abstract partial class AclTypeTable : SqlTable
    {
        void Construct( AclTable acl )
        {
        }

        [SqlProcedure( "sAclTypeCreate" )]
        public abstract Task<int> CreateAclTypeAsync( ISqlCallContext ctx, int actorId );

        [SqlProcedure( "sAclTypeDestroy" )]
        public abstract Task DestroyAclTypeAsync( ISqlCallContext ctx, int actorId, int aclTypeId );

        [SqlProcedure( "sAclTypeConstrainedGrantLevelSet" )]
        public abstract Task SetConstrainedGrantLevelAsync( ISqlCallContext ctx, int actorId, int aclTypeId, bool set );

        [SqlProcedure( "sAclTypeGrantLevelSet" )]
        public abstract Task SetGrantLevelAsync( ISqlCallContext ctx, int actorId, int aclTypeId, byte grantLevel, bool set );

        /// <summary>
        /// Creates a typed acl.
        /// </summary>
        /// <param name="ctx">Call context to use.</param>
        /// <param name="actorId">Calling actor identifier.</param>
        /// <param name="aclTypeId">The acl type identifier.</param>
        /// <returns>A new Acl identifier that is bound to the Acl type.</returns>
        [SqlProcedure( "transform:sAclCreate" )]
        public abstract Task<int> CreateAclAsync( ISqlCallContext ctx, int actorId, int aclTypeId );

    }
}
