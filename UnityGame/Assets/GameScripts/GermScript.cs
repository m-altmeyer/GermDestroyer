using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GermScript : MonoBehaviour
{
    private Animator animator;

    public GameObject bubbleSpawn;
    public GameObject plus1Spawn;

    bool isDying;
    float timer = 0.0f;
    float deathDuration = 1.0f;
    Vector3 originalScale;

    private void Awake(){
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        isDying = false;
        timer = 0.0f;
        deathDuration = 1.0f;
        randomizeAnimations();
    }

    void Update(){
        if (isDying) {dyingAction();}
    }

    void randomizeAnimations()
    {
        animator.SetFloat("speed", Random.Range(0.9f, 1.1f));
    }

    void dyingAction(){
        float factor = (deathDuration - timer)/deathDuration;
        transform.localScale = originalScale * factor;
        timer += Time.deltaTime;
        if (timer >= deathDuration){
            Destroy(gameObject);
        }
    }


    //Animations
    public void startScaredAnimation() {
        if (!isDying) {
            animator.SetInteger("animState", 1);
            Debug.Log("idle animation");
        }
    }

    public void startAngryAnimation()
    {
        if (!isDying) {animator.SetInteger("animState", 3); Debug.Log("angry animation"); }
    }

    public void startWashedAnimation()
    {
        if (!isDying) { animator.SetInteger("animState", 2); Debug.Log("washed animation"); }
    }

    public void startDeath(){
        animator.SetInteger("animState", 4);
        Debug.Log(animator);
        bubbleSpawn.SetActive(true);
        plus1Spawn.SetActive(true);
        GetComponent<AudioSource>().Play();
        isDying = true;
    }
}
