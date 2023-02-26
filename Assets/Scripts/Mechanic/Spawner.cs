using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<Transform> Spawnpoints;
    [SerializeField] private GameObject[] mobPrefab;
    public bool canSpawn = false;
    public event Action StartSpawn;
    public event Action StopSpawn;
    public void RandomSpawn()
    {
        int randomMob = UnityEngine.Random.Range(0, mobPrefab.Length);
        int randomPos = UnityEngine.Random.Range(0, Spawnpoints.Count);
        Spawn(mobPrefab[randomMob], Spawnpoints[randomPos].position, Spawnpoints[randomPos].rotation);
    }

    public void SpecificSpawnByIndex(int index)
    {
        int randomPos = UnityEngine.Random.Range(0, Spawnpoints.Count);
        Spawn(mobPrefab[index], Spawnpoints[randomPos].position, Spawnpoints[randomPos].rotation);
    }

    public void SpecificSpawnByIndex(int index, Vector3 position, Quaternion rotation)
    {
        Spawn(mobPrefab[index], position, rotation);
    }

    public void SpecificSpawnByName(string name)
    {
        int randomPos = UnityEngine.Random.Range(0, Spawnpoints.Count);
        int index = Array.FindIndex(mobPrefab, goName => goName.name == name);
        Spawn(mobPrefab[index], Spawnpoints[randomPos].position, Spawnpoints[randomPos].rotation);
    }
    public void SpecificSpawnByName(string name, Vector3 position, Quaternion rotation)
    {
        int index = Array.FindIndex(mobPrefab, goName => goName.name == name);
        Spawn(mobPrefab[index], position, rotation);
    }

    private GameObject Spawn(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        return Instantiate(prefab, position, rotation);
    }

    private List<int> spawnId;
    private List<Coroutine> spawnWave;
    public void WaveSpawn(int id,int[] pattern, int spawnAmount,float delaySpawn,float delayWave,bool overrideWave = false)
    {
        int indexOfID = -99;
        for (int i = 0; i < spawnId.Count; i++)
        {
            if(spawnId[i] == id)
            {
                indexOfID = i;
                break;
            }
        }
        if(spawnId.Contains(id) == false)
        {
            spawnId.Add(id);
        }
        else
        {
            if (overrideWave)
            {
                if (spawnWave[indexOfID] != null)
                {
                    StopCoroutine(spawnWave[indexOfID]);
                    print($"Found Exist Spawn Wave! Stop Old Coroutine({indexOfID})");
                }
                else
                {
                    print("Target Spawn Wave Not Found! Maybe it's already end?");
                }
            }
            else
            {
                print("Spawn Wave has Started! Reject new request.");
                return;
            }
        }
        spawnWave.Add(StartCoroutine(ThreadWave(pattern,spawnAmount,delaySpawn,delayWave)));
    }

    public void DirectWave(WaveData data)
    {
        StartCoroutine(ThreadWave(null, data.SpawnAmount, data.DelaySpawn, data.DelayWave));
    }

    public void StopWave()
    {
        StopAllCoroutines();
    }

    private IEnumerator ThreadWave(int[] pattern, int spawnAmount, float delaySpawn, float delayWave)
    {
        while (true)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                RandomSpawn();
                yield return new WaitForSeconds(delaySpawn);
            }

            yield return new WaitForSeconds(delayWave);
        }
    }

    [System.Serializable]
    public struct SpawnPattern
    {
        public int[,] patterns;
    }

    [System.Serializable]
    public struct WaveData
    {
        public int SpawnAmount;
        public float DelaySpawn;
        public float DelayWave;
    }
}
