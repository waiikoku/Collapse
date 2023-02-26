using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static VFX_Gethit;

public class TriggerEscape : MonoBehaviour
{
    [SerializeField] private string targetTag = "Untagged";
    [SerializeField] private float countTime = 1f;
    private bool countDown = false;
    private bool counted = false;
    private float timer = 0f;
    private bool track = false;
    private bool hasExit = false;
    [SerializeField] private ExitArea exitArea;

    [SerializeField] private bool standToExit = false;
    [SerializeField] private float progress;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform destination;
    [SerializeField] private AudioSource sfx;
    private Transform player;
    [SerializeField] private float ropeDuration = 3f;
    private float ropeTimer = 0f;
    private bool ropeCount = false;

    private void FixedUpdate()
    {
        if (track)
        {
            if (ropeCount)
            {
                ropeTimer += Time.deltaTime;
            }
            if (hasExit == false)
            {
                if (ropeTimer / ropeDuration > 0.9f)
                {
                    sfx.Stop();
                    GameManager.Instance.ActivateVictory(true);
                    hasExit = true;
                }
            }
            if (player == null)
            {
                this.enabled = false;
                return;
            }
            if(ropeTimer >= ropeDuration)
            {
                ropeTimer = ropeDuration;
                ropeCount = false;
            }
            progress = ropeTimer / ropeDuration;
            player.position = Vector3.Lerp(startPosition.position, destination.position, progress);
        }
    }

    private void LateUpdate()
    {
        if (standToExit)
        {
            if (countDown)
            {
                timer += Time.deltaTime;
                if (timer > countTime)
                {
                    if (counted == false)
                    {
                        counted = true;
                        //exitArea.Exit();
                        GameManager.Instance.ActivateVictory(true);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (standToExit)
            {
                countDown = true;
            }
            else
            {
                player = other.GetComponentInParent<PlayerAgent>().transform;
                track = true;
                ropeCount = true;
                GameManager.Instance.PlayerDisableMovement();
                GameManager.Instance.ActiveFade(true,ropeDuration);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (standToExit)
            {
                countDown = false;
                timer = 0f;
            }
        }
    }
}
