using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtil
{
    public static bool ThreeRaycasts(
        Vector3 origin, 
        Vector3 dir, float 
        spacing, 
        Transform transform, 
        out List<RaycastHit> hits, 
        float distance, 
        LayerMask layer, 
        bool debugDraw = false)
    {
        hits = new List<RaycastHit>();
        bool hitFound = false;

        RaycastHit centerHit, leftHit, rightHit;

        if (Physics.Raycast(origin, Vector3.down, out centerHit, distance, layer))
        {
            hitFound = true;
            hits.Add(centerHit);
        }

        if (Physics.Raycast(origin - transform.right * spacing, Vector3.down, out leftHit, distance, layer))
        {
            hitFound = true;
            hits.Add(leftHit);
        }

        if (Physics.Raycast(origin + transform.right * spacing, Vector3.down, out rightHit, distance, layer))
        {
            hitFound = true;
            hits.Add(rightHit);
        }

        if (hitFound && debugDraw)
        {
            Debug.DrawLine(origin, centerHit.point, Color.red);
            Debug.DrawLine(origin - transform.right * spacing, leftHit.point, Color.red);
            Debug.DrawLine(origin + transform.right * spacing, rightHit.point, Color.red);
        }

        return hitFound;
    }
}
