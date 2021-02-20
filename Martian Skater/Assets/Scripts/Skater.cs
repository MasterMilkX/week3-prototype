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

    //speed
    public float acceleration = 0.25f;
    public float maxSpeed = 5.0f;
    public Vector2 velocity = Vector2.zero;

    //jump 
    public float maxJumpForce = 10.0f;
    public float ollieTime = 0.0f;
    public float tdiff = 0.0f;
    private float maxJumpTime = 0.75f;

    //other tricks
    public bool inTrick = false;
    public bool grinding = false;
    
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

		//accelerate
		if(Input.GetKey(rightKey)){
			if(velocity.x < maxSpeed){
				velocity.x += acceleration*Time.deltaTime;
			}
		}if(Input.GetKey(leftKey)){
			if(velocity.x > -maxSpeed){
				velocity.x -= acceleration*Time.deltaTime;
			}
		}
		//decelerate
		if(!Input.GetKey(leftKey) && !Input.GetKey(rightKey) && rb.velocity.y == 0){
			if(velocity.x > 0){
				velocity.x -= acceleration*Time.deltaTime;
			}else if(velocity.x < 0){
				velocity.x += acceleration*Time.deltaTime;
			}

			//hit a wall just reset it
			if(rb.velocity.x == 0){
				velocity.x = 0;
			}
		}

		velocity.y = rb.velocity.y;
		rb.velocity = velocity;

		

		//flip direction
		if(rb.velocity.x < -0.5f){
			dir = "left";
			transform.eulerAngles = new Vector3(0,180,0);
			//GetComponent<SpriteRenderer>().flipX = true;
		}else if(rb.velocity.x > 0.5f){
			dir = "right";
			transform.eulerAngles = new Vector3(0,0,0);
			//GetComponent<SpriteRenderer>().flipX = false;
		}


        //ollie code to allow jumping
        Ollie();


        //allow tricks
        if(inTrick && sprAnim.curAnim.name == "normal"){
        	inTrick = false;
        }

        //test tricks
        if(!sprAnim.animating){sprAnim.animating = true;}
        
        //air tricks
        if(rb.velocity.y != 0){
        	if(Input.GetKeyDown(actionKey)){
        		if(Input.GetKey(upKey)){DoTrick("kickflip");}
        		else if(Input.GetKey(downKey)){DoTrick("360_flip");}
        		
        	}
        }
        //ground tricks
        else{
        	if(Input.GetKey(actionKey)){
        		if(grinding){
        			sprAnim.PlayAnim("5-0_grind");
        		}
        	}else if(Input.GetKey(downKey)){
	        	sprAnim.PlayAnim("manual");
	        }else if(Input.GetKey(upKey)){
	        	sprAnim.PlayAnim("nose_manual");
	        }else{
	        	sprAnim.PlayAnim("normal");
	        }
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
        	rb.AddForce(new Vector2(rb.velocity.x,jump*maxJumpForce));
        	ollieTime = 0.0f;
        	jumpBarUI.SetActive(false);
        }
    }

    //perform a trick on the board
    void DoTrick(string trick){
    	if(!inTrick){
    		sprAnim.PlayAnimOnce(trick);
    		inTrick = true;
    	}
    	
    }

    //check for specific object collisions
    void OnCollisionEnter2D(Collision2D c){
    	if(c.gameObject.tag == "rail"){
    		grinding = true;
    	}

    	if(inTrick && sprAnim.curAnim.name.Contains("flip")){
    		Debug.Log("failed!");
    		inTrick = false;
    	}
    }
    void OnCollisionExit2D(Collision2D c){
    	if(c.gameObject.tag == "rail"){
    		grinding = false;
    	}
    }

}
