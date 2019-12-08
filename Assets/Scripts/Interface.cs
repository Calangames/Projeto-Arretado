using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.ComponentModel;

public class Interface : MonoBehaviour
{
    public static Interface instance = null;

    public string defaultDescription = "[Not yet achieved]";

    public Image redActionImage, blueActionImage, redPadHudImage, bluePadHudImage, mouseHudImage;

    public GameObject achievements, vignette;

    public Sprite blueOpen, blueKiss, bluePet, blueUse, blueEat, blueClose, blueTake, mouseBlue, mouseBoth, mouseRed;

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