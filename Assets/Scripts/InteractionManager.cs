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
    public List<Storyline> storylines;

    public static InteractionManager instance = null;
    //Adding a static (if possible) dictonary of <int sceneIndex, InteractionManager instance> should work for multiple scenes

    [SerializeField]
    private List<Interactable> interactables = new List<Interactable>();

    public Queue<string> Nameplates { get; set; }
    public Queue<string> Sentences { get; set; }

    public bool Running { get; set; }
    public bool Typing { get; set; }
    public Interactable StoredInteractable { get; set; }

    private WaitForSeconds delay = new WaitForSeconds(0.01f);
    private FirstPersonController firstPersonController;
    private UnityEvent storedFunctionsAfterDialogue = null;
    private bool checkConditions;


    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        Nameplates = new Queue<string>();
        Sentences = new Queue<string>();
        StoredInteractable = null;
    }

    public void AdvanceStoryline(string storylineName)
    {
        foreach (Storyline storyline in storylines)
        {
            if (storyline.name.Equals(storylineName))
            {
                storyline.currentStage++;
                return;
            }
        }
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
        if (StoredInteractable == null || StoredInteractable.iGameObject.GetInstanceID() != id)
        {
            foreach (Interactable interactable in interactables)
            {
                if (interactable.iGameObject.GetInstanceID() == id || (interactable.iGameObject == null && interactable.tag.Equals(tag)))
                {
                    StoredInteractable = interactable;
                    for (int i = interactable.interactions.Count - 1; i >= 0; i--)
                    {
                        if (FulfillConditions(interactable.interactions[i]))
                        {
                            actionImage.sprite = interactable.interactions[i].actionSprite;
                            actionImage.enabled = true;
                            mouseHudImage.enabled = true;
                            checkConditions = false;
                            return;
                        }
                    }
                    HideMouseHUD();
                    checkConditions = false;
                }
            }
        }
        else if (StoredInteractable != null)
        {
            if (checkConditions)
            {
                for (int i = StoredInteractable.interactions.Count - 1; i >= 0; i--)
                {
                    if (FulfillConditions(StoredInteractable.interactions[i]))
                    {
                        actionImage.sprite = StoredInteractable.interactions[i].actionSprite;
                        actionImage.enabled = true;
                        mouseHudImage.enabled = true;
                        checkConditions = false;
                        return;
                    }
                }
                HideMouseHUD();
                checkConditions = false;
            }
        }       
    }

    public void HideMouseHUD()
    {
        actionImage.enabled = false;
        mouseHudImage.enabled = false;
    }

    public void StartInteraction()
    {
        Running = true;
        animator.SetBool("IsOpen", true);
        Nameplates.Clear();
        Sentences.Clear();
        if (StoredInteractable != null)
        {
            for (int i = StoredInteractable.interactions.Count - 1; i >= 0; i--)
            {
                if (FulfillConditions(StoredInteractable.interactions[i]))
                {
                    StoredInteractable.interactions[i].functions.Invoke();
                    storedFunctionsAfterDialogue = StoredInteractable.interactions[i].functionsAfterDialogue;
                    foreach (string nameplate in StoredInteractable.interactions[i].nameplates)
                    {
                        Nameplates.Enqueue(nameplate);
                    }
                    foreach (string sentence in StoredInteractable.interactions[i].sentences)
                    {
                        Sentences.Enqueue(sentence);
                    }
                    DisplayNextSentence();
                    return;
                }
            }
        }
        EndDialogue();
    }

    public void StartInteraction (int id, string tag)
	{
        Running = true;
		animator.SetBool("IsOpen", true);
        Nameplates.Clear();
		Sentences.Clear();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.iGameObject.GetInstanceID() == id || (interactable.iGameObject == null && interactable.tag.Equals(tag)))
            {
                for (int i = interactable.interactions.Count - 1; i >= 0; i--)
                {
                    if (FulfillConditions(interactable.interactions[i]))
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

    private bool FulfillConditions(Interactable.Interaction interaction)
    {
        for (int i = 0; i < storylines.Count; i++)
        {
            if (interaction.storylinesData[i].isRequired)
            {
                if (storylines[i].currentStage < interaction.storylinesData[i].minStage || storylines[i].currentStage > interaction.storylinesData[i].maxStage)
                {
                    return false;
                }                
            }
        }
        for (int i = 0; i < interaction.conditions.Count; i++)
        {
            if (interaction.conditions[i] == -1 && booleans[i].value)
            {
                return false;
            }
            else if (interaction.conditions[i] == 1 && !booleans[i].value)
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
        checkConditions = true;
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
public class Storyline
{
    public string name;
    public int stages;
    public int currentStage;

    public Storyline(string name, int stages, int currentStage)
    {
        this.name = name;
        this.stages = stages;
        this.currentStage = currentStage;
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
        public List<StorylineData> storylinesData = new List<StorylineData>();

        [System.Serializable]
        public class StorylineData
        {
            public bool isRequired;
            public int minStage, maxStage;

            public StorylineData(bool isRequired, int minStage, int maxStage)
            {
                this.isRequired = isRequired;
                this.minStage = minStage;
                this.maxStage = maxStage;
            }
        }
    }
}