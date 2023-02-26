using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindManager : MonoBehaviour
{
    [Range(1.0f, 17.0f)]
    public float GlobalSensitivity = 1f;
    [Range(1.0f, 17.0f)]
    public float HorizontalLookSensitivity = 1f;
    [Range(1.0f, 17.0f)]
    public float VerticalLookSensitivity = 1f;
    [Range(1.0f, 17.0f)]
    public float HorizontalAimingSensitivity = 1f;
    [Range(1.0f, 17.0f)]
    public float VerticalAimingSensitivity = 1f;
    public bool SeparateSensitivity = false;
    public bool FovRelativeSensitivity = false;
    public bool InvertYAxis = false;
    public bool HoldToAim = true;
    public bool HoldToRun = true;

    public KeyCode MoveForward = KeyCode.W;
    public KeyCode MoveBack = KeyCode.S;
    public KeyCode MoveLeft = KeyCode.A;
    public KeyCode MoveRight = KeyCode.D;
    public KeyCode FireWeapon = KeyCode.Mouse0;
    public KeyCode AimDownSight = KeyCode.Mouse1;
    public KeyCode Secondary = KeyCode.Alpha1;
    public KeyCode Primary = KeyCode.Alpha2;
    public KeyCode SwitchWeapon = KeyCode.C;
    public KeyCode Reload = KeyCode.R;
    public KeyCode WeaponFireMode = KeyCode.V; //Switch Single/Burst/Auto
    public KeyCode UseThrowable = KeyCode.Alpha3;
    public KeyCode Run = KeyCode.LeftShift;
    public KeyCode Jump = KeyCode.Space;
    public KeyCode MeleeAttack = KeyCode.E;

}
