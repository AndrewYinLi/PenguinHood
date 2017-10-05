using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RopeScript : MonoBehaviour
{

    // Destination of the last segment of the rope
    public Vector2 destination;
    //Speed of rope generation
    public float speed = 1;
    // length of rope segments
    public float distance = 2;
    //Gmae objects
    public GameObject jointPrefab;
    public GameObject player;
    public GameObject lastJoint;
    PlayerController playerController;
    //Line renderer component
    public LineRenderer lr;
    //Default vertex count. You need at least two vertices to draw a line!
    int vertexCount = 2;
    //List of joints
    public List<GameObject> joints = new List<GameObject>();
    bool complete = false;

    // Use this for initialization
    void Start()
    {
        //init game components
        lr = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        // If the arrow is attached to the box, attach rope to box
        if (player.GetComponent<PlayerController>().swingingArrow.GetComponent<ArrowController>().follow)
        {
            lastJoint = player.GetComponent<PlayerController>().swingingArrow.GetComponent<ArrowController>().followObject;
  
        }
        else // Attach rope to player
        {
            lastJoint = transform.gameObject;
         
        }
        //Add initial joint to joints list
        joints.Add(lastJoint);

        //Moving pointer representing the end of the last joint closer to the destination 
        transform.position = Vector2.MoveTowards(transform.position, destination, speed);
        //If pointer is not at the desired position for the last joint
        if ((Vector2)transform.position != destination)
        {
            //Create a new joint and move pointer
            if (Vector2.Distance(player.transform.position, lastJoint.transform.position) > distance)
            {
                Createjoint();
            }
        }
        else if (!complete)
        {
            complete = true;
            //While pointer is not at the desired position, create joints
            while (Vector2.Distance(player.transform.position, lastJoint.transform.position) > distance)
            {
                Createjoint();
            }
            // convert joints list to array
            GameObject[] jointsArr = joints.ToArray();

            //connect last joint to player
            player.GetComponent<HingeJoint2D>().connectedBody = jointsArr[jointsArr.Length - 1].GetComponent<Rigidbody2D>();
            player.GetComponent<HingeJoint2D>().enabled = true;

        }
        //Render line since rope is current invisible
        RenderLine();
    }

   

    // Update is called once per frame
    void Update()
    {
        //Redraw the line every frame
        RenderLine();
    }


    void RenderLine()
    {
        // Setting number of position in the line renderer to the number of rope segments
        lr.positionCount = vertexCount;
        int i;
        for (i = 0; i < joints.Count; i++)
        {
            // each vertice is a rope segment
            lr.SetPosition(i, joints[i].transform.position);

        }
        // setting last vertice to player
        lr.SetPosition(i, player.transform.position);

    }


    void Createjoint()
    {

        Vector2 pos2Create = player.transform.position - lastJoint.transform.position;
        pos2Create.Normalize();
        pos2Create *= distance;
        pos2Create += (Vector2)lastJoint.transform.position;

        GameObject temp = Instantiate(jointPrefab, pos2Create, Quaternion.identity);
        temp.transform.SetParent(transform);
        temp.GetComponent<HingeJoint2D>().connectedBody = lastJoint.GetComponent<Rigidbody2D>();
        lastJoint = temp;
        joints.Add(lastJoint);
        vertexCount++;

    }



}