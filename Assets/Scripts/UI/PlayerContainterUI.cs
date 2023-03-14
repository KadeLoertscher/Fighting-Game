using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContainterUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Image healthBar;
    public Image chargeBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initialize(Color color)
    {
        scoreText.color = color;
        healthBar.color = color;

        scoreText.SetText("0");
        healthBar.fillAmount = 1;
        chargeBar.fillAmount = 0;
    }

    public void updateScore(int score)
    {
        scoreText.SetText(score.ToString());
    }

    public void setHealthFill(int curHealth, int maxHealth)
    {
        healthBar.fillAmount = (float) curHealth / (float) maxHealth;
    }

    public void setChargeFill(float curCharge, float maxCharge)
    {
        chargeBar.fillAmount = curCharge / maxCharge;
    }
}
