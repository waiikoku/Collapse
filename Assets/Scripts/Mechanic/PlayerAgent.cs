using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : MonoBehaviour
{
    public CharacterData Profile => m_combat.Profile;
    [SerializeField] private CharacterMovement1 m_movement;
    [SerializeField] private CharacterCombat m_combat;
    public CharacterMovement1 Movement => m_movement;
    public CharacterCombat Combat => m_combat;
}
