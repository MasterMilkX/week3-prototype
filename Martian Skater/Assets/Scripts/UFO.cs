using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
	private SpriteAnimator anim;
	
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<SpriteAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
    	//loop hover animation
       	anim.PlayAnim("hover");
       	anim.animating = true;
    	
    }
}
