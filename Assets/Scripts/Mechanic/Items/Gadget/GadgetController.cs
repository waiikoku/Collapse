using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GadgetController : MonoBehaviour
{
    public GadgetData Profile;

    public abstract void TriggerAction(InputManager.CallbackContext callback);
    public abstract void PrimaryAction(InputManager.CallbackContext callback);
    public abstract void SecondaryAction(InputManager.CallbackContext callback);

    public abstract void Unequip();
}
