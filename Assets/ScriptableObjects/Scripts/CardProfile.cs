using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/ScriptableObjects/Cards", menuName = "Card")]
public class CardProfile : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
}
