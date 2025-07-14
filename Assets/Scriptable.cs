using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global", menuName = "ScriptableObjects/Global", order = 1)]
public class Scriptable : ScriptableObject
{
    public bool isSpecialMode;
    public Player _player;
}
