using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverMovement : MonoBehaviour
{
    private bool goRight = true;
    public float speed = 2.0f;

    private SpriteRenderer spr;

    void Start(){
    	spr = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {	
    	//patrol back and forth
        if(goRight){
        	transform.Translate(Vector3.right * speed * Time.deltaTime);
        }else{
        	transform.Translate(-Vector3.right * speed * Time.deltaTime);
        }
    }

    //alternate between left and right
    void OnTriggerEnter2D(Collider2D c){
    	if(c.gameObject.name == "roverpt"){
    		goRight = !goRight;
    		spr.flipX = !spr.flipX;
    	}
    }	
}
