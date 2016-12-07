﻿using CK.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.DB.Auth
{
    /// <summary>
    /// Immutable object that holds a scope name, its status and, when read from the database,
    /// its <see cref="StatusLastWriteTime"/>.
    /// Two AuthScope are equal when their <see cref="ScopeName"/> and <see cref="Status"/> are equal.
    /// </summary>
    public class AuthScope : IEquatable<AuthScope>
    {
        /// <summary>
        /// The scope name.
        /// </summary>
        public readonly string ScopeName;
        
        /// <summary>
        /// The WAR wtatus.
        /// </summary>
        public readonly ScopeWARStatus Status;
        
        /// <summary>
        /// The last status write time. This field is relevant only when read from the database.
        /// </summary>
        public readonly DateTime StatusLastWriteTime;

        /// <summary>
        /// Initializes a new immutable <see cref="AuthScope"/> (<see cref="StatusLastWriteTime"/> is set to <see cref="Util.UtcMinValue"/>).
        /// </summary>
        /// <param name="scopeName">The scope name. Can not be empty.</param>
        /// <param name="status">The WAR status.</param>
        public AuthScope( string scopeName, ScopeWARStatus status = ScopeWARStatus.Waiting )
            : this( scopeName, status, Util.UtcMinValue )
        {
        }

        /// <summary>
        /// Initializes a new <see cref="AuthScope"/>.
        /// </summary>
        /// <param name="scopeName">The scope name. Can not be empty.</param>
        /// <param name="status">The WAR status.</param>
        /// <param name="statusLastWriteTime">The last write time of the status.</param>
        public AuthScope( string scopeName, ScopeWARStatus status, DateTime statusLastWriteTime )
        {
            if( string.IsNullOrWhiteSpace( scopeName ) ) throw new ArgumentException( "Scope name can not be empty." );
            ScopeName = scopeName;
            Status = status;
            StatusLastWriteTime = statusLastWriteTime;
        }

        /// <summary>
        /// Equality relies on <see cref="ScopeName"/> and <see cref="Status"/>.
        /// </summary>
        /// <param name="other">Other scope to test.</param>
        /// <returns>True if the scopes are considered equal, false otherwise.</returns>
        public bool Equals( AuthScope other ) => ScopeName == other.ScopeName && Status == other.Status;

        /// <summary>
        /// Overridden to call <see cref="Equals(AuthScope)"/>.
        /// </summary>
        /// <param name="obj">The object to caompare to.</param>
        /// <returns>True if the scopes are considered equal, false otherwise.</returns>
        public override sealed bool Equals( object obj ) => obj is AuthScope && Equals( (AuthScope)obj );

        /// <summary>
        /// Overridden to compute a has based on <see cref="ScopeName"/> and <see cref="Status"/>.
        /// </summary>
        /// <returns></returns>
        public override sealed int GetHashCode() => ScopeName.GetHashCode() ^ (int)Status;

        /// <summary>
        /// Overridden to return the [<see cref="Status"/>]<see cref="ScopeName"/>.
        /// </summary>
        /// <returns>The status and name of this scope.</returns>
        public override sealed string ToString() => $"[{Status.ToString()[0]}]{ScopeName}";
    }
}
