﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.TSP;
using OsmSharp.Routing.TSP.Genetic;
using OsmSharp.UI;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles.MapCSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace OsmSharp.WinForms.UI.Sample
{
    /// <summary>
    /// A simple demo form demonstrating the rendering using MapCSS.
    /// </summary>
    public partial class MapControlForm : Form
    {
        /// <summary>
        /// Creates a new map control form.
        /// </summary>
        public MapControlForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the OnLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // initialize mapcss interpreter.
            var mapCSSInterpreter = new MapCSSInterpreter(
                new FileInfo(@"default.mapcss").OpenRead(), new MapCSSDictionaryImageSource());

            // initialize map.
            var map = new OsmSharp.UI.Map.Map();

            // initialize router.
            _router = Router.CreateLiveFrom(new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(
                new FileInfo(@"kempen.osm.pbf").OpenRead()), new OsmRoutingInterpreter());

            //Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), new List<float>(new float[] {
            //    16, 14, 12, 10 }));
            //StyleOsmStreamSceneTarget target = new StyleOsmStreamSceneTarget(
            //    mapCSSInterpreter, scene, new WebMercator());
            //FileInfo testFile = new FileInfo(@"kempen.osm.pbf");
            //Stream stream = testFile.OpenRead();
            //OsmStreamSource source = new PBFOsmStreamSource(stream);
            //OsmStreamFilterProgress progress = new OsmStreamFilterProgress(source);
            //target.RegisterSource(progress);
            //target.Pull();

            //var merger = new Scene2DObjectMerger();
            //scene = merger.BuildMergedScene(scene);

            //map.AddLayer(new LayerScene(scene));
            var dataSource = MemoryDataSource.CreateFromPBFStream(
                new FileInfo(@"kempen.osm.pbf").OpenRead());
            map.AddLayer(new LayerOsm(dataSource, mapCSSInterpreter, map.Projection));
            // map.AddLayer(new LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png", 200));
            //map.AddLayer(new LayerScene(
            //    Scene2D.Deserialize(new FileInfo(@"kempen-big.osm.pbf.scene.layered").OpenRead(),
            //        true)));

            // initialize route/points layer.
            _layerRoute = new LayerRoute(new OsmSharp.Math.Geo.Projections.WebMercator());
            map.AddLayer(_layerRoute);
            _layerPrimitives = new LayerPrimitives(new OsmSharp.Math.Geo.Projections.WebMercator());
            map.AddLayer(_layerPrimitives);

            // set control properties.
            this.mapControl1.Map = map;
            this.mapControl1.MapCenter = new GeoCoordinate(51.26371, 4.7854); // wechel
            this.mapControl1.MapZoom = 16;
            this.mapControl1.MapMouseClick += mapControl1_MapMouseClick;
        }

        private Router _router;

        private List<RouterPoint> _points = new List<RouterPoint>();

        private LayerRoute _layerRoute;

        private LayerPrimitives _layerPrimitives;

        void mapControl1_MapMouseClick(MapControlEventArgs e)
        {
            if(_router != null)
            {
                var routerPoint = _router.Resolve(Vehicle.Car, e.Position);
                if(routerPoint != null)
                {
                    _points.Add(routerPoint);
                    _layerPrimitives.AddPoint(routerPoint.Location, 15, SimpleColor.FromKnownColor(KnownColor.Black).Value);
                    _layerPrimitives.AddPoint(routerPoint.Location, 7, SimpleColor.FromKnownColor(KnownColor.White).Value);
                }

                if(_points.Count > 1)
                {
                    var tspRouter = new RouterTSPWrapper<RouterTSPAEXGenetic>(
                        new RouterTSPAEXGenetic(), _router);

                    var route = tspRouter.CalculateTSP(Vehicle.Car, _points.ToArray());
                    if(route != null)
                    {
                        _layerRoute.Clear();
                        _layerRoute.AddRoute(route);
                    }
                }
            }
        }
    }
}