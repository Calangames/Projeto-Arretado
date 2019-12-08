using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.ComponentModel;

public class Interface : MonoBehaviour
{
    public static Interface instance = null;

    public Image redActionImage, mouseHudImage;

    public GameObject achievements, vignette;

    public Sprite blueAbrir, blueFalar, blueOlhar, blueUsar, blueFechar, bluePegar;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {    

    }

    void Update()
    {
        
    }

    public void Menu()
    {
        AudioManager.instance.Crossfade("Gameplay Song Loop", "Menu Theme", 1f);
        SceneManager.LoadScene(0);
    }
}