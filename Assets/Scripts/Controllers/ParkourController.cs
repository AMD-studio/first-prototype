using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpDownAction;
    [SerializeField] float autoDropHeightLimit = 1f;

    EnvironmentScanner environmentScanner;
    Animator animator;
    PlayerController playerController;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();

        if (Input.GetButton("Jump") && !playerController.InAction && !playerController.IsHanging)
        {
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoAction(action));
                        break;
                    }
                }
            }
        }

        if (playerController.IsOnLedge && !playerController.InAction && !hitData.forwardHitFound)
        {
            bool shouldJump = true;
            if (playerController.LedgeData.height > autoDropHeightLimit && !Input.GetButton("Jump"))
                shouldJump = false;

            if (shouldJump && playerController.LedgeData.angle <= 50)
            {
                playerController.IsOnLedge = false;
                StartCoroutine(DoAction(jumpDownAction));
            }
        }
    }

    public IEnumerator DoAction(ParkourAction action)
    {
        playerController.SetControl(false);

        MatchTargetModel matchModel = null;
        if (action.EnableTargetMatching)
        {
            matchModel = new MatchTargetModel
            {
                Position = action.MatchPos,
                BodyPart = action.MatchBodyPart,
                PositionWeight = action.MatchPosWeight,
                StartTime = action.MatchStartTime,
                TargetTime = action.MatchTargetTime
            };
        }

        var actionParams = new ActionParameters
        {
            Rotate = action.RotateToObstacle,
            PostDelay = action.PostActionDelay,
            Mirror = action.Mirror
        };

        yield return playerController.DoAction(action.AnimName, matchModel, action.TargetRotation, actionParams);

        playerController.SetControl(true);
    }
}