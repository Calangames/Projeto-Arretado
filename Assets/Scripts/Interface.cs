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
    public Sprite blueOpen_pt_br, blueKiss_pt_br, bluePet_pt_br, blueUse_pt_br, blueEat_pt_br, blueClose_pt_br, blueTake_pt_br;

    [Header("Gamepad e Tradução")]
    public Image pause, stop;
    public Sprite pause_en_us, stop_en_us, diary_en_us, pause_pt_br, stop_pt_br, diary_pt_br;

    
    //private CGameID m_GameID;
    private bool m_bRequestedStats, gamepadConnected = false;

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