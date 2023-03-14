using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player Refs")]
    public Color[] pColors;
    public List<PlayerScript> players = new List<PlayerScript>();
    public Transform[] spawns;

    [Header("Prefab Refs")]
    public GameObject playerContainerPrefab;

    [Header("Objects")]
    public GameObject playerContainerParent;
    public TextMeshProUGUI timeText;

    [Header("Level Vars")]
    [SerializeField]
    private int startTime;
    [SerializeField]
    private float curTime;

    public static GameManager instance;


    private void Awake()
    {
        instance = this;
        startTime = PlayerPrefs.GetInt("RoundTime", 100);
    }

    // Start is called before the first frame update
    void Start()
    {
        curTime = startTime;
        timeText.SetText(curTime.ToString());
    }

    private void FixedUpdate()
    {
        curTime -= Time.deltaTime;
        timeText.SetText(((int)curTime).ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime <= 0)
        {
            int highscore = 0;
            int winningPlayerIndex = 0;
            List<PlayerScript> winningPlayers = new List<PlayerScript>();

            foreach (PlayerScript player in players)
            {
                if (player.score > highscore)
                {
                    highscore = player.score;
                    winningPlayerIndex = players.IndexOf(player);
                    winningPlayers.Clear();
                    winningPlayers.Add(player);
                }
                else if (player.score == highscore)
                {
                    winningPlayers.Add(player);
                }
            }
            
            if (winningPlayers.Count == 1)
            {
                PlayerPrefs.SetInt("Winner", winningPlayerIndex);
                //PlayerPrefs.SetInt("Tied", 0);
                SceneManager.LoadScene("Win Screen");
            }
            else
            {
                //PlayerPrefs.SetInt("Tied", 1);
                foreach (PlayerScript player in players)
                {
                    if (!winningPlayers.Contains(player))
                    {
                        player.enabled = false;
                    }
                }
                curTime = 30;
                timeText.color = Color.red;
            }
        }
    }

    

    public void onPlayerJoin(PlayerInput player)
    {
        player.GetComponentInChildren<SpriteRenderer>().color = pColors[players.Count];
        PlayerContainterUI container = Instantiate(playerContainerPrefab, playerContainerParent.transform).GetComponent<PlayerContainterUI>();
        player.GetComponent<PlayerScript>().setUiContainer(container);
        container.initialize(pColors[players.Count]);
        players.Add(player.GetComponent<PlayerScript>());
        player.transform.position = spawns[Random.Range(0, spawns.Length)].position;
    }

    /*public void onPlayerDeath(PlayerScript player, PlayerScript curAtker)
    {
        if (curAtker != null)
        {
            curAtker.addScore();
        }
        player.die();
    }*/
}
