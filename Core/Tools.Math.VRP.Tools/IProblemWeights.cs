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

namespace Tools.Math.VRP.Core
{
    /// <summary>
    /// The definition of the problem weights.
    /// </summary>
    public interface IProblemWeights
    {
        /// <summary>
        /// Returns the weight matrix if any, else returns null.
        /// </summary>
        float[][] WeightMatrix
        {
            get;
        }

        /// <summary>
        /// Returns the weight between two customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        float Weight(int from, int to);

        /// <summary>
        /// Returns the size.
        /// </summary>
        int Size
        {
            get;
        }

        /// <summary>
        /// Returns the 10 nearest neighbours of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        NearestNeighbours10 Get10NearestNeighbours(int customer);
    }
}