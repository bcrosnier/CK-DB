﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.SqlServer.Setup;
using CK.Setup;
using CK.SqlServer;

namespace CK.DB.Zone
{

    [SqlPackage( ResourcePath = "Res", ResourceType = typeof(Package) ), Versions( "3.0.0" )]
    public abstract class Package : Actor.Package
    {
        public new GroupTable GroupTable { get { return (GroupTable)base.GroupTable; } }

        [InjectContract]
        public ZoneTable ZoneTable { get; protected set; }

    }
}