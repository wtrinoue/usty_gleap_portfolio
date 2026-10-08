using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ComponentRegister", menuName = "Usty/Component Register")]
public class ComponentRegister : ScriptableObject
{
    public List<ComponentRegisterScript> scripts = new List<ComponentRegisterScript>();
}

[Serializable]
public class ComponentRegisterScript
{
    public ComponentCategory category;
    public MonoScript script;
}

public enum ComponentCategory
{
    CharacterComponent,
}
