using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCursor : MonoBehaviour {

    // Use this for initialization
    public Camera AimCam;
    private RaycastHit hitaim;

    public Vector3 teleportposition;
    private GameObject player;

	void Start () {

        player = GameObject.FindGameObjectWithTag("Player");
        AimCam = GameObject.FindGameObjectWithTag("AimCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {


      
            Ray ray = AimCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray , out hitaim,10f))
            {
                this.transform.position = hitaim.point ;

            if (hitaim.point != null)
            {
                if (Input.GetKeyDown(KeyCode.K))
                {

                    teleportposition = hitaim.point;
                    player.gameObject.SendMessageUpwards("ShadowPort", teleportposition);
                }
            }
           
               
              
            }
            
        
      
       
	}
}
