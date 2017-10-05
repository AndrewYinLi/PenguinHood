using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float XModifier;
    public float YModifier;

    Vector2 cameraOriginalPosition;
    Vector2 originalPosition;
    GameObject player;
    GameObject mainCamera;

    Canvas canvas;
    SpriteRenderer fadeBlack;


    //Different statess
    const int NEUTRAL = 0;
    const int TRANSITION_FADE = 1;
    const int TRANSITION_CAMERA = 2;
    public int state = TRANSITION_FADE;


    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player");
        mainCamera = GameObject.FindWithTag("MainCamera");
        canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        cameraOriginalPosition = mainCamera.transform.position;
        originalPosition = transform.position;
        fadeBlack = GameObject.FindWithTag("FadeBlack").GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case NEUTRAL: // Transform background according to player's position
                transform.position = new Vector3(originalPosition.x - (player.transform.position.x - cameraOriginalPosition.x) / XModifier, originalPosition.y + (player.transform.position.y - cameraOriginalPosition.y) / YModifier, transform.position.z);
                break;
            case TRANSITION_FADE: // Fade the black sprite covering camera to make fade illusion
                if (fadeBlack.color.a > 0)
                {
                    fadeBlack.color = new Color(0, 0, 0, fadeBlack.color.a - 0.01f);
                }
                else // Transition from fade state to camera movement
                {
                    Destroy(fadeBlack);
                    state = TRANSITION_CAMERA;
                }
                break;
            case TRANSITION_CAMERA: // move camera focus from exit of level to player
                if(mainCamera.transform.position.x > 0)
                {
                    mainCamera.transform.position = new Vector3(mainCamera.transform.position.x - 0.25f, 0, -10f);
                    transform.position = new Vector3(originalPosition.x - (mainCamera.transform.position.x - cameraOriginalPosition.x) / XModifier, originalPosition.y + (mainCamera.transform.position.y - cameraOriginalPosition.y) / YModifier, transform.position.z);
                }
                else
                {
                    mainCamera.transform.position = new Vector3(0, 0, -10f);
                    transform.position = new Vector3(originalPosition.x - (mainCamera.transform.position.x - cameraOriginalPosition.x) / XModifier, originalPosition.y + (mainCamera.transform.position.y - cameraOriginalPosition.y) / YModifier, transform.position.z);
                    canvas.enabled = true;
                    canvas.transform.parent = player.transform;
                    mainCamera.transform.parent = player.transform;
                    state = NEUTRAL;
                }
                break;
                
        }
    }
}
