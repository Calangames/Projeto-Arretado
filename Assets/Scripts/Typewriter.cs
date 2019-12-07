using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Typewriter : MonoBehaviour 
{
	public float typingSpeed = 40f;
	public Text text;

    // todo: create a list of [string replaceString, string newString] and for inside type text
    public string newString = "";

	private char[] charArray;
	private WaitForSeconds delay;
	private IEnumerator coroutine;
	private string colorHex = "#CE0000FF";
	private bool completedTyping = false;

	// Use this for initialization
	void Start () 
	{
		delay = new WaitForSeconds ( typingSpeed == 0f ? 0f: 1f/typingSpeed);
	}

	public bool Completed()
	{
		return completedTyping;
	}

	public void Write(string randomEvent)
	{
		completedTyping = false;
		coroutine = TypeText (randomEvent);
		StartCoroutine (coroutine);
	}

	private IEnumerator TypeText(string randomEvent)
	{
		int position = randomEvent.IndexOf ("@");
		text.text = randomEvent.Replace ("@", newString.ToString());

		charArray = text.text.ToCharArray ();
		text.text = "";

		int i = -1;
		foreach (char letter in charArray) {
			yield return delay;
			i++;
			if (position >= 0 && i >= position && i <= position + newString.Length) {
				text.text += "<color=" + colorHex + ">" + letter + "</color>";
			} else {
				text.text += letter;
			}
		}
		completedTyping = true;
	}
}