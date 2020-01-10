#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InteractionManager))]
public class InteractionManagerEditor : Editor
{
    SerializedProperty dialogueText, nameplateText, actionImage, mouseHudImage, animator, interactables, booleans, storylines;
    static bool[] showI = new bool[0];
    static bool[][] showAction = new bool[0][];

    public Sprite teste;

    void OnEnable()
    {
        dialogueText = serializedObject.FindProperty("dialogueText");
        nameplateText = serializedObject.FindProperty("nameplateText");
        actionImage = serializedObject.FindProperty("actionImage");
        mouseHudImage = serializedObject.FindProperty("mouseHudImage");
        animator = serializedObject.FindProperty("animator");
        interactables = serializedObject.FindProperty("interactables");
        booleans = serializedObject.FindProperty("booleans");
        storylines = serializedObject.FindProperty("storylines");
        if (interactables.arraySize == 0)
        {
            interactables.arraySize = 1;
            interactables.GetArrayElementAtIndex(0).FindPropertyRelative("tag").arraySize = 1;
            interactables.GetArrayElementAtIndex(0).FindPropertyRelative("tag").stringValue = "Untagged";
            SerializedProperty interactions = interactables.GetArrayElementAtIndex(0).FindPropertyRelative("interactions");
            interactions.arraySize = 1;
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("description").stringValue = "No description";
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("nameplates").arraySize = 1;
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("nameplates").GetArrayElementAtIndex(0).stringValue = "";
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("sentences").arraySize = 1;
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("sentences").GetArrayElementAtIndex(0).stringValue = "";
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("conditions").arraySize = booleans.arraySize;
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("storylinesData").arraySize = storylines.arraySize;
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("storylinesData").GetArrayElementAtIndex(0).FindPropertyRelative("isRequired").boolValue = false;
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("storylinesData").GetArrayElementAtIndex(0).FindPropertyRelative("minStage").intValue = 1;
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("storylinesData").GetArrayElementAtIndex(0).FindPropertyRelative("maxStage").intValue = 1;
            interactions.GetArrayElementAtIndex(0).FindPropertyRelative("storylinesData").GetArrayElementAtIndex(0).intValue = 1;
            serializedObject.ApplyModifiedProperties();
        }
        if (showI.Length == 0)
        {
            showI = new bool[interactables.arraySize];
            showI[0] = interactables.arraySize <= 1 ? true : false;
        }
        if (showAction.Length == 0)
        {
            showAction = new bool[interactables.arraySize][];
            for (int i = 0; i < interactables.arraySize; i++)
            {
                showAction[i] = new bool[interactables.GetArrayElementAtIndex(i).FindPropertyRelative("interactions").arraySize];
            }
            showAction[0][0] = interactables.arraySize <= 1 ? true : false;
        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(dialogueText);
        EditorGUILayout.PropertyField(nameplateText);
        EditorGUILayout.PropertyField(actionImage);
        EditorGUILayout.PropertyField(mouseHudImage);
        EditorGUILayout.PropertyField(animator);
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Booleans", EditorStyles.boldLabel);
        int i = 0;
        int hBooleansMaxN = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth) / 90f);
        int hBooleansN = Mathf.Min(hBooleansMaxN, booleans.arraySize);
        float maxWidth = 70f;

        for (int bIndex = 0; bIndex < booleans.arraySize; bIndex++)
        {
            if (i == 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10f);
            }            
            SerializedProperty boolName = booleans.GetArrayElementAtIndex(bIndex).FindPropertyRelative("name");
            boolName.stringValue = EditorGUILayout.DelayedTextField(boolName.stringValue, GUILayout.MaxWidth(maxWidth));
            GUILayout.Space(10f);
            if (boolName.stringValue.Equals(""))
            {
                booleans.DeleteArrayElementAtIndex(bIndex);
                for (int iIndex = 0; iIndex < interactables.arraySize; iIndex++)
                {
                    SerializedProperty interactions = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("interactions");
                    for (int actionIndex = 0; actionIndex < interactions.arraySize; actionIndex++)
                    {
                        interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("conditions").DeleteArrayElementAtIndex(bIndex);
                    }
                }
            }
            if (bIndex == booleans.arraySize - 1)
            {
                if (i == hBooleansN - 1 && hBooleansN == hBooleansMaxN)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10f);
                    PlusBooleanButton(maxWidth);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    PlusBooleanButton(maxWidth);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else if (i == hBooleansN - 1)
            {
                EditorGUILayout.EndHorizontal();
                i = 0;
            }
            else
            {
                i++;
            }
        }
        if (booleans.arraySize == 0)
        {
            PlusBooleanButton(maxWidth);
        }
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Storylines", EditorStyles.boldLabel);
        for (int sIndex = 0; sIndex < storylines.arraySize; sIndex++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                SerializedProperty storylineName = storylines.GetArrayElementAtIndex(sIndex).FindPropertyRelative("name");
                storylineName.stringValue = EditorGUILayout.DelayedTextField(storylineName.stringValue, GUILayout.MaxWidth(150f));
                SerializedProperty storylineStages = storylines.GetArrayElementAtIndex(sIndex).FindPropertyRelative("stages");
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Stages", GUILayout.MaxWidth(50f));
                EditorGUILayout.IntSlider(storylineStages, 2, 20, GUIContent.none, GUILayout.MinWidth(80f), GUILayout.MaxWidth(180f));
                if (storylineName.stringValue.Equals(""))
                {
                    storylines.DeleteArrayElementAtIndex(sIndex);
                    for (int iIndex = 0; iIndex < interactables.arraySize; iIndex++)
                    {
                        SerializedProperty interactions = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("interactions");
                        for (int actionIndex = 0; actionIndex < interactions.arraySize; actionIndex++)
                        {
                            interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("storylinesData").DeleteArrayElementAtIndex(sIndex);
                        }
                    }
                }
            }
        }
        PlusStorylineButton(150f);
        EditorGUILayout.Separator();

        for (int iIndex = 0; iIndex < interactables.arraySize; iIndex++)
        {
            using (var rect = new EditorGUILayout.VerticalScope())
            {
                SerializedProperty interactableGOProp = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("iGameObject");
                SerializedProperty tag = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("tag");
                SerializedProperty texture = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("texture");
                GUIStyle style = GUIStyle.none;
                style.fontSize = 17;
                style.alignment = TextAnchor.MiddleLeft;
                style.padding.right = 0;

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button((Texture)texture.objectReferenceValue, GUILayout.Width(30f), GUILayout.Height(30f)))
                    {
                        showI[iIndex] = !showI[iIndex];
                    }
                    string s = interactableGOProp.objectReferenceValue != null ? interactableGOProp.objectReferenceValue.name :
                        !tag.stringValue.Equals("Untagged") ? "All " + tag.stringValue + "s" : "None";
                    EditorGUILayout.LabelField(new GUIContent(s), style, GUILayout.Height(30f));
                }

                if (showI[iIndex])
                {
                                                                              
                    using (new EditorGUI.IndentLevelScope())
                    {
                        #region Game Object, Tag and Icon
                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(interactableGOProp, new GUIContent("Game Object"));
                            if (check.changed)
                            {
                                if (texture.objectReferenceValue != null && interactableGOProp.objectReferenceValue == null)
                                {
                                    texture.objectReferenceValue = null;
                                }
                                else
                                {
                                    GameObject interactableGO = interactableGOProp.objectReferenceValue as GameObject;
                                    MeshRenderer interactableMesh = interactableGO.GetComponentInChildren<MeshRenderer>();
                                    if (interactableMesh)
                                    {
                                        texture.objectReferenceValue = interactableMesh.sharedMaterial.mainTexture;
                                    }
                                }
                            }
                            using (new EditorGUI.DisabledScope(interactableGOProp.objectReferenceValue != null))
                            {
                                tag.stringValue = EditorGUILayout.TagField(tag.stringValue);
                            }
                        }
                        EditorGUILayout.PropertyField(texture, new GUIContent("Icon"));
                        #endregion
                        
                        SerializedProperty interactions = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("interactions");

                        #region Interactions
                        for (int actionIndex = 0; actionIndex < interactions.arraySize; actionIndex++)
                        {
                            SerializedProperty description = interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("description");
                            showAction[iIndex][actionIndex] = EditorGUILayout.Foldout(showAction[iIndex][actionIndex], actionIndex + ": " + description.stringValue);
                            if (showAction[iIndex][actionIndex])
                            {                                
                                description.stringValue = EditorGUILayout.TextField("Description", description.stringValue);
                                if (description.stringValue.Equals(""))
                                {
                                    description.stringValue = "No description";
                                }

                                SerializedProperty actionSprite = interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("actionSprite");
                                EditorGUILayout.PropertyField(actionSprite, new GUIContent("Action"));

                                int newActionIndex = actionIndex;
                                newActionIndex = EditorGUILayout.DelayedIntField("Priority", newActionIndex);
                                if (newActionIndex != actionIndex && newActionIndex >= 0 && newActionIndex < interactions.arraySize)
                                {
                                    interactions.MoveArrayElement(actionIndex, newActionIndex);
                                    showAction[iIndex][actionIndex] = false;
                                    showAction[iIndex][newActionIndex] = true;
                                }
                                string[] booleansArray = new string[booleans.arraySize];
                                for (int bIndex = 0; bIndex < booleansArray.Length; bIndex++)
                                {
                                    booleansArray[bIndex] = booleans.GetArrayElementAtIndex(bIndex).FindPropertyRelative("name").stringValue;
                                }
                                EditorGUILayout.Separator();

                                #region Storylines
                                if (storylines.arraySize > 0)
                                {
                                    SerializedProperty storylinesData = interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("storylinesData");
                                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                                    {
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            EditorGUILayout.LabelField("", GUILayout.MaxWidth(160f));
                                            GUILayout.FlexibleSpace();
                                            EditorGUILayout.LabelField("Between Stages:", EditorStyles.boldLabel, GUILayout.MaxWidth(205f));
                                            
                                        }
                                        for (int index = 0; index < storylines.arraySize; index++)
                                        {
                                            SerializedProperty storylineData = storylinesData.GetArrayElementAtIndex(index);
                                            SerializedProperty isRequired = storylineData.FindPropertyRelative("isRequired");
                                            SerializedProperty minStage = storylineData.FindPropertyRelative("minStage");
                                            SerializedProperty maxStage = storylineData.FindPropertyRelative("maxStage");
                                            SerializedProperty storyline = storylines.GetArrayElementAtIndex(index);
                                            string label = storyline.FindPropertyRelative("name").stringValue;
                                            int stages = storyline.FindPropertyRelative("stages").intValue;
                                            
                                            
                                            
                                            using (new EditorGUILayout.HorizontalScope())
                                            {
                                                EditorGUILayout.LabelField(label, GUILayout.MaxWidth(120f));
                                                isRequired.boolValue = EditorGUILayout.Toggle(isRequired.boolValue, GUILayout.MaxWidth(40f));

                                                GUILayout.FlexibleSpace();
                                                using (new EditorGUI.DisabledScope(!isRequired.boolValue))
                                                {
                                                    minStage.intValue = EditorGUILayout.DelayedIntField(minStage.intValue, GUILayout.MaxWidth(40f));
                                                    minStage.intValue = minStage.intValue > stages ? stages : minStage.intValue < 0 ? 0 : minStage.intValue;
                                                    maxStage.intValue = maxStage.intValue < minStage.intValue ? minStage.intValue : maxStage.intValue;
                                                    float min = minStage.intValue, max = maxStage.intValue;
                                                    EditorGUILayout.MinMaxSlider(ref min, ref max, 0, stages, GUILayout.MaxWidth(200f));
                                                    minStage.intValue = Mathf.RoundToInt(min);
                                                    maxStage.intValue = Mathf.RoundToInt(max);
                                                    maxStage.intValue = EditorGUILayout.DelayedIntField(maxStage.intValue, GUILayout.MaxWidth(40f));
                                                    maxStage.intValue = maxStage.intValue > stages ? stages : maxStage.intValue < 0 ? 0 : maxStage.intValue;
                                                    minStage.intValue = minStage.intValue > maxStage.intValue ? maxStage.intValue : minStage.intValue;
                                                }
                                            }
                                        }
                                    }
                                }                               
                                #endregion

                                EditorGUILayout.Separator();

                                #region Conditions                                
                                if (booleans.arraySize > 0)
                                {
                                    SerializedProperty conditions = interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("conditions");
                                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                                    {
                                        style.padding.right = 10;
                                        style.alignment = TextAnchor.MiddleCenter;
                                        for (int column = -1; column <= 1; column++)
                                        {
                                            using (new EditorGUILayout.VerticalScope())
                                            {
                                                GUILayout.Space(24f);
                                                for (int index = 0; index < conditions.arraySize; index++)
                                                {
                                                    if (column >= 0)
                                                    {
                                                        SerializedProperty condition = conditions.GetArrayElementAtIndex(index);
                                                        using (new EditorGUI.DisabledScope(condition.intValue != column))
                                                        {
                                                            if (GUILayout.Button(new GUIContent("<"), GUILayout.Height(15f), GUILayout.Width(15f)))
                                                            {
                                                                condition.intValue--;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                                            {
                                                style.fontSize = 17;                                                
                                                string columnLabel = column == -1 ? "False" : column == 0 ? "Ignore" : "True";
                                                EditorGUILayout.LabelField(columnLabel, style, GUILayout.MaxWidth((EditorGUIUtility.currentViewWidth - 180f) / 3f));
                                                style.fontSize = 12;
                                                for (int index = 0; index < conditions.arraySize; index++)
                                                {
                                                    using (new EditorGUILayout.HorizontalScope())
                                                    {
                                                        SerializedProperty condition = conditions.GetArrayElementAtIndex(index);

                                                        string s = "";
                                                        if (condition.intValue == column)
                                                        {
                                                            s = booleans.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue;
                                                        }
                                                        EditorGUILayout.LabelField(s, style, GUILayout.MaxWidth((EditorGUIUtility.currentViewWidth - 180f) / 3f));

                                                    }
                                                }
                                            }
                                            using (new EditorGUILayout.VerticalScope())
                                            {
                                                GUILayout.Space(24f);
                                                for (int index = 0; index < conditions.arraySize; index++)
                                                {
                                                    if (column <= 0)
                                                    {
                                                        SerializedProperty condition = conditions.GetArrayElementAtIndex(index);
                                                        using (new EditorGUI.DisabledScope(condition.intValue != column))
                                                        {
                                                            if (GUILayout.Button(new GUIContent(">"), GUILayout.Height(15f), GUILayout.Width(15f)))
                                                            {
                                                                condition.intValue++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region Sentences and Functions
                                EditorGUILayout.Separator();
                                SerializedProperty nameplates = interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("nameplates");
                                SerializedProperty sentences = interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("sentences");
                                SerializedProperty functions = interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("functions");
                                if (sentences.arraySize == 0)
                                {
                                    EditorGUILayout.PropertyField(functions);
                                }
                                else
                                {
                                    EditorGUILayout.PropertyField(functions, new GUIContent("Functions Before Dialogue"));
                                }
                                for (int sIndex = 0; sIndex < sentences.arraySize; sIndex++)
                                {
                                    SerializedProperty sentenceNameplate = nameplates.GetArrayElementAtIndex(sIndex);
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        EditorGUILayout.PropertyField(sentenceNameplate, new GUIContent("Nameplate"), GUILayout.MaxWidth(120f));
                                        EditorGUILayout.PropertyField(sentences.GetArrayElementAtIndex(sIndex), GUIContent.none);
                                    }
                                }
                                EditorGUILayout.Separator();
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    using (new EditorGUI.DisabledScope(sentences.arraySize <= 0))
                                    {
                                        if (GUILayout.Button(new GUIContent("Remove sentence")))
                                        {
                                            nameplates.arraySize--;
                                            sentences.arraySize--;
                                        }
                                    }
                                    if (GUILayout.Button(new GUIContent("Add sentence")))
                                    {
                                        nameplates.arraySize++;
                                        sentences.arraySize++;
                                        sentences.GetArrayElementAtIndex(sentences.arraySize - 1).stringValue = "";
                                    }
                                }
                                if (sentences.arraySize != 0)
                                {
                                    EditorGUILayout.Separator();
                                    SerializedProperty functionsAfterDialogue = interactions.GetArrayElementAtIndex(actionIndex).FindPropertyRelative("functionsAfterDialogue");
                                    EditorGUILayout.PropertyField(functionsAfterDialogue);
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region Interactions Buttons
                        EditorGUILayout.Separator();
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUI.DisabledScope(interactions.arraySize <= 1))
                            {
                                if (GUILayout.Button(new GUIContent("Remove interaction"), GUILayout.Height(27f)))
                                {
                                    showAction[iIndex][interactions.arraySize - 1] = true;
                                    interactions.arraySize--;
                                }
                            }
                            if (GUILayout.Button(new GUIContent("Add interaction"), GUILayout.Height(27f)))
                            {
                                interactions.arraySize++;
                                if (interactions.arraySize > showAction[iIndex].Length)
                                {
                                    ResizeShowD(iIndex);
                                }
                                interactions.GetArrayElementAtIndex(interactions.arraySize - 1).FindPropertyRelative("nameplates").arraySize = 1;
                                interactions.GetArrayElementAtIndex(interactions.arraySize - 1).FindPropertyRelative("nameplates").GetArrayElementAtIndex(0).stringValue = "";
                                interactions.GetArrayElementAtIndex(interactions.arraySize - 1).FindPropertyRelative("sentences").arraySize = 1;
                                interactions.GetArrayElementAtIndex(interactions.arraySize - 1).FindPropertyRelative("sentences").GetArrayElementAtIndex(0).stringValue = "";
                                interactions.GetArrayElementAtIndex(interactions.arraySize - 1).FindPropertyRelative("description").stringValue = "No description";
                            }
                        }
                        #endregion
                    }
                }
            }
        }
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        #region Interactables Buttons
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(interactables.arraySize <= 1))
            {
                if (GUILayout.Button(new GUIContent("Remove interactable"), GUILayout.Height(36f)))
                {
                    showI[interactables.arraySize - 1] = true;
                    showAction[interactables.arraySize - 1][interactables.GetArrayElementAtIndex(interactables.arraySize - 1).FindPropertyRelative("interactions").arraySize - 1] = true;
                    interactables.arraySize--;
                }
            }
            if (GUILayout.Button(new GUIContent("Add interactable"), GUILayout.Height(36f)))
            {
                interactables.arraySize++;
                if (interactables.arraySize > showI.Length)
                {
                    CopyShowValues();
                }
                if (interactables.arraySize > showAction.Length)
                {
                    ResizeShowD();
                }
                SerializedProperty newInteractable = interactables.GetArrayElementAtIndex(interactables.arraySize - 1);
                newInteractable.FindPropertyRelative("iGameObject").objectReferenceValue = null;
                newInteractable.FindPropertyRelative("tag").stringValue = "Untagged";
                newInteractable.FindPropertyRelative("texture").objectReferenceValue = null;
                SerializedProperty interactions = newInteractable.FindPropertyRelative("interactions");
                interactions.arraySize = 1;
                interactions.GetArrayElementAtIndex(0).FindPropertyRelative("description").stringValue = "No description";
                interactions.GetArrayElementAtIndex(0).FindPropertyRelative("nameplates").arraySize = 1;
                interactions.GetArrayElementAtIndex(0).FindPropertyRelative("nameplates").GetArrayElementAtIndex(0).stringValue = "";
                interactions.GetArrayElementAtIndex(0).FindPropertyRelative("sentences").arraySize = 1;
                interactions.GetArrayElementAtIndex(0).FindPropertyRelative("sentences").GetArrayElementAtIndex(0).stringValue = "";
                SerializedProperty newConditions = interactions.GetArrayElementAtIndex(0).FindPropertyRelative("conditions");
                newConditions.arraySize = booleans.arraySize;
                for (int cIndex = 0; cIndex < newConditions.arraySize; cIndex++)
                {
                    newConditions.GetArrayElementAtIndex(cIndex).intValue = 0;
                }
            }
        }
        #endregion

        serializedObject.ApplyModifiedProperties();
    }

    private void PlusBooleanButton(float maxWidth)
    {
        if (GUILayout.Button(new GUIContent("+"), GUILayout.MaxWidth(maxWidth)))
        {
            booleans.arraySize++;
            booleans.GetArrayElementAtIndex(booleans.arraySize - 1).FindPropertyRelative("name").stringValue = "condition";
            booleans.GetArrayElementAtIndex(booleans.arraySize - 1).FindPropertyRelative("value").boolValue = false;
            for (int iIndex = 0; iIndex < interactables.arraySize; iIndex++)
            {
                SerializedProperty interactions = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("interactions");
                for (int dIndex = 0; dIndex < interactions.arraySize; dIndex++)
                {
                    SerializedProperty conditions = interactions.GetArrayElementAtIndex(dIndex).FindPropertyRelative("conditions");
                    conditions.arraySize++;
                    conditions.GetArrayElementAtIndex(conditions.arraySize - 1).intValue = 0;
                }
            }

        }
    }

    private void PlusStorylineButton(float maxWidth)
    {
        if (GUILayout.Button(new GUIContent("+"), GUILayout.MaxWidth(maxWidth)))
        {
            storylines.arraySize++;
            storylines.GetArrayElementAtIndex(storylines.arraySize - 1).FindPropertyRelative("name").stringValue = "storyline";
            storylines.GetArrayElementAtIndex(storylines.arraySize - 1).FindPropertyRelative("stages").intValue = 2;
            storylines.GetArrayElementAtIndex(storylines.arraySize - 1).FindPropertyRelative("currentStage").intValue = 0;
            for (int iIndex = 0; iIndex < interactables.arraySize; iIndex++)
            {
                SerializedProperty interactions = interactables.GetArrayElementAtIndex(iIndex).FindPropertyRelative("interactions");
                for (int dIndex = 0; dIndex < interactions.arraySize; dIndex++)
                {
                    SerializedProperty storylinesData = interactions.GetArrayElementAtIndex(dIndex).FindPropertyRelative("storylinesData");
                    storylinesData.arraySize++;
                    storylinesData.GetArrayElementAtIndex(storylinesData.arraySize - 1).FindPropertyRelative("isRequired").boolValue = false;
                    storylinesData.GetArrayElementAtIndex(storylinesData.arraySize - 1).FindPropertyRelative("minStage").intValue = 1;
                    storylinesData.GetArrayElementAtIndex(storylinesData.arraySize - 1).FindPropertyRelative("maxStage").intValue = 1;
                }
            }
        }
    }

    private void CopyShowValues()
    {
        bool[] tempArray = new bool[interactables.arraySize];
        tempArray[tempArray.Length - 1] = true;
        System.Array.Copy(showI, tempArray, showI.Length);
        showI = tempArray;
    }

    private void ResizeShowD()
    {
        bool[][] tempArray = new bool[interactables.arraySize][];
        for (int i = 0; i < interactables.arraySize; i++)
        {
            SerializedProperty interactions = interactables.GetArrayElementAtIndex(i).FindPropertyRelative("interactions");
            tempArray[i] = new bool[interactions.arraySize];
        }
        int dLength = tempArray[tempArray.Length - 1].Length;
        tempArray[tempArray.Length - 1][dLength - 1] = true;
        showAction = tempArray;
    }

    private void ResizeShowD(int iIndex)
    {
        bool[][] tempArray = new bool[interactables.arraySize][];
        for (int i = 0; i < interactables.arraySize; i++)
        {
            SerializedProperty interactions = interactables.GetArrayElementAtIndex(i).FindPropertyRelative("interactions");
            tempArray[i] = new bool[interactions.arraySize];
        }
        int dLength = tempArray[iIndex].Length;
        tempArray[iIndex][dLength - 1] = true;
        showAction = tempArray;
    }
}
#endif