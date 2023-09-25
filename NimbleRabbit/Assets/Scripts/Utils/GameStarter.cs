using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameStarter : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() {
        // Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("yguo83");
        Time.timeScale = 1.0f;
    }
}
