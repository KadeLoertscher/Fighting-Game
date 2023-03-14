using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowProjectileScript : ProjectileScript
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerScript>().takeIceDamage(damage, owner);
        }
        gameAudio.PlayOneShot(deathFx);
        Destroy(gameObject);

    }
}
