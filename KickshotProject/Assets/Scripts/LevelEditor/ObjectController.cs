using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// Moves the active object given a translation
    /// </summary>
    public class ObjectController : MonoBehaviour
    {
        public void Move(Vector3 translation)
        {
            Manager.Instance.activeSelection.transform.Translate(translation);
        }
    }
}