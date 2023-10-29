using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameStarter : MonoBehaviour
{
    

    void Start()
    {
         

    }


    void Update() 
    {
        
 
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
        Time.timeScale=1.0f;
    }

}
