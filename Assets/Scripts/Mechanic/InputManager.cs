using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private Vector2 axis2D;
    private const string hKey = "Horizontal";
    private const string vKey = "Vertical";


    [Header("Old Reference")]
    [SerializeField] private CharacterMovement cm;
    [SerializeField] private CharacterMovement1 cm1;
    [SerializeField] private CharacterCombat cb1;

    [Header("New Reference")]
    [SerializeField] private PlayerAgent agent;
    [SerializeField] private CameraManager cam;
    [SerializeField] private bool debugMode = false;
    public enum CallbackContext
    {
        None,
        Perform,
        Cancel
    }
    public enum MouseState
    {
        Mouse0,
        Mouse1
    }

    private void Update()
    {
        AxisInput();
        CommandAgent();
        CommandUI();
    }

    private void CommandUI()
    {
        if (GameManager.Instance.gameStage == GameManager.GameStage.Gameplay)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OptionMenu.Instance.SwitchPause();
            }
        }
    }

    private void AxisInput()
    {
        horizontal = Input.GetAxis(hKey);
        vertical = Input.GetAxis(vKey);
        axis2D.x = horizontal;
        axis2D.y = vertical;
        axis2D.Normalize();
    }

    private void FixedUpdate()
    {
        if (agent == null) return;
        agent.Movement.RotateTowardCamera();
    }

    public void SetAgent(PlayerAgent player)
    {
        agent = player;
    }

    public PlayerAgent GetAgent()
    {
        return agent == null ? null : agent;
    }

    public void SetCam(CameraManager manager)
    {
        cam = manager;
        if(manager != null)
        {
            if(agent != null)
            {
                agent.Movement.cm = manager;
                if(agent.Combat.PrimaryWeapon is Gun)
                {
                    Gun primaryGun = agent.Combat.PrimaryWeapon as Gun;
                    primaryGun.SetupAim(manager.GetCenter());
                }
            }
        }
    }

    public CameraManager GetCam()
    {
        return cam == null ? null : cam;
    }

    private void CommandAgent()
    {
        if (OptionMenu.Instance.IsPause()) return;
        if (agent == null) return;
        agent.Movement.SetDirection(axis2D);
        #region PrimaryClick
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Debug("Left Click!");
            agent.Combat.Interact(MouseState.Mouse0, CallbackContext.Perform);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Debug("Release LeftMouse.");
            agent.Combat.Interact(MouseState.Mouse0, CallbackContext.Cancel);
        }
        #endregion
        #region SecondaryClick
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            cam.Zoom(CameraManager.ZoomState.ZoomIn);
            agent.Combat.Interact(MouseState.Mouse1, CallbackContext.Perform);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            cam.Zoom(CameraManager.ZoomState.None);
            agent.Combat.Interact(MouseState.Mouse1, CallbackContext.Cancel);
        }
        #endregion
        #region Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            agent.Combat.Interactable = false;
            agent.Movement.SetMoveSpeedState(true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            agent.Combat.Interactable = true;
            agent.Movement.SetMoveSpeedState(false);
        }
        #endregion
        #region Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            agent.Movement.EnqueueJump();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {

        }
        #endregion
        #region Crounch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            cam.Zoom(CameraManager.ZoomState.ZoomOut);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            cam.Zoom(CameraManager.ZoomState.None);
        }
        #endregion
        #region AlphaNumber
        if (Input.GetKey(KeyCode.Alpha1))
        {

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            agent.Combat.EquipWeapon(0, CallbackContext.Perform);
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {

        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            agent.Combat.EquipWeapon(1, CallbackContext.Perform);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {

        }
        #endregion
        #region QWE_Tab
        if (Input.GetKey(KeyCode.Q))
        {
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            agent.Combat.UseGadget(0, CallbackContext.Perform);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {

        }

        if (Input.GetKey(KeyCode.E))
        {
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            agent.Combat.UseGadget(1,CallbackContext.Perform);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {

        }
        #endregion

        if (Input.GetKeyDown(KeyCode.R))
        {
            agent.Combat.ReloadGun();
        }
    }

    private void Debug(string message)
    {
        if (debugMode == false) return;
        print(message);
    }
    #region OldBroken
    /*
    private void Update()
    {
        horizontal = Input.GetAxis(hKey);
        vertical = Input.GetAxis(vKey);
        axis2D.x = horizontal;
        axis2D.y = vertical;
        axis2D.Normalize();
        if (cm != null)
        {
            cm.SetDirection(axis2D);
        }
        if (cm1 != null)
        {
            cm1.SetDirection(axis2D);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cm != null)
            {
                cm.EnqueueJump();
            }
            if (cm1 != null)
            {
                cm1.EnqueueJump();
            }

        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (cb1 != null)
            {
                cb1.InteractWeapon(MouseState.Mouse0, CallbackContext.Perform);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            //if (cb1 != null)
            //{
            //    cb1.InteractWeapon(MouseState.Mouse0, CallbackContext.Perform);
            //}

        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (cb1 != null)
            {
                cb1.InteractWeapon(MouseState.Mouse0, CallbackContext.Cancel);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {

            //if(cm1 != null)
            //{
            //    cm1.SetDesireWeight(1f);
            //    cm1.SpeedMultipier(0.5f);
            //}
            if (cb1 != null)
            {
                cb1.InteractWeapon(MouseState.Mouse1, CallbackContext.Perform);
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {

            //if (cm1 != null)
            //{
            //    cm1.SetDesireWeight(0f);
            //    cm1.SpeedMultipier(1f);
            //}

            if (cb1 != null)
            {
                cb1.InteractWeapon(MouseState.Mouse1, CallbackContext.Perform);
            }
        }
    }
    */
    #endregion
}
