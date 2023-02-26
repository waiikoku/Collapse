using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
    public WeaponData Profile;

    public abstract void PrimaryAction(InputManager.CallbackContext callback);

    public abstract void SecondaryAction(InputManager.CallbackContext callback);
}
