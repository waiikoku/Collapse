using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AirdropManager : Singleton<AirdropManager>
{
    [SerializeField] private Transform ground;
    [SerializeField] private Transform frontAirdrop;
    [SerializeField] private Transform insideAirdrop;

    [SerializeField] private float heightFromGround = 100f;
    [SerializeField] private GameObject airdropPrefab;
    private bool isCollected = false;
    private float timer;
    [SerializeField] private float delayNextAirdrop = 10f;
    [Header("Addition")]
    [SerializeField] private bool visualAirplane = false;
    private bool flying = false;
    [SerializeField] private GameObject virtualAirplane;
    [SerializeField] private AudioSource airplanePlayer;
    private Transform airplane;
    [SerializeField] private AudioClip airplaneSfx;
    [SerializeField] private AudioClip dropSfx;
    [SerializeField] private float flyingDistance = 100f;
    [SerializeField] private float flyingDuration = 10f;
    private Vector3 direction;
    private float velocity;
    public enum DropLocation
    {
        Front,
        Inside
    }

    protected override void Awake()
    {
        base.Awake();
        virtualAirplane.SetActive(false);
    }

    private void Start()
    {
        airplane = virtualAirplane.transform;
        Drop(DropLocation.Front);
    }

    private void FixedUpdate()
    {
        if (visualAirplane == false) return;
        if (flying)
        {
            airplane.position += Time.deltaTime * velocity * airplane.forward;
        }
    }

    private void LateUpdate()
    {
        if (isCollected)
        {
            timer += Time.deltaTime;
            if(timer > delayNextAirdrop)
            {
                timer = 0;
                isCollected = false;
                Drop(DropLocation.Front);
            }
        }
    }

    public void Drop(DropLocation location)
    {
        switch (location)
        {
            case DropLocation.Front:
                if (visualAirplane)
                {
                    virtualAirplane.SetActive(true);
                    airplane.position = frontAirdrop.position + new Vector3(0, heightFromGround, 0);
                    airplane.position -= airplane.forward * flyingDistance;
                    velocity = flyingDistance / flyingDuration;
                    flying = true;
                }
                SpawnBox(frontAirdrop,visualAirplane);
                break;
            case DropLocation.Inside:
                SpawnBox(insideAirdrop);
                break;
            default:
                break;
        }
    }

    private void SpawnBox(Transform target,bool isDelay = false)
    {
        GameObject go = Instantiate(airdropPrefab, target.position + new Vector3(0, heightFromGround,0), Quaternion.identity);
        if (isDelay)
        {
            go.SetActive(false);
            StartCoroutine(DelayDrop(go));
        }
    }

    private IEnumerator DelayDrop(GameObject airDrop)
    {
        yield return new WaitForSeconds(flyingDuration);
        airplanePlayer.PlayOneShot(dropSfx, 0.33f);
        //SoundManager.Instance.Play_SFX_AtLocation(airDrop.transform.position, dropSfx, 1f, 1.5f);
        airDrop.SetActive(true);
        yield return new WaitForSeconds(flyingDuration);
        virtualAirplane.SetActive(false);
        flying = false;
    }

    public void Collected()
    {
        isCollected = true;
    }

}
