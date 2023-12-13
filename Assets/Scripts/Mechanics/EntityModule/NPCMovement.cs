using System.Collections;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public Animator animator;
    public LayerMask groundLayer;
    public float movementSpeed = 3.0f;
    public float stopDuration = 2.0f;
    public float moveRadius = 10.0f;
    public bool IsDebug = false;

    private Vector3 centerPoint;
    private bool isMoving = true;
    private Vector3 targetPoint;
    private Vector3 lookDirection;

    private void Start()
    {
        centerPoint = transform.position;
        SetNewRandomTarget();
    }

    private void Update()
    {
        if (isMoving)
        {
            Vector3 newPosition = Vector3.MoveTowards(
                new Vector3(transform.position.x, 0, transform.position.z), 
                new Vector3(targetPoint.x, 0, targetPoint.z), 
                movementSpeed * Time.deltaTime);

            if (IsDebug)
            {
                Debug.Log("New position: " + newPosition);
                Debug.Log("Target point: " + targetPoint);
            }

            transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);

            lookDirection = (new Vector3(targetPoint.x, 0, targetPoint.z) - 
                new Vector3(newPosition.x, 0, newPosition.z)).normalized;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * movementSpeed);

            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPoint.x, 0, targetPoint.z)) < 0.01f)
            {
                animator.SetFloat("Velocity", 0f); // Установить значение Velocity в 0
                StartCoroutine(StopAndSetNewRandomTarget());
            }
            else
            {
                animator.SetFloat("Velocity", 3.0f); // Установить значение Velocity
            }
        }
    }

    private void SetNewRandomTarget()
    {
        targetPoint = centerPoint + Random.insideUnitSphere * moveRadius;

        Ray ray = new(targetPoint + Vector3.up * 1000, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 pointOnGround = hit.point;

            if (hit.collider.attachedRigidbody)
            {
                pointOnGround -= new Vector3(0, hit.collider.attachedRigidbody.transform.position.y, 0);
            }

            targetPoint = pointOnGround;

            if (IsDebug)
            {
                Debug.Log("Target point after raycast: " + targetPoint);
            }
        }
    }

    private IEnumerator StopAndSetNewRandomTarget()
    {
        isMoving = false;
        yield return new WaitForSeconds(stopDuration);
        SetNewRandomTarget();
        isMoving = true;
    }
}
