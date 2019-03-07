using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {


    public Item item;
	// Use this for initialization

    public Image icon_image;

	void Start () {

        icon_image = this.transform.Find("Image").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateSlot()
    {
        if (item)
        {
            item.Use();
            icon_image.sprite = item.icon_image;
        }
        else
        {
            icon_image.sprite = null;
        }
    }

    public void Icon_Stick()
    {

    }

}
