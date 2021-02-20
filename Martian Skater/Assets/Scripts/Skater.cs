using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Vector2 velocity = Vector2.zero;

    //jump 
    public float maxJumpForce = 10.0f;
    public float ollieTime = 0.0f;
    public float tdiff = 0.0f;
    private float maxJumpTime = 0.75f;
    
    //game camera control
    public Transform cam;
    public Transform airHeight;				//max air height point for the camera to start following the skater upward
    public Vector3 camOffset;

    //ui
    public GameObject jumpBarUI;
    private Image jumpBar;

    // Start is called before the first frame update
    void Start()
    {
        sprAnim = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();

        //set ui
        jumpBarUI.SetActive(false);
        jumpBar = jumpBarUI.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    	/*
        transform.Translate(curVelocity * Time.deltaTime);

        if(rb){
        	rb.velocity = new Vector2(curVelocity.x,rb.velocity.y);
        }else if(cc){
        	cc.Move(new Vector2(moveInput,,0) * Time.deltaTime * maxSpeed);
        }
        

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
		*/

		/*
		//set acceleration and speed
		if(Input.GetKey(leftKey) && velocity.x > -maxSpeed){
			velocity.x += -acceleration*Time.deltaTime;
		}else if(Input.GetKey(rightKey) && velocity.x < maxSpeed){
			velocity.x += acceleration*Time.deltaTime;
		}

		//cap it
		if(velocity.x > maxSpeed){velocity.x = maxSpeed;}
		else if(velocity.x < -maxSpeed){velocity.x = -maxSpeed;}


		//decelerate
		else if(!Input.GetKey(leftKey) && !Input.GetKey(downKey) && velocity.y == 0){
			if(velocity.x > 0){
				velocity.x -= acceleration*Time.deltaTime;
			}else if(velocity.x < 0){
				velocity.x -= -acceleration*Time.deltaTime;
			}

			//hit a wall just reset it
			if(rb.velocity.x == 0){
				velocity.x = 0;
			}
		}



		if(cc){
			cc.Move(velocity*Time.deltaTime);
		}else if(rb){
			if(Mathf.Abs(rb.velocity.x) < maxSpeed){
				rb.AddForce(velocity);
			}
		}
		*/

		/*
		if(Input.GetKey(rightKey) && rb.velocity.x < maxSpeed){
			rb.AddForce(new Vector2(maxSpeed,0));
		}
		if(Input.GetKey(leftKey) && rb.velocity.x > -maxSpeed){
			rb.AddForce(new Vector2(-maxSpeed,0));
		}
		*/

		/*
		float move = 0;
		if(Input.GetKey(leftKey)){
			move = -maxSpeed;
		}else if(Input.GetKey(rightKey)){
			move = maxSpeed;
		}
		rb.velocity = new Vector2(move, rb.velocity.y);
		*/

		//flip direction
		if(rb.velocity.x < 0){
			dir = "left";
			GetComponent<SpriteRenderer>().flipX = true;
		}else if(rb.velocity.x > 0){
			dir = "right";
			GetComponent<SpriteRenderer>().flipX = false;
		}


        //ollie code to allow jumping
        Ollie();


       
       	/*
        if(Input.GetKeyUp(jumpKey) && rb.velocity.y == 0){
        	rb.AddForce(new Vector2(0,maxJumpForce));
        }
		*/

        //camera movement
        if(transform.position.y > airHeight.position.y){	//if above the max height, follow x + y
        	cam.position = new Vector3(transform.position.x, transform.position.y,camOffset.z);
        }else{												//otherwise just follow as normal
        	cam.position = new Vector3(transform.position.x+camOffset.x, camOffset.y,camOffset.z);
        }

    }

    //makes the skater ollie higher based on how long the key was held down
    void Ollie(){
    	//start of ollie
    	if(Input.GetKey(jumpKey) && rb.velocity.y == 0){
    		//initialize it
    		if(ollieTime == 0.0f){
        		ollieTime = Time.time;
        		jumpBarUI.SetActive(true);
        		jumpBar.rectTransform.sizeDelta = new Vector2(jumpBar.rectTransform.sizeDelta.x,0.0f);
    		}
    		//get time held down the button
    		else{
    			tdiff = Time.time - ollieTime;	
    			if(tdiff >= maxJumpTime){			//cap the bar
    				jumpBar.color = new Color32(0,225,0,100);
    				jumpBar.rectTransform.sizeDelta = new Vector2(jumpBar.rectTransform.sizeDelta.x,0.4f);
    			}else{								//otherwise grow it
    				float p = (tdiff/maxJumpTime);
    				jumpBar.color = new Color32(255,0,0,100);
    				jumpBar.rectTransform.sizeDelta = new Vector2(jumpBar.rectTransform.sizeDelta.x,0.4f*p);
    			}
    		}
        }

        //released 
        if(!Input.GetKey(jumpKey) && ollieTime != 0.0f){
        	float jump = (tdiff > maxJumpTime ? 1.0f : (tdiff / maxJumpTime));		//use percentage to set jump
        	rb.AddForce(new Vector2(0,jump*maxJumpForce));
        	ollieTime = 0.0f;
        	jumpBarUI.SetActive(false);
        }
    }

}
