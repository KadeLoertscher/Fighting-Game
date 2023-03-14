using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [Header("Max Values")]
    public int maxHp;
    public int maxJumps;
    public float moveSpeed;
    public float maxPwrDmg;

    [Header("Current Values")]
    [SerializeField]
    private int curHp;
    [SerializeField]
    private int curJumps;
    public int score;
    [SerializeField]
    private float moveInput;
    [SerializeField]
    private float lastAtkTime;
    [SerializeField]
    private bool slowed;
    [SerializeField]
    private float curSpeed;
    [SerializeField]
    private float lastSlowed;
    [SerializeField]
    private bool isCharging;

    [Header("Modifiers")]
    public float jumpForce;

    [Header("Audio")]
    public AudioClip[] playerFx;
    // Jump 0 
    // Thud 1
    // Taunt1 2
    // Spawn 3
    // Death 4
    // Std Shoot 5
    // Std Death 6
    // Ice Shoot 7
    // Pwr Shoot 8

    [Header("Attacking")]
    [SerializeField]
    private PlayerScript curAttacker;
    public float atkRate;
    public float atkSpeed;
    public int atkDamage;
    public float slowTime;
    public GameObject[] atkPrefabs;
    // Std 0
    // Slw 1
    // Pwr 2
    public float pwrAtkDmg;
    public float pwrRate;

    [Header("Components")]
    public PlayerContainterUI uiContainer;
    public GameObject deathEffectPrefab;
    private Rigidbody2D rig;
    private Animator anim;
    private AudioSource audio;
    private GameManager gameManager;
    private Transform muzzle;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
        muzzle = GetComponentInChildren<MuzzleScript>().GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        curJumps = maxJumps;
        curHp = maxHp;
        score = 0;
        curSpeed = moveSpeed;

        audio.PlayOneShot(playerFx[3]);
        uiContainer.setHealthFill(curHp, maxHp);
    }

    private void FixedUpdate()
    {
        move();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.y < -10) || (curHp <= 0))
        {
            die();
        }
        if (slowed)
        {
            if (Time.time - lastSlowed > slowTime)
            {
                slowed = false;
                curSpeed = moveSpeed;
            }
        }
        if (isCharging)
        {
            pwrAtkDmg += pwrRate;
            if (pwrAtkDmg > maxPwrDmg)
            {
                pwrAtkDmg = maxPwrDmg;
            }
        }
        uiContainer.setChargeFill(pwrAtkDmg, maxPwrDmg);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(ContactPoint2D x in collision.contacts)
        {
            if (x.collider.CompareTag("Ground"))
            {
                if (x.point.y < transform.position.y)
                {
                    curJumps = maxJumps;
                    audio.PlayOneShot(playerFx[1]);
                }
                /*if (x.point.x != transform.position.x && (x.point.y < transform.position.y))
                {
                    if (curJumps < maxJumps)
                    {
                        curJumps++;
                    }
                }*/
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void jump()
    {
        rig.velocity = new Vector2(rig.velocity.x, 0);
        audio.PlayOneShot(playerFx[0]);
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void move()
    {
        rig.velocity = new Vector2(moveInput * curSpeed, rig.velocity.y);

        if (moveInput != 0.0f)
        {
            transform.localScale = new Vector3(moveInput > 0 ? 1 : -1, 1, 1);
        }
    }

    public void die()
    {
        
        if (curAttacker != null)
        {
            curAttacker.addScore();
        }
        else
        {
            subScore();
        }
        Destroy(Instantiate(deathEffectPrefab, transform.position, Quaternion.identity), 1);
        audio.PlayOneShot(playerFx[4]);
        respawn();
    }

    public void addScore()
    {
        score++;
        uiContainer.updateScore(score);
    }

    public void subScore()
    {
        score--;
        if (score < 0)
        {
            score = 0;
        }
        uiContainer.updateScore(score);
    }

    public void takeDamage(int amount, PlayerScript attacker)
    {
        
        curHp -= amount;
        curAttacker = attacker;
        cancelPwrAtk();
        uiContainer.setHealthFill(curHp, maxHp);
    }
    // Overload method for a float
    public void takeDamage(float amount, PlayerScript attacker)
    {
        curHp -= (int)amount;
        curAttacker = attacker;
        cancelPwrAtk();
        uiContainer.setHealthFill(curHp, maxHp);
    }

    public void takeIceDamage(float amount, PlayerScript attacker)
    {
        curHp -= (int)amount;
        curAttacker = attacker;
        if (!slowed)
        {
            slowed = true;
            curSpeed /= 2;
            lastSlowed = Time.time;
        }
        cancelPwrAtk();
        uiContainer.setHealthFill(curHp, maxHp);
    }

    public void takeIceDamage(int amount, PlayerScript attacker)
    {
        curHp -= amount;
        curAttacker = attacker;
        if (!slowed)
        {
            slowed = true;
            curSpeed /= 2;
            lastSlowed = Time.time;
        }
        cancelPwrAtk();
        uiContainer.setHealthFill(curHp, maxHp);
    }

    private void respawn()
    {
        curJumps = maxJumps;
        curHp = maxHp;
        curSpeed = moveSpeed;
        pwrAtkDmg = 0;
        isCharging = false;
        curAttacker = null;
        transform.position = gameManager.spawns[Random.Range(0, gameManager.spawns.Length)].position;
        rig.velocity = Vector2.zero;
        uiContainer.setHealthFill(curHp, maxHp);
        uiContainer.setChargeFill(pwrAtkDmg, maxPwrDmg);
    }

    private void spawnStdAtk(float damage, float speed)
    {
        audio.PlayOneShot(playerFx[5]);
        GameObject fireball = Instantiate(atkPrefabs[0]);
        fireball.transform.rotation = Quaternion.identity;
        fireball.transform.position = muzzle.position;

        fireball.GetComponent<ProjectileScript>().onSpawn(damage, speed, transform.localScale.x > 0 ? 1 : -1, this);
    }

    private void spawnSlwAtk(float damage, float speed)
    {
        audio.PlayOneShot(playerFx[7]);
        GameObject iceball = Instantiate(atkPrefabs[1]);
        iceball.transform.rotation = Quaternion.identity;
        iceball.transform.position = muzzle.position;

        iceball.GetComponent<ProjectileScript>().onSpawn(damage, speed, transform.localScale.x > 0 ? 1 : -1, this);
    }

    private void spawnPwrAtk(float speed)
    {
        audio.PlayOneShot(playerFx[8]);
        GameObject chargeBall = Instantiate(atkPrefabs[2]);
        chargeBall.transform.rotation = Quaternion.identity;
        chargeBall.transform.position = muzzle.position;

        chargeBall.GetComponent<ProjectileScript>().onSpawn(pwrAtkDmg, speed, transform.localScale.x > 0 ? 1 : -1, this);


    }

    private void cancelPwrAtk()
    {
        if (isCharging)
        {
            pwrAtkDmg = 0;
            isCharging = false;
        }
    }

    public void setUiContainer(PlayerContainterUI container)
    {
        uiContainer = container;
    }

    // Input system methods


    public void onMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
        if (moveInput > .5f)
        {
            moveInput = 1;
        }
        if (moveInput < -.5f)
        {
            moveInput = -1;
        }
    }

    public void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (curJumps > 0)
            {
                curJumps--;
                jump();
                cancelPwrAtk();
            }
        }
    }

    public void onStdAtkInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAtkTime > atkRate)
        {
            lastAtkTime = Time.time;
            spawnStdAtk(atkDamage, atkSpeed);
            cancelPwrAtk();
        }
    }

    public void onPwrAtkInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isCharging = true;
        }
        if (context.phase == InputActionPhase.Canceled && Time.time - lastAtkTime > atkRate && isCharging)
        {
            lastAtkTime = Time.time;
            spawnPwrAtk(atkSpeed);
            cancelPwrAtk();
        }
    }

    public void onSlwAtkInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAtkTime > atkRate)
        {
            lastAtkTime = Time.time;
            spawnSlwAtk(atkDamage * 3/4, atkSpeed * 2/3);
            cancelPwrAtk();
        }
    }

    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed block");
            cancelPwrAtk();
        }
    }

    public void onTaunt1Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt1");
            audio.PlayOneShot(playerFx[2]);
        }
    }

    public void onTaunt2Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt2");
        }
    }

    public void onTaunt3Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt3");
        }
    }

    public void onTaunt4Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt4");
        }
    }

    public void onPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed pause");
        }
    }
}
