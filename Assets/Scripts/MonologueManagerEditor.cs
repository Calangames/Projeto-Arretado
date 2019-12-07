#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonologueManager))]
public class MonologueManagerEditor : Editor
{
    SerializedProperty dialogueText, animator, interactables;

    static bool[] show = new bool[0];

    void OnEnable()
    {
        dialogueText = serializedObject.FindProperty("dialogueText");
        animator = serializedObject.FindProperty("animator");
        interactables = serializedObject.FindProperty("interactables");
        if (interactables.arraySize == 0)
        {
            serializedObject.Update();
            interactables.arraySize = 1;
            interactables.GetArrayElementAtIndex(0).FindPropertyRelative("sentences").arraySize = 1;
            serializedObject.ApplyModifiedProperties();
        }
        if (show.Length == 0)
        {
            show = new bool[interactables.arraySize];
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(dialogueText);
        EditorGUILayout.PropertyField(animator);
        for (int iIndex = 0; iIndex < interactables.arraySize; iIndex++)
        {
            using (var rect = new EditorGUILayout.VerticalScope())
            {
                SerializedProperty interactable = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("iGameObject");
                if (interactable.objectReferenceValue == null)
                {
                    show[iIndex] = EditorGUILayout.Foldout(show[iIndex], "None");
                }
                else
                {
                    show[iIndex] = EditorGUILayout.Foldout(show[iIndex], interactable.objectReferenceValue.name);
                }
                
                if (show[iIndex])
                {
                    SerializedProperty redAction = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("redAction");                    
                    SerializedProperty sentences = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("sentences");                    
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(interactable, GUIContent.none);
                        EditorGUILayout.PropertyField(redAction, new GUIContent("Red Action"));                        
                        for (int sIndex = 0; sIndex < sentences.arraySize; sIndex++)
                        {
                            EditorGUILayout.PropertyField(sentences.GetArrayElementAtIndex(sIndex), GUIContent.none);
                        }
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUI.DisabledScope(sentences.arraySize <= 1))
                            {
                                if (GUILayout.Button(new GUIContent("Remove sentence")))
                                {
                                    sentences.arraySize--;
                                }
                            }
                            if (GUILayout.Button(new GUIContent("Add sentence")))
                            {
                                sentences.arraySize++;
                                sentences.GetArrayElementAtIndex(sentences.arraySize - 1).stringValue = "";
                            }
                        }                        
                    }
                }
            }
        }
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(interactables.arraySize <= 1))
            {
                if (GUILayout.Button(new GUIContent("Remove interactable"), GUILayout.Height(36f)))
                {
                    interactables.arraySize--;
                }
            }
            if (GUILayout.Button(new GUIContent("Add interactable"), GUILayout.Height(36f)))
            {
                interactables.arraySize++;
                if (interactables.arraySize > show.Length)
                {
                    CopyShowValues();
                }
                SerializedProperty newInteractable = interactables.GetArrayElementAtIndex(interactables.arraySize - 1);
                newInteractable.FindPropertyRelative("iGameObject").objectReferenceValue = null;
                newInteractable.FindPropertyRelative("sentences").arraySize = 1;
                newInteractable.FindPropertyRelative("sentences").GetArrayElementAtIndex(0).stringValue = "";
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void CopyShowValues()
    {
        bool[] tempArray = new bool[interactables.arraySize];
        tempArray[tempArray.Length - 1] = true;
        System.Array.Copy(show, tempArray, show.Length);
        show = tempArray;
    }
}
#endif