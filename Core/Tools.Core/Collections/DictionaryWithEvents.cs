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

namespace Tools.Core.Collections
{
    public class DictionaryWithEvents<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public event AddEventHandler AddEvent;

        public void Add(TKey pKey, TValue pValue)
        {
            if (AddEvent != null)
                AddEvent(new AddEventArgs(pKey, pValue));
            base.Add(pKey, pValue);
        }

        public delegate void AddEventHandler(AddEventArgs pAddEventArgs);

        public class AddEventArgs : EventArgs
        {
            private TKey _key;
            private TValue _value;

            public AddEventArgs(TKey key, TValue value)
            {

            }

            public TKey Key
            {
                get
                {
                    return _key;
                }
            }

            public TValue Value
            {
                get
                {
                    return _value;
                }
            }
        }
    }
}