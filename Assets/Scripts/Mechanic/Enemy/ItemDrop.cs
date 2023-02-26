using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemDrop : MonoBehaviour
{
    [SerializeField] private DropTrigger[] dropPrefabs;

    [SerializeField] private AgentMovement am;
    public bool randomCircle = false;
    private void Start()
    {
        if (am != null)
        {
            am.OnDied += Drop;
        }
    }

    public void Drop()
    {
        for (int i = 0; i < dropPrefabs.Length; i++)
        {
            Vector3 offset = Vector3.zero;
            if(randomCircle)
            {
                offset = Random.insideUnitCircle * Random.Range(0.1f,1.5f);
            }
            Instantiate(dropPrefabs[i], transform.position + offset, Quaternion.identity);
        }
    }
}
