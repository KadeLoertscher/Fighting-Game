using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void togglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void loadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}