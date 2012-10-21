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
using Tools.Math.StateMachines;
using Tools.Math.Geo;
using Tools.Math.Geo.Meta;

namespace Osm.Routing.Instructions.MicroPlanning.Machines
{
    internal class PoiWithTurnMachine : MicroPlannerMachine
    {
        public PoiWithTurnMachine(MicroPlanner planner)
            : base(PoiWithTurnMachine.Initialize(), planner, 1001)
        {

        }

        /// <summary>
        /// Initializes this machine.
        /// </summary>
        /// <returns></returns>
        private static FiniteStateMachineState Initialize()
        {
            // generate states.
            List<FiniteStateMachineState> states = FiniteStateMachineState.Generate(3);

            // state 3 is final.
            states[2].Final = true;

            // 0
            FiniteStateMachineTransition.Generate(states, 0, 1, typeof(MicroPlannerMessageArc));

            // 1
            FiniteStateMachineTransition.Generate(states, 1, 1, typeof(MicroPlannerMessageArc));
            FiniteStateMachineTransition.Generate(states, 1, 1, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition.FiniteStateMachineTransitionConditionDelegate(TestNonSignificantTurnNonPoi));
            FiniteStateMachineTransition.Generate(states, 1, 2, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition.FiniteStateMachineTransitionConditionDelegate(TestPoi));

            // return the start automata with intial state.
            return states[0];
        }

        /// <summary>
        /// Tests if the given turn is significant.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestNonSignificantTurnNonPoi(object test)
        {
            if (!PoiWithTurnMachine.TestPoi(test))
            {
                if (test is MicroPlannerMessagePoint)
                {
                    MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
                    if (point.Point.Angle != null)
                    {
                        if (point.Point.ArcsNotTaken == null || point.Point.ArcsNotTaken.Count == 0)
                        {
                            return true;
                        }
                        switch (point.Point.Angle.Direction)
                        {
                            case Tools.Math.Geo.Meta.RelativeDirectionEnum.StraightOn:
                            case RelativeDirectionEnum.SlightlyLeft:
                            case RelativeDirectionEnum.SlightlyRight:
                                return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Tests if the given point is a poi.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestPoi(object test)
        {
            if (test is MicroPlannerMessagePoint)
            {
                MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
                if (point.Point.Points != null && point.Point.Points.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Succes()
        {
            Osm.Routing.Core.ArcAggregation.Output.AggregatedPoint pois_point = (this.FinalMessages[this.FinalMessages.Count - 1] as MicroPlannerMessagePoint).Point;
            Osm.Routing.Core.ArcAggregation.Output.AggregatedArc previous_arc = (this.FinalMessages[this.FinalMessages.Count - 2] as MicroPlannerMessageArc).Arc;

            // get the pois list.
            List<Osm.Routing.Core.ArcAggregation.Output.PointPoi> pois = (this.FinalMessages[this.FinalMessages.Count - 1] as MicroPlannerMessagePoint).Point.Points;

            // get the angle from the pois point.
            RelativeDirection direction = pois_point.Angle;

            // calculate the box.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            foreach (Osm.Routing.Core.ArcAggregation.Output.PointPoi poi in pois)
            {
                coordinates.Add(poi.Location);
            }
            coordinates.Add(pois_point.Location);
            GeoCoordinateBox box = new GeoCoordinateBox(coordinates.ToArray());

            // let the scentence planner generate the correct information.
            this.Planner.SentencePlanner.GeneratePoi(box, pois, direction);
        }
//<<<<<<< .mine
        
//        public override bool Equals(object obj)
//        {
//            if (obj is ImmidateTurnMachine)
//            {
//                return true;
//            }
//            return false;
//        }

//        public override int GetHashCode()
//        {
//            return this.GetType().GetHashCode();
//        }
//=======

        public override bool Equals(object obj)
        {
            if (obj is PoiWithTurnMachine)
            { // if the machine can be used more than once 
                // this comparision will have to be updated.
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {// if the machine can be used more than once 
            // this hashcode will have to be updated.
            return this.GetType().GetHashCode();
        }
//>>>>>>> .r303
    }
}