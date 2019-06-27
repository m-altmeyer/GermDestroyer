using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gm;
    GameStateManager stateManager;
    public GameObject bluetoothCanvas;
    public Animator clockAnimator;

    public CentralNordicScript bluetoothStatusScript;

    CanvasGroup bluetoothCG;
    bool bluetoothUIstatus = true;

    public Slider slider;

    public Text text;

    public GameObject waterIndicator;
    public GameObject handIndicator;
    public GameObject bluetoothIndicator;

    // Start is called before the first frame update
    void Start()
    {
        bluetoothCG = bluetoothCanvas.GetComponent<CanvasGroup>();
        gm = GetComponent<GameManager>();
        stateManager = GetComponent<GameStateManager>();

        if (PlayerPrefs.HasKey("TodayDate"))
        {
            if (PlayerPrefs.GetString("TodayDate") != System.DateTime.Now.ToString("dd-MM-yyyy")){
                PlayerPrefs.SetString("TodayDate", System.DateTime.Now.ToString("dd-MM-yyyy"));
                PlayerPrefs.SetFloat("KillCount", 0);
            }
        }
        else
        {
            PlayerPrefs.SetString("TodayDate", System.DateTime.Now.ToString("dd-MM-yyyy"));
            PlayerPrefs.SetFloat("KillCount", 0);
        }
        text.text = "Germs Killed Today: " + PlayerPrefs.GetFloat("KillCount").ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown("1"))
        //{
        //    if (bluetoothUIstatus){
        //        turnOffBTUI();
        //    }
        //    else {
        //        turnOnBTUI();
        //    }
        //}

        refreshUI();
    }

    public void turnOffBTUI()
    {
        turnOffUI(bluetoothCG);
        bluetoothUIstatus = false;
    }

    public void turnOnBTUI()
    {
        turnOnUI(bluetoothCG);
        bluetoothUIstatus = true;
    }


    void turnOffUI(CanvasGroup cg)
    {
        cg.interactable = true;
        cg.blocksRaycasts = false;
        cg.alpha = 0;
    }

    void turnOnUI(CanvasGroup cg)
    {
        cg.interactable = true;
        cg.blocksRaycasts = true;
        cg.alpha = 1;
    }

    public void increaseKillCounter()
    {
        float killcount = PlayerPrefs.GetFloat("KillCount");
        PlayerPrefs.SetFloat("KillCount", killcount+1);
        text.text = "Germs Killed Today: " + PlayerPrefs.GetFloat("KillCount").ToString();
    }

    void refreshUI()
    {
        slider.value = gm.washingTimer / gm.washingTime;
        if(gm.currentGermState == GameManager.GermState.Washed)
        {
            clockAnimator.SetBool("Vibrate", true);
        }
        else
        {
            clockAnimator.SetBool("Vibrate", false);
        }

        GameStateManager.GameState currentState = stateManager.getCurrentState();
        if(currentState == GameStateManager.GameState.HandsWashing)
        {
            handIndicator.SetActive(true);
        }
        else
        {
            handIndicator.SetActive(false);
        }

        if(currentState != GameStateManager.GameState.Idle)
        {
            waterIndicator.SetActive(true);
        }
        else
        {
            waterIndicator.SetActive(false);
        }

       if(bluetoothStatusScript.getConnected())
        {
            bluetoothIndicator.SetActive(false);
        }
        else
        {
            bluetoothIndicator.SetActive(true);
        }
    }
}
