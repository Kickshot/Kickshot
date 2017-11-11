using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed;
        public float quickMoveSpeed;

        [Header("Mouse Settings")]
        public float lookSensitivity;
        public float lookSmoothing;
        public bool lockCursor = true;
        public bool mouseSmooth = true;

        [HideInInspector]
        public bool isQuick = false;

        private Vector3 moveInput;
        private Vector2 lookDelta;
        private Vector2 smoothMouse = Vector2.zero;
        private Vector2 mouseAbsolute = Vector2.zero;

        public void Move(Vector3 mInput)
        {
            moveInput = mInput;
        }

        public void Look(Vector2 lDelta)
        {
            lookDelta = lDelta;
        }

        private void LateUpdate()
        {
            if (lockCursor) Cursor.lockState = CursorLockMode.Locked;

            //WASD Movement
            Vector3 moveDelta = moveInput.y * Vector3.forward + moveInput.x * Vector3.right + moveInput.z * Vector3.up;
            moveDelta *= isQuick ? quickMoveSpeed : moveSpeed;
            transform.Translate(moveDelta * Time.deltaTime);

            //Mouse Look
            Vector2 mouseDelta = lookDelta * lookSensitivity;
            mouseDelta *= mouseSmooth ? lookSmoothing : 1f;

            smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, mouseSmooth ? 1f / lookSmoothing : 1f);
            smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, mouseSmooth ? 1f / lookSmoothing : 1f);
            mouseAbsolute += smoothMouse;

            mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -90f, 90f);

            transform.localRotation = Quaternion.Euler(-mouseAbsolute.y, mouseAbsolute.x, 0f);
        }
    }
}