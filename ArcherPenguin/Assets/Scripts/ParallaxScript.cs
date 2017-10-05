using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScript : MonoBehaviour
{

    public float XModifier;
    public float YModifier;

    Vector2 cameraOriginalPosition;
    Vector2 originalPosition;
    GameObject player;
    GameObject mainCamera;

    // Game states
    const int NEUTRAL = 0;
    const int TRANSITION_FADE = 1;
    const int TRANSITION_CAMERA = 2;
    public int state = TRANSITION_FADE;


    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        mainCamera = GameObject.FindWithTag("MainCamera");
        cameraOriginalPosition = mainCamera.transform.position;
        originalPosition = transform.position;
        

    }

    // Update is called once per frame
    void Update()
    {
        state = GameObject.FindWithTag("CameraScript").GetComponent<CameraScript>().state;
        switch (state)
        {
            case NEUTRAL: // Transform background according to player position
                transform.position = new Vector3(originalPosition.x - (player.transform.position.x - cameraOriginalPosition.x) / XModifier, originalPosition.y + (player.transform.position.y - cameraOriginalPosition.y) / YModifier, transform.position.z);
                break;
            case TRANSITION_CAMERA: // Transition camera focus from exit to player
                if (mainCamera.transform.position.x > 0)
                {
                    
                    transform.position = new Vector3(originalPosition.x - (mainCamera.transform.position.x - cameraOriginalPosition.x) / XModifier, originalPosition.y + (mainCamera.transform.position.y - cameraOriginalPosition.y) / YModifier, transform.position.z);
                }
                break;

        }
       
    }
}
