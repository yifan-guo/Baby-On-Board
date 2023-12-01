using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameStarter : MonoBehaviour
{
    private GameObject GameTitle;
    private GameObject Startbutton;
    private GameObject Quitbutton;
    private GameObject intro00Story;  
    private GameObject NextButton;
    private GameObject BackButton;
    private GameObject LetsPlayButton;

    void Awake()
    {
        GameTitle = transform.Find("GameNameText").gameObject;
        Startbutton = transform.Find("StartButton").gameObject;
        Quitbutton = transform.Find("QuitButton").gameObject;

        intro00Story = transform.Find("intro00Story").gameObject;
        NextButton = transform.Find("NextButton").gameObject;
        BackButton = transform.Find("BackButton").gameObject;
        LetsPlayButton = transform.Find("LetsPlayButton").gameObject;

        GameTitle.SetActive(true);
        Startbutton.SetActive(true);
        Quitbutton.SetActive(true);

        intro00Story.SetActive(false);
        NextButton.SetActive(false);
        BackButton.SetActive(false);
        LetsPlayButton.SetActive(false);
    }


    void Update() 
    {
        

    }

    public void StartGame()
    {
        //SceneManager.LoadScene("Level1");
        //Time.timeScale=1.0f;

        //turn off the game title, Play button and Quit button, start game instructions
        GameTitle.SetActive(false);
        Startbutton.SetActive(false);
        Quitbutton.SetActive(false);

        intro00Story.SetActive(true);
        NextButton.SetActive(true);
        LetsPlayButton.SetActive(true);

    }

    

}
