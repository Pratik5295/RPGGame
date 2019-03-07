using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour {

    // Use this for initialization

    public Transform target;
    private Animator anim;
    public Transform RightHand;
    private RaycastHit rhit;
    private Vector3 rhpos;
    public bool righthit;
    public bool useIK = false;

	void Start () {

        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        //move hand with target

        IKPro();
	}

    void IKPro()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            useIK = !useIK;
        }


        if (useIK)
        {
            if (Physics.Raycast(RightHand.transform.position,transform.forward,out rhit,0.5f))
            {
                if(rhit.collider.gameObject.tag == "Check")
                {
                    righthit = true;
                    rhpos = rhit.point;
                }
             
            }

            else
            {
                righthit = false;

            }
        }
    }

    void OnAnimatorIK()
    {
        if (useIK)
        {
            anim.SetIKPosition(AvatarIKGoal.RightHand, target.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }
    }


   

    
}
