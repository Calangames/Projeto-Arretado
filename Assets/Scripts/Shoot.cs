using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.EventSystems;

public class Shoot : MonoBehaviour
{
    [Range(0f, 10f)]
    public float maxDistance = 5f;
    public GameObject ending;
    public GameObject iracema;
    //public GameObject doggie, fishBowl, fishFood, shower, skeleton, serial, milk, bowl, cerealBowCanvas, bathroomFaucet, sinkFaucet, wife, mask, arms, chainsaw, sword, car, mirrorJacket, mirrorMask;
    //public Vector3 bowlSinkPosition;
    //public ParticleSystem showerWater, bathroomFaucetWater, sinkFaucetWater;
    //public Material bowlEmpty, bowlSerialAndMilk, bowlSerial;
    //public Transform bedroomDoor, bathroomDoor, closetDoor, kitchenDoor, tvDoor, garageDoor, outsideDoor, fridgeDoor1, fridgeDoor2;
    public int lockedCounter = 0;

    [TextArea(1, 10)]
    public string[] iracemaSemChave;

    private FirstPersonController firstPersonController;
    private LayerMask layerMask;
    private bool firing1, pressingEsc, pressingDiary, paused = false, fedFish, pettedDoggie, kissed, masked, ended, armed, cleaned, red;
    private enum NextStep
    {
        Iracema =1, Sandoval =2, EntraQuarto =4, FechaPorta = 8
    };
    private NextStep nextStep = NextStep.Iracema;
    //private Door[] doors = new Door[9];

    void Start()
    {
        layerMask = LayerMask.GetMask("Default");
        firstPersonController = GetComponent<FirstPersonController>();
        MonologueManager.instance.FirstPersonController(firstPersonController);
        //doors[0] = new Door(bedroomDoor.GetChild(0).gameObject);
        //doors[1] = new Door(bathroomDoor.GetChild(0).gameObject, true);
        //doors[2] = new Door(closetDoor.GetChild(0).gameObject);
        //doors[3] = new Door(kitchenDoor.GetChild(0).gameObject);
        //doors[4] = new Door(tvDoor.GetChild(0).gameObject);
        //doors[5] = new Door(garageDoor.GetChild(0).gameObject);
        //doors[6] = new Door(outsideDoor.GetChild(0).gameObject);
        //doors[7] = new Door(fridgeDoor1.gameObject, true);
        //doors[8] = new Door(fridgeDoor2.gameObject, true);
    }

    void LateUpdate()
    {
        if (!FirstPersonController.waking && !ending.activeSelf)
        {
            if (Input.GetAxisRaw("Diary") == 1f)
            {
                if (!pressingDiary && Diary.instance.diaryObject && !Diary.instance.diaryObject.activeSelf)
                {
                    paused = true;
                    Diary.instance.Show();
                }
            }
            else
            {
                pressingDiary = false;
                if (Diary.instance.diaryObject && !Diary.instance.diaryObject.activeSelf)
                {
                    Diary.instance.Hide();
                    if (!Interface.instance.achievements.activeSelf)
                    {
                        paused = false;
                    }
                }
            }

            if (Input.GetAxisRaw("Cancel") == 1f)
            {
                if (!pressingEsc)
                {
                    pressingEsc = true;
                    if (Interface.instance.achievements.activeSelf)
                    {
                        Interface.instance.achievements.SetActive(false);
                        Interface.instance.vignette.SetActive(true);
                        if (!MonologueManager.instance.Running)
                        {
                            firstPersonController.Locked = false;
                            Cursor.lockState = CursorLockMode.None;
                        }                        
                        Cursor.visible = false;
                        paused = false;
                    }
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
                        Interface.instance.achievements.SetActive(true);
                        Interface.instance.vignette.SetActive(false);
                        firstPersonController.Locked = true;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = false;
                        paused = true;
                    }
                }
            }
            else
            {
                pressingEsc = false;
            }
        }
        if (!Interface.instance.achievements.activeSelf && !Diary.instance.diaryInterface.enabled)
        {
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;
        }        
        if (!paused && !MonologueManager.instance.Typing)
        {
            RaycastHit hit;
            Vector3 direction = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
            Ray ray = Camera.main.ScreenPointToRay(direction);
            Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
            if (hit.transform)
            {
                if (hit.transform.CompareTag("Interactable"))
                {
                    int hitId = hit.transform.gameObject.GetInstanceID();
                    if (!MonologueManager.instance.Running)
                    {
                        if (/* hitId != car.GetInstanceID() || */ !ended)
                        {
                            //Interface.instance.redActionImage.sprite = MonologueManager.instance.RedAction(hitId);
                            //red = Interface.instance.redActionImage.sprite != null;
                            //Interface.instance.redActionImage.enabled = red;

                            CheckBlueAndChangeSprite(hitId);
                            Interface.instance.mouseHudImage.enabled = red;
                        }
                        else
                        {
                            HideMouseHUD();
                        }
                    }
                    else
                    {
                        HideMouseHUD();
                    }
                    if (Input.GetAxisRaw("Fire1") == 1 && !firing1 /*&& !MonologueManager.instance.Running*/)
                    {
                        firing1 = true;
                        int interactableId = hit.transform.gameObject.GetInstanceID();
                        switch (nextStep)
                        {
                            case NextStep.Iracema:
                                if (interactableId == iracema.GetInstanceID())
                                {
                                    //AudioSource doggieAudioSource = doggie.GetComponent<AudioSource>();
                                    //doggieAudioSource.Play();

                                    if (MonologueManager.instance.Running)
                                    {
                                        if (MonologueManager.instance.Sentences.Count == 0)
                                        {
                                            firstPersonController.Locked = false;
                                            nextStep = NextStep.Sandoval;
                                        }
                                        MonologueManager.instance.DisplayNextSentence();
                                    }
                                    else
                                    {
                                        MonologueManager.instance.StartMonologue(iracemaSemChave);
                                        firstPersonController.Locked = true;
                                    }
                                }
                                break;
                        }
                                
                        /*
                        else if (interactableId == wife.GetInstanceID())
                        {
                            if (!kissed)
                            {
                                AudioSource kissAudioSource = GetComponent<PlayerSounds>().kiss.GetComponent<AudioSource>();
                                kissAudioSource.Play();
                                Interface.instance.NecroRomancer();
                            }
                            kissed = true;
                        }
                        else if (interactableId == fishBowl.GetInstanceID() || interactableId == fishFood.GetInstanceID())
                        {
                            if (!fedFish)
                            {
                                AudioSource fishAudioSource = fishBowl.GetComponent<AudioSource>();
                                fishAudioSource.PlayDelayed(0.6f);
                                fedFish = true;
                                firstPersonController.Locked = true;
                                fishFood.GetComponent<Animator>().SetTrigger("Feed");
                                Invoke("UnlockCharacter", 2.15f);
                                Interface.instance.Invoke("SwimmingWithTheFishes", 2.15f);
                            }
                        }
                        else if (interactableId == bathroomFaucet.GetInstanceID())
                        {
                                                    
                        }
                        else if (interactableId == Diary.instance.diaryObject.GetInstanceID())
                        {
                            Diary.instance.diaryObject.SetActive(false);
                            AudioManager.instance.PlayDelayed("Task Completed", 0.5f);
                            Diary.instance.CrossObjective("wake", 0.5f);
                            doors[0].unlocked = true;
                        }
                        else
                        {
                            switch (nextStep)
                            {
                                case NextStep.Shower:
                                    if (interactableId == shower.GetInstanceID())
                                    {
                                        nextStep = NextStep.ChangeClothes;
                                        showerWater.Play();
                                        shower.GetComponent<AudioSource>().Play();
                                        AudioSource singAudioSource = GetComponent<PlayerSounds>().singing.GetComponent<AudioSource>();
                                        singAudioSource.PlayDelayed(0.5f);
                                        doors[2].unlocked = true;
                                        Interface.instance.SittingDuckObjective();
                                        AudioManager.instance.PlayDelayed("Task Completed", 4f);
                                        Diary.instance.CrossObjective("shower", 4f);
                                    }
                                    break;
                                case NextStep.ChangeClothes:
                                    if (interactableId == skeleton.GetInstanceID())
                                    {
                                        nextStep = NextStep.Serial;
                                        skeleton.transform.GetChild(0).gameObject.SetActive(false);
                                        AudioSource jacketAudioSource = GetComponent<PlayerSounds>().jacket.GetComponent<AudioSource>();
                                        jacketAudioSource.Play();
                                        doors[3].unlocked = true;
                                        mirrorJacket.SetActive(true);
                                        mirrorJacket.transform.parent.GetComponent<BoxCollider>().enabled = false;
                                        Interface.instance.InTheClosetObjective();
                                        AudioManager.instance.PlayDelayed("Task Completed", 1f);
                                        Diary.instance.CrossObjective("jacket", 1f);
                                        return;
                                    }
                                    break;
                                case NextStep.Serial:
                                    if (interactableId == serial.GetInstanceID())
                                    {
                                        nextStep = NextStep.Milk;
                                        AudioSource cerealSound = bowl.gameObject.transform.GetChild(0).GetComponent<AudioSource>();
                                        cerealSound.Play();
                                    }
                                    else if (interactableId == arms.GetInstanceID())
                                    {
                                        hit.transform.gameObject.SetActive(false);
                                    }
                                    break;
                                case NextStep.Milk:
                                    if (interactableId == milk.GetInstanceID())
                                    {
                                        nextStep = NextStep.Eat;
                                        AudioSource milkSound = bowl.gameObject.transform.GetChild(1).GetComponent<AudioSource>();
                                        milkSound.Play();
                                    }
                                    else if (interactableId == arms.GetInstanceID())
                                    {
                                        hit.transform.gameObject.SetActive(false);
                                    }
                                    break;
                                case NextStep.Eat:
                                    if (interactableId == bowl.GetInstanceID())
                                    {
                                        if (!firstPersonController.Locked)
                                        {
                                            nextStep = NextStep.MaskAndWeapons;
                                            firstPersonController.Locked = true;
                                            bowl.SetActive(false);
                                            bowl.transform.localPosition = bowlSinkPosition;
                                            bowl.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                                            cerealBowCanvas.SetActive(true);
                                            doors[4].unlocked = true;
                                            doors[5].unlocked = true;
                                            Invoke("DisableCerealCanvas", 1.05f);
                                            AudioSource eatingAudioSource = GetComponent<PlayerSounds>().eating.GetComponent<AudioSource>();
                                            eatingAudioSource.Play();
                                            AudioSource yumAudioSource = GetComponent<PlayerSounds>().yumYum.GetComponent<AudioSource>();
                                            yumAudioSource.PlayDelayed(1.2f);
                                            Interface.instance.Invoke("CerealKillerObjective", 1.05f);
                                            AudioManager.instance.PlayDelayed("Task Completed", 1.5f);
                                            Diary.instance.CrossObjective("cereal", 1.5f);
                                        }
                                    }
                                    else if (interactableId == arms.GetInstanceID())
                                    {
                                        hit.transform.gameObject.SetActive(false);
                                    }
                                    break;
                                case NextStep.MaskAndWeapons:
                                    if (interactableId == mask.GetInstanceID())
                                    {
                                        if (armed)
                                        {
                                            nextStep = NextStep.DriveOff;
                                        }
                                        AudioSource niceAudioSource = GetComponent<PlayerSounds>().nice.GetComponent<AudioSource>();
                                        niceAudioSource.Play();
                                        mask.SetActive(false);
                                        mirrorMask.SetActive(true);
                                        mirrorJacket.GetComponent<BoxCollider>().enabled = false;
                                        masked = true;
                                        Interface.instance.LookAtTheClownObjective();
                                        AudioManager.instance.PlayDelayed("Task Completed", 1f);
                                        Diary.instance.CrossObjective("mask", 1f);
                                    }
                                    else if (interactableId == arms.GetInstanceID() || interactableId == chainsaw.GetInstanceID() || interactableId == sword.GetInstanceID())
                                    {
                                        hit.transform.gameObject.SetActive(false);
                                        armed = !arms.activeSelf && !chainsaw.activeSelf && !sword.activeSelf;
                                        if (armed)
                                        {
                                            Interface.instance.ToolsOfTheTradeObjective();
                                            AudioManager.instance.PlayDelayed("Task Completed", 0.2f);
                                            Diary.instance.CrossObjective("tools", 0.2f);
                                            if (masked)
                                            {
                                                nextStep = NextStep.DriveOff;
                                            }
                                        }
                                    }
                                    else if ((interactableId == bowl.GetInstanceID() || interactableId == sinkFaucet.GetInstanceID()))
                                    {
                                        if (!firstPersonController.Locked && !cleaned)
                                        {
                                            cleaned = true;
                                            firstPersonController.Locked = true;
                                            sinkFaucetWater.Play();
                                            sinkFaucet.GetComponent<AudioSource>().Play();
                                            bowl.GetComponent<Animator>().SetTrigger("Clean");
                                            Invoke("UnlockCharacter", 2f);
                                            Interface.instance.Invoke("DishesUnecessary", 2f);
                                        }
                                    }
                                    break;
                                case NextStep.DriveOff:
                                    if (interactableId == car.GetInstanceID() && !firstPersonController.Locked)
                                    {
                                        firstPersonController.Locked = true;
                                        ending.SetActive(true);
                                        ended = true;
                                        AudioManager.instance.FadeOut("Gameplay Song Loop", 1f);
                                        AudioManager.instance.PlayDelayed("Car Start", 1f);
                                        AudioManager.instance.PlayDelayed("Car Drive Off", 2.25f);
                                        AudioManager.instance.FadeIn("Credits Theme", 2f, 5f);
                                        Interface.instance.Invoke("TGIF", 2.25f);
                                        AudioManager.instance.PlayDelayed("Task Completed", 1f);
                                        Diary.instance.CrossObjective("car", 1f);
                                    }
                                    else if ((interactableId == bowl.GetInstanceID() || interactableId == sinkFaucet.GetInstanceID()))
                                    {
                                        if (!firstPersonController.Locked && !cleaned)
                                        {
                                            cleaned = true;
                                            firstPersonController.Locked = true;
                                            sinkFaucetWater.Play();
                                            sinkFaucet.GetComponent<AudioSource>().Play();
                                            bowl.GetComponent<Animator>().SetTrigger("Clean");
                                            Invoke("UnlockCharacter", 2f);
                                            Interface.instance.Invoke("DishesUnecessary", 2f);
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }                           
                        }
                        */
                    }                    
                }
                else if (hit.transform.CompareTag("Door"))
                {
                    int doorId = hit.transform.gameObject.GetInstanceID();
                    /*
                    foreach (Door door in doors)
                    {
                        if (doorId == door.gameObject.GetInstanceID())
                        {
                            if (door.unlocked)
                            {
                                if (door.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                                {
                                    if (GamepadDetector.instance.GamepadConnected)
                                    {
                                        Interface.instance.mouseHudImage.enabled = false;
                                        Interface.instance.bluePadHudImage.enabled = true;
                                        Interface.instance.redPadHudImage.enabled = false;
                                    }
                                    else
                                    {
                                        Interface.instance.bluePadHudImage.enabled = false;
                                        Interface.instance.redPadHudImage.enabled = false;
                                        Interface.instance.mouseHudImage.sprite = Interface.instance.mouseBlue;
                                        Interface.instance.mouseHudImage.enabled = true;
                                    }
                                    if (door.closed)
                                    {
                                        Interface.instance.blueActionImage.sprite = Interface.instance.blueOpen;
                                        Interface.instance.blueActionImage.enabled = true;
                                        blue = true;
                                        if (Input.GetAxisRaw("Fire1") == 1 && !firing1 && !MonologueManager.instance.Running)
                                        {
                                            firing1 = true;
                                            OpenDoor(door);
                                        }
                                        
                                    }
                                    else
                                    {
                                        Interface.instance.blueActionImage.sprite = Interface.instance.blueClose;
                                        Interface.instance.blueActionImage.enabled = true;
                                        blue = true;
                                        if (Input.GetAxisRaw("Fire1") == 1 && !firing1 && !MonologueManager.instance.Running)
                                        {                                            
                                            firing1 = true;
                                            CloseDoor(door);
                                        }                                        
                                    }
                                }
                            }
                            else
                            {
                                if (Input.GetAxisRaw("Fire1") == 1 && !firing1 && !MonologueManager.instance.Running)
                                {
                                    firing1 = true;
                                    AudioSource doorClosedAudioSource = door.gameObject.GetComponent<AudioReferences>().doorClosed.GetComponent<AudioSource>();
                                    if (doorClosedAudioSource)
                                    {
                                        doorClosedAudioSource.Play();
                                    }
                                    lockedCounter++;

                                    if (lockedCounter % 3 == 0)
                                    {
                                        AudioSource pissedAudioSource = GetComponent<PlayerSounds>().pissed.GetComponent<AudioSource>();
                                        pissedAudioSource.PlayDelayed(0.3f);
                                    }
                                }                                
                            }
                            return;
                        }
                    }                    
                    */
                }
                else
                {
                    HideMouseHUD();
                }
            }
            else
            {
                HideMouseHUD();
            }            
            if (Input.GetAxisRaw("Fire1") == 0)
            {
                firing1 = false;
            }
        }
        else
        {
            HideMouseHUD();
        }
    }

    private void CheckBlueAndChangeSprite(int hitId)
    {
        
        if (iracema.GetInstanceID() == hitId)
        {
            if (nextStep == NextStep.Iracema) 
            {
                Interface.instance.redActionImage.sprite = Interface.instance.blueFalar;
                Interface.instance.redActionImage.enabled = true;
                red = true;
            }            
        }
        /*
        else if ((fishBowl.GetInstanceID() == hitId || fishFood.GetInstanceID() == hitId) && !fedFish)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueUse;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }else if (bathroomFaucet.GetInstanceID() == hitId)
        {
                    
        }
        else if (shower.GetInstanceID() == hitId && nextStep == NextStep.Shower)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueOpen;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (skeleton.GetInstanceID() == hitId && nextStep == NextStep.ChangeClothes)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueTake;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (serial.GetInstanceID() == hitId && nextStep == NextStep.Serial)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueUse;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (milk.GetInstanceID() == hitId && nextStep == NextStep.Milk)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueUse;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (bowl.GetInstanceID() == hitId && nextStep == NextStep.Eat)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueEat;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (bowl.GetInstanceID() == hitId && !cleaned && (nextStep == NextStep.MaskAndWeapons || nextStep == NextStep.DriveOff))
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueUse;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (sinkFaucet.GetInstanceID() == hitId && !cleaned && (nextStep == NextStep.MaskAndWeapons || nextStep == NextStep.DriveOff))
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueUse;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (wife.GetInstanceID() == hitId && !kissed)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueKiss;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (mask.GetInstanceID() == hitId && !masked)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueTake;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (arms.GetInstanceID() == hitId || chainsaw.GetInstanceID() == hitId || sword.GetInstanceID() == hitId)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueTake;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (car.GetInstanceID() == hitId && nextStep == NextStep.DriveOff && !ended)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueOpen;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else if (Diary.instance.diaryObject.GetInstanceID() == hitId)
        {
            Interface.instance.blueActionImage.sprite = Interface.instance.blueTake;
            Interface.instance.blueActionImage.enabled = true;
            blue = true;
        }
        else
        {
            Interface.instance.blueActionImage.enabled = false;
            blue = false;
        }
        */
    }

    private void HideMouseHUD()
    {
        Interface.instance.redActionImage.enabled = false;
        Interface.instance.mouseHudImage.enabled = false;
        red = false;
    }

    private void UnlockCharacter()
    {
        firstPersonController.Locked = false;
    }

    private void OpenDoor(Door door)
    {
        door.animator.SetTrigger("Open");
        door.closed = false;
        AudioReferences audioReferences = door.gameObject.GetComponent<AudioReferences>();
        if (audioReferences)
        {
            AudioSource doorOpeningAudioSource = audioReferences.doorOpening.GetComponent<AudioSource>();
            if (doorOpeningAudioSource)
            {
                doorOpeningAudioSource.Play();
            }
        }
    }

    private void CloseDoor(Door door)
    {
        door.animator.SetTrigger("Close");
        door.closed = true;
        AudioReferences audioReferences = door.gameObject.GetComponent<AudioReferences>();
        if (audioReferences)
        {
            AudioSource doorClosingAudioSource = null;
            if (audioReferences.doorClosing)
            {
                doorClosingAudioSource = audioReferences.doorClosing.GetComponent<AudioSource>();
            }
            if (doorClosingAudioSource)
            {
                doorClosingAudioSource.PlayDelayed(0.8f);
            } else
            {
                AudioSource doorOpeningAudioSource = audioReferences.doorOpening.GetComponent<AudioSource>();
                if (doorOpeningAudioSource)
                {
                    doorOpeningAudioSource.Play();
                }
            }
            
        }
    }

    private class Door
    {
        public GameObject gameObject;
        public Animator animator;
        public bool unlocked = false;
        public bool closed = true;

        public Door(GameObject gameObject)
        {
            this.gameObject = gameObject;
            animator = gameObject.GetComponent<Animator>();
        }

        public Door(GameObject gameObject, bool unlocked)
        {
            this.gameObject = gameObject;
            animator = gameObject.GetComponent<Animator>();
            this.unlocked = unlocked;
        }
    }
}