using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BareGraph
{
    public class BareEdgeModel : ScriptableObject
    {
        public string guid { get; private set; }
        public BareNodeModel input;
        public BareNodeModel output;

        public string assetName { get { return "Edge_" + guid; } private set { } }

        public BareEdgeModel()
        {
            guid = Guid.NewGuid().ToString();
        }
    }
}