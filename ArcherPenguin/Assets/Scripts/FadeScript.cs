using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour {

    public bool fadeActive = false;
    public bool fadeText = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (fadeActive) // if fade
        {
         
            if (fadeText) // if text shoukd be faded
            {
                // remove 0.015 from the text's initial alpha of 1 per frame
                Color oldColor = GetComponent<Text>().color;
                GetComponent<Text>().color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a - 0.015f);
                if(GetComponent<Text>().color.a <= 0)
                {
                    fadeActive = false;
                }
            }
          
        }
	}
}
