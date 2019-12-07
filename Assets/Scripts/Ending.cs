using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public void LoadMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            AudioManager.instance.Crossfade("Credits Theme", "Menu Theme", 1f);
            SceneManager.LoadScene(0);
        }
        else
        {
            AudioManager.instance.Crossfade("Credits Theme", "Menu Theme", 1f);
            MenuController.instance.SwapNavigation();
            MenuController.instance.SelectButton(MenuController.instance.creditsButton);
        }
    }
}