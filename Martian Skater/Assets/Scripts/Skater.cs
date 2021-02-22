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
    public string dir = "right";			//direction the player is facing

    //speed
    public float acceleration = 0.25f;		//acceleration of the player
    public float maxSpeed = 5.0f;			//maximum horizontal speed of the player
    public Vector2 velocity = Vector2.zero;	//x axis movement velocity

    //jump 
    public float maxJumpForce = 10.0f;		//amount of force to jump in the air and do an ollie
    public float ollieTime = 0.0f;			//current time button was held down to start the ollie
    public float tdiff = 0.0f;				//difference in time between first held down ollie button and current time
    public float maxJumpTime = 0.65f;		//time needed to hold down the ollie button for maximum effect

    //other tricks
    public bool inTrick = false;			//check if currently in the middle of an air trick 
    public bool grinding = false;			//check if currently grinding
    private string lockedGrind = "";		//locked in animation grind
    public string combo = "";				//current combo text 
    public int comboVal = 0;				//current combo value
    public int highScore = 0;				//high score for the session
    private float comboTime = 0;			//time to release the combo
    private float comboGrace = 0.3f;		//grace period to string combos together
    private int comboMultiplier = 1;		//combo multiplier value
    private bool cancelCombo = false;		//check if combo cancellation in progress (neutral idle state)
    private bool addContCombo = false;		//added continous combo text (manual, grind) to trick combo string
    
    //game camera control
    public Transform cam;					//camera that follows the player
    public Transform airHeight;				//max air height point for the camera to start following the skater upward
    public Vector3 camOffset;				//positional offset the camera from the player

    //jump ui
    public GameObject jumpBarUI;			//whole ui for the jump bar
    private Image jumpBar;					//actual jump bar image

    //trick ui
    public GameObject cameraUI;				//game object for the camera ui 
    public Text comboText;					//ui text for the combo string
    public Text pointText;					//ui text for the point system
    public Text highScoreText; 				//ui text for the high score system

    // Start is called before the first frame update
    void Start()
    {
        sprAnim = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();

        //set ui
        cameraUI.SetActive(true);
        jumpBarUI.SetActive(false);
        jumpBar = jumpBarUI.GetComponent<Image>();

        //set high score
        if(PlayerPrefs.HasKey("highscore")){
            highScore = PlayerPrefs.GetInt("highscore");
            highScoreText.text = "High Score: " + highScore.ToString();
            Debug.Log("High score not found!");
        }else{
            highScore = 0;
            PlayerPrefs.SetInt("highscore", highScore);
        }
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
		if(!Input.GetKey(leftKey) && !Input.GetKey(rightKey) && System.Math.Round(rb.velocity.y,3) == 0){
			if(velocity.x > 0){
				velocity.x -= acceleration*Time.deltaTime;
			}else if(velocity.x < 0){
				velocity.x += acceleration*Time.deltaTime;
			}

			//hit a wall just reset it
			if(System.Math.Round(rb.velocity.x,3) == 0){
				velocity.x = 0;
			}
		}

		//APPLY VELOCITIES
		velocity.y = rb.velocity.y;
		rb.velocity = velocity;
		

		//flip direction
		if(rb.velocity.x < -0.25f){
			dir = "left";
			transform.eulerAngles = new Vector3(0,180,0);
			//GetComponent<SpriteRenderer>().flipX = true;
		}else if(rb.velocity.x > 0.25f){
			dir = "right";
			transform.eulerAngles = new Vector3(0,0,0);
			//GetComponent<SpriteRenderer>().flipX = false;
		}


        //ollie code to allow jumping
        Ollie();

        //show animations
        if(!sprAnim.animating){sprAnim.animating = true;}
        
        //air tricks
        if(System.Math.Round(rb.velocity.y,3) != 0){
        	if(Input.GetKeyDown(actionKey)){
        		if(Input.GetKey(upKey)){DoTrick("kickflip",30);}
        		else if(Input.GetKey(downKey)){DoTrick("360_flip",50);}
        	}
        	else if(!inTrick){
        		sprAnim.PlayAnim("normal");
        	}
        }
        //ground tricks
        else{
        	if(Input.GetKey(actionKey) && System.Math.Round(rb.velocity.x,3) != 0){		//5-0 grind 
        		if(grinding){
        			if(lockedGrind == ""){lockedGrind = "5-0_grind";}
					AddToComboContinuous("5-0 Grind","green");
        		}
				//inTrick = true;
        	}else if(Input.GetKey(downKey) && System.Math.Round(rb.velocity.x,3) != 0){				//manuals and tail slide
        		if(grinding){
        			if(lockedGrind == ""){lockedGrind = "manual";}
		        	AddToComboContinuous("Tail Slide", "green");
        		}else{
		        	sprAnim.PlayAnim("manual");
		        	AddToComboContinuous("Manual");
		        }
	        	//inTrick = true;
	        }else if(Input.GetKey(upKey) && System.Math.Round(rb.velocity.x,3) != 0){					//nose manual and nose slide
	        	if(grinding){
	        		if(lockedGrind == ""){lockedGrind = "nose_manual";}
	        		AddToComboContinuous("Nose Slide", "green");
	        	}else{
	        		sprAnim.PlayAnim("nose_manual");
	        		AddToComboContinuous("Nose Manual");
	        	}
	        	//inTrick = true;
	        }

	        if(!Input.GetKey(downKey) && !Input.GetKey(upKey)){		//default		
	        	if(grinding && System.Math.Round(rb.velocity.x,3) != 0){
	        		if(lockedGrind == ""){lockedGrind = "normal";}
	        		AddToComboContinuous("50-50 Grind","green");
        		}else{
        			sprAnim.PlayAnim("normal");
		        	inTrick = false;
		        	if(!cancelCombo && comboVal > 0){		//set timer to cancel the combo
		        		comboTime = Time.time;
		        		cancelCombo = true;
		        		addContCombo = false;
		        	}else if(cancelCombo && (comboVal > 0) && ((Time.time - comboTime) > comboGrace)){		//cancel the combo if given a certain time
		        		FinishCombo();
		        	}
        		}
	        	
	        }
        }

        //set the grind animation
        if(grinding && lockedGrind != ""){
        	sprAnim.PlayAnim(lockedGrind);
        }else if(!grinding && lockedGrind != ""){
        	lockedGrind = "";
        }


        //camera movement
        if(transform.position.y > airHeight.position.y){	//if above the max height, follow x + y
        	cam.position = new Vector3(transform.position.x, transform.position.y,camOffset.z);
        }else{												//otherwise just follow as normal
        	cam.position = new Vector3(transform.position.x+camOffset.x, camOffset.y,camOffset.z);
        }


        //reset
        if(Input.GetKeyDown(KeyCode.R)){
        	transform.position = airHeight.position;
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
        	AddToCombo("Ollie",10);
        }
    }

    //perform a trick on the board
    void DoTrick(string trick,int pts){
    	if(!inTrick){
    		sprAnim.PlayAnimOnce(trick);
    		inTrick = true;
    		string tname = char.ToUpper(trick[0]) + trick.Substring(1).Replace('_',' ');
    		AddToCombo(tname,pts,"red");
    	}
    }


    //adds the trick name and the combo pt amount to the ui
    void AddToCombo(string trick, int pts,string color = "white"){
    	//float curComboDiff = Time.time - comboTime;
    	//if(curComboDiff <= comboGrace){		//within grace period, add to current combo
    	if(comboVal > 0){
    		float pluses = combo.Split('+').Length;
    		comboMultiplier = (int)(Mathf.Ceil(pluses/2.0f));
    		comboVal += pts;
    		combo += (" + <color=" + color + ">" + trick + "</color>");
    	}else{								//otherwise count as new combo
    		//FinishCombo();
    		comboMultiplier = 1;
    		comboVal = pts;
    		combo = "<color=" + color + ">" + trick + "</color>";
    	}
    	//set ui text
    	pointText.text = comboVal.ToString() + " x " + comboMultiplier.ToString();
    	comboText.text = combo;

    	//reset timer to add next trick
    	comboTime = Time.time;
    	cancelCombo = false;
    }

    //added to continuous combo (manual, grind)
    void AddToComboContinuous(string trick,string color="white"){
    	if(!addContCombo){
    		if(comboVal > 0){
    			combo += (" + <color=" + color + ">" + trick + "</color>");
    		}else{
    			combo = "<color=" + color + ">" + trick + "</color>";
    		}
    		
    		addContCombo = true;
    		comboText.text = combo;
    	}
    	comboVal += 1;
    	pointText.text = comboVal.ToString() + " x " + comboMultiplier.ToString();
    	cancelCombo = false;
    }

    //finishes the current combo out
    void FinishCombo(){
    	if(comboVal > highScore){
    		highScore = comboVal*comboMultiplier;
    		highScoreText.text = "High Score: " + highScore.ToString();
    	}
    	comboVal = 0;
    	combo = "";

		pointText.text = "0";
    	comboText.text = "";

    	Debug.Log("end combo");

    	cancelCombo = false;
    	addContCombo = false;
    }

    //check for specific object collisions
    void OnCollisionEnter2D(Collision2D c){
    	if(c.gameObject.tag == "rail"){
    		grinding = true;
    	}
    	if(c.gameObject.name.Contains("ufo") && System.Math.Round(rb.velocity.y,3) != 0){
    		//hop off ufo!
    		AddToCombo("Pop off UFO!", 100, "#00FFF7");
    	}
    	if(c.gameObject.name.Contains("rover") && System.Math.Round(rb.velocity.y,3) != 0 && c.transform.position.y < transform.position.y){
    		//the old college try
    		AddToCombo("Over the Rover!", 100, "#00FFF7");
    	}

    	/*
    	if(inTrick && sprAnim.curAnim.name.Contains("flip")){
    		Debug.Log("failed!");
    		inTrick = false;
    	}
    	*/
    }
    void OnCollisionExit2D(Collision2D c){
    	if(c.gameObject.tag == "rail"){
    		grinding = false;
    	}
    }

}
