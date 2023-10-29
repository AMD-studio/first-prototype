using System.Collections.Generic;
using UnityEngine;
using System;
using Climbing.DependencyInjection;
using Unity.VisualScripting;
using System.Linq;

namespace Climbing
{
    [Flags]
    public enum VaultActions
    {
        Nothing = 0,
        Vault_Obstacle = 1 << 0,
        Vault_Over = 1 << 1,
        Slide = 1 << 2,
        Reach = 1 << 3,
        Climb_Ledge = 1 << 4,
        Jump_Prediction = 1 << 5,
        Vault_Down = 1 << 6,
    }

    public class VaultingController : MonoBehaviour
    {
        public bool debug;
        public VaultActions vaultActions;

        [HideInInspector] public ThirdPersonController controller;
        [HideInInspector] public Animator animator;

        private readonly List<VaultAction> actions = new();
        private VaultAction curAction;

        private void AddActionIfFlagSet(VaultActions vaultActions, VaultActions actionFlag, IVaultActionCreator actionCreator)
        {
            var tmpAction = actionCreator.CreateAction();

            if (vaultActions.HasFlag(actionFlag) && 
                tmpAction != null)
            {
                actions.Add(tmpAction);
            }
        }

        public void Start()
        {
            controller = GetComponent<ThirdPersonController>();
            animator = GetComponent<Animator>();

            IActionLoader actionLoader = new ActionLoader();

            AddActionIfFlagSet(vaultActions, 
                VaultActions.Vault_Obstacle, 
                new VaultCreator<VaultObstacle>(controller, actionLoader, "ActionsConfig/VaultObstacle"));

            AddActionIfFlagSet(vaultActions, 
                VaultActions.Vault_Over, 
                new VaultCreator<VaultOver>(controller, actionLoader, "ActionsConfig/VaultOver"));
            
            AddActionIfFlagSet(vaultActions, 
                VaultActions.Slide, 
                new VaultCreator<VaultSlide>(controller, actionLoader, "ActionsConfig/VaultSlide"));
            
            AddActionIfFlagSet(vaultActions, 
                VaultActions.Slide, 
                new VaultCreator<VaultReach>(controller, actionLoader, "ActionsConfig/VaultReach"));
            
            AddActionIfFlagSet(vaultActions, 
                VaultActions.Climb_Ledge, 
                new VaultCreator<VaultClimbLedge>(controller));
            
            AddActionIfFlagSet(vaultActions, 
                VaultActions.Jump_Prediction, 
                new VaultCreator<VaultJumpPrediction>(controller));
            
            AddActionIfFlagSet(vaultActions, 
                VaultActions.Vault_Down, 
                new VaultCreator<VaultDown>(controller));
        }

        private void Update()
        {
            if (!controller.isVaulting)
            {
                curAction = null;
            }

            //Check if vaulting action can be performed
            foreach (var item in actions)
            {
                if (item.CheckAction())
                {
                    curAction = item;
                    controller.isVaulting = true;
                    break;
                }
            }

            //Update logic of current vaulting Action
            if (curAction != null && controller.isVaulting)
            {
                if (!curAction.Update())
                    controller.isVaulting = false;
            }
        }

        private void FixedUpdate()
        {
            //Fixed Update logic of current vaulting Action
            if (curAction != null && controller.isVaulting)
            {
                if(!curAction.FixedUpdate())
                    controller.isVaulting = false;
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            curAction?.OnAnimatorIK(layerIndex);
        }

        private void OnDrawGizmos()
        {
            if (curAction != null && debug)
            {
                curAction.DrawGizmos();
            }
        }
    }

}