using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Weapon" , menuName = "Weapons")]
public class Weapon_Sword : Item
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Use()
    {
        Debug.Log("Sword is selected and use! ");
    }
}
