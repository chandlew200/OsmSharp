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
using Tools.Math.Geo;
using Osm.Renderer.Gdi.Layers;

namespace Demo.RandomHeatMap
{
    class DemoHeatLayer : HeatLayerBoxed
    {

        public DemoHeatLayer()
            :base(30, 75)
        {

        }

        protected override List<GeoCoordinate> GetExtraPoints(GeoCoordinateBox geoCoordinateBox)
        {
            List<GeoCoordinate> coords = new List<GeoCoordinate>();

            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                coords.Add(geoCoordinateBox.GenerateRandomIn(rand));
            }
            return coords;
        }
    }
}