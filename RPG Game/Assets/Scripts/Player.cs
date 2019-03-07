using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Use this for initialization


    public bool debugfactor;
    public float debugrad;

    private Animator anim;
    private Rigidbody rigi;
    private CapsuleCollider bodycollider;
    public float speed;

    //jump
    public Vector3 moveVector;

    //Camera Follow

    private float heading;
    public Transform CameraArm;
    public Transform Camera;
    public Transform Playerpivot;


    //Ground Check
    public GameObject Groundcheck;
    private RaycastHit groundhit;
    private bool Grounded;
    public float jumpfactor;
    private bool jumplock = false;
    private float jumpspeed;
    public bool isjump;

  


    //hanging/climbing 

    public bool IsLedge;
    public GameObject climbinghelper;
    private RaycastHit climbhit;
    private Vector3 climbpoint;
    private bool IsClimbing;
    public GameObject helperprefab;
    private GameObject climbobj;

    //zipline
    public float zipdist;
    private bool onZipStart;
    private bool Zipping;
    [SerializeField]
    private GameObject currentZipline;
    [SerializeField]
    private GameObject startZippoint;
    [SerializeField]
    private GameObject endZippoint;

    //ladder
    private bool ladder;

    //jump while climbing
    public GameObject targetjump;
    //left
    public GameObject climbLhand;
    private RaycastHit climblHit;
    [SerializeField]
    private GameObject climbLtarget;
    [SerializeField]
    private Vector3 ltargetpos;
    //right
    public GameObject climbRhand;
    private RaycastHit climbrHit;
    [SerializeField]
    private GameObject climbRtarget;
    [SerializeField]
    private Vector3 rtargetpos;
    //current ledge position
    //if the player is on the same ledge as the target then dont jump
    [SerializeField]
    private GameObject currentLedge;



    //Camera
    public GameObject CamMain;
    public GameObject CamAim;



    //IK!////

    public Transform righthand;
    public Transform lefthand;
    public bool useIK;
    private Vector3 leftpos;
    private Vector3 rightpos;
    private RaycastHit leftout;
    private RaycastHit rightout;


    //performance
    private bool switchcounter;
    private float jumpdist;
    private float jumptime;

    void Start () {

        anim = this.GetComponent<Animator>();
        rigi = this.GetComponent<Rigidbody>();
        bodycollider = this.GetComponent<CapsuleCollider>();
       
      
	}

    private void LateUpdate()
    {
        //jumping update
        if (isjump)
        {
            jumpfactor += Time.deltaTime;
            
        }

        if(jumpfactor >= 0.5f)
        {
            isjump = false;

        }
        //end of jumping update

        //zipline input update
        if (onZipStart)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //start zipping!
                Zipping = true;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate () {

        //input for movement

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ground"))
        {

            //Code for teleport
            ShadowMove();


            useIK = false;
            bodycollider.enabled = true;
            if (!IsLedge)
            {
                anim.SetBool("Ledge", false);
                rigi.useGravity = true;
             
            }

            
          
            
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
        {

            if (IsLedge && !isjump)
            {
                anim.SetFloat("JumpFactor", 2);
            }
            if (IsLedge)
            {
                rigi.useGravity = false;
                bodycollider.enabled = false;

            }

            

            if (!isjump)
            {
                anim.SetFloat("JumpFactor", 0);
                bodycollider.enabled = true;
                rigi.useGravity = true;
            }
           
            if (!IsLedge)
            {
                anim.SetBool("Ledge", false);
                anim.SetFloat("JumpFactor", 0);
                rigi.useGravity = true;
                bodycollider.enabled = true;
                
            }

            if (isjump)
            {
                anim.SetFloat("JumpFactor", 1);
            }
        }
      
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hanging"))
        {
            rigi.useGravity = false;
            jumpspeed = 0;
            isjump = false;
            bodycollider.enabled = false;

            if (!IsLedge)
            {
                bodycollider.enabled = true;
            }

            AssignLeftClimbTarget();
            AssignRightClimbTarget();
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LeftFly"))
        {
            rigi.useGravity = false;
            jumpspeed = 0;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Zipline"))
        {
            rigi.useGravity = false;
            bodycollider.enabled = false;
        }

        GroundCheck();

        if (!IsLedge || Grounded)
        {
            GroundMovement();
        }

      
       
       
        AirMovement();

        if (IsLedge)
        {
            jumpspeed = 0f;
            Climbing();
            ClimbingMovement();
        }

        if (switchcounter)
        {
         

            jumptime += Time.deltaTime;
            if(jumptime >= anim.GetCurrentAnimatorStateInfo(0).length - 0.743f)
            {
                bodycollider.enabled = true;
                //climbcollider.enabled = true;
                switchcounter = false;
            }
           
        }

        //zipping
        if (Zipping)
        {
            anim.SetBool("UseZipline", true);
            UseZipline();
        }
    }

    void UseZipline()
    {
        //finding start and endpoints
        //zipline first child is has a child called start point

        if (Zipping)
        {
            startZippoint = currentZipline.transform.GetChild(0).GetChild(0).gameObject;
            endZippoint = currentZipline.transform.GetChild(1).GetChild(0).gameObject;

            //now make the player go from start point to end point
            this.transform.position = Vector3.MoveTowards(this.transform.position, endZippoint.transform.position, 2f * Time.deltaTime);
            zipdist = Vector3.Distance(this.transform.position, endZippoint.transform.position);
            if (zipdist < 0.8f)
            {
                Zipping = false;
                anim.SetBool("UseZipline", false);
            }



        }
    }

    //The code for shadow movement / teleport

    void ShadowMove()
    {
        
        if (Input.GetMouseButton(1))
        {
            Debug.Log("Shadow mode on!");
            CamMain.SetActive(false);
            CamAim.SetActive(true);
        }

        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Shadow Mode off");
            CamMain.SetActive(true);
            CamAim.SetActive(false);
        }
    }

    public void ShadowPort(Vector3 teleposition)
    {
        Debug.Log("Player is teleporting");
        this.transform.position = teleposition;
    }

    void ClimbingMovement()
    {
        //press a for left shimmy
        //press d for right shimmy
        //press w for climb

        // moveVector = new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * 0.5f, 0, 0);
        //this.transform.Translate(moveVector);
        //anim.SetFloat("Hang", Input.GetAxis("Horizontal"));

        if (Input.GetKeyDown(KeyCode.S))
        {
            rigi.useGravity = true;
            anim.SetTrigger("HangDrop");
        }


        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(climbLtarget == null)
            {
                Debug.Log("No target");
                return;
            }
            anim.SetTrigger("FlyLeft");
           // useIK = false;
           // switchcounter = true;

        }
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            if (climbRtarget == null)
            {
                Debug.Log("No target");
                return;
            }
            anim.SetTrigger("FlyRight");
          //  useIK = false;
           // switchcounter = true;

        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LeftFly"))
        {
            if (climbLtarget)
            {
           
            
                 anim.MatchTarget(climbLtarget.transform.position, climbLtarget.transform.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0), 0.375f, 0.72f);
                this.transform.LookAt(climbLtarget.transform.forward);
            }

        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("RightFly"))
        {
            if(climbRtarget)
            {
                anim.MatchTarget(climbRtarget.transform.position, climbRtarget.transform.rotation, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), 0.375f, 0.72f);
                this.transform.forward = climbRtarget.transform.forward;
            }
        }

       // if (switchcounter)
       // {
            //climbcollider.enabled = false;
        //    bodycollider.enabled = false;
      //  }
    }
    /// <summary>
    /// This code is for determining the target for left fly.
    /// </summary>
    void AssignLeftClimbTarget()
    {
        Debug.DrawRay(climbLhand.transform.position, -transform.right, Color.red);
        if(Physics.SphereCast(climbLhand.transform.position,0.5f,-transform.right,out climblHit, 1.5f))
        {

            if (climblHit.collider.gameObject.tag == "Ledge")
            {
                if (GameObject.ReferenceEquals(currentLedge, climblHit.collider.gameObject))
                {
                    Debug.Log("Same left object");
                    climbLtarget = null;
                    return;
                }
                else
                {
                    //then its our target
                    Debug.Log(climblHit.collider.gameObject.name);
                    climbLtarget = climblHit.collider.gameObject;
                    ltargetpos = climbLtarget.transform.position;
                }
                
            }
            else
            {
                climbLtarget = null;
            }
        }
       
    }
    /// <summary>
    /// This code is for determining the target for right fly.
    /// </summary>
    void AssignRightClimbTarget()
    {
        if (Physics.SphereCast(climbRhand.transform.position, 0.5f, transform.right, out climbrHit, 1.5f))
        {

            if (climbrHit.collider.gameObject.tag == "Ledge")
            {
                if (GameObject.ReferenceEquals(currentLedge, climbrHit.collider.gameObject))
                {
                    Debug.Log("Same Right object");
                    climbRtarget = null;
                    return;
                }
                else
                {
                    climbRtarget = climbrHit.collider.gameObject;
                    rtargetpos = climbRtarget.transform.position;

                }
                   
              
              
            }
            else
            {
                climbRtarget = null;
            }
        
    }
     
    }

    /// <summary>
    /// This code is for air movement only
    /// </summary>
    void AirMovement()
    {

       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //jump
            //jump limit
            if (Grounded) {
                
                isjump = true;
                jumpspeed = 4f;
                anim.SetFloat("JumpFactor", 1);
                anim.SetBool("IsGrounded", false);
            }
          
          

            if (isjump)
            {
                jumpfactor += Time.deltaTime;
            }

            if (jumpfactor >= 0.4f)
            {
                jumplock = true;
            }

            if (IsLedge)
            {
                jumpspeed = 0;
            }
            
        }


        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpspeed = 0;
            if (IsLedge)
            {
              

            }

            if (!IsLedge)
            {
                //stop jumping
                Debug.Log("No ledge");
                jumpspeed = 0f;
                
                anim.SetFloat("JumpFactor", 0);
                rigi.useGravity = true;
            }
          
        }

        if (jumplock == true)
        {
            Debug.Log("Jump lock activated");
            jumpspeed = 0f;
            anim.SetFloat("JumpFactor", 0f);
        }

       
    }

    void GroundCheck()
    {
        if(Physics.Raycast(Groundcheck.transform.position,-transform.up,out groundhit, 0.027f))
        {
            Grounded = true;
            anim.SetBool("IsGrounded", Grounded);
           
        
        }

       
     
    }



    /// <summary>
    /// IK Code
    /// </summary>
    /// 

    void IKUsage()
    {
        if (useIK)
        {
            if(Physics.Raycast(righthand.position,-transform.up,out rightout, 1f))
            {
                if(rightout.collider.gameObject.tag == "Ledge")
                {
                    rightpos = rightout.point;
                }
            }

            if(Physics.Raycast(lefthand.position,-transform.up,out leftout, 1f))
            {
                if(leftout.collider.gameObject.tag == "Ledge")
                {
                    leftpos = leftout.point;
                }
            }
        }
    }

    private void OnAnimatorIK()
    {
        if (useIK)
        {
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightpos);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.7f);

            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftpos);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.7f);

            

        }
    }



    void Climbing()
    {

        if(Physics.Raycast(climbinghelper.transform.position,-transform.up,out climbhit, 1f))
        {
            Debug.Log("Climb point found");
            climbpoint = climbhit.point;

            
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (climbobj == null)
            {
               climbobj =  Instantiate(helperprefab, climbpoint, Quaternion.identity);
            }
            useIK = false;
            anim.SetTrigger("Climb");


        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Climbing"))
        {
            useIK = false;
            //capsule collider false
            bodycollider.enabled = false;
            IsClimbing = true;
            anim.MatchTarget(climbobj.transform.position, climbobj.transform.rotation, AvatarTarget.Root , new MatchTargetWeightMask(Vector3.one, 0), 0.301f, 0.715f);
             Destroy(climbobj, anim.GetCurrentAnimatorStateInfo(0).length);
            isjump = false;


            IsLedge = false;
           
        }

       

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ledge")
        {
            
          

            if (!Grounded) {

                  Debug.Log("Hit ledge" + collision.gameObject.name);
                  IsLedge = true;
                  anim.SetBool("Ledge", IsLedge);
              // useIK = true;
                  rigi.useGravity = false;
                  jumpspeed = 0;

                  IKUsage();
              }
              if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hanging"))
              {
                  IsLedge = true;
                  anim.SetBool("Ledge", IsLedge);
                  rigi.useGravity = true;
                  jumpspeed = 0;
                currentLedge = collision.gameObject;
                AssignLeftClimbTarget();
                AssignRightClimbTarget();
              }
              if (anim.GetCurrentAnimatorStateInfo(0).IsName("LeftFly"))
              {
                  IsLedge = true;
                  anim.SetBool("Ledge", IsLedge);
                  rigi.useGravity = true;
                  jumpspeed = 0;
              }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("RightFly"))
            {
                IsLedge = true;
                anim.SetBool("Ledge", IsLedge);
                rigi.useGravity = true;
                jumpspeed = 0;
            }
          

        }
        if(collision.gameObject.tag == "Ground")
        {
            Grounded = true;
            jumpfactor = 0f;
            anim.SetBool("IsGrounded", Grounded);
        }
      
        if(collision.gameObject.tag == "Ladder")
        {
            ladder = true;
            rigi.useGravity = false;
            Grounded = false;
            anim.SetBool("Ladder", true);
        }

     
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Grounded = false;
            anim.SetBool("IsGrounded", Grounded);
            if(jumpspeed <= 0f)
            {
                anim.SetFloat("JumpFactor", 0);
            }
        }

        if (collision.gameObject.tag == "Ledge")
        {
            IsLedge = false;
            anim.SetBool("Ledge", IsLedge);
            currentLedge = null;
            climbRtarget = null;
            climbLtarget = null;

           useIK = false;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Zipline")
        {
            currentZipline = other.gameObject;
            onZipStart = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Zipline")
        {
            
            if (!Zipping)
            {
                currentZipline = null;
                startZippoint = null;
                endZippoint = null;
                onZipStart = false;
            }
           
            
        }
    }

    void GroundMovement()
    {
        if (Grounded)
        {
            jumplock = false;
           // rigi.useGravity = true;
        }

        heading += Input.GetAxis("Mouse X") * Time.deltaTime * 180;
        CameraArm.rotation = Quaternion.Euler(0, heading, 0);

        Vector3 camF;
        Vector3 camR;
        
        camF = Camera.forward;
        camR = Camera.right;

        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;
        CameraArm.transform.position = this.transform.position;

        this.transform.LookAt(Playerpivot);
        this.transform.position += (camF * Input.GetAxis("Vertical") + camR * Input.GetAxis("Horizontal")) * Time.deltaTime * speed;
        moveVector.x = 0;
        moveVector.z = 0;
        if (jumpspeed == 0)
        {
            moveVector.y = 0;
        }
        if(jumpspeed == 4)
        {
            moveVector.y = jumpspeed * Time.deltaTime;
        }
        this.transform.Translate(moveVector);
        anim.SetFloat("Speed", Input.GetAxis("Vertical"));
        anim.SetFloat("Direction", Input.GetAxis("Horizontal"));
    }

   

   

    private void OnDrawGizmosSelected()
    {
        if (debugfactor)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(climbLhand.transform.position, debugrad);
        }
    }


}
