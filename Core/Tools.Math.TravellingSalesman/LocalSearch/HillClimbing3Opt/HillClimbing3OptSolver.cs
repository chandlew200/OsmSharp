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
using Tools.Math.TSP;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.TSP.Problems;
using Tools.Math.TSP.ArbitraryInsertion;
using Tools.Math.VRP.Core;

namespace Tools.Math.TSP.LocalSearch.HillClimbing3Opt
{
    /// <summary>
    /// Uses the 3-opt local search procedure to generate ATSP solutions.
    /// </summary>
    public class HillClimbing3OptSolver : ISolver, IImprovement
    {
        /// <summary>
        /// Boolean to stop execution.
        /// </summary>
        private bool _stopped = false;

        /// <summary>
        /// Only check nearest neighbours.
        /// </summary>
        private bool _nearest_neighbours = false;

        /// <summary>
        /// Use don't check flags.
        /// </summary>
        private bool _dont_look = false;

        /// <summary>
        /// The route this solver was initialized with.
        /// </summary>
        private IRoute _route;

        /// <summary>
        /// The minium difference to register improvement(s).
        /// </summary>
        private float _epsilon = 0.001f;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="nearest_neighbours"></param>
        /// <param name="dont_look"></param>
        public HillClimbing3OptSolver(bool nearest_neighbours, bool dont_look) 
        {
            _dont_look = dont_look;
            _nearest_neighbours = nearest_neighbours;
        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public string Name
        {
            get
            {
                if (_nearest_neighbours && _dont_look)
                {
                    return "3OHC_(NN)_(DL)";
                }
                else if (_nearest_neighbours)
                {
                    return "3OHC_(NN)";
                }
                else if (_dont_look)
                {
                    return "3OHC_(DL)";
                }
                return "3OHC";
            }
        }

        private ArbitraryInsertionSolver solver = new ArbitraryInsertionSolver();

        /// <summary>
        /// Solves the problem.
        /// </summary>
        /// <returns></returns>
        public IRoute Solve(IProblem problem)
        {
            // generate some random route first.
            //IRoute route = Tools.Math.TravellingSalesman.Random.RandomSolver.DoSolve(
            //    problem);
            IRoute route = solver.Solve(problem);

            // improve the existing solution.
            float difference;
            this.Improve(problem, route, out difference);

            return route;
        }

        /// <summary>
        /// Improves an existing solution.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        public bool Improve(IProblemWeights problem, IRoute route, out float difference)
        {
            if (!route.IsValid())
            {
                throw new Exception();
            }

            _dont_look_bits = new bool[problem.Size];
            float[][] weights = problem.WeightMatrix;

            // loop over all customers.
            bool any_improvement = false;
            bool improvement = true;
            difference = -1;
            while (improvement)
            {
                improvement = false;
                foreach (int v_1 in route)
                {
                    if (!this.Check(problem, v_1))
                    {
                        if (this.Try3OptMoves(problem, weights, route, v_1))
                        {
                            any_improvement = true;
                            improvement = true;
                            break;
                        }
                        this.Set(problem, v_1, true);
                    }
                }
            }
            return any_improvement;
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="v_1"></param>
        /// <returns></returns>
        public bool Try3OptMoves(IProblemWeights problem, float[][] weights, IRoute route, int v_1)
        {
            // get v_2.
            int v_2 = route.GetNeigbours(v_1)[0];
            IEnumerable<int> between_v_2_v_1 = route.Between(v_2, v_1);
            float weight_1_2 = weights[v_1][v_2];
            int v_3 = -1;
            NearestNeighbours10 neighbours = null;
            if (_nearest_neighbours)
            {
                neighbours = problem.Get10NearestNeighbours(v_1);
            }

            foreach (int v_4 in between_v_2_v_1)
            {
                if (v_3 >= 0 && v_3 != v_1)
                { 
                    //if (!_nearest_neighbours ||
                    //    neighbours.Max <= weight_1_4) 
                    if (!_nearest_neighbours ||
                        neighbours.Contains(v_4))
                    {
                        float weight_1_4 = weights[v_1][v_4];
                        float weight_1_2_plus_3_4 = weight_1_2 + weights[v_3][v_4];
                        float[] weights_3 = weights[v_3];
                        if (this.Try3OptMoves(problem, weights, route, v_1, v_2, v_3, weights_3, v_4, weight_1_2_plus_3_4, weight_1_4))
                        {
                            return true;
                        }
                    }
                }
                v_3 = v_4;
            }
            return false;
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="v_1"></param>
        /// <returns></returns>
        public bool Try3OptMoves(IProblemWeights problem, float[][] weights, IRoute route, 
            int v_1, int v_2, float weight_1_2,
            int v_3)
        {
            // get v_4.
            int v_4 = route.GetNeigbours(v_3)[0];
            float weight_1_2_plus_3_4 = weight_1_2 + weights[v_3][v_4];
            float weight_1_4 = weights[v_1][v_4];
            float[] weights_3 = weights[v_3];
            return this.Try3OptMoves(problem, weights, route, v_1, v_2, v_3, weights_3, v_4, weight_1_2_plus_3_4, weight_1_4);
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="v_1"></param>
        /// <param name="v_2"></param>
        /// <param name="v_3"></param>
        /// <returns></returns>
        public bool Try3OptMoves(IProblemWeights problem, float[][] weights, IRoute route, 
            int v_1, int v_2, int v_3, float[] weights_3, int v_4, float weight_1_2_plus_3_4, float weight_1_4)
        {
            //IEnumerable<int> between_v_4_v_1 = route.Between(v_4, v_1);
            //foreach (int v_5 in between_v_4_v_1)
            //{
            //    if (v_5 != v_1)
            //    {
            //        if (this.Try3OptMove(problem, weights, route, v_1, v_2, v_3, weights_3, v_4, weight_1_2_plus_3_4, weight_1_4, v_5))
            //        {
            //            return true;
            //        }
            //    }
            //}
            //return false;

            IEnumerator<int> between_v_4_v_1_enumerator = route.Between(v_4, v_1).GetEnumerator();
            if (between_v_4_v_1_enumerator.MoveNext())
            {
                int v_5 = between_v_4_v_1_enumerator.Current;
                if (v_5 != v_1)
                {
                    while (between_v_4_v_1_enumerator.MoveNext())
                    {
                        int v_6 = between_v_4_v_1_enumerator.Current;
                        //Console.WriteLine(v_6);
                        //if (this.Try3OptMove(problem, weights, 
                        //    route, v_1, v_2, v_3, weights_3, v_4, 
                        //    weight_1_2_plus_3_4, weight_1_4, v_5, v_6))
                        //{
                        // calculate the total weight of the 'new' arcs.
                        float weight_new = weight_1_4 +
                            weights_3[v_6] +
                            weights[v_5][v_2];

                        // calculate the total weights.
                        float weight = weight_1_2_plus_3_4 + weights[v_5][v_6];

                        if (weight - weight_new > _epsilon)
                        { // actually do replace the vertices.
                            route.Insert(v_1, v_4, -1);
                            route.Insert(v_3, v_6, -1);
                            route.Insert(v_5, v_2, -1);

                            // set bits.
                            //this.Set(problem, v_1, false);
                            this.Set(problem, v_3, false);
                            this.Set(problem, v_5, false);

                            if (!route.IsValid())
                            {
                                throw new Exception();
                            }

                            return true; // move succeeded.
                        }
                        //}
                        v_5 = v_6;
                    }
                }
            }
            return false;
        }

        ///// <summary>
        ///// Tries a 3opt move
        ///// </summary>
        ///// <param name="problem"></param>
        ///// <param name="route"></param>
        ///// <param name="v_1"></param>
        ///// <param name="v_2"></param>
        ///// <param name="v_3"></param>
        ///// <param name="v_4"></param>
        ///// <param name="v_5"></param>
        ///// <param name="v_6"></param>
        ///// <returns></returns>
        //public bool Try3OptMove(IProblemWeights problem, float[][] weights, IRoute route, 
        //    int v_1, int v_2, 
        //    int v_3, float[] weights_3, int v_4, float weight_1_2_plus_3_4, float weight_1_4,
        //    int v_5)
        //{
        //    // get v_6.
        //    int v_6 = route.GetNeigbours(v_5)[0];
        //    return this.Try3OptMove(problem, weights, route, v_1, v_2, v_3, weights_3, v_4, weight_1_2_plus_3_4, weight_1_4, v_5, v_6);
        //}

        #region Don't Look

        /// <summary>
        /// Holds all the don't look bits.
        /// </summary>
        private bool[] _dont_look_bits;

        private bool Check(IProblemWeights problem, int customer)
        {
            if (_dont_look)
            {
                return _dont_look_bits[customer];
            }
            return false;
        }

        private void Set(IProblemWeights problem, int customer, bool value)
        {
            if (_dont_look)
            {
                _dont_look_bits[customer] = value;
            }
        }

        #endregion

        /// <summary>
        /// Stops execution.
        /// </summary>
        public void Stop()
        {

        }

        public event SolverDelegates.IntermidiateDelegate IntermidiateResult;
    }

    
}