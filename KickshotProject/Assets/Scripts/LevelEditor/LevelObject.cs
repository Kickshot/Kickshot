using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class LevelObject : MonoBehaviour
    {
        public int objectID;
    }

    [System.Serializable]
    public struct LevelObjectData
    {
        public int objectID;
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }
}