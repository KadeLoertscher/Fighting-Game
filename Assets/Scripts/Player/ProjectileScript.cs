using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float lifeSpan;
    protected float damage;
    protected float speed;

    public PlayerScript owner;
    public AudioClip deathFx;

    protected Rigidbody2D rig;
    protected AudioSource gameAudio;

    // Shoot 0
    // Explode 1


    protected void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        gameAudio = GameObject.FindGameObjectWithTag("GameController").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        Destroy(gameObject, lifeSpan);
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerScript>().takeDamage(damage, owner);
        }
        gameAudio.PlayOneShot(deathFx);
        Destroy(gameObject);
        
    }

    public void onSpawn(float dmg, float spd, int facingDir, PlayerScript newOwner)
    {
        setOwner(newOwner);
        setDamage(dmg);
        setSpeed(spd);

        rig.AddForce(Vector2.right * facingDir * speed, ForceMode2D.Impulse);
    }

    public void setOwner(PlayerScript newOwner)
    {
        owner = newOwner;
    }

    public void setDamage(float dmg)
    {
        damage = dmg;
    }

    public void setSpeed(float spd)
    {
        speed = spd;
    }

}
