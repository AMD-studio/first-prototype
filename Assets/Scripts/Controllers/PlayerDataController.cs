using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;

    private bool hasControl = true;
    public bool InAction { get; private set; }
    public bool IsHanging { get; set; }
    public bool isGrounded;

    private Vector3 desiredMoveDir;
    private Vector3 moveDir;
    private Vector3 velocity;

    public bool IsOnLedge { get; set; }
    public LedgeData LedgeData { get; set; }

    private float ySpeed;
    private Quaternion targetRotation;

    private CameraController cameraController;
    private Animator animator;
    private CharacterController characterController;
    private EnvironmentScanner environmentScanner;

    public bool HasControl
    {
        get { return hasControl; }
        set { hasControl = value; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set { rotationSpeed = value; }
    }

    public LayerMask GroundLayer
    {
        get { return groundLayer; }
        set { groundLayer = value; }
    }

    public Vector3 GroundCheckOffset
    {
        get { return groundCheckOffset; }
        set { groundCheckOffset = value; }
    }

}