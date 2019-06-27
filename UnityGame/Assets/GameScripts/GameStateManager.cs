using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public enum GameState { Idle, Water, HandsWashing};
    GameState currentState = GameState.Idle;
    Logger logger;
    float handwashTimer;
    float waterflowTimer;

    // Update is called once per frame
    void Update()
    {
        if (currentState != GameState.Idle) {waterflowTimer += Time.deltaTime;}
        if(currentState == GameState.HandsWashing) { handwashTimer += Time.deltaTime; }
        gameStateManager();
    }

    private void Start()
    {
        waterflowTimer = 0.0f;
        handwashTimer = 0.0f;
        logger = FindObjectOfType<Logger>();
    }

    void gameStateManager()
    {
        if (Input.GetKeyDown("space"))
        { 
            if (currentState == GameState.Idle)
            {
                initWaterflowState();
            }
            else if (currentState == GameState.Water || currentState == GameState.HandsWashing)
            {
                initIdleState();
            }
            Debug.Log(currentState);
        }
        if (Input.GetKeyDown("h"))
        {
            if (currentState == GameState.Water)
            {
                initHandwashingState();
            }
            else if (currentState == GameState.HandsWashing)
            {
                initWaterflowState();
            }

            Debug.Log(currentState);
        }
        //Debug.Log(currentState);
    }

    void initWaterflowState()
    {
        if(currentState != GameState.Water)
        {
            if (currentState == GameState.HandsWashing) { logHandwashDuration(); }
            currentState = GameState.Water;
            logEvent();
        }

    }

    void initIdleState()
    {
        if(currentState != GameState.Idle)
        {
            if (currentState == GameState.Water) { logWaterflowDuration(); }
            else if (currentState == GameState.HandsWashing)
            {
                logWaterflowDuration();
                logHandwashDuration();
            }
            logger.logState("newUser");
            currentState = GameState.Idle;
            logEvent();
        }
    }

    void initHandwashingState()
    {
        if(currentState != GameState.HandsWashing)
        {
            currentState = GameState.HandsWashing;
            logEvent();
        }

    }

    void logEvent()
    {
        logger.logEvent(currentState.ToString());
    }

    void logWaterflowDuration()
    {
        logger.logState("waterflow", waterflowTimer);
        Debug.Log("waterflow " + waterflowTimer);
        waterflowTimer = 0.0f;

    }

    void logHandwashDuration()
    {
        logger.logState("handwash", handwashTimer);
        Debug.Log("handwash " + handwashTimer);
        handwashTimer = 0.0f;
        
    }

    public void setGameState(string s){
        switch (s)
        {
            case "0":
                initIdleState();
                break;
            case "1":
                initWaterflowState();
                break;
            case "2":
                initHandwashingState();
                break;
        }
    }

    public GameState getCurrentState(){
        return currentState;
    }



}
