using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderScript : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderValue;

    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            sliderValue.text = v.ToString();
            PlayerPrefs.SetInt("RoundTime", (int)v);
        });
        slider.value = PlayerPrefs.GetInt("RoundTime", 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
