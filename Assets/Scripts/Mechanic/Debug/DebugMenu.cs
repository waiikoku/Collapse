using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugMenu : MonoBehaviour
{
    [SerializeField] private Button spawn;
    [SerializeField] private Button despawn;
    [SerializeField] private Button randSpawn;

    [SerializeField] private Spawner spawner;

    private void Start()
    {
        spawn.onClick.AddListener(GM_Spawn);
        despawn.onClick.AddListener(GM_Despawn);
        randSpawn.onClick.AddListener(SN_Random);
    }

    public void GM_Spawn()
    {
        if (GameManager.Instance.InputAvailable() == true)
        {
            print("Invalid Command! Player has already Spawned.");
            return;
        }
        GameManager.Instance.SpawnPlayer();
    }

    public void GM_Despawn()
    {
        if (GameManager.Instance.InputAvailable() == false)
        {
            print("Invalid Command! Player has already Despawned.");
            return;
        }
        GameManager.Instance.DespawnPlayer(true);
    }

    public void SN_Random()
    {
        spawner.RandomSpawn();
    }
}
