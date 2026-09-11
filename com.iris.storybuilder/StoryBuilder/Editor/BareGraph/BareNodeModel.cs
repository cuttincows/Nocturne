using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BareGraph
{
    public class BareNodeModel : ScriptableObject
    {
        public string guid { get; private set; }
        public Vector2 position;
        public string title;

        public string assetName { get { return "Node_" + guid; } private set { } }

        public BareNodeModel()
        {
            guid = Guid.NewGuid().ToString();
        }
    }
}
