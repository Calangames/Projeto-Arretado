using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;

public class InteractionManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI dialogueText, nameplateText;
    public Image actionImage, mouseHudImage;
    public Animator animator;
    public List<Boolean> booleans;

    public static InteractionManager instance = null;
    //Adding a static (if possible) dictonary of <int sceneIndex, InteractionManager instance> should work for multiple scenes

    [SerializeField]
    private List<Interactable> interactables = new List<Interactable>();

    public Queue<string> Nameplates { get; set; }
    public Queue<string> Sentences { get; set; }

    public bool Running { get; set; }
    public bool Typing { get; set; }
    public int InteractableId { get; set; }

    private WaitForSeconds delay = new WaitForSeconds(0.01f);
    private FirstPersonController firstPersonController;
    private UnityEvent storedFunctionsAfterDialogue = null;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        Nameplates = new Queue<string>();
        Sentences = new Queue<string>();
        InteractableId = -1;
    }

    public void ChangeBoolToFalse(string boolName)
    {
        foreach (Boolean boolean in booleans)
        {
            if (boolean.name.Equals(boolName))
            {
                boolean.value = false;
                return;
            }
        }
    }

    public void ChangeBoolToTrue(string boolName)
    {
        foreach (Boolean boolean in booleans)
        {
            if (boolean.name.Equals(boolName))
            {
                boolean.value = true;
                return;
            }
        }
    }

    public void ChangeActionSprite(int id, string tag)
    {
        foreach (Interactable interactable in interactables)
        {
            if (interactable.iGameObject.GetInstanceID() == id || (interactable.iGameObject == null && interactable.tag.Equals(tag)))
            {
                for (int i = interactable.interactions.Count - 1; i >= 0; i--)
                {
                    if (FulfillConditions(interactable.interactions[i].conditions))
                    {
                        actionImage.sprite = interactable.interactions[i].actionSprite;
                        actionImage.enabled = true;
                        mouseHudImage.enabled = true;
                        return;
                    }
                }
            }
        }
    }

    public void HideMouseHUD()
    {
        actionImage.enabled = false;
        mouseHudImage.enabled = false;
    }

    public void StartInteraction (int id, string tag)
	{
        Running = true;
        InteractableId = id;
		animator.SetBool("IsOpen", true);
        Nameplates.Clear();
		Sentences.Clear();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.iGameObject.GetInstanceID() == id || (interactable.iGameObject == null && interactable.tag.Equals(tag)))
            {
                for (int i = interactable.interactions.Count - 1; i >= 0; i--)
                {
                    if (FulfillConditions(interactable.interactions[i].conditions))
                    {
                        interactable.interactions[i].functions.Invoke();
                        storedFunctionsAfterDialogue = interactable.interactions[i].functionsAfterDialogue;
                        foreach (string nameplate in interactable.interactions[i].nameplates)
                        {
                            Nameplates.Enqueue(nameplate);
                        }
                        foreach (string sentence in interactable.interactions[i].sentences)
                        {
                            Sentences.Enqueue(sentence);
                        }
                        DisplayNextSentence();
                        return;
                    }
                }                
            }
        }
        EndDialogue();
    }

    private bool FulfillConditions(List<int> conditions)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i] == -1 && booleans[i].value)
            {
                return false;
            }
            else if (conditions[i] == 1 && !booleans[i].value)
            {
                return false;
            }
        }
        return true;
    }

    public void DisplayNextSentence()
	{
		if (Sentences.Count == 0) 
		{
			EndDialogue();
			return;
		}
        nameplateText.text = Nameplates.Dequeue();
        string sentence = Sentences.Dequeue ();
		//StopAllCoroutines ();
		StartCoroutine (TypeSentence (sentence));
	}

    //Legacy
    public Sprite Action(int hitId, string tag)
    {
        foreach (Interactable interactable in interactables)
        {
            if (interactable.iGameObject.GetInstanceID() == hitId || interactable.tag.Equals(tag))
            {
                return interactable.redAction;
            }
        }
        return null;
    }

	IEnumerator TypeSentence (string sentence)
	{
        Typing = true;
		dialogueText.text = "";
        int richTextIndex = sentence.IndexOf('<');
        char[] letters = sentence.ToCharArray();
        yield return delay;
        for (int i = 0; i < sentence.Length; i++) 
		{
            if (richTextIndex > -1 && i == richTextIndex) 
            {
                //needs multi tag support
                string previousText = dialogueText.text;
                richTextIndex = sentence.IndexOf('>', richTextIndex + 1);
                string openingTag = sentence.Substring(i, richTextIndex - i);
                i = richTextIndex++;
                richTextIndex = sentence.IndexOf("</", richTextIndex + 1);
                int j = richTextIndex;
                richTextIndex = sentence.IndexOf('>', richTextIndex + 1);
                string closingTag = sentence.Substring(j, richTextIndex + 1 - j);
                string newText = "";
                while (i < j)
                {
                    newText += letters[i];
                    dialogueText.text = previousText + openingTag + newText + closingTag;
                    i++;
                    yield return null;
                }
                i = richTextIndex;
                richTextIndex = sentence.IndexOf('<', richTextIndex + 1);
            }
            else 
            {
                dialogueText.text += letters[i];
                yield return null;
            }
			
		}
        Typing = false;
    }

    public void EndDialogue()
    {
		animator.SetBool("IsOpen", false);
        firstPersonController.Locked = false;
        StartCoroutine(Wait());
        if (storedFunctionsAfterDialogue != null)
        {
            storedFunctionsAfterDialogue.Invoke();
            storedFunctionsAfterDialogue = null;
        }        
    }

    IEnumerator Wait()
    {
        yield return delay;
        Running = false;
    }

    public void FirstPersonController(FirstPersonController firstPersonController)
    {
        this.firstPersonController = firstPersonController;
    }
}

[System.Serializable]
public class Boolean
{
    public string name;
    public bool value;

    public Boolean(string name, bool value)
    {
        this.name = name;
        this.value = value;
    }
}

[System.Serializable]
public class Interactable
{
    public GameObject iGameObject;
    public string tag;
    public Texture texture;
    [TextArea(1, 10)]
    public string[] sentences;
    public Sprite redAction; //Legacy
    public List<Interaction> interactions = new List<Interaction>();

    [System.Serializable]
    public class Interaction
    {
        public string description;
        public Sprite actionSprite;
        [TextArea(1, 1)]
        public string[] nameplates;
        [TextArea(1, 10)]
        public string[] sentences;
        public UnityEvent functions, functionsAfterDialogue;
        public List<int> conditions = new List<int>();        
    }
}