using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

[RequireComponent(typeof(Rigidbody))]
public class DropTrigger : MonoBehaviour
{
    [SerializeField] private ItemQuantity[] drops;
    private bool used = false;

    [SerializeField] private bool randomAmount;
    [SerializeField] private Vector2 randomRange;
    private void Start()
    {
        CharacterData cd = GameManager.Instance.GetCurrentPlayer();
        for (int i = 0; i < drops.Length; i++)
        {
            drops[i].characterID = cd.CharacterID;
            if (randomAmount)
            {
                drops[i].quantity = Random.Range((int)randomRange.x, (int)randomRange.y);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (other.CompareTag("Player"))
        {
            used = true;
            GameManager.Instance.AddItems(drops);
            SoundManager.Instance.PlaySFXByID(1);
            Destroy(gameObject, 0.1f);
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (used) return;
        if (collision.collider.CompareTag("Player"))
        {
            used = true;
            GameManager.Instance.AddItems(drops);
            //SoundManager.Instance.Play_SFX_AtLocation(transform.position, breakSFX, 1f, 1f);
            Destroy(gameObject, 0.1f);
        }
    }
    */
}
