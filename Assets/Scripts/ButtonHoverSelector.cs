using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSelector : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    /*
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetInstanceID() == gameObject.GetInstanceID())
        {
            EventSystem.current.SetSelectedGameObject(null);
        }  
    }
    */

    public void OnDeselect(BaseEventData baseEventData)
    {
        animator.SetTrigger("Normal");
    }
}