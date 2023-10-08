using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float distance = 5;
    [SerializeField] private float minVerticalAngle = -45;
    [SerializeField] private float maxVerticalAngle = 45;
    [SerializeField] private Vector2 framingOffset;
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;

    private float rotationX;
    private float rotationY;
    private float invertXVal;
    private float invertYVal;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleRotationInput();
        UpdateCameraPosition();
    }

    private void HandleRotationInput()
    {
        invertXVal = invertX ? -1 : 1;
        invertYVal = invertY ? -1 : 1;

        rotationX += Input.GetAxis("Camera Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Camera X") * invertXVal * rotationSpeed;
    }

    private void UpdateCameraPosition()
    {
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}