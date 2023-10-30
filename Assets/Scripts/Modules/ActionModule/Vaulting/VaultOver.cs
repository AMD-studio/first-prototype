using UnityEngine;

namespace Climbing
{
    public class VaultOver : VaultAction
    {
        public VaultOver() { }

        public VaultOver(ThirdPersonController _vaultingController, Action action) : base(_vaultingController, action)
        {
        }


        /// <summary>
        /// Checks if can Vault the Box Obstacle
        /// </summary>
        public override bool CheckAction()
        {
            if (!controller.characterInput.jump || controller.isVaulting)
            {
                return false;
            }

            Vector3 origin = controller.transform.position + kneeRaycastOrigin;

            //Checks if Vault obstacle in front
            if (controller.characterDetection.ThrowRayOnDirection(origin, controller.transform.forward, kneeRaycastLength, out RaycastHit hit) &&
                (hit.normal == hit.collider.transform.forward || hit.normal == -hit.collider.transform.forward) != false &&
                Mathf.Abs(Vector3.Dot(-hit.normal, controller.transform.forward)) >= 0.60 &&
                hit.transform.CompareTag(tag))
            {
                //Gets Box width and adds an offset for the downward ray
                Vector3 origin2 = origin + (-hit.normal * (hit.transform.localScale.z + landOffset));

                //Get landing position and set target position
                if (controller.characterDetection.ThrowRayOnDirection(origin2, Vector3.down, 10, out RaycastHit hit2) && hit2.collider)
                {
                    controller.characterAnimation.animator.CrossFade("Deep Jump", 0.05f);

                    startTransform = new TransformData(controller.transform.position, controller.transform.rotation);
                    targetTransform = new TransformData(hit2.point, Quaternion.LookRotation(hit2.point - startTransform.Position));

                    vaultTime = startDelay;
                    animLength = 0.85f;
                    controller.DisableController();

                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Executes Vaulting Animation
        /// </summary>
        public override bool Update()
        {
            if (controller.isVaulting)
            {
                // Actual speep mul to speed from anim state
                vaultTime += (Time.deltaTime / animLength) * animator.animState.speed;

                if (vaultTime >= 1)
                {
                    controller.EnableController();
                }
                else
                {
                    if (vaultTime >= 0)
                    {
                        controller
                            .transform
                            .SetPositionAndRotation(
                                Vector3.Lerp(startTransform.Position, targetTransform.Position, vaultTime),
                                Quaternion.Lerp(startTransform.Rotation, targetTransform.Rotation, vaultTime * 4));
                    }
                    return true;
                }
            }

            return false;
        }

        public override void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetTransform.Position, 0.08f);
        }
    }
}
