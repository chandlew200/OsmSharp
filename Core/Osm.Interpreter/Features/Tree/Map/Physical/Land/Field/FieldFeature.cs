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
using Osm.Core.Filters;

namespace Osm.Interpreter.Features.Tree.Map.Physical.Land.Field
{
    
    public class FieldFeature : Feature
    {
        /// <summary>
        /// Creates a new existence feature.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="meta"></param>
        private FieldFeature(Filter filter, IDictionary<string, IList<string>> meta)
            : base("Map.ExistenceFeature.PhysicalFeature.LandFeature.FieldFeature", filter, meta)
        {

        }

        #region Singleton

        /// <summary>
        /// Holds the one and only instance of this feature.
        /// </summary>
        private static FieldFeature _instance;

        /// <summary>
        /// Creates the existence feature.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="meta"></param>
        public static void Create(Filter filter, IDictionary<string, IList<string>> meta)
        {
            _instance = new FieldFeature(filter, meta);
        }

        /// <summary>
        /// Returns the one and only instance of this feature.
        /// </summary>
        public static FieldFeature Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Child Features

        /// <summary>
        /// Creates the list with child features.
        /// </summary>
        /// <returns></returns>
        internal override IList<Feature> GetChildFeatures()
        {
            IList<Feature> features = new List<Feature>();

            // TODO: Add features here!

            return features;
        }

        #endregion
    }
}