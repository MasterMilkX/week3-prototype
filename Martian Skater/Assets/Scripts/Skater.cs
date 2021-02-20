using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skater : MonoBehaviour
{

	//controls
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode jumpKey = KeyCode.Z;
    public KeyCode actionKey = KeyCode.X;

    //character controls
    private SpriteAnimator sprAnim;
    private Rigidbody2D rb;

    //sprite direction
    public string dir = "right";
    public int moveInput = 0;

    //speed
    public float acceleration = 0.25f;
    public float maxSpeed = 5.0f;
    public Vector2 curVelocity = Vector2.zero;

    //jump 
    public float maxJumpForce = 10.0f;
    
    //game camera control
    public Transform cam;
    public Transform airHeight;				//max air height point for the camera to start following the skater upward
    public Vector3 camOffset;

    // Start is called before the first frame update
    void Start()
    {
        sprAnim = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(curVelocity * Time.deltaTime);

        //set direction
        if(Input.GetKey(leftKey)){
        	moveInput = -1;
        }
       	else if(Input.GetKey(rightKey)){
       		moveInput = 1;
       	}else{
       		moveInput = 0;
       	}

       	//acceleration/deceleration
       	if (moveInput != 0){
			curVelocity.x = Mathf.MoveTowards(curVelocity.x, maxSpeed * moveInput, acceleration * Time.deltaTime);
		}
		else{
			curVelocity.x = Mathf.MoveTowards(curVelocity.x, 0, acceleration * Time.deltaTime);
		}
       	//curVelocity.x = Mathf.MoveTowards(curVelocity.x, maxSpeed * moveInput, acceleration * Time.deltaTime);

        //ollie
        if(Input.GetKeyDown(jumpKey) && rb.velocity.y == 0){
        	Debug.Log("jump around!");
        	rb.AddForce(new Vector2(0,maxJumpForce));
        }


        //camera movement
        if(transform.position.y > airHeight.position.y){	//if above the max height, follow x + y
        	cam.position = new Vector3(transform.position.x, transform.position.y,camOffset.z);
        }else{												//otherwise just follow as normal
        	cam.position = new Vector3(transform.position.x+camOffset.x, camOffset.y,camOffset.z);
        }

    }

    //makes the skater ollie higher based on how long the key was held down
    void Ollie(){

    }

}
