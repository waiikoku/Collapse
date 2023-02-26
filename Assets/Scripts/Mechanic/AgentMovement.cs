using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking.Types;

public class AgentMovement : MonoBehaviour, IDamagable
{
    [SerializeField] private Animator anim;
    [SerializeField] private NavMeshAgent agent;
    public Transform m_target;
    [SerializeField] private float distanceFromTarget;
    public Vector3 desirePosition;

    public Waypoint waypointData;

    private Transform m_transform;
    [SerializeField] private Transform headAim;

    public bool controlable = false;
    public bool moveable = false;

    public bool equippedMelee = false;
    public bool equippedRange = false;
    public float meleeDistance = 1.5f;
    public float meleeAttackDelay = 1f;
    public float meleeAttackTimer = 0f;
    public float rangeDistance = 10f;
    public float rangeAttackDelay = 0.3f;
    private float rangeAttackTimer = 0f;
    public float currentHp;
    public float maximumHp;
    private float hpPercentage;
    [Range(0.0f, 1.0f)]
    public float lowHpThreshold = 0.2f;
    public int medKitAmount = 0;
    public bool medicineAvailable = false;

    public bool hasCoverNearby = false;
    public float reloadTime = 3f;
    public bool reloading = false;
    public int currentAmmo;
    public int maximumAmmo;
    private float ammoPercentage;
    public float lowAmmoThreshold = 0.4f;
    public bool outOfAmmo = false;
    public int currentMagazine;
    //public int ammoPerMagazine;
    private bool targetInsight = false;
    [SerializeField] private bool standStill = false;
    [SerializeField] private Gun gun;
    [SerializeField] private WeaponController melee;

    public event Action OnDamaged;
    public event Action OnDied;
    private bool diedByNatural = false;
    public void AddDamagedListenr(Action callback)
    {
        if (callback == null) return;
        print($"{callback.Method.Name}");
        OnDamaged += callback;
    }
    public enum HealthState
    {
        Healthy,
        Hurt
    }
    public enum CoverState
    {
        Cover,
        Exposed
    }
    public enum AmmoState
    {
        Enough,
        Empty
    }
    public enum ActionState
    {
        Attack,
        Defense,
        Reload,
        Heal
    }
    public HealthState healthState;
    public CoverState coverState;
    public AmmoState ammoState;
    public ActionState actionState;
    private void Awake()
    {
        m_transform = transform;
    }
    /*
    private void OnDestroy()
    {
        if (diedByNatural) return;
        WaypointManager.Instance.RemoveWaypoint(m_transform);
    }
    */

    private void Start()
    {
        GameObject findPl = GameObject.FindGameObjectWithTag("Player");
        if (findPl == null)
        {
            this.enabled = false;
            return;
        }
        m_target = findPl.transform;
        m_target = m_target.Find("Target");
        equippedRange = gun != null;
        if (equippedRange && gun is Gun)
        {
            Gun g = (Gun)gun;
            g.SetupAim(m_target);
            Debugf($"{gameObject.name} set gun aim to {m_target.name}");
            g.OnAmmoChange += UpdateAmmo;
            g.OnMagazineChange += UpdateMagazine;
            g.UpdateInfo();
        }
        equippedMelee = melee != null;
        if (equippedMelee && melee is Melee)
        {
            Melee m = (Melee)melee;

        }
        //WaypointManager.Instance.AddWaypoint(m_transform, waypointData);
        currentHp = maximumHp;
        healthState = HealthState.Healthy;
        currentAmmo = maximumAmmo;
        ammoPercentage = 1f;
        ammoState = AmmoState.Enough;
        if (equippedRange)
        {
            if (anim != null)
            {
                anim.SetBool("Aim", true);
            }
        }
    }
    private void FixedUpdate()
    {
        if (m_target == null) return;
        if (currentHp == 0f) return;
        distanceFromTarget = Vector3.Distance(m_target.position, m_transform.position);
        RotateToTarget();
        UpdateTarget();
        RaycastTarget();
    }

    private void LateUpdate()
    {
        if (m_target == null) return;
        if (currentHp == 0f) return;
        rangeAttackTimer += Time.deltaTime;
        meleeAttackTimer += Time.deltaTime;
        if (standStill)
        {
            if (agent.isStopped == false)
            {
                agent.isStopped = true;
            }
            if (equippedRange)
            {
                if (distanceFromTarget < rangeDistance)
                {
                    if (targetInsight)
                    {
                        switch (ammoState)
                        {
                            case AmmoState.Enough:
                                Debugf("Try to shoot");
                                Shoot();
                                break;
                            case AmmoState.Empty:
                                Debugf("Try to reload");
                                Reload();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        if (healthState == HealthState.Healthy)
        {
            if (equippedRange)
            {
                if (distanceFromTarget > rangeDistance)
                {
                    //Keep walking
                    if (reload != null) return;
                    if (agent.isStopped)
                    {
                        agent.isStopped = false;
                    }
                    Debugf("Chasing (Healthy)");
                }
                else
                {
                    //Decision
                    if (equippedMelee)
                    {
                        if (distanceFromTarget < meleeDistance)
                        {
                            if (agent.isStopped == false)
                            {
                                agent.isStopped = true;
                                Debugf("StopMove to Melee");
                            }
                            //Melee combat when get to close!
                            DoMelee();
                            Debugf("Re From Melee");
                            return;
                        }
                    }
                    if (targetInsight)
                    {
                        if (agent.isStopped == false)
                        {
                            agent.isStopped = true;
                            Debugf("StopMove to Shoot");
                        }
                        switch (ammoState)
                        {
                            case AmmoState.Enough:
                                Debugf("Try to shoot");
                                Shoot();
                                break;
                            case AmmoState.Empty:
                                Debugf("Try to reload");
                                Reload();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (agent.isStopped == true)
                        {
                            agent.isStopped = false;
                            Debugf("Avoid Blocker");
                        }
                    }
                }
            }
            else if (equippedMelee)
            {
                if (distanceFromTarget > meleeDistance)
                {
                    //Keep walking
                    if (agent.isStopped)
                    {
                        agent.isStopped = false;   
                    }
                    anim.SetBool("Run", true);
                }
                else
                {
                    if (agent.isStopped == false)
                    {
                        agent.isStopped = true;
                        anim.SetBool("Run", false);
                    }
                    DoMelee();
                }
            }
        }
        else if (healthState == HealthState.Hurt)
        {
            switch (coverState)
            {
                case CoverState.Cover:

                    break;
                case CoverState.Exposed:
                    if (hasCoverNearby)
                    {
                        //Go behind cover opposite of target direction
                    }
                    else
                    {
                        //Aggressive Suicidal Decision
                        if (equippedRange)
                        {
                            //Use range to defense itself
                        }
                        else if (equippedMelee)
                        {
                            //Charge target to attack until someone has died!
                        }
                        else
                        {
                            //Nothing to do! Suicide! Kaboom!
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public void SetDestTarget(Transform target)
    {
        m_target = target;
    }

    public void ReceiveDamage(float dmg)
    {
        if (currentHp <= 0) return;
        currentHp -= dmg;
        currentHp = Mathf.Clamp(currentHp, 0, maximumHp);
        hpPercentage = currentHp / maximumHp;
        //healthState = hpPercentage < lowHpThreshold ? HealthState.Hurt : HealthState.Healthy; It will bug cuz no code in 'Hurt' State
        OnDamaged?.Invoke();
        if (hpPercentage == 0)
        {
            OnDied?.Invoke();
            diedByNatural = true;
            //WaypointManager.Instance.RemoveWaypoint(m_transform);
            Destroy(gameObject,0.1f);
            return;
        }
    }

    public void Shoot()
    {
        if (rangeAttackTimer < rangeAttackDelay) return;
        Debugf("Shoot!");
        gun.PrimaryAction(InputManager.CallbackContext.Perform);
        gun.PrimaryAction(InputManager.CallbackContext.Cancel);
        rangeAttackTimer = 0f;
        /*
        if (currentAmmo <= 0)
        {
            outOfAmmo = true;
            return;
        }
        if (rangeAttackTimer < rangeAttackDelay) return;
        Debugf("Shoot!");
        gun.PrimaryAction(InputManager.CallbackContext.Perform);
        rangeAttackTimer = 0f;
        currentAmmo--;
        ammoPercentage = (float)currentAmmo / (float)maximumAmmo;
        ammoState = ammoPercentage < lowAmmoThreshold ? AmmoState.Empty : AmmoState.Enough;
        */
    }

    private void UpdateAmmo(GunData.GunAmmo ammo)
    {
        currentAmmo = ammo.Ammo;
        maximumAmmo = ammo.MaxAmmo;
        ammoPercentage = (float)currentAmmo / (float)maximumAmmo;
        if (currentAmmo <= 0)
        {
            outOfAmmo = true;
        }
        ammoState = ammoPercentage < lowAmmoThreshold ? AmmoState.Empty : AmmoState.Enough;
    }

    private int lastMagazine;
    private void UpdateMagazine(GunData.GunMagazine mag)
    {
        currentMagazine = mag.Magazine;
        if(lastMagazine > currentMagazine)
        {
            reloading = false;
            ammoPercentage = 1f;
            ammoState = AmmoState.Enough;
            Debugf("Reloaded!");
        }
        lastMagazine = currentMagazine;
    }

    public void Reload()
    {
        if (reloading == true) return;
        reloading = true;
        moveable = false;
        gun.Reload();
        Debugf("Reloading!");
        /*
        if (reload != null) return;
        moveable = false;
        reload = StartCoroutine(ReloadThread());
        */
    }

    private Coroutine reload;
    private IEnumerator ReloadThread()
    {
        Debugf("Reloading!");
        yield return new WaitForSeconds(reloadTime);
        int needAmmo = maximumAmmo - currentAmmo;
        currentMagazine -= needAmmo;
        currentAmmo += needAmmo;
        ammoPercentage = 1f;
        ammoState = AmmoState.Enough;
        Debugf("Reloaded!");
        reload = null;
        yield break;
    }

    private void DoMelee()
    {
        if(meleeAttackTimer < meleeAttackDelay)
        {
            return;
        }
        meleeAttackTimer = 0;
        Debugf("Hit target with Knife!");
        melee.PrimaryAction(InputManager.CallbackContext.Perform);
        anim.SetTrigger("Attack");
    }

    private void FindCover()
    {

    }

    private void TakeCover()
    {

    }

    #region CoreMovement
    public void SetTarget(Transform target)
    {
        m_target = target;
    }

    private void UpdateTarget()
    {
        if (standStill) return;
        if (m_target == null) return;
        agent.SetDestination(m_target.position);
    }
    private void RaycastTarget()
    {
        Vector3 direction = m_target.position - headAim.position;
        Debug.DrawLine(headAim.position, headAim.position + (direction.normalized * 10),Color.red);
        if (Physics.Raycast(headAim.position, direction.normalized,out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debugf("Found!");
                targetInsight = true;
            }
            else
            {
                Debugf("Not Player! " + hit.collider.gameObject.name);
                targetInsight = false;
            }
        }
        else
        {
            Debugf("Empty Space");
            targetInsight = false;
        }
    }

    private void RotateToTarget()
    {
        Vector3 direction = m_target.position - m_transform.position;
        direction.y = 0f;
        m_transform.rotation = Quaternion.LookRotation(direction);
    }

    public void MoveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }
    #endregion

    [SerializeField] private bool debugMode = false;
    private void Debugf(string message)
    {
        if (debugMode == false) return;
        print(message);
    }
}
