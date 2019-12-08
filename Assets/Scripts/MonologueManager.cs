using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class MonologueManager : MonoBehaviour
{
	public Text dialogueText;
	public Animator animator;

    public static MonologueManager instance = null;

    [SerializeField]
    private List<Interactable> interactables = new List<Interactable>();

	public Queue<string> Sentences{ get; set; }

    public bool Running { get; set; }
    public bool Typing { get; set; }
    public int InteractableId { get; set; }

    private WaitForSeconds delay = new WaitForSeconds(0.03f);
    private FirstPersonController firstPersonController;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () 
	{
		Sentences = new Queue<string> ();
        InteractableId = -1;
    }

    public void StartMonologue(string sentence)
    {
        Running = true;
        animator.SetBool("IsOpen", true);
        Sentences.Clear();
        Sentences.Enqueue(sentence);
        DisplayNextSentence();
    }

    public void StartMonologue(string[] sentences)
    {
        Running = true;
        animator.SetBool("IsOpen", true);
        Sentences.Clear();
        if (sentences.Length > 0)
        {
            foreach (string sentence in sentences)
            {
                Sentences.Enqueue(sentence);
            }
            DisplayNextSentence();
            return;
        }
        EndMonologue();
    }

    public void StartMonologue (int id)
	{
        Running = true;
        InteractableId = id;
		animator.SetBool("IsOpen", true);
		Sentences.Clear ();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.iGameObject.GetInstanceID() == id)
            {
                foreach (string sentence in interactable.sentences)
                {
                    Sentences.Enqueue(sentence);
                }
                DisplayNextSentence();
                return;
            }
        }
        EndMonologue();
    }

	public void DisplayNextSentence()
	{
		if (Sentences.Count == 0) 
		{
			EndMonologue ();
			return;
		}

		string sentence = Sentences.Dequeue ();
		StopAllCoroutines ();
		StartCoroutine (TypeSentence (sentence));
	}

    public Sprite RedAction(int hitId)
    {
        foreach (Interactable interactable in interactables)
        {
            if (interactable.iGameObject.GetInstanceID() == hitId)
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
                    yield return delay;
                }
                i = richTextIndex;
                richTextIndex = sentence.IndexOf('<', richTextIndex + 1);
            }
            else 
            {
                dialogueText.text += letters[i];
                yield return delay;
            }
			
		}
        Typing = false;
    }

	public void EndMonologue()
	{
		animator.SetBool("IsOpen", false);
        firstPersonController.Locked = false;
        StartCoroutine(Wait());
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
public class Interactable
{
    public GameObject iGameObject;
    [TextArea(1, 10)]
    public string[] sentences;
    public Sprite redAction;
}