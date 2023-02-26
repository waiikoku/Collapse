using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Airdrop : MonoBehaviour , IDamagable
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private string[] contactTag;

    private bool firstContact = false;
    private bool used = false;
    [SerializeField] private AudioClip breakSFX;

    [SerializeField] private ItemQuantity[] reward;
    [SerializeField] private ItemDrop itemDrop;

    [SerializeField] private GameObject advice;
    private void Start()
    {
        if(advice != null)
        {
            advice.SetActive(false);
            //Pattern();
        }
        colliders = GetComponentsInChildren<Collider>();
        CharacterData data = GameManager.Instance.GetCurrentPlayer();
        for (int i = 0; i < reward.Length; i++)
        {
            reward[i].characterID = data.CharacterID;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (firstContact) return;   
        string myTag = collision.collider.tag;
        foreach (var targetTag in contactTag)
        {
            if(myTag == targetTag)
            {
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
                firstContact = true;
                if (advice != null)
                {
                    advice.SetActive(true);
                }
                print("Contact " + targetTag);
                break;
            }
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (firstContact == false) return;
        if (used) return;
        if (other.CompareTag("Player"))
        {
            used = true;
            AirdropManager.Instance.Collected();
            GameManager.Instance.AddItems(reward);

            itemDrop.Drop();
            SoundManager.Instance.Play_SFX_AtLocation(transform.position, breakSFX, 1f, 1f);
            Destroy(gameObject,0.1f);
        }
    }
    */

    public void ReceiveDamage(float dmg)
    {
        if (firstContact == false) return;
        if (used) return;
        used = true;
        AirdropManager.Instance.Collected();
        itemDrop.Drop();
        SoundManager.Instance.Play_SFX_AtLocation(transform.position, breakSFX, 1f, 1f);
        Destroy(gameObject, 0.1f);
        print("Collected Airdrop!");
    }
}
