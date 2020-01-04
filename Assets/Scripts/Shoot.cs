using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.EventSystems;

public class Shoot : MonoBehaviour
{
    [Range(0f, 10f)]
    public float maxDistance = 5f;
    public GameObject ending;
    public GameObject jucilene;
    //public GameObject doggie, fishBowl, fishFood, shower, skeleton, serial, milk, bowl, cerealBowCanvas, bathroomFaucet, sinkFaucet, wife, mask, arms, chainsaw, sword, car, mirrorJacket, mirrorMask;
    //public Vector3 bowlSinkPosition;
    //public ParticleSystem showerWater, bathroomFaucetWater, sinkFaucetWater;
    //public Material bowlEmpty, bowlSerialAndMilk, bowlSerial;
    //public Transform bedroomDoor, bathroomDoor, closetDoor, kitchenDoor, tvDoor, garageDoor, outsideDoor, fridgeDoor1, fridgeDoor2;
    public int lockedCounter = 0;

    private FirstPersonController firstPersonController;
    private LayerMask layerMask;
    private bool firing1, pressingEsc, pressingDiary, paused = false, fedFish, pettedDoggie, kissed, masked, ended = false, armed, cleaned;
    private enum NextStep
    {
        Jucilene = 1, Iracema =2, Sandoval =4, ProcurandoChave = 8, EntraQuarto =16, FechaPorta = 32
    };
    private NextStep nextStep = NextStep.Jucilene;
    //private Door[] doors = new Door[9];

    void Start()
    {
        layerMask = LayerMask.GetMask("Default");
        firstPersonController = GetComponent<FirstPersonController>();
        InteractionManager.instance.FirstPersonController(firstPersonController);
        
        firstPersonController.Locked = true;
        StartCoroutine(WaitAndTalk());
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

    private IEnumerator WaitAndTalk()
    {
        yield return new WaitForSeconds(0.5f);
        if (!InteractionManager.instance.Running)
        {
            InteractionManager.instance.StartInteraction(jucilene.GetInstanceID(), jucilene.tag);
        }
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

            if (Input.GetAxisRaw("Cancel") == 1f && !pressingEsc)
            {	
		        pressingEsc = true;
                Interface.instance.Menu();
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
        if (!paused && !InteractionManager.instance.Typing)
        {
            RaycastHit hit;
            Vector3 direction = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
            Ray ray = Camera.main.ScreenPointToRay(direction);
            Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
            if (hit.transform)
            {
                if (!hit.transform.CompareTag("Untagged"))
                {
                    int hitId = hit.transform.gameObject.GetInstanceID();
                    if (!InteractionManager.instance.Running)
                    {
                        if (!ended)
                        {
                            InteractionManager.instance.ChangeActionSprite(hit.transform.gameObject.GetInstanceID(), hit.transform.gameObject.tag);
                        }
                        else
                        {
                            InteractionManager.instance.HideMouseHUD();
                        }
                    }
                    else
                    {
                        InteractionManager.instance.HideMouseHUD();
                    }
                    if (Input.GetAxisRaw("Fire1") == 1 && !firing1)
                    {
                        firing1 = true;
                        if (InteractionManager.instance.Running)
                        {
                            if (InteractionManager.instance.Sentences.Count == 0)
                            {
                                firstPersonController.Locked = false;
                            }
                            InteractionManager.instance.DisplayNextSentence();
                        }
                        else
                        {
                            firstPersonController.Locked = true;
                            InteractionManager.instance.StartInteraction();
                        }
                    }                    
                }                
                else
                {
                    InteractionManager.instance.StoredInteractable = null;
                    InteractionManager.instance.HideMouseHUD();
                }
            }
            else
            {
                InteractionManager.instance.StoredInteractable = null;
                InteractionManager.instance.HideMouseHUD();
            }            
            if (Input.GetAxisRaw("Fire1") == 0)
            {
                firing1 = false;
            }
        }
        else
        {
            InteractionManager.instance.HideMouseHUD();
        }
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