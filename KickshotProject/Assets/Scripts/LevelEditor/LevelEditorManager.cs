using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class LevelEditorManager : MonoBehaviour
    {

        public string objectTag = "LevelObject";

        public void Export()
        {
            List<GameObject> objs = new List<GameObject>(GameObject.FindGameObjectsWithTag(objectTag));
            LevelObject testObj = ComputeTransformStruct(objs[0]);
            Debug.Log(JsonUtility.ToJson(testObj));
        }

        private LevelObject ComputeTransformStruct(GameObject o)
        {
            LevelObject ret;
            ret.name = o.name;
            ret.position = o.transform.position;
            ret.rotation = o.transform.rotation;
            ret.scale = o.transform.localScale;
            return ret;
        }
    }

    [System.Serializable]
    public struct LevelObject
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

}