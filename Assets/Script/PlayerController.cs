using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject voidWorld, voidTimer, cdTimer;
    public float speed, jumpForce, gravity;
    private bool hasJump = true, onVoid = false, canVoid = true, exitingVoid = false;
    private Vector3 spawnPos;

    void Start() {
        rb = this.GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
        spawnPos = transform.position;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space) && hasJump && !onVoid){
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            hasJump = false;
        }
        if(Input.GetKeyDown(KeyCode.Q) && canVoid){
            StartCoroutine("VoidCountdown");
        }
    }

    void FixedUpdate() {
        if(onVoid)
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, Input.GetAxisRaw("Vertical") * speed);
        else
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, rb.velocity.y);
    }

    public IEnumerator VoidCountdown() {
        this.GetComponent<SpriteRenderer>().color = new Color32(133, 17, 124, 255);
        cdTimer.SetActive(false);
        voidTimer.SetActive(true);
        voidWorld.SetActive(true);
        voidTimer.GetComponent<Animator>().Play("timerAnim");
        onVoid = true;
        canVoid = false;
        // hasJump = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(.5f);
        this.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 255);
        voidWorld.SetActive(false);
        voidTimer.SetActive(false);
        cdTimer.SetActive(true);
        voidTimer.GetComponent<Animator>().Play("idle");
        cdTimer.GetComponent<Animator>().Play("cdTimer");
        onVoid = false;
        hasJump = true;
        rb.gravityScale = gravity;
        exitingVoid = true;
        this.GetComponent<BoxCollider2D>().enabled = true;
        yield return new WaitForSeconds(.05f);
        exitingVoid = false;
        // rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(1.95f);
        print("done");
        canVoid = true;
    }

    public void Die() {
        transform.position = spawnPos;
        hasJump = canVoid = true;
        onVoid = exitingVoid = false;
        rb.velocity = new Vector2(0, 0);
        cdTimer.GetComponent<Animator>().Play("idle");
    }
    
    public void OnCollisionEnter2D(Collision2D col) {
        // col.gameObject.tag == "floor" ||
        if((exitingVoid && (col.gameObject.tag == "wall")) || col.gameObject.tag == "trap")
            Die();
        else if(col.gameObject.tag == "floor")
            hasJump = true;
    }
}
