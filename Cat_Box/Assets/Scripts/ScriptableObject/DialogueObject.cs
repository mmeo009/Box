using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueObject", menuName = "ScriptableObject/DialogObject")]
public class DialogueObject : ScriptableObject
{
    public List<string> dialogues = new List<string>();
}
