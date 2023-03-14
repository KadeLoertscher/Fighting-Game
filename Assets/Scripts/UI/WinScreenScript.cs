using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreenScript : MonoBehaviour
{
    public Color[] playerColors;
    public TextMeshProUGUI winText;
    // Start is called before the first frame update
    void Start()
    {
        /*if(PlayerPrefs.GetInt("Tied", 0) == 0)
        {
            winText.color = playerColors[PlayerPrefs.GetInt("Winner", 0)];
            winText.SetText("Player " + (PlayerPrefs.GetInt("Winner") + 1).ToString() + " wins!");
        }
        else if (PlayerPrefs.GetInt("Tied") == 1)
        {
            winText.SetText("It's a tie!");
        }*/
        winText.color = playerColors[PlayerPrefs.GetInt("Winner", 0)];
        winText.SetText("Player " + (PlayerPrefs.GetInt("Winner") + 1).ToString() + " wins!");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
