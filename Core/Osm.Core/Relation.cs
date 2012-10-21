﻿// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core
{
    /// <summary>
    /// Relation class.
    /// </summary>
    [Serializable]
    public class Relation : OsmGeo
    {
        /// <summary>
        /// Holds the members of this relation.
        /// </summary>
        private IList<RelationMember> _members;

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="id"></param>
        internal protected Relation(long id)
            :base(id)
        {
            _members = new List<RelationMember>();
        }

        /// <summary>
        /// Returns the relation type.
        /// </summary>
        public override OsmType Type
        {
            get { return OsmType.Relation; }
        }

        /// <summary>
        /// Gets the relation members.
        /// </summary>
        public IList<RelationMember> Members
        {
            get
            {
                return _members;
            }
        }

        /// <summary>
        /// Find a member in this relation with the given role.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public OsmBase FindMember(string role)
        {
            if (this.Members != null)
            {
                foreach (RelationMember member in this.Members)
                {
                    if (member.Role == role)
                    {
                        return member.Member;
                    }
                }
            }
            return null;
        }
    }
}