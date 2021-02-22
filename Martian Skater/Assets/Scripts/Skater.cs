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
    public float maxJumpTime = 0.65f;

    //other tricks
    public bool inTrick = false;
    public bool grinding = false;
    public string combo = "";
    public int comboVal = 0;
    public int highScore = 0;
    private float comboTime = 0;
    private float comboGrace = 0.5f;		//grace period to string combos together
    private int comboMultiplier = 1;
    private bool cancelCombo = false;
    private bool addContCombo = false;		//added continous combo text (manual, grind) to trick combo string
    
    //game camera control
    public Transform cam;
    public Transform airHeight;				//max air height point for the camera to start following the skater upward
    public Vector3 camOffset;

    //jump ui
    public GameObject jumpBarUI;
    private Image jumpBar;

    //trick ui
    public GameObject cameraUI;
    public Text comboText;
    public Text pointText;
    public Text highScoreText; 

    // Start is called before the first frame update
    void Start()
    {
        sprAnim = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();

        //set ui
        cameraUI.SetActive(true);
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
        /*
        if(inTrick && sprAnim.curAnim.name == "normal"){
        	inTrick = false;
        }
        */

        //test tricks
        if(!sprAnim.animating){sprAnim.animating = true;}
        
        //air tricks
        if(rb.velocity.y != 0){
        	if(Input.GetKeyDown(actionKey)){
        		if(Input.GetKey(upKey)){DoTrick("kickflip",30);}
        		else if(Input.GetKey(downKey)){DoTrick("360_flip",50);}
        	}
        	if(!inTrick){
        		sprAnim.PlayAnim("normal");
        	}
        }
        //ground tricks
        else{
        	if(grinding && Input.GetKey(actionKey)){		//grind 
				sprAnim.PlayAnim("5-0_grind");
				AddToComboContinuous("5-0 grind");
				//inTrick = true;
        	}else if(Input.GetKey(downKey)){				//manuals
	        	sprAnim.PlayAnim("manual");
	        	AddToComboContinuous("Manual");
	        	//inTrick = true;
	        }else if(Input.GetKey(upKey)){
	        	sprAnim.PlayAnim("nose_manual");
	        	AddToComboContinuous("Nose Manual");
	        	//inTrick = true;
	        }else if(!Input.GetKey(downKey) && !Input.GetKey(upKey)){		//default		
	        	if(grinding){
	        		AddToComboContinuous("50-50 Grind");
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
    		AddToCombo(tname,pts,"#ff0000");
    	}
    }


    //adds the trick name and the combo pt amount to the ui
    void AddToCombo(string trick, int pts,string color = "#ffffff"){
    	//float curComboDiff = Time.time - comboTime;
    	//if(curComboDiff <= comboGrace){		//within grace period, add to current combo
    	if(comboVal > 0){
    		comboMultiplier = combo.Split('+').Length;
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
    void AddToComboContinuous(string trick){
    	if(!addContCombo){
    		if(comboVal > 0){
    			combo += (" + <color=white>" + trick + "</color>");
    		}else{
    			combo = "<color=white>" + trick + "</color>";
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
    		highScore = comboVal;
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
    	if(c.gameObject.name.Contains("ufo")){
    		//hop off ufo!
    	}
    	if(c.gameObject.name.Contains("rover")){
    		//the old college try
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
