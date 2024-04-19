using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum States // used by all logic
{
    None,
    Idle,
    Walk,
    Jump,
    Dead,
};

public class PlayerScript : MonoBehaviour
{
    States state;

    Rigidbody rb;
    bool grounded;
    public GameObject cylinder;

    // Start is called before the first frame update
    void Start()
    {
        state = States.Idle;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        DoLogic();
    }

    void FixedUpdate()
    {
        grounded=false;
    }


    void DoLogic()
    {
        if( state == States.Idle )
        {
            PlayerIdle();
        }

        if( state == States.Jump )
        {
            PlayerJumping();
        }

        if( state == States.Walk )
        {
            PlayerWalk();
        }

        if( state == States.Dead)
        {
            PlayerDeath();
        }
    }


    void PlayerIdle()
    {
        if( Input.GetKeyDown(KeyCode.Space))
        {
            // simulate jump
            state = States.Jump;
            rb.velocity = new Vector3( 0,10,0);
        }

        if( Input.GetKey("left"))
        {
            transform.Rotate( 0, 0.5f, 0, Space.Self );

        }
        if( Input.GetKey("right"))
        {
            transform.Rotate( 0,-0.5f, 0, Space.Self );
        }

        if( Input.GetKey("up"))
        {
            state = States.Walk;
        }

    }

    void PlayerJumping()
    {
        // player is jumping, check for hitting the ground
        if( grounded == true )
        {
            //player has landed on floor
            state = States.Idle;
        }
    }

    void PlayerWalk()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 5f);

        //magnitude = the player's speed
        float magnitude = rb.velocity.magnitude;

        rb.AddForce(transform.forward * 5f);

        if ( Input.GetKey("up") == false)
        {                    
            rb.AddForce(transform.forward * -5f);      
            
            if (rb.velocity.magnitude < 0.1f)
            {
                state = States.Idle;
            }
        }
    }

    void PlayerDeath()
    {
        cylinder.SetActive(false);
        rb.velocity = Vector3.zero;

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);

        gameObject.transform.position = new Vector3(0, 0, 0);
        cylinder.SetActive(true);
        state = States.Idle;
        print("Respawned");
    }

    void OnCollisionEnter( Collision col )
    {
        if( col.gameObject.tag == "Floor")
        {
            grounded=true;
            print("landed!");
        }

        if ( col.gameObject.tag == "Bad")
        {
            PlayerDeath();
            state = States.Dead;
            print("Dead");
        }
    }
    

    private void OnGUI()
    {
        //debug text
        string text = "Left/Right arrows = Rotate\nSpace = Jump\nUp Arrow = Forward\nCurrent state=" + state;

        // define debug text area
        GUILayout.BeginArea(new Rect(10f, 450f, 1600f, 1600f));
        GUILayout.Label($"<size=16>{text}</size>");
        GUILayout.EndArea();
    }
}
