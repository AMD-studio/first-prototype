using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Climbing
{

    [RequireComponent(typeof(ThirdPersonController))]
    public class DetectionCharacterController : MonoBehaviour
    {
        public bool showDebug = true;

        [Header("Layers")]
        public LayerMask ledgeLayer;
        public LayerMask climbLayer;

        [Header("Rays")]
        [SerializeField] private Vector3 OriginLedgeRay;
        [SerializeField] private Vector3 OriginFeetRay;
        [SerializeField] private float LedgeRayLength = 1.5f;
        [SerializeField] private float FeetRayLength = 0.6f;
        [SerializeField] private float FindLedgeNumRays = 7;
        [SerializeField] private float DropLedgeNumRays = 8;

        public bool FindLedgeCollision(out RaycastHit hit)
        {
            Vector3 rayOrigin = transform.TransformDirection(OriginLedgeRay) + transform.position;

            for (int i = 0; i < FindLedgeNumRays; i++)
            {
                if (ThrowRayToLedge(rayOrigin + new Vector3(0, 0.15f * i, 0), out hit))
                {
                    return true;
                }
            }

            //Set invalid hit
            Physics.Raycast(Vector3.zero, Vector3.forward, out hit, 0, -1);
            return false;
        }

        public bool FindDropLedgeCollision(out RaycastHit hit)
        {
            for (int i = 0; i < DropLedgeNumRays; i++)
            {
                Vector3 origin = transform.position + transform.forward * 0.8f - new Vector3(0, i * 0.15f, 0);

                Debug.DrawLine(origin, transform.position - new Vector3(0, i * 0.15f, 0));

                if(Physics.Raycast(origin, -transform.forward, out hit, 0.8f, ledgeLayer))
                {
                    if (showDebug) //Normal
                    {
                        Debug.DrawLine(hit.point, hit.point + hit.normal, Color.cyan);
                    }

                    if (hit.normal == -hit.transform.forward)
                    {
                        return true;
                    }
                }
            }

            //Set invalid hit
            Physics.Raycast(Vector3.zero, Vector3.forward, out hit, 0, -1);
            return false;
        }

        public bool FindFootCollision(Vector3 targetPos, Quaternion rot, Vector3 normal)
        {
            Vector3 pointFoot1 = targetPos + rot * (new Vector3(-0.15f, -0.10f, 0) + OriginFeetRay);
            Vector3 pointFoot2 = targetPos + rot * (new Vector3(0.10f, 0, 0) + OriginFeetRay);

            return Physics.Raycast(pointFoot1, -normal, FeetRayLength) && 
                   Physics.Raycast(pointFoot2, -normal, FeetRayLength);
        }


        public bool ThrowRayToLedge(Vector3 origin, out RaycastHit hit)
        {
            if (showDebug)
            {
                Debug.DrawLine(origin, origin + transform.forward * LedgeRayLength, Color.green);
            }

            if (Physics.Raycast(origin, transform.forward, out hit, LedgeRayLength, ledgeLayer))
            {
                if (showDebug) //Normal
                {
                    Debug.DrawLine(hit.point, hit.point + hit.normal, Color.cyan);
                }

                if (hit.normal == hit.transform.forward || hit.normal == -hit.transform.forward)
                    return true;

            }
            return false;

        }
        public bool ThrowClimbRay(Vector3 origin, Vector3 direction, float length, out RaycastHit hit)
        {

            Vector3 origin1 = origin + new Vector3(0, 1.8f, 0);
            Vector3 origin2 = origin + new Vector3(0, 0.5f, 0);

            if (showDebug)
            {
                Debug.DrawLine(origin1, origin1 + direction * length, Color.green);
                Debug.DrawLine(origin2, origin2 + direction * length, Color.green);
            }

            if (!Physics.Raycast(origin1, direction, out hit, length) && 
                !Physics.Raycast(origin2, direction, out hit, length)) //Check Forward
            {
                Vector3 origin3 = origin + direction * 0.15f + new Vector3(0,0.5f,0);

                if (showDebug)
                {
                    Debug.DrawLine(origin3, origin3 - Vector3.up * length, Color.cyan);
                }

                if (Physics.Raycast(origin3, -Vector3.up, out hit, length))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ThrowHandRayToLedge(Vector3 origin, Vector3 direction, float length, out RaycastHit hit)
        {
            if (showDebug)
            {
                Debug.DrawLine(origin, origin + transform.TransformDirection(direction) * length, Color.green);
            }

            return Physics.Raycast(origin, transform.TransformDirection(direction), out hit, length, ledgeLayer);
        }

        public bool ThrowFootRayToLedge(Vector3 origin, Vector3 direction, float length, out RaycastHit hit)
        {
            if (showDebug)
            {
                Debug.DrawLine(origin, origin + transform.TransformDirection(direction) * length, Color.green);
            }

            return Physics.Raycast(origin, transform.TransformDirection(direction), out hit, length, climbLayer);
        }

        public bool ThrowRayOnDirection(Vector3 origin, Vector3 direction, float length, out RaycastHit hit, LayerMask layer)
        {
            if (showDebug)
            {
                Debug.DrawLine(origin, origin + direction * length, Color.green);
            }

            return Physics.Raycast(origin, direction, out hit, length, layer);
        }
        public bool ThrowRayOnDirection(Vector3 origin, Vector3 direction, float length, out RaycastHit hit)
        {
            if (showDebug)
            {
                Debug.DrawLine(origin, origin + direction * length, Color.green);
            }

            return Physics.Raycast(origin, direction, out hit, length);
        }

        public bool ThrowRayOnDirection(Vector3 origin, Vector3 direction, float length)
        {
            if (showDebug)
            {
                Debug.DrawLine(origin, origin + direction * length, Color.green);
            }

            return Physics.Raycast(origin, direction, length);
        }

        public bool IsGrounded(float stepHeight) {
            if (showDebug)
            {
                Debug.DrawLine(transform.position + new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, 0.5f, 0) + Vector3.down * 0.8f, Color.green);
            }

            return Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), Vector3.down, out _, 0.7f);//0.2f
        }

        public void FindAheadPoints(ref List<HandlePoints> list)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 5);

            foreach (var item in cols)
            {
                if (Vector3.Dot(item.transform.position, transform.position) > 0)
                {
                    HandlePoints handle = item.GetComponentInChildren<HandlePoints>();
                    if (handle)
                        list.Add(handle);
                }
            }
        }
    }
}
