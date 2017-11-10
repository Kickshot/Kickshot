using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// Input script for determining how the object will be moved.
    /// </summary>
    public class ObjectInput : MonoBehaviour
    {
        private ObjectController controller;

        private void Start()
        {
            controller = GetComponent<ObjectController>();
        }

        private void Update()
        {
            Vector2 mouseInput = Input.mousePosition;

            Vector3 translation = Vector3.zero;
            if (Input.GetKey(KeyCode.T))
                translation.x = 1f;
            if (Input.GetKey(KeyCode.Y))
                translation.x = -1f;
            if (Input.GetKey(KeyCode.G))
                translation.y = 1f;
            if (Input.GetKey(KeyCode.H))
                translation.y = -1f;
            if (Input.GetKey(KeyCode.B))
                translation.z = 1f;
            if (Input.GetKey(KeyCode.N))
                translation.z = -1f;

            controller.Move(translation * Time.deltaTime);
        }

    }
}