using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TriggerSpawn : MonoBehaviour
{
    [SerializeField] private bool manualSpawn = false;
    [SerializeField] private Spawner hive;
    private bool once = false;
    [SerializeField] private GameObject[] fixedMob;
    [SerializeField] private string targetTag = "Untagged";
    [SerializeField] private Spawner.WaveData waveConfig;

    [SerializeField] private float waveLimitedDuration = 15f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (manualSpawn)
            {
                if (once == false)
                {
                    once = true;
                    foreach (var mob in fixedMob)
                    {
                        mob.SetActive(true);
                    }
                }
            }
            else
            {
                if(once == false)
                {
                    once = true;
                    if(hive != null)
                    {
                        hive.DirectWave(waveConfig);
                        StartCoroutine(WaveTemporary());
                    }
                }
            }
        }
    }

    public void EjectWave()
    {
        if (hive != null)
        {
            hive.StopAllCoroutines();
        }
    }

    private IEnumerator WaveTemporary()
    {
        yield return new WaitForSeconds(waveLimitedDuration);
        hive.StopWave();
    }
}
