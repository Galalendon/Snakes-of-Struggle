using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("Player")]
    public MovementController player;
    public string snakeTag;
    public string wallTag = "Wall";

    [Header("Mine Stats")]
    public float mineDelay;
    public bool mineTrigger;
    public float explosionRange;

    [Header("Mine Juice")]
    public AudioClip explosionSound;
    public AudioClip mineTrip;
    public SpriteRenderer explosionRenderer;
    public Sprite explosionSprite;

    [Header("Mine Colliders")]
    public Collider2D mineTripColl;
    public Collider2D explosionColl;

    private AudioSource audioSource;
    private MovementController snakeCollision;
    private float timer;
    private float explosionSoundTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = mineTrip;
        //explosionRenderer.sprite = explosionSprite;
        explosionRenderer.enabled = false;
        timer = mineDelay;
        mineTrigger = false;
        explosionSoundTime = explosionSound.length;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

    private void Update()
    {
        if ((mineTrigger) && (timer > 0))
        {
            timer -= Time.deltaTime;
        }
        else if ((mineTrigger) && (timer <= 0))
        {
            BlowUpLeadUp();
        }

    }
    private void BlowUpLeadUp()
    {
        mineTrigger = false;
        audioSource.Stop();
        audioSource.clip = explosionSound;
        audioSource.Play();
        explosionRenderer.enabled = true;
        Invoke("BlowUp", explosionSoundTime);
    }

    private void BlowUp()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gameObject.transform.position, explosionRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag(snakeTag))
            {
                MovementController snakeToBlowUp = hitCollider.GetComponent<MovementController>();
                snakeToBlowUp.Starve();
            }            
            
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag(snakeTag))
        {
            Debug.Log("Mine Hit");
            snakeCollision = collision.gameObject.GetComponent<MovementController>();
            if (snakeCollision != player)
            {
                Debug.Log("Mine Tripped");
                mineTrigger = true;
                audioSource.Play();
                
            }
        }
    }

}
