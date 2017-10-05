using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour {

    public int level; // level that will be accessed by the ui

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setLevel()
    {
        GameObject.FindWithTag("Canvas").GetComponent<MenuController>().level = level;
    }
}
