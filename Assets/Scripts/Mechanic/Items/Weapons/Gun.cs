using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : WeaponController ,IInteractable
{
    [SerializeField] private bool addable = false;
    [SerializeField] private BulletData bullet;

    private bool loaded = false;
    private bool reloading = false;
    public int CurrentAmmo;
    public int MaximumAmmo;

    public int CurrentMagazine;
    public int MaximumMagazine;

    public float Firerate;
    private float fireRateDuration;
    private float fireRateTimer;
    public float BulletSpeed;
    public float ReloadTime;
    private float reloadTimer;

    [SerializeField] private float maxRecoil = -20;
    [SerializeField] private float recoilSpeed = 10;
    [SerializeField] private float recoil = 0.0f;
    private bool enableRecoil = false;
    private bool resetRecoil = false;
    [SerializeField] private float resetRecoilDelay = 1f;
    private float resetRecoilTimer = 0f;
    public float overrideSfxValue = 1f;

    [SerializeField] private Transform barrel;
    [SerializeField] private Transform aimDirection;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private AudioSource shootSFX;
    [SerializeField] private AudioClip sfx;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private string targetTag = "Untagged";

    public event Action<GunData.GunAmmo> OnAmmoChange;
    public event Action<GunData.GunMagazine> OnMagazineChange;
    public event Action<float> OnReloading;
    public enum ReloadState
    {
        None,
        Jam,
        Reloading,
        Reloaded
    }
    public ReloadState reloadState;

    public enum AimState
    {
        None,
        Hipfire,
        Aim
    }
    public AimState aimState;

    private void Start()
    {
        debugRay = new RayData();
        debugRay.Pos = barrel.position;
        debugRay.Dir = barrel.forward;
        debugRay.Col = Color.white;
        if(addable) GameManager.Instance.OnAddItem += AddBullet;
        LoadProfile();
    }

    private void LateUpdate()
    {
        Debug.DrawRay(debugRay.Pos, debugRay.Dir, debugRay.Col);
        if(enableRecoil == false)
        {
            if(resetRecoil == false)
            {
                resetRecoilTimer += Time.deltaTime;
                if(resetRecoilTimer > resetRecoilDelay)
                {
                    resetRecoilTimer = 0f;
                    resetRecoil = true;
                    recoil = 0;
                }
            }
        }
    }

    private void LoadProfile()
    {
        if (Profile == null) return;
        GunData gunData = (GunData)Profile;
        MaximumAmmo = gunData.MaxAmmo;
        CurrentAmmo = MaximumAmmo;
        //MaximumMagazine = gunData.MaxMagazine;
        CurrentMagazine = gunData.StartMagazine;
        Firerate = gunData.Firerate;
        BulletSpeed = gunData.BulletSpeed;
        ReloadTime = gunData.ReloadTime;
        loaded = true;
    }

    public void UpdateFirerate()
    {
        fireRateDuration = 1f / Firerate;
    }

    public override void PrimaryAction(InputManager.CallbackContext callback)
    {
        if (callback == InputManager.CallbackContext.Cancel)
        {
            enableRecoil = false;
        }
        if (reloading) return;
        float time = float.Parse(System.DateTime.Now.ToString("mmssff"));
        if (time < fireRateTimer) return;
        float convertSecond = (fireRateDuration * 100);
        fireRateTimer = time + convertSecond;
        if (CurrentAmmo == 0) return;
        CurrentAmmo--;
        OnAmmoChange?.Invoke(GetAmmoInfo());
        //OnAmmoChange?.Invoke(CurrentAmmo);
        //print(string.Format("DateTime:{0} | GameTime:{1}",time,Time.time));
        if (callback == InputManager.CallbackContext.Perform)
        {
            if(enableRecoil == false)
            {
                enableRecoil = true;
                resetRecoil = false;
                resetRecoilTimer = 0f;
            }
            recoil += Time.deltaTime * recoilSpeed;
            recoil = Mathf.Clamp(recoil, 0, maxRecoil);
            Shoot();
        }

    }



    public void SetupAim(Transform transform)
    {
        if (transform == null)
        {
            print("Warning! Aim_Assistant is Empty");
        }
        aimDirection = transform;
    }
    [SerializeField] private Material trailLine;
    private void Shoot()
    {
        //print("Shooting!");
        if (aimDirection == null) return;
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.GetComponent<MeshRenderer>().enabled = false;
        Transform tf = bullet.transform;
        tf.localScale = Vector3.one * 0.1f;
        tf.position = barrel.position;
        tf.rotation = barrel.rotation;
        bullet.GetComponent<SphereCollider>().isTrigger = true;
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        Vector3 direction = aimDirection.position - barrel.position + Recoil();
        Vector3 velocity = direction.normalized * BulletSpeed;
        rb.AddForce(velocity, ForceMode.Impulse);
        TrailRenderer tr = bullet.AddComponent<TrailRenderer>();
        tr.time = 0.5f;
        tr.startColor = Color.yellow;
        tr.endColor = Color.white;
        tr.material = trailLine;
        tr.widthCurve = curve;
        SoundManager.Instance.Play_SFX_AtLocation(barrel.position,sfx, overrideSfxValue);
        //shootSFX.PlayOneShot(sfx);
        Destroy(bullet, 5f);
        /* WIP Predict BulletVelocity to hit
        if(Physics.Raycast(barrel.position,direction, out RaycastHit Hit, 100f))
        {
            float hitDist = Hit.distance;
            float timeUntilReach = Mathf.Clamp(hitDist / velocity.magnitude,0,Mathf.Infinity);
            Destroy(bullet, timeUntilReach);
        }
        else
        {

        }
        */
        RaycastTarget(direction);
    }

    private Vector3 Recoil()
    {
        //return new Vector3(UnityEngine.Random.Range(0.01f, 0.1f), UnityEngine.Random.Range(0.01f, 0.1f), UnityEngine.Random.Range(0.01f, 0.1f));
        return new Vector3(UnityEngine.Random.Range(-recoil,recoil),UnityEngine.Random.Range(0f, recoil),0);
    }

    private struct RayData
    {
        public Vector3 Pos;
        public Vector3 Dir;
        public Color Col;
    }
    private RayData debugRay;
    private float hitDistance;
    private void RaycastTarget(Vector3 dir)
    {
        if (Profile == null) return;
        float randomDamage = UnityEngine.Random.Range(Profile.Stat.MinDamage, Profile.Stat.MaxDamage);
        RayData temp = new RayData();
        temp.Pos = barrel.position;
        temp.Dir = dir;
        temp.Col = Color.yellow;
        debugRay = temp;
        if(Physics.Raycast(barrel.position,dir,out RaycastHit hit))
        {
            Debugf($"Hit ({hit.collider.gameObject.name})");
            IDamagable damagable = hit.collider.GetComponentInParent<IDamagable>();
            if (damagable == null) return;
            damagable.ReceiveDamage(randomDamage);
            Debugf(string.Format("Apply Damage to Target({0}) {1}Dmg", hit.collider.gameObject.name, randomDamage));
        }
    }

    public override void SecondaryAction(InputManager.CallbackContext callback)
    {
        switch (callback)
        {
            case InputManager.CallbackContext.Perform:
                SetAim(true);
                break;
            case InputManager.CallbackContext.Cancel:
                SetAim(false);
                break;
            default:
                break;
        }
    }

    public void Reload()
    {
        if (CurrentMagazine <= 0)
        {
            print("No Magazine Left!");
            return;
        }
        int needAmmo = MaximumAmmo - CurrentAmmo;
        if(CurrentMagazine - needAmmo < 0)
        {
            print("last magazine!");
            needAmmo = CurrentMagazine;
        }
        if (reloading == true) return;
        reloading = true;
        print("Reloading!");
        StartCoroutine(ReloadDelay(needAmmo));
    }

    public bool OutOfAmmo()
    {
        return CurrentAmmo <= 0;
    }

    public GunData.GunAmmo GetAmmoInfo()
    {
        return new GunData.GunAmmo(CurrentAmmo, MaximumAmmo);
    }

    public GunData.GunMagazine GetMagazineInfo()
    {
        return new GunData.GunMagazine(CurrentMagazine, MaximumMagazine);
    }

    private IEnumerator ReloadDelay(int needAmmo)
    {
        float percentage;
        reloadTimer = 0;
        while (true)
        {
            reloadTimer += Time.deltaTime;
            percentage = reloadTimer / ReloadTime;
            OnReloading?.Invoke(percentage);
            if(reloadTimer > ReloadTime)
            {
                OnReloading?.Invoke(1f);
                reloadTimer = 0f;
                break;
            }
            yield return null;
        }

        CurrentMagazine -= needAmmo;
        OnMagazineChange?.Invoke(GetMagazineInfo());
        CurrentAmmo = MaximumAmmo;
        OnAmmoChange?.Invoke(GetAmmoInfo());
        print("Reloaded!");
        reloading = false;
    }

    public void AddBullet(ItemQuantity bullet)
    {
        if (bullet.item.ItemID != this.bullet.ItemID) return;
        CurrentMagazine += bullet.quantity;
        Debugf($"Add {bullet.quantity} Bullet!");
        /*
        if(CurrentMagazine + bullet.quantity > MaximumMagazine)
        {
            CurrentMagazine = MaximumMagazine;
            Debugf($"Add Bullet! (Max)");
        }
        else
        {
            CurrentMagazine += bullet.quantity;
            Debugf($"Add {bullet.quantity} Bullet!");
        }
        */
        OnMagazineChange?.Invoke(GetMagazineInfo());
    }

    public void UpdateInfo()
    {
        if (loaded == false)
        {
            StartCoroutine(WaitUntilLoad());
        }
        else
        {
            OnAmmoChange?.Invoke(GetAmmoInfo());
            OnMagazineChange?.Invoke(GetMagazineInfo());
        }
    }

    private IEnumerator WaitUntilLoad()
    {
        while (loaded == false)
        {
            yield return null;
        }
        OnAmmoChange?.Invoke(GetAmmoInfo());
        OnMagazineChange?.Invoke(GetMagazineInfo());
        yield break;
    }

    private void SetAim(bool value)
    {

    }
    [SerializeField] private bool debugMode = false;
    private void Debugf(string message)
    {
        if (debugMode == false) return;
        print(message);
    }

    [SerializeField] private GameObject model;
    public void Activate(bool value)
    {
        model.SetActive(value);
    }
}
