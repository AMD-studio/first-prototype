using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegBehavior : MonoBehaviour
{
    public LayerMask terrainLayer;

    public Transform leftUpLeg;
    public Transform rightUpLeg;
    public float legRayLength = 2.0f;

    void Update()
    {
        AdjustLegToTerrain(leftUpLeg);
        AdjustLegToTerrain(rightUpLeg);
    }

    private void AdjustLegToTerrain(Transform leg)
    {
        if (leg == null)
            return;

        // Создаем луч, который будет выпущен с текущей позиции ноги вниз
        Ray ray = new Ray(leg.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, legRayLength, terrainLayer))
        {
            Vector3 surfaceNormal = hit.normal;

            // Наклоняем ногу так, чтобы она смотрела в сторону поверхности
            leg.LookAt(leg.position + surfaceNormal, Vector3.up);

            // Рекурсивно применяем коррекцию ко всем дочерним объектам ноги
            foreach (Transform child in leg)
            {
                AdjustLegToTerrain(child);
            }
        }
    }
}
