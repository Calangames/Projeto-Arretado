using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [Range(0.1f, 1f)]
    public float fadeDuration = 0.5f;
    public Button creditsButton, startButton, exitButton, englishButton, portuguesButton;
    public Animator credits;
    public Image fade, backgroundTitle;

    public static MenuController instance = null;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(FadeFromBlack());
        AudioManager.instance.FadeOut("Car Drive Off", 1.5f);
        AudioManager.instance.FadeOut("Credits Theme", 1.5f);
        AudioManager.instance.FadeIn("Menu Theme", 0.01f, 0.5f);   
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitButton.Select();
        }
    }

    public void CreditsButton()
    {
        SwapNavigation();
        AudioManager.instance.Play("UI Select");
        credits.SetTrigger("RollCredits");
        AudioManager.instance.FadeOut("Menu Theme", 1f);
        AudioManager.instance.FadeIn("Credits Theme", 1f);
    }

    public void StartButton()
    {
        SwapNavigation();
        AudioManager.instance.Play("UI Select");
        AudioManager.instance.FadeOut("Menu Theme", 0.5f);
        StartCoroutine(FadeToBlackAndStart());
    }

    public void ExitButton()
    {
        SwapNavigation();
        AudioManager.instance.Play("UI Select");
        Application.Quit();
    }

    public void SelectButton(Button button)
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }

    public void SwapNavigation()
    {
        EventSystem.current.sendNavigationEvents = !EventSystem.current.sendNavigationEvents;
        SwapButtonNavigation(startButton);
        SwapButtonNavigation(exitButton);
        SwapButtonNavigation(creditsButton);
        if (!EventSystem.current.sendNavigationEvents)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void SwapButtonNavigation(Button button)
    {
        Navigation nav = button.navigation;
        nav.mode = nav.mode == Navigation.Mode.Automatic ? Navigation.Mode.None : Navigation.Mode.Automatic;
        button.navigation = nav;
    }

    private IEnumerator FadeFromBlack()
    {
        EventSystem.current.sendNavigationEvents = false;
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        fade.color = Color.black;
        for (int i = 5; i > 0; i--)
        {
            yield return null;
        }
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= (Time.deltaTime / fadeDuration);
            fade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        SwapNavigation();
    }

    private IEnumerator FadeToBlackAndStart()
    {
        fade.color = new Color(0f, 0f, 0f, 0f);
        yield return null;
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += (Time.deltaTime / fadeDuration);
            fade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
}