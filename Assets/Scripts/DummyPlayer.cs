using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class DummyPlayer : MonoBehaviour
{
    public float MinimumX = -120f;
    public float MaximumX = 120f;
    public float MinimumY = -4f;
    public float MaximumY = 20f;
    public Transform dummyHorizontal, dummyVertical;
    private Camera dummyCamera;
    public GameObject eyesCanvas, alarmClock;

    public static bool startedWaking = false;

    private Animator _animator;
    private Quaternion m_DummyHorizontalRot, m_DummyVerticalRot;
    private bool firing1, firing2, hitAlarm, canTurnHead, stopHidingHUD;
    private float maxDistance = 5f;
    private LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        m_DummyHorizontalRot = dummyHorizontal.localRotation;
        m_DummyVerticalRot = dummyVertical.localRotation;
        mask = LayerMask.GetMask("Default");
        dummyCamera = dummyVertical.GetComponent<Camera>();
        alarmClock.GetComponent<AlarmAudio>().alarmBeep.GetComponent<AudioSource>().PlayDelayed(0.5f);
        FindObjectOfType<PlayerSounds>().grumpy02.GetComponent<AudioSource>().PlayDelayed(2.5f);
        FindObjectOfType<PlayerSounds>().grumpy01.GetComponent<AudioSource>().PlayDelayed(5.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (canTurnHead)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * 2f;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * 2f;

            m_DummyHorizontalRot *= Quaternion.Euler(0f, yRot, 0f);
            m_DummyHorizontalRot = ClampRotationAroundYAxis(m_DummyHorizontalRot);
            dummyHorizontal.localRotation = m_DummyHorizontalRot;

            m_DummyVerticalRot *= Quaternion.Euler(-xRot, 0f, 0f);
            m_DummyVerticalRot = ClampRotationAroundXAxis(m_DummyVerticalRot);
            dummyVertical.localRotation = m_DummyVerticalRot;
        }
    }

    void LateUpdate()
    {
        if (!hitAlarm)
        {
            canTurnHead = startedWaking && !MonologueManager.instance.Running;
            if (!MonologueManager.instance.Typing)
            {
                RaycastHit hit;
                Vector3 direction = dummyCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, dummyCamera.nearClipPlane));
                Ray ray = dummyCamera.ScreenPointToRay(direction);
                Physics.Raycast(ray, out hit, maxDistance, mask, QueryTriggerInteraction.Ignore);
                if (hit.transform)
                {
                    if (hit.transform.CompareTag("Interactable"))
                    {
                        int hitId = hit.transform.gameObject.GetInstanceID();
                        if (!MonologueManager.instance.Running)
                        {
                            Interface.instance.redActionImage.sprite = MonologueManager.instance.RedAction(hitId);
                            Interface.instance.redActionImage.enabled = true;
                            if (alarmClock.GetInstanceID() == hitId)
                            {
                                Interface.instance.redActionImage.enabled = true;
                            }
                            else
                            {
                                Interface.instance.redActionImage.enabled = false;
                            }

                            Interface.instance.mouseHudImage.enabled = true;
                        }
                        else
                        {
                            HideMouseHUD();
                        }
                        if (Input.GetAxisRaw("Fire1") == 1 && !firing1)
                        {
                            firing1 = true;
                            if (!MonologueManager.instance.Running && alarmClock.GetInstanceID() == hitId)
                            {
                                startedWaking = false;
                                hitAlarm = true;
                                alarmClock.GetComponent<AlarmAudio>().alarmBeep.GetComponent<AudioSource>().Stop();
                                alarmClock.GetComponent<AlarmAudio>().alarmClick.GetComponent<AudioSource>().Play();
                                _animator.enabled = true;
                                eyesCanvas.GetComponent<Animator>().SetTrigger("Open");
                                AudioManager.instance.FadeIn("Gameplay Song Intro", 0.1f, 0.5f);
                                AudioManager.instance.PlayDelayed("Gameplay Song Loop", 16.5f);
                                FindObjectOfType<PlayerSounds>().yawn.GetComponent<AudioSource>().PlayDelayed(2.5f);
                            }
                        }
                        else if (Input.GetAxisRaw("Fire2") == 1 && !firing2)
                        {
                            firing2 = true;
                            if (MonologueManager.instance.Running && MonologueManager.instance.InteractableId == hitId)
                            {
                                if (MonologueManager.instance.Sentences.Count == 0)
                                {
                                    canTurnHead = true;
                                }
                                MonologueManager.instance.DisplayNextSentence();
                            }
                            else
                            {
                                canTurnHead = false;
                                MonologueManager.instance.StartMonologue(hitId);
                            }
                            return;
                        }
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
                if (Input.GetAxisRaw("Fire2") == 0)
                {
                    firing2 = false;
                }
            }
            else
            {
                HideMouseHUD();
            }
        }
        else if (!stopHidingHUD)
        {
            HideMouseHUD();
            stopHidingHUD = true;
        }
    }

    private void HideMouseHUD()
    {
        Interface.instance.redActionImage.enabled = false;
        Interface.instance.mouseHudImage.enabled = false;
    }

    public void Wake()
    {
        dummyCamera.depth = -10f;
        UnityStandardAssets.Characters.FirstPerson.FirstPersonController.waking = false;
        gameObject.SetActive(false);
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumY, MaximumY);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    Quaternion ClampRotationAroundYAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
