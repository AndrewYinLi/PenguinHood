using UnityEngine;
using System;

public class ArrowController : MonoBehaviour {

    public int Type = 1;
    public bool Active = false;
    public bool follow = false;
    public int sign = 1;
   
    public GameObject explosionPrefab;
    public GameObject ropePrefab;
    public GameObject followObject;

    private int blink = 0;
    private float time = 3.0f;

    private bool rotate = true;
    private bool die = false;

    

    GameObject newRope;
    GameObject player;
    PlayerController playerController;

    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

   

    // Update is called once per frame
    void Update () {
        if (rotate)
        {
            transform.up = Vector3.Slerp(transform.up, sign * GetComponent<Rigidbody2D>().velocity.normalized, Time.deltaTime);
        }
        if (die)
        {
            try
            {
                if (GetComponent<Rigidbody2D>().velocity.x == 0 && GetComponent<Rigidbody2D>().velocity.y == 0)
                {
                    Destroy(GetComponent<Rigidbody2D>());
                    Destroy(GetComponent<BoxCollider2D>());
                    GetComponent<SpriteRenderer>().sortingOrder = 600;
                    Destroy(this);
                }
            }
            catch (Exception e) { }
        }
        else
        {
           
            if (Active)
            {
                switch (Type)
                {
                    case 2:
                        playerController.swingingArrow = gameObject;
                        playerController.swinging = true;
                        newRope = Instantiate(ropePrefab, transform.position, Quaternion.identity);
                        newRope.GetComponent<RopeScript>().destination = transform.GetChild(0).transform.position;

                        playerController.rope = newRope;

                        Active = false;
                        break;
                    case 3:

                        if (blink <= 20)
                        {
                            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                            blink++;
                        }
                        if (blink == 20)
                        {
                            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.2f, 0, 1);
                            blink = 0;
                        }

                        if ((time -= Time.deltaTime) <= 0)
                        {
                            GameObject temp = Instantiate(explosionPrefab, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.1f), new Quaternion(0, 0, 0, 0));
                            Destroy(gameObject);
                        }

                        break;
                }
            }
        }
       
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Box")
        {
            if (col.gameObject.tag == "Box" && !follow) 
            {
                if(Type == 2)
                {
                    Destroy(gameObject.GetComponent<BoxCollider2D>());
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
                Destroy(gameObject.GetComponent<Rigidbody2D>());
                GameObject preserveScale = new GameObject();
                gameObject.transform.SetParent(preserveScale.transform);
                preserveScale.transform.SetParent(col.transform);
                followObject = col.gameObject;
                follow = true;
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            if (Type != 1)
            {
                Active = true;
            }
            rotate = false;
            gameObject.GetComponent<Collider2D>().isTrigger = false;

            if (col.gameObject.tag == "Box")
            {
                Vector3 dir = col.transform.position - transform.position;
                dir = dir.normalized;
                col.GetComponent<Rigidbody2D>().AddForce(dir * 3000);
            }
            
        }
        else if(col.gameObject.tag == "Ground")
        {
            rotate = false;
            GetComponent<Collider2D>().isTrigger = false;
            die = true;
        }
        
       


    }

}
