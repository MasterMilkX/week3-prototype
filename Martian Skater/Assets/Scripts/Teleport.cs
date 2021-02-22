using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform other;
    public float offset;

    public void Start(){
    	GetComponent<SpriteAnimator>().animating = true;
    	GetComponent<SpriteAnimator>().PlayAnim("idle");	//play same animation over and over
    }

    //teleport to opposite teleporter
    public void OnTriggerEnter2D(Collider2D c){
    	if(c.gameObject.tag == "player"){
    		c.transform.position = new Vector3(other.position.x+offset,other.transform.position.y+0.05f,c.transform.position.z);
    		//c.GetComponent<Rigidbody2D>().AddForce(new Vector2(50*(other.position.x > transform.position.x ? 1 : -1),0));
    	}
    }
}
