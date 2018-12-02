using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    OnGround,
    UnderGround,
    Jumping,
    Flying,
    Space
}

public class Player : MonoBehaviour
{
    public float Speed = 1f;
    public float Jump = 50f;

    public PlayerState curPlayerState = PlayerState.Jumping;

    Rigidbody2D rb;
    Collider2D col;

    public Vector2 minPos;
    public Vector2 maxPos;

    public Animator animator;

    public GameObject Fire;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        SoundManager.Load("Overworld", new []{ "Overworld", "Game" });
        SoundManager.Load("Space", new[] { "Space", "Game" });
        SoundManager.Load("Underground", new[] { "Underground", "Game" });

        SoundManager.Load("Click", new[] { "Click", "SFX", "Game" });
        SoundManager.Load("BubblePop", new[] { "BobblePop", "SFX", "Game" });
        SoundManager.Load("Select", new[] { "Select", "SFX", "Game" });

        SoundManager.PlayBG("Overworld");
    }

    // Update is called once per frame
    void Update()
    {
        float curSpeed = Speed * Time.deltaTime;
        Vector3 offsetPos = 
            new Vector3(
                Input.GetAxis("Horizontal"),
                //((Input.GetKey(KeyCode.D)) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
                //((Input.GetKey(KeyCode.W)) ? 5 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0),
                0,
                0);

        //if (Input.GetKey(KeyCode.A)) offsetPos.x -= Speed * Time.deltaTime;
        //if (Input.GetKey(KeyCode.D)) offsetPos.x += Speed * Time.deltaTime;
        //if (Input.GetKey(KeyCode.W)) offsetPos.y += Speed * Time.deltaTime;
        //if (Input.GetKey(KeyCode.S)) offsetPos.y -= Speed * Time.deltaTime;

        //transform.position += offsetPos;
        if (curPlayerState == PlayerState.OnGround)
        {
            if (Input.GetButtonDown("Jump")) offsetPos.y = Jump;
            else if(Input.GetAxis("Vertical") < -Mathf.Epsilon)
            {
                col.isTrigger = true;
                rb.gravityScale = 0;
                offsetPos.y -= 5f;
                curPlayerState = PlayerState.UnderGround;

                StartCoroutine(TransitionToSong("Underground"));
            }
        }

        else if (curPlayerState == PlayerState.UnderGround)
        {
            offsetPos.y = Input.GetAxis("Vertical");
        }

        else if(curPlayerState == PlayerState.Jumping)
        {
            if(Input.GetButtonDown("Jump")) curPlayerState = PlayerState.Flying;
        }

        else if(curPlayerState == PlayerState.Flying)
        {
            if (Input.GetButtonUp("Jump"))
            {
                curPlayerState = PlayerState.Jumping;
                Fire.SetActive(false);
            }
            else if (transform.position.y > 50f)
            {
                curPlayerState = PlayerState.Space;
                StartCoroutine(TransitionToSong("Space"));
            }
            else
            {
                offsetPos.y += 1.5f;
                Fire.SetActive(true);
            }
        }

        else if(curPlayerState == PlayerState.Space)
        {
            if (transform.position.y < 50f)
            {
                curPlayerState = PlayerState.Jumping;
                StartCoroutine(TransitionToSong("Overworld"));
            }

            else if (Input.GetButton("Jump"))
            {
                offsetPos.y += 1.5f;
                Fire.SetActive(true);
            }

            else Fire.SetActive(false);
        }

        rb.AddForce(offsetPos * Speed);

        Vector3 curPos = transform.position;
        Vector3 vel = rb.velocity;
        if(curPos.x < minPos.x || curPos.x > maxPos.x)
        {
            vel.x = 0;
            curPos.x = Mathf.Clamp(curPos.x, minPos.x, maxPos.x);
        }
        if (curPos.y < minPos.y || curPos.y > maxPos.y)
        {
            vel.y = 0;
            curPos.y = Mathf.Clamp(curPos.y, minPos.y, maxPos.y);
        }

        rb.velocity = vel;
        transform.position = curPos;

        if(vel.x > Mathf.Epsilon)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, 1);
            if(curPlayerState == PlayerState.OnGround || curPlayerState == PlayerState.UnderGround) animator.SetBool("isWalking", true);
        }
        else if (vel.x < -Mathf.Epsilon)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, 1);
            if (curPlayerState == PlayerState.OnGround || curPlayerState == PlayerState.UnderGround) animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (curPlayerState != PlayerState.OnGround && curPlayerState != PlayerState.UnderGround)
        {
            animator.SetBool("isWalking", false);
        }
    }

    IEnumerator TransitionToSong(string newSong, float animTime = 0.5f)
    {
        for(float t = animTime; t > 0; t -= Time.deltaTime)
        {
            SoundManager.SetVolume(t / animTime);
            yield return null;
        }

        SoundManager.SetVolume(0);
        SoundManager.PlayBG(newSong);

        for (float t = 0; t < animTime; t += Time.deltaTime)
        {
            SoundManager.SetVolume(t / animTime);
            yield return null;
        }

        SoundManager.SetVolume(1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter " + collision.gameObject.name);
        if(collision.gameObject.tag == "Ground")
        {
            Debug.Log("On Ground");
            curPlayerState = PlayerState.OnGround;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("OnCollisionExit " + collision.gameObject.name);
        if (collision.gameObject.tag == "Ground" && curPlayerState == PlayerState.OnGround)
        {
            Debug.Log("Off Ground");
            curPlayerState = PlayerState.Jumping;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit " + collision.gameObject.name);
        if(collision.gameObject.tag == "Ground" && transform.position.y > collision.transform.position.y)
        {
            Debug.Log("On Ground");
            col.isTrigger = false;
            rb.gravityScale = 1;
            curPlayerState = PlayerState.Jumping;
            StartCoroutine(TransitionToSong("Overworld"));
        }
    }
}
