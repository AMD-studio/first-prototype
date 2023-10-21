﻿using UnityEngine;

namespace Climbing
{
    public class CameraController : MonoBehaviour
    {
        private CinemachineCameraOffset cameraOffset;

        public Vector3 _offset;
        public Vector3 _default;
        private Vector3 _target;

        public float maxTime = 2.0f;
        private float curTime = 0.0f;
        private bool anim = false;


        void Start()
        {
            cameraOffset = GetComponent<CinemachineCameraOffset>();
        }


        void Update()
        {
            if (anim)
            {
                curTime += Time.deltaTime / maxTime;
                cameraOffset.m_Offset = Vector3.Lerp(cameraOffset.m_Offset, _target, curTime);
            }

            if (curTime >= 1.0f)
                anim = false;
        }

        /// <summary>
        /// Adds Offset to the camera while being on Climbing or inGround
        /// </summary>
        public void NewOffset(bool offset)
        {
            _target = offset ? 
                _offset : 
                _default;
            anim = true;
            curTime = 0;
        }
    }
}