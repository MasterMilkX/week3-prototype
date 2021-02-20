using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [System.Serializable]
    public struct SimpleAnimation {
        public string name;
        public int fps;
        public bool flipped;
        public bool flipHalf;
        public Sprite[] frames;
    }
    public SimpleAnimation[] simAnims;
    public SimpleAnimation curAnim;
    private int animIndex = 0;
    private SpriteRenderer spRend;
    public bool animating = false;


    // Start is called before the first frame update
    void Start()
    {
        spRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!animating) {
            CancelInvoke();
        }
    }

    public void PlayAnim(string name) {

        SimpleAnimation sa = simAnims[GetAnimation(name)];
        if(!(curAnim.Equals(sa)) && animating) {
            CancelInvoke();
            animIndex = 0;
            curAnim = sa;
            InvokeRepeating("LoopAnimation", 0.0f, 1.0f / curAnim.fps);
        }
    }

    public void LoopAnimation() {
        //reset the index
        animIndex++;
        if(animIndex == curAnim.frames.Length) {
            animIndex = 0;
        }

        spRend.sprite = curAnim.frames[animIndex];

        //flip the sprite accordingly
        if (animIndex >= (curAnim.frames.Length / 2) && curAnim.flipHalf)
            spRend.flipX = true;
        else if (curAnim.flipped)
            spRend.flipX = true;
        else
            spRend.flipX = false;
    }

    //only play an animation once
    public void PlayAnimOnce(string name){
        SimpleAnimation sa = simAnims[GetAnimation(name)];
        if(!(curAnim.Equals(sa)) && animating) {
            CancelInvoke();
            animIndex = 0;
            curAnim = sa;
            InvokeRepeating("IterateAnimation", 0.0f, 1.0f / curAnim.fps);
        }
    }
    //plays through an animation once then goes to a default animation
    public void IterateAnimation(){
         //reset the index
        animIndex++;

        //if at end of animation, stop and go back to default
        if(animIndex == curAnim.frames.Length) {
            CancelInvoke();
            PlayAnim("normal");
        }

        spRend.sprite = curAnim.frames[animIndex];

        //flip the sprite accordingly
        if (animIndex >= (curAnim.frames.Length / 2) && curAnim.flipHalf)
            spRend.flipX = true;
        else if (curAnim.flipped)
            spRend.flipX = true;
        else
            spRend.flipX = false;
    }


    private int GetAnimation(string anim_name) {
        for(int i = 0;i<simAnims.Length;i++) {
            SimpleAnimation sa = simAnims[i];
            if (sa.name == anim_name)
                return i;
        }
        return -1;
    }

}
