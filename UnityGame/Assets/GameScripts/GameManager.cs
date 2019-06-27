using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    UIManager uiManager;
    GameObject germsInstance;
    public List<Transform> germs;
    public int condition;

    public enum GermState {Idle, Scared, Washed, Angry, Dead};
    public GermState currentGermState = GermState.Idle;

    public float washingTimer = 0.0f;
    float nextUserTimer = 0.0f;
    bool resetProcess = false;
    bool germsInitialized = false;

    public GameObject waterflowParticle;
    public GameObject soapParticle;

    public GameObject gameLayer;
    public GameObject germsGroupPrefab;

    public GameObject waitingOnUserScreen;
    public GameObject cleanHandScreen;

    //TimeMarks:
    public float washingTime = 20.0f;
    public float deathFrequency = 1.0f;
    float lastDeath = 0.0f;

    GameStateManager stateManager;
    // Start is called before the first frame update
    private void Awake()
    {
        condition = 0;
        stateManager = this.GetComponent<GameStateManager>();
        uiManager = this.GetComponent<UIManager>();

        deathFrequency = washingTime / 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            onStartGameButton();
        }

        if (resetProcess)
        {
            timeOutReset();
        }
        else if (condition == 1)
        {
            stateHandler();
        }

    }

    void initGerms()
    {
        germsInitialized = false;
        if (germsInstance)
        {
            Destroy(germsInstance);
        }
        germs.Clear();

        germsInstance = Instantiate(germsGroupPrefab, GameObject.Find("GameLayer").GetComponent<Transform>());

        if (germsInstance)
        {
            foreach (Transform germ in germsInstance.GetComponent<Transform>())
            {
                if (germ.GetComponent<GermScript>())
                {
                    germs.Add(germ);
                }
            }
        }

        germsInitialized = true;
    }

    void stateHandler()
    {
        if (currentGermState != GermState.Dead)
        {
            switch (stateManager.getCurrentState())
            {
                case (GameStateManager.GameState.Idle):
                    idleMode();
                break;
                case (GameStateManager.GameState.Water):
                    waitingOnUserScreen.SetActive(false);
                    waterMode();
                break;
                case (GameStateManager.GameState.HandsWashing):
                    waitingOnUserScreen.SetActive(false);
                    washingMode();
                break;

            }
        }
        else
        {
            cleanHandScreen.SetActive(true);
            if (stateManager.getCurrentState() == GameStateManager.GameState.Idle)
            {
                resetProcess = true;
            }

            //Finish Game Screen
        }
    }

    void timeOutReset()
    {
        if(currentGermState == GermState.Angry && stateManager.getCurrentState() != GameStateManager.GameState.Idle)
        {
            restartGameLoop();
        }
        nextUserTimer += Time.deltaTime;
        if (nextUserTimer >= 4)
        {
            restartGameLoop();
        }
    }
    
    void idleMode(){
        if (currentGermState != GermState.Angry && currentGermState != GermState.Idle)
        {
            waterflowParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            soapParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            currentGermState = GermState.Angry;
            if (germsInitialized)
            {
                foreach (Transform germ in germs)
                {
                    germ.GetComponent<GermScript>().startAngryAnimation();
                }
            }


        }else if (currentGermState == GermState.Angry)
        {
            waterflowParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            soapParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            resetProcess = true;
        }
    }

    void waterMode(){


        if (currentGermState != GermState.Scared)
        {
            if (currentGermState == GermState.Idle)
            {
                waterflowParticle.GetComponent<ParticleSystem>().Play();
                soapParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                currentGermState = GermState.Scared;
                foreach (Transform germ in germs)
                {
                    germ.GetComponent<GermScript>().startScaredAnimation();
                }
            }
            else if (currentGermState != GermState.Angry)
            {
                waterflowParticle.GetComponent<ParticleSystem>().Play();
                soapParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                currentGermState = GermState.Angry;
                foreach (Transform germ in germs)
                {
                    germ.GetComponent<GermScript>().startAngryAnimation();
                }
            }
        }
    }

    void washingMode()
    {
        if (currentGermState != GermState.Washed)
        {
            waterflowParticle.GetComponent<ParticleSystem>().Play();
            soapParticle.GetComponent<ParticleSystem>().Play();

            currentGermState = GermState.Washed;
            if (germsInitialized)
            {
                foreach (Transform germ in germs)
                {
                    germ.GetComponent<GermScript>().startWashedAnimation();
                }
            }

        }

        washingTimer += Time.deltaTime;

        lastDeath -= Time.deltaTime;
        if (lastDeath < 0.0f)
        {
            if(germs.Count != 0)
            {
                int r = Random.Range(0, germs.Count - 1);
                germs[r].GetComponent<GermScript>().startDeath();
                germs.RemoveAt(r);
                GetComponent<UIManager>().increaseKillCounter();
                lastDeath = deathFrequency;
            }
            else
            {
                currentGermState = GermState.Dead;
            }
        }
    }

    void angryGermsMode()
    {
        if (currentGermState != GermState.Angry)
        {
            currentGermState = GermState.Angry;
            foreach (Transform germ in germs)
            {
                germ.GetComponent<GermScript>().startAngryAnimation();
            }
        }
    }

    void restartGameLoop()
    {
        waterflowParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        soapParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        nextUserTimer = 0.0f;
        washingTimer = 0.0f;
        currentGermState = GermState.Idle;
        initGerms();
        waitingOnUserScreen.SetActive(true);
        cleanHandScreen.SetActive(false);
        resetProcess = false;

    }

    public void onStartGameButton()
    {
        uiManager.turnOffBTUI();

        gameLayer.SetActive(true);
        restartGameLoop();
        condition = 1;
    }
}
