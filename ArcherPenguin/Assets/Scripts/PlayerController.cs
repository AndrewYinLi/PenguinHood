using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public int currentAnimationState = STATE_IDLE; // Current state
    public int arrowType = 1; // 1 = regular, 2 = grappling, 3 = explosive
    public int remainingArrows = 10; 
    public int level;
    public Quaternion arrowRotation;

    string facing = "right"; // Direction player is facing
    int walkSpeed = 4; // Velocity multiplier for when player is moving
    int score = 0;
    bool isAiming = false;

    //animation states
    const int STATE_IDLE = 0;
    const int STATE_WALK = 1;
    const int STATE_JUMP = 2;
    const int STATE_SHOOT = 3;
    const int STATE_SHALK = 4;

    //arm positions
    static float arm1RightX = -0.6f;
    static float arm2RightX = 0.15f;
    static float arm1LeftX = 0.6f;
    static float arm2LeftX = -0.15f;
    static float arm1Y = -0.548f;

    // Game components
    Animator animator;
    Rigidbody2D playerRB;
    SpriteRenderer playerSR;
    SpriteRenderer arm1SR;
    SpriteRenderer arm2SR;
    GameObject arm1;
    GameObject arm2;
    Text arrowText, remainingText, completeText;
    FadeScript arrowTextFadeScript;
    Image score1, score2, score3;

    // Prefabs
    public GameObject arrowPrefab;
    public GameObject dotPrefab;

    // Grappling arrow handling
    public GameObject swingingArrow;
    public GameObject rope;
    public bool swinging = false;

    //trajectory
    private float power = 25;
    private int numOfTrajectoryPoints = 8;
    private GameObject[] trajectoryPoints;
    private GameObject arrow;

    // Use this for initialization
    void Start()
    {

        // Finding components in the Worldspace and assigning a variable to them
        animator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        playerSR = GetComponent<SpriteRenderer>();
        arm1 = transform.GetChild(0).gameObject;
        arm2 = transform.GetChild(1).gameObject;
        arm1SR = arm1.GetComponent<SpriteRenderer>();
        arm2SR = arm2.GetComponent<SpriteRenderer>();
        arrowText = GameObject.FindWithTag("ArrowText").GetComponent<Text>();
        arrowTextFadeScript = GameObject.FindWithTag("ArrowText").GetComponent<FadeScript>();
        score1 = GameObject.FindWithTag("Score1").GetComponent<Image>();
        score2 = GameObject.FindWithTag("Score2").GetComponent<Image>();
        score3 = GameObject.FindWithTag("Score3").GetComponent<Image>();
        remainingText = GameObject.FindWithTag("RemainingText").GetComponent<Text>();
        remainingText.text = remainingArrows.ToString();
        completeText = GameObject.FindWithTag("CompleteText").GetComponent<Text>();

        // Setting trajectory dots
        trajectoryPoints = new GameObject[8];
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            GameObject dot = Instantiate(dotPrefab);
            dot.GetComponent<Renderer>().enabled = false;
            trajectoryPoints[i] = dot;
        }

    }

    void Update()
    {
        // On right click
        if (Input.GetMouseButtonDown(1))
        {
            // If the player wasn't aiming, the player is now aiming
            if (!isAiming)
            {
                // Checking to make sure that the player has arrows left
                if (remainingArrows != 0)
                {
                    //Enable arm sprites
                    isAiming = true;
                    changeState(STATE_SHOOT);
                    arm1SR.enabled = true;
                    arm2SR.enabled = true;

                    //Instantiate the arrow object
                    arrow = Instantiate(arrowPrefab);
                    arrow.SetActive(false);
                    arrow.GetComponent<ArrowController>().Type = arrowType;
                    Vector3 pos = transform.position;
                    pos.z = 1;
                    arrow.transform.position = pos;
                }
                else
                {
                    //If no more arrows are left, display message
                    arrowText.text = "No more arrows left!";
                    arrowText.color = new Color(1, 1, 1, 1);
                    arrowTextFadeScript.fadeActive = true;
                }

            }
            else
            {
                //Disable arm sprites
                disableArms();
                //Disable parabola dots 
                for (int i = 0; i < numOfTrajectoryPoints; i++)
                {
                    trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0)) // On left mouse click
        {
            if (isAiming)
            {
                //Subtract from ammo because arrow has been shot
                remainingArrows--;
                remainingText.text = remainingArrows.ToString();
                disableArms();
               
                //Determining arrow rotation according to the parabola
                Quaternion newArrowRotation = arrowRotation;
                float newArrowX = gameObject.transform.position.x;
                float newArrowY = gameObject.transform.position.y;

                if (facing.Equals("right"))
                {
                    //arrowTip = new Vector2(arm1RightX + 1.0f, arm1Y);
                }
                else
                {
                    
                    newArrowRotation = Quaternion.Euler(new Vector3(arrowRotation.eulerAngles.x, arrowRotation.eulerAngles.y, arrowRotation.eulerAngles.z + 180.0f));
                }

                // Setting the new arrow rotation
                arrow.transform.position = new Vector2(newArrowX, newArrowY);
                arrow.transform.rotation = newArrowRotation;
                arrow.SetActive(true);
                
                // If player is currently swinging and the player attempts to shoot another grappling arrow
                if (swinging && arrowType == 2)
                {
                    //Disconnect the player from the current rope to prevent the player from being connected to two ropes simultaneously
                    try
                    {
                        //Disconnecting from the distancejoint component
                        GetComponent<DistanceJoint2D>().connectedBody = null;
                    }
                    catch (Exception e) { }
                    try
                    {
                        //Disable component
                        GetComponent<DistanceJoint2D>().enabled = false;
                    }
                    catch (Exception e) { }
                    //Destroy current rope
                    Destroy(swingingArrow);
                    Destroy(rope);
                    swinging = false;
                }

                //Add force to the arrow according the direction the player is facing
                if (facing.Equals("right"))
                {
                    if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < arrow.transform.position.x)
                    {
                        arrow.GetComponent<Rigidbody2D>().AddForce(getForce(arrow.transform.position, new Vector3(arrow.transform.position.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, Camera.main.ScreenToWorldPoint(Input.mousePosition).z)));
                    }
                    else
                    {
                        arrow.GetComponent<Rigidbody2D>().AddForce(getForce(arrow.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)), ForceMode2D.Impulse);
                    }
                }
                else
                {
                    if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > arrow.transform.position.x)
                    {
                        arrow.GetComponent<Rigidbody2D>().AddForce(getForce(arrow.transform.position, new Vector3(arrow.transform.position.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, Camera.main.ScreenToWorldPoint(Input.mousePosition).z)));
                    }
                    else
                    {
                        arrow.GetComponent<Rigidbody2D>().AddForce(getForce(arrow.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)), ForceMode2D.Impulse);
                    }
                }

                //Disable trajectory dots
                for (int i = 0; i < numOfTrajectoryPoints; i++)
                {
                    trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
                }
            }
        }

        //follow mouse
        if (isAiming)
        {
            Vector3 pos = transform.position;
            pos.z = 1;
            arrow.transform.position = pos; // Setting arrow position
            int sign; // Sign is the multiplier for the arrow
            if (facing.Equals("right"))
            {
                sign = 1;
                arrow.GetComponent<ArrowController>().sign = sign; // Setting sign variable in arrow object
            }
            else
            {
                sign = -1;
                arrow.GetComponent<ArrowController>().sign = sign; // Setting sign variable in arrow object
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // self explanatory
            
            //Vector multiplication to determine rotation of player arms
            Quaternion upwards = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, sign * 90) * (mousePos - transform.position));

            //if past bounds to prevent arms from reaching behind in unnatural, awkward angles
            if (sign == 1) // If facing right
            {
                if (upwards.eulerAngles.z > 60.0f && upwards.eulerAngles.z < 330.0f)
                { 
                    if (upwards.eulerAngles.z > 60.0f && upwards.eulerAngles.z < 195.0f) // If too far up
                    {
                        upwards = Quaternion.Euler(new Vector3(upwards.eulerAngles.x, upwards.eulerAngles.y, 60.0f));
                    }
                    else // If too far down
                    {
                        upwards = Quaternion.Euler(new Vector3(upwards.eulerAngles.x, upwards.eulerAngles.y, 330.0f));
                    }
                }
            }
            else // if facing left
            {
                if (upwards.eulerAngles.z > 30.0f && upwards.eulerAngles.z < 300.0f)
                {

                    if (upwards.eulerAngles.z > 30.0f && upwards.eulerAngles.z < 165.0f) // if too far down
                    {
                        upwards = Quaternion.Euler(new Vector3(upwards.eulerAngles.x, upwards.eulerAngles.y, 30.0f));
                    }
                    else // if too far up
                    {
                        upwards = Quaternion.Euler(new Vector3(upwards.eulerAngles.x, upwards.eulerAngles.y, 300.0f));
                    }
                }
            }
            arrowRotation = upwards; // Setting new rotation of arrow
            transform.GetChild(0).gameObject.transform.rotation = upwards; // Setting arm1 to new rotation
            transform.GetChild(1).gameObject.transform.rotation = upwards; // Setting arm2 to new rotation

            //Theoretical velocity of the arrow gameObject according to distance between the player and the mouse position
            Vector3 vel = getForce(arrow.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (sign == 1) // If facing right
            {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < arrow.transform.position.x) // If the mouse is behind the player despite the player facing right
                {
                    vel = getForce(arrow.transform.position, new Vector3(arrow.transform.position.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, Camera.main.ScreenToWorldPoint(Input.mousePosition).z));
                }

            }
            else // Facing left
            {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > arrow.transform.position.x) // If the mouse is behind the player despite the player facing left
                {
                    vel = getForce(arrow.transform.position, new Vector3(arrow.transform.position.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, Camera.main.ScreenToWorldPoint(Input.mousePosition).z));
                }

            }

            //Set trajectory parabola dots
            setTrajectoryPoints(arrow.transform.position, vel / arrow.GetComponent<Rigidbody2D>().mass);
        }


    }

    // Difference between two points multiplied by the power multiplier
    private Vector2 getForce(Vector3 fromPos, Vector3 toPos)
    {
        return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;
    }

    void setTrajectoryPoints(Vector3 startPos, Vector3 vel)
    {
        float velocity = Mathf.Sqrt((vel.x * vel.x) + (vel.y * vel.y)); // Distance formula
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(vel.y, vel.x)); // Angle between two points
        float time = 0;

        time += 0.1f;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            //Calculating x and y difference between time = x and time = x+0.1
            float xMod = velocity * time * Mathf.Cos(angle * Mathf.Deg2Rad);
            float yMod = velocity * time * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * time * time / 2.0f);
            Vector3 pos = new Vector3(startPos.x + xMod, startPos.y + yMod, 2);
            //setting trajetory point to new point
            trajectoryPoints[i].transform.position = pos;
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(vel.y - Physics.gravity.magnitude * time, vel.x) * Mathf.Rad2Deg);
            time += 0.1f;
        }
    }

    void disableArms()
    {
        //Essentially disables arm sprite renderers and resets location of the arms 
        isAiming = false;
        arm1SR.enabled = false;
        arm2SR.enabled = false;
        if (facing.Equals("right"))
        {
            arm1.transform.localPosition = new Vector3(arm1RightX, arm1Y);
            arm2.transform.localPosition = new Vector3(arm2RightX, arm1Y);
        }
        else
        {
            arm1.transform.localPosition = new Vector3(arm1LeftX, arm1Y);
            arm2.transform.localPosition = new Vector3(arm2LeftX, arm1Y);
        }
        if (!isGrounded())
        {
            changeState(STATE_JUMP);
        }
    }
    void FixedUpdate()
    {
        //WASD controls
        if (Input.GetKey(KeyCode.W))
        {
            if (isGrounded())
            {
                if (swingingArrow != null) // Regular jump
                {
                    playerRB.velocity = new Vector2(playerRB.velocity.x, (int)(walkSpeed * 3));
                }
                else // Jump while attached to a rope
                {
                    playerRB.velocity = new Vector2(playerRB.velocity.x, (int)(walkSpeed * 2.5));
                }
                if (!isAiming)
                {
                    changeState(STATE_JUMP); // change animation state
                }
            }

        }
        if (Input.GetKey(KeyCode.D))
        {
            changeDirection("right"); // facing right
            //Casting ray to see if there is a wall in front of the player
            if (!Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 1f), Vector2.right, 1.15f)) {
                if (!isGrounded()) // If airborne, add different speed to prevent momentum from accumulating too much
                {
                    if (swingingArrow != null)
                    {
                        if (playerRB.velocity.x > walkSpeed)
                        {

                            playerRB.velocity = new Vector2(-walkSpeed, playerRB.velocity.y);
                        }
                        else
                        {
                            playerRB.AddForce(new Vector2(60f, 0));
                        }
                    }
                    else
                    {
                        playerRB.velocity = new Vector2(walkSpeed, playerRB.velocity.y);
                    }

                }
                else
                {
                    playerRB.velocity = new Vector2(walkSpeed, playerRB.velocity.y);
                }
            }

            if (isGrounded()) 
            {

                if (!isAiming)
                {
                    changeState(STATE_WALK); // If not aiming, regular walking animation
                }
                else
                {
                    changeState(STATE_SHALK); // If shooting and walking change animation state to "shalking"
                }
            }




        }
        else if (Input.GetKey(KeyCode.A))
        {

            changeDirection("left");
            //Casting ray to see if there is a wall in front of the player
            if (!Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1f), Vector2.left, 1.15f))
            {
                if (!isGrounded()) // If airborne, add different speed to prevent momentum from accumulating too much
                {
                    if (swingingArrow != null)
                    {
                        if (playerRB.velocity.x > walkSpeed)
                        {

                            playerRB.velocity = new Vector2(walkSpeed, playerRB.velocity.y);
                        }
                        else
                        {
                            playerRB.AddForce(new Vector2(-60f, 0));
                        }
                    }
                    else // Regular walkspeed
                    {
                        playerRB.velocity = new Vector2(-walkSpeed, playerRB.velocity.y);
                    }
                }
                else
                {
                    playerRB.velocity = new Vector2(-walkSpeed, playerRB.velocity.y);
                }
                if (isGrounded())
                {

                    if (!isAiming)
                    {
                        changeState(STATE_WALK);  // If not aiming, regular walking animation
                    }
                    else
                    {
                        changeState(STATE_SHALK); // If shooting and walking change animation state to "shalking"
                    }
                }
            }

        }
        else // if WAS all weren't pressed
        {
            if (isGrounded()) // Change animation states
            {
                if (!isAiming)
                {
                    changeState(STATE_IDLE);
                }
                else
                {
                    changeState(STATE_SHOOT);
                }
            }
        }


        // switch to regular arrow
        if (Input.GetKey(KeyCode.Alpha1))
        {
            arrowChangeUI(); // change ui text
            if (arrowType != 1)
            {
                arrowType = 1;

                if (isAiming)
                {
                    arrow.GetComponent<ArrowController>().Type = arrowType;
                    arrow.SetActive(false);
                    Vector3 pos = transform.position;
                    pos.z = 1;
                    arrow.transform.position = pos;
                }
            }

        }
        else if (Input.GetKey(KeyCode.Alpha2)) // switch to grappling arrow
        {
            arrowChangeUI(); // change ui text
            if (arrowType != 2)
            {
                arrowType = 2;

                if (isAiming)
                {
                    arrow.GetComponent<ArrowController>().Type = arrowType;
                    arrow.SetActive(false);
                    Vector3 pos = transform.position;
                    pos.z = 1;
                    arrow.transform.position = pos;
                }
            }
        }
        else if (Input.GetKey(KeyCode.Alpha3)) // switch to explosive arrow
        {
            arrowChangeUI(); // change ui text
            if (arrowType != 3)
            {
                arrowType = 3;

                if (isAiming)
                {
                    arrow.GetComponent<ArrowController>().Type = arrowType;
                    arrow.SetActive(false);
                    Vector3 pos = transform.position;
                    pos.z = 1;
                    arrow.transform.position = pos;
                }
            }
        }
        if (Input.GetKey("space")) // attempt to disconnect player from rope
        {

            if (swinging) // only disconnect if player is currently connected to a rope
            {
                GetComponent<DistanceJoint2D>().connectedBody = null;
                GetComponent<DistanceJoint2D>().enabled = false;
                Destroy(swingingArrow);
                Destroy(rope);
                swinging = false;
            }

        }
    }




    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Money")
        {
            //Destroy money object but add to score and update UI
            Destroy(col.gameObject);
            score++;
            scoreAddUI();
        }

    }

    private void arrowChangeUI()
    {
        if (remainingArrows != 0)
        {
            switch (arrowType) // eyyy i learned switch statements in cs151. Basically update UI text according to an int
            {
                case 1: 
                    arrowText.text = "Regular Arrow";
                    break;
                case 2:
                    arrowText.text = "Grapple Arrow";
                    break;
                case 3:
                    arrowText.text = "Explosive Arrow";
                    break;
            }
        }
        else
        {
            arrowText.text = "No more arrows left!";
        }
        arrowText.color = new Color(1, 1, 1, 1); // Fade UI text
        arrowTextFadeScript.fadeActive = true;
    }

    private void scoreAddUI() // Enable sprite for money bags corresponding to the score
    {
        switch (score)
        {
            case 1:
                score1.enabled = true;
                break;
            case 2:
                score2.enabled = true;
                break;
            case 3:
                score3.enabled = true;
                break;
        }
    }



    bool isGrounded()
    {
        bool rayCast = Physics2D.Raycast(transform.position, Vector2.down, 1.4f); // is there ground below middle of player

        RaycastHit2D rayCastHit = Physics2D.Raycast(transform.position, Vector2.down, 1.4f); // raycast object

        if (!rayCast)
        {
            // check if ground is below front of player
            rayCast = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.down, 1.4f);
            rayCastHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.down, 1.4f);
        }
        if (!rayCast)
        {
            // check if ground is below rear of player
            rayCast = Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.down, 1.4f);
            rayCastHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.down, 1.4f);
        }

        if (rayCast) // If object below player
        {
            // Check to see if valid tag associated with ground
            if (rayCastHit.transform.gameObject.tag == "Ground" || rayCastHit.transform.gameObject.tag == "Box" || rayCastHit.transform.gameObject.tag == "Arrow" || rayCastHit.transform.gameObject.tag == "Wall" || rayCastHit.transform.gameObject.tag == "Friar")
            {
                if (currentAnimationState != STATE_SHOOT && currentAnimationState != STATE_SHALK)
                {
                    changeState(STATE_IDLE);
                }
            }
            else
            {
                if (currentAnimationState != STATE_SHOOT && currentAnimationState != STATE_SHALK)
                {
                    changeState(STATE_JUMP);
                }
                rayCast = false;
            }
        }
        else
        {
            if (currentAnimationState != STATE_SHOOT && currentAnimationState != STATE_SHALK)
            {
                changeState(STATE_JUMP);
            }
        }


        return rayCast;
    }


    //change animation state 
    public void changeState(int playerState)
    {

        if (currentAnimationState == playerState)
            return;
        
        // switch animation state
        switch (playerState)
        {
            case STATE_WALK:
                animator.SetInteger("playerState", STATE_WALK);
                break;

            case STATE_JUMP:
                animator.SetInteger("playerState", STATE_JUMP);
                break;

            case STATE_IDLE:
                animator.SetInteger("playerState", STATE_IDLE);
                break;

            case STATE_SHOOT:
                animator.SetInteger("playerState", STATE_SHOOT);
                break;

            case STATE_SHALK:
                animator.SetInteger("playerState", STATE_SHALK);
                break;
        }

        currentAnimationState = playerState;
    }

    //eponymous 
    void changeDirection(string direction)
    {
        //Flip sprites according to facing direction
        if (facing != direction)
        {
            if (direction == "right")
            {
                playerSR.flipX = false;
                arm1SR.flipX = false;
                arm2SR.flipX = false;
                facing = "right";
                arm1.transform.localPosition = new Vector3(arm1RightX, arm1Y);
                arm2.transform.localPosition = new Vector3(arm2RightX, arm1Y);
            }
            else if (direction == "left")
            {
                playerSR.flipX = true;
                arm1SR.flipX = true;
                arm2SR.flipX = true;
                facing = "left";
                arm1.transform.localPosition = new Vector3(arm1LeftX, arm1Y);
                arm2.transform.localPosition = new Vector3(arm2LeftX, arm1Y);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // if collide with friar, check to see if all money bags are collected
        if (col.gameObject.tag == "Friar")
        {
            if (score == 3)
            {
                completeText.enabled = true;
                SceneManager.LoadScene("Level_Select");
            }
        }
        else if(col.gameObject.tag == "Hazard")
        {
            SceneManager.LoadScene("Level_" + level); // Restart level because touch level = game over
        }
    }

    public void restartButton()
    {
        SceneManager.LoadScene("Level_" + level); // Restart level
    }

    public void mainMenuButton()
    {
        SceneManager.LoadScene("Level_Select"); // Go back to main menu
    }

    
}
