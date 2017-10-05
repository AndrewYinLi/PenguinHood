using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    RectTransform TopHalf;
    RectTransform BottomHalf;
    RectTransform Levels;
    Image fadeBlack;

    const int NEUTRAL = 0;
    const int TRANSITION_OUT_LEVELS = 1;
    const int TRANSITION_IN_LEVELS = 2;
    const int TRANSITION_OUT_MENU = 3;
    const int TRANSITION_IN_MENU = 4;
    const int FADE_BLACK = 5;

    public int level;

    int state = 0;
    bool backEnabled = false;
    
    float time;

    // Use this for initialization
    void Start () {
        TopHalf = transform.GetChild(0).GetComponent<RectTransform>();
        BottomHalf = transform.GetChild(1).GetComponent<RectTransform>();
        Levels = transform.GetChild(2).GetComponent<RectTransform>();
        fadeBlack = transform.GetChild(3).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case TRANSITION_OUT_LEVELS:
                {
                    if (Levels.localPosition.x > -680f)
                    {
                        Levels.localPosition = new Vector2(Levels.localPosition.x - 20f, 0);
                    }
                    else
                    {
                        Levels.localPosition = new Vector2(-680, 0);
                        if (backEnabled)
                        {
                            backEnabled = false;
                            state = TRANSITION_IN_MENU;
                        }
                        else
                        {
                            state = NEUTRAL;
                        }
                    }
                }
                break;
            case TRANSITION_IN_LEVELS:
                if (Levels.localPosition.x < 0)
                {
                    Levels.localPosition = new Vector2(Levels.localPosition.x + 20f, 0);
                }
                else
                {
                    Levels.localPosition = new Vector2(0, 0);
                    state = NEUTRAL;
                }
                break;
            case TRANSITION_OUT_MENU:
                if(TopHalf.localPosition.y < 290f)
                {
                    TopHalf.localPosition = new Vector2(0, TopHalf.localPosition.y + 25f);
                    BottomHalf.localPosition = new Vector2(0, BottomHalf.localPosition.y - 25f);
                }
                else
                {
                    state = TRANSITION_IN_LEVELS;
                }
                break;
            case TRANSITION_IN_MENU:
                if (TopHalf.localPosition.y > 0f)
                {
                    TopHalf.localPosition = new Vector2(0, TopHalf.localPosition.y - 25f);
                    BottomHalf.localPosition = new Vector2(0, BottomHalf.localPosition.y + 25f);
                }
                else
                {
                    TopHalf.localPosition = new Vector2(0, 0);
                    BottomHalf.localPosition = new Vector2(0, 0);
                    state = NEUTRAL;
                }
                break;
            case FADE_BLACK:
                if(fadeBlack.color.a < 1)
                {
                    fadeBlack.color = new Color(0,0,0,fadeBlack.color.a+0.01f);
                }
                else
                {
                    state = NEUTRAL;
                    SceneManager.LoadScene("Level_" + level);             
                }
                break;

        }
    }

    public void LoadLevel()
    {
        if (state == NEUTRAL)
        {
            fadeBlack.transform.localPosition = new Vector2(0, 0);
            state = FADE_BLACK;
        }
    }

    public void PlayButton()
    {
        state = TRANSITION_OUT_MENU;
    }

    public void BackButton()
    {
        backEnabled = true;
        state = TRANSITION_OUT_LEVELS;
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Level_Select");
    }

    public void LoadInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }


}
