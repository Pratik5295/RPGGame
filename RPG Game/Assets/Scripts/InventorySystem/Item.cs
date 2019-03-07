using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject {


    public enum TypeOfObject
    {
        Weapon,
        Potion,
        Powers
    }

    public TypeOfObject ItemType;

    public string name;

    public int quantity;

    public Sprite icon_image;


    public virtual void Use()
    {
        Debug.Log("You are using the item ");
    }

}
