using System.Collections.Generic;
using UnityEngine;

namespace Climbing
{
    [RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(Animator))]
    public class AnimationCharacterController : MonoBehaviour
    {
        private ThirdPersonController controller;
        private Vector3 animVelocity;

        [HideInInspector] 
        public Animator animator;
        public SwitchCameras switchCameras;
        public AnimatorStateInfo animState;

        private MatchTargetWeightMask matchTargetWeightMask = new(Vector3.one, 0);

        private readonly Dictionary<Vector2, string> directionToAnimation = new()
        {
            { new Vector2(-1, 0), "Braced Hang Hop Left" },
            { new Vector2(-1, 1), "Braced Hang Hop Left" },
            { new Vector2(-1, -1), "Braced Hang Hop Left" },
            { new Vector2(1, 0), "Braced Hang Hop Right" },
            { new Vector2(1, 1), "Braced Hang Hop Right" },
            { new Vector2(1, -1), "Braced Hang Hop Right" },
            { new Vector2(0, 1), "Braced Hang Hop Up" },
            { new Vector2(0, -1), "Braced Hang Hop Down" }
        };

        void Start()
        {
            controller = GetComponent<ThirdPersonController>();
            animator = GetComponent<Animator>();
            switchCameras = Camera.main.GetComponent<SwitchCameras>();
        }

        void Update()
        {
            animator.SetFloat("Velocity", animVelocity.magnitude);

            animState = animator.GetCurrentAnimatorStateInfo(0);

            // One of tags
            animator.applyRootMotion = animState.IsTag("Root") || animState.IsTag("Drop");
        }

        public void SetAnimVelocity(Vector3 value) { animVelocity = value; animVelocity.y = 0; }
        public Vector3 GetAnimVelocity() { return animVelocity; }

        public bool RootMotion() { return animator.applyRootMotion; }

        public void Fall()
        {
            animator.SetBool("Jump", false);
            animator.SetBool("onAir", true);
            animator.SetBool("Land", false);
            controller.characterMovement.DisableFeetIK();
        }

        public void Land()
        {
            animator.SetBool("Jump", false);
            animator.SetBool("onAir", false);
            animator.SetBool("Land", true);
            controller.characterMovement.EnableFeetIK();
        }

        public void HangLedge(ClimbController.ClimbState state)
        {
            if (state == ClimbController.ClimbState.BHanging)
                animator.CrossFade("Idle To Braced Hang", 0.2f);
            else if (state == ClimbController.ClimbState.FHanging)
                animator.CrossFade("Idle To Freehang", 0.2f);

            animator.SetBool("Land", false);
            animator.SetInteger("Climb State", (int)state);
            animator.SetBool("Hanging", true);
        }

        public void LedgeToLedge(ClimbController.ClimbState state, Vector3 direction, ref float startTime, ref float endTime)
        {
            if (state == ClimbController.ClimbState.BHanging)
            {
                if (directionToAnimation.TryGetValue(direction, out string animation))
                {
                    animator.CrossFade(animation, 0.2f);

                    if (direction == Vector3.up || direction == Vector3.down)
                    {
                        startTime = 0.3f;
                        endTime = (direction == Vector3.down) ? 0.7f : 0.48f;
                    }
                    else
                    {
                        startTime = 0.2f;
                        endTime = 0.49f;
                    }
                }
            }

            animator.SetInteger("Climb State", (int)state);
            animator.SetBool("Hanging", true);
        }

        public void BracedClimb()
        {
            animator.CrossFade("Braced Hang To Crouch", 0.2f);
        }
        public void FreeClimb()
        {
            animator.CrossFade("Freehang Climb", 0.2f);
        }
        public void DropToFree(int state)
        {
            animator.CrossFade("Drop To Freehang", 0.1f);
            animator.SetInteger("Climb State", (int)state);
            animator.SetBool("Hanging", true);
            SetAnimVelocity(Vector3.forward);
        }
        public void DropToBraced(int state)
        {
            animator.CrossFade("Drop To Bracedhang", 0.1f);
            animator.SetInteger("Climb State", (int)state);
            animator.SetBool("Hanging", true);
            SetAnimVelocity(Vector3.forward);
        }

        public void DropLedge(int state)
        {
            animator.SetBool("Hanging", false);
            animator.SetInteger("Climb State", state);
        }

        public void HangMovement(float value, int climbstate)
        {
            animator.SetFloat("Horizontal", Mathf.Lerp(animator.GetFloat("Horizontal"), value, Time.deltaTime * 15));
            animator.SetInteger("Climb State", climbstate);
        }
        public void JumpPrediction(bool state)
        {
            controller.characterAnimation.animator.CrossFade("Predicted Jump", 0.1f);
            animator.SetBool("Crouch", state);
        }

        public void EnableIKSolver()
        {
            controller.characterMovement.EnableFeetIK();
        }
        public void EnableController()
        {
            controller.EnableController();
        }

        public void SetMatchTarget(AvatarTarget avatarTarget, Vector3 targetPos, Quaternion targetRot, Vector3 offset, float startnormalizedTime, float targetNormalizedTime)
        {
            if (animator.isMatchingTarget)
                return;

            float normalizeTime = Mathf.Repeat(animState.normalizedTime, 1f);

            if (normalizeTime > targetNormalizedTime)
                return;

            animator.SetTarget(avatarTarget, targetNormalizedTime); //Sets Target Bone for reference motion
            animator.MatchTarget(targetPos + offset, targetRot, avatarTarget, matchTargetWeightMask, startnormalizedTime, targetNormalizedTime, true);
        }
    }

}