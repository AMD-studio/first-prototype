using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    private void InitializeComponents()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        environmentScanner = GetComponent<EnvironmentScanner>();
    }

    private void Awake()
    {
        InitializeComponents();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    private float HandleMoveAmountInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        var moveInput = new Vector3(h, 0, v).normalized;

        desiredMoveDir = cameraController.PlanarRotation * moveInput;
        moveDir = desiredMoveDir;

        return moveAmount;
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void Update()
    {
        if (!hasControl || IsHanging)
            return;

        float moveAmount = HandleMoveAmountInput();

        velocity = Vector3.zero;
        bool isGrounded = IsGrounded();

        UpdateAnimator(isGrounded);

        if (isGrounded)
        {
            HandleGroundedMovement();
        }
        else
        {
            HandleAirborneMovement();
        }

        HandleRotation(moveAmount);
    }

    private void UpdateAnimator(bool isGrounded)
    {
        animator.SetBool("isGrounded", isGrounded);
    }

    private void HandleGroundedMovement()
    {
        ySpeed = -0.5f;
        velocity = desiredMoveDir * moveSpeed;

        IsOnLedge = environmentScanner.ObstacleLedgeCheck(desiredMoveDir, out LedgeData ledgeData);
        if (IsOnLedge)
        {
            LedgeData = ledgeData;
            LedgeMovement();
        }

        animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);

        ApplyVelocity();
    }

    private void HandleAirborneMovement()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;
        velocity = transform.forward * moveSpeed / 2;
        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        velocity.y = ySpeed;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleRotation(float moveAmount)
    {
        if (moveAmount > 0 && moveDir.magnitude > 0.2f)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void LedgeMovement()
    {
        float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, desiredMoveDir, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        if (Vector3.Angle(desiredMoveDir, transform.forward) >= 80)
        {
            // Don't move, but rotate
            velocity = Vector3.zero;
            return;
        }

        if (angle < 60)
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
        else if (angle < 90)
        {
            // Angle is between 60 and 90, so limit the velocity to horizontal direction
            var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
            var dir = left * Mathf.Sign(signedAngle);

            velocity = velocity.magnitude * dir;
            moveDir = dir;
        }
    }

    public IEnumerator DoAction(string animName, MatchTargetModel matchParams, Quaternion targetRotation, ActionParameters actionParams)
    {
        InAction = true;

        animator.SetBool("mirrorAction", actionParams.Mirror);
        animator.CrossFadeInFixedTime(animName, 0.2f);

        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);

        if (!animState.IsName(animName))
            Debug.LogError("The parkour animation is wrong!");

        float rotateStartTime = (matchParams != null) ? matchParams.StartTime : 0f;

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (actionParams.Rotate && normalizedTime > rotateStartTime)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (matchParams != null)
                MatchTarget(matchParams);

            if (animator.IsInTransition(0) && timer > 0.5f)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(actionParams.PostDelay);

        InAction = false;
    }

    private void MatchTarget(MatchTargetModel mp)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(mp.Position, transform.rotation, mp.BodyPart, new MatchTargetWeightMask(mp.PositionWeight, 0), mp.StartTime, mp.TargetTime);
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }
}