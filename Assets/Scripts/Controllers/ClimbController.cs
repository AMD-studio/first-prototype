using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    private ClimbPoint currentPoint;
    private PlayerController playerController;
    private ParkourController parkourController;
    private EnvironmentScanner envScanner;
    [SerializeField] private ParkourAction jumpDownAction;
    [SerializeField] private ParkourAction clibUpAction;

    private void Awake()
    {
        parkourController = GetComponent<ParkourController>();
        playerController = GetComponent<PlayerController>();
        envScanner = GetComponent<EnvironmentScanner>();
    }

    private void Update()
    {
        if (!playerController.IsHanging)
        {
            TryToHang();
        }
        else
        {
            HandleHangingInput();
        }
    }


    private void TryToHang()
    {
        if (CanHang())
            StartCoroutine(HangOnLedge());   
    }

    private bool CanHang()
    {
        if (Input.GetButton("Jump") && !playerController.InAction && envScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
        {
            currentPoint = ledgeHit.transform.GetComponent<ClimbPoint>();
            return true;
        }
        return false;
    }

    private IEnumerator HangOnLedge()
    {
        playerController.SetControl(false);

        string animation = "IdleToHang";
        Transform ledgeTransform = currentPoint.transform;
        float matchStartTime = 0.41f;
        float matchTargetTime = 0.54f;

        AvatarTarget hand = AvatarTarget.RightHand;
        Vector3 handOffset = new Vector3(0.25f, 0.1f, 0.1f);

        MatchTargetModel matchParams = new MatchTargetModel
        {
            Position = GetHandPosition(ledgeTransform, hand, handOffset),
            BodyPart = hand,
            StartTime = matchStartTime,
            TargetTime = matchTargetTime,
            PositionWeight = Vector3.one
        };

        Quaternion targetRotation = Quaternion.LookRotation(-ledgeTransform.forward);

        yield return playerController.DoAction(animation, matchParams, targetRotation, new ActionParameters { Rotate = true });

        playerController.IsHanging = true;
    }

    private void HandleHangingInput()
    {
        float horizontalInput = Mathf.Round(Input.GetAxisRaw("Horizontal"));
        float verticalInput = Mathf.Round(Input.GetAxisRaw("Vertical"));

        Vector2 inputDir = new Vector2(horizontalInput, verticalInput);

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetButton("Jump"))
        {
            Debug.Log("GetDown");
            HandleGetDown();
        }

        if (playerController.InAction || inputDir == Vector2.zero)
            return;

        ActionOnNeighbour(inputDir);
    }

    private void ActionOnNeighbour(Vector2 inputDir)
    {
        var neighbour = currentPoint.GetNeighbour(inputDir);

        if (neighbour == null)
            return;

        var connectionType = neighbour.connectionType;

        if (Input.GetButton("Jump"))
        {
            if ((connectionType & ConnectionType.Jump) != 0)
            {
                Debug.Log("Jump");
                HandleJump(neighbour);
            }
            else if ((connectionType & ConnectionType.GetUp) != 0 && Input.GetKey(KeyCode.W))
            {
                Debug.Log("GetUp");
                HandleGetUp();
            }
        }
        else if ((connectionType & ConnectionType.Move) != 0)
        {
            Debug.Log("Move");
            HandleMove(neighbour);
        }
    }

    private void HandleGetUp()
    {
        LeaveEdge();
        StartCoroutine(parkourController.DoAction(clibUpAction));
    }

    private void HandleGetDown()
    {
        LeaveEdge();
        StartCoroutine(parkourController.DoAction(jumpDownAction));
    }

    void LeaveEdge()
    {
        playerController.IsHanging = false;
        currentPoint = null;
    }

    private void HandleJump(Neighbour neighbour)
    {
        currentPoint = neighbour.point;
        Transform ledgeTransform = currentPoint.transform;

        string animation;
        if (neighbour.direction.y == 1)
        {
            animation = "HangHopUp";
            StartCoroutine(JumpToLedge(animation, ledgeTransform, 0.35f, 0.65f, AvatarTarget.RightHand, new Vector3(0.25f, 0.08f, 0.15f)));
        }
        else if (neighbour.direction.y == -1)
        {
            animation = "HangHopDown";
            StartCoroutine(JumpToLedge(animation, ledgeTransform, 0.31f, 0.65f, AvatarTarget.RightHand, new Vector3(0.25f, 0.08f, 0.15f)));
        }
        else if (neighbour.direction.x == 1)
        {
            animation = "HangHopRight";
            StartCoroutine(JumpToLedge(animation, ledgeTransform, 0.20f, 0.50f, AvatarTarget.RightHand, new Vector3(0.20f, 0.05f, 0.1f)));
        }
        else if (neighbour.direction.x == -1)
        {
            animation = "HangHopLeft";
            StartCoroutine(JumpToLedge(animation, ledgeTransform, 0.20f, 0.50f, AvatarTarget.LeftHand, new Vector3(0.20f, 0.05f, 0.1f)));
        }
    }

    private void HandleMove(Neighbour neighbour)
    {
        currentPoint = neighbour.point;
        Transform ledgeTransform = currentPoint.transform;

        string animation;
        if (neighbour.direction.x == 1)
        {
            animation = "ShimmyRight";
            StartCoroutine(JumpToLedge(animation, ledgeTransform, 0f, 0.38f, AvatarTarget.RightHand, new Vector3(0.25f, 0.05f, 0.1f)));
        }
        else if (neighbour.direction.x == -1)
        {
            animation = "ShimmyLeft";
            StartCoroutine(JumpToLedge(animation, ledgeTransform, 0f, 0.38f, AvatarTarget.LeftHand, new Vector3(0.25f, 0.05f, 0.1f)));
        }
    }

    private Vector3 GetHandPosition(Transform ledge, AvatarTarget hand, Vector3? handOffset)
    {
        Vector3 offsetValue = handOffset.GetValueOrDefault();

        Vector3 handDirection = (hand == AvatarTarget.RightHand) ? ledge.right : -ledge.right;
        return ledge.position + ledge.forward * offsetValue.z + Vector3.up * offsetValue.y - handDirection * offsetValue.x;
    }

    IEnumerator JumpToLedge(
        string animation, 
        Transform ledge, 
        float matchStartTime, 
        float matchTargetTime,
      AvatarTarget hand = AvatarTarget.RightHand, Vector3? handOffset = null)
    {
        var matchParams = new MatchTargetModel()
        {
            Position = GetHandPosition(ledge, hand, handOffset),
            BodyPart = hand,
            StartTime = matchStartTime,
            TargetTime = matchTargetTime,
            PositionWeight = Vector3.one
        };

        var targetRot = Quaternion.LookRotation(-ledge.forward);

        yield return playerController.DoAction(animation, matchParams, targetRot, new ActionParameters { Rotate = true });

        playerController.IsHanging = true;
    }
 
}
