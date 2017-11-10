using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class PlayerInput : MonoBehaviour
    {

        private PlayerController controller;

        private void Start()
        {
            controller = GetComponent<PlayerController>();
            Debug.Assert(controller != null);
        }

        private void Update()
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            if (Input.GetKey(KeyCode.Q))
                move.z = -1f;
            else if (Input.GetKey(KeyCode.E))
                move.z = 1f;
            Vector2 look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            bool quick = Input.GetKey(KeyCode.LeftShift);

            controller.isQuick = quick;
            controller.Move(move);
            controller.Look(look);
        }
    }
}