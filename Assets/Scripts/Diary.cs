using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diary : MonoBehaviour
{
    public static Diary instance = null;

    public GameObject diaryObject, objectiveHUD;

    public Image diaryInterface, wakeSlash, cerealSlash, maskSlash, toolsSlash, showerSlash, jacketSlash, carSlash;

    public RectTransform hudSlash;

    public Sprite wakeHud, cerealHud, maskHud, toolsHud, showerHud, jacketHud, carHud;

    public AnimationClip objectiveHudCrossClip;

    private Image objectiveHUDImage;

    private Animator objectiveHUDAnimator;

    private Queue<IEnumerator> coroutines = new Queue<IEnumerator>();

    private WaitForSeconds clipLength;

    private bool waitingEndOfCoroutine, runningCoroutine;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        objectiveHUDImage = objectiveHUD.GetComponent<Image>();
        objectiveHUDAnimator = objectiveHUD.GetComponent<Animator>();
        clipLength = new WaitForSeconds(0.1f /* objectiveHudCrossClip.length */);
    }

    public void CrossObjective(string objective, float delay)
    {
        coroutines.Enqueue(CrossObjectiveCoroutine(objective, delay));
        if (!waitingEndOfCoroutine)
        {
            StartCoroutine(WaitEndOfCoroutine());
        }
    }

    private IEnumerator WaitEndOfCoroutine()
    {
        waitingEndOfCoroutine = true;
        while (coroutines.Count > 0)
        {
            if (!runningCoroutine)
            {
                StartCoroutine(coroutines.Dequeue());
            }
            yield return null;
        }
        waitingEndOfCoroutine = false;
    }

    private IEnumerator CrossObjectiveCoroutine(string objective, float delay)
    {
        runningCoroutine = true;
        yield return new WaitForSeconds(delay);
        switch (objective)
        {
            case "wake":
                objectiveHUDImage.sprite = wakeHud;
                wakeSlash.GetComponent<Animator>().SetTrigger("Cross");
                break;
            case "cereal":
                objectiveHUDImage.sprite = cerealHud;
                cerealSlash.GetComponent<Animator>().SetTrigger("Cross");
                break;
            case "mask":
                objectiveHUDImage.sprite = maskHud;
                maskSlash.GetComponent<Animator>().SetTrigger("Cross");
                break;
            case "tools":
                objectiveHUDImage.sprite = toolsHud;
                toolsSlash.GetComponent<Animator>().SetTrigger("Cross");
                break;
            case "shower":
                objectiveHUDImage.sprite = showerHud;
                showerSlash.GetComponent<Animator>().SetTrigger("Cross");
                break;
            case "jacket":
                objectiveHUDImage.sprite = jacketHud;
                jacketSlash.GetComponent<Animator>().SetTrigger("Cross");
                break;
            case "car":
                objectiveHUDImage.sprite = carHud;
                carSlash.GetComponent<Animator>().SetTrigger("Cross");
                break;
            default:
                break;
        }
        objectiveHUDAnimator.SetTrigger("Cross");
        yield return clipLength;
        runningCoroutine = false;
    }

    public void Show()
    {
        diaryInterface.enabled = true;
        wakeSlash.enabled = true;
        cerealSlash.enabled = true;
        maskSlash.enabled = true;
        toolsSlash.enabled = true;
        showerSlash.enabled = true;
        jacketSlash.enabled = true;
        carSlash.enabled = true;
    }

    public void Hide()
    {
        diaryInterface.enabled = false;
        wakeSlash.enabled = false;
        cerealSlash.enabled = false;
        maskSlash.enabled = false;
        toolsSlash.enabled = false;
        showerSlash.enabled = false;
        jacketSlash.enabled = false;
        carSlash.enabled = false;
    }
}