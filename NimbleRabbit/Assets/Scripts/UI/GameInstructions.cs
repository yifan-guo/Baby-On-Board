using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameInstructions : MonoBehaviour
{
    private GameObject intro00Story;
    private GameObject intro01Player;
    private GameObject intro02Package;
    private GameObject intro03Indicator;
    private GameObject intro04DeliveryZone;
    private GameObject intro05Bandit;
    private GameObject intro06Cop;
    private GameObject intro07Boost;
    private GameObject intro08Health;
    private GameObject intro09Controls;

    private GameObject NextButton;
    private GameObject BackButton;
    private GameObject LetsPlayButton;

    //private TextMeshProUGUI nextButtonText;

    public int showIndex;

    // Start is called before the first frame update
    void Awake()
    {
        showIndex = 0;

        intro00Story = transform.Find("intro00Story").gameObject;
        intro01Player = transform.Find("intro01Player").gameObject;
        intro02Package = transform.Find("intro02Package").gameObject;
        intro03Indicator = transform.Find("intro03Indicator").gameObject;
        intro04DeliveryZone = transform.Find("intro04DeliveryZone").gameObject;
        intro05Bandit = transform.Find("intro05Bandit").gameObject;
        intro06Cop = transform.Find("intro06Cop").gameObject;
        intro07Boost = transform.Find("intro07Boost").gameObject;
        intro08Health = transform.Find("intro08Health").gameObject;
        intro09Controls = transform.Find("intro09Controls").gameObject;

        NextButton = transform.Find("NextButton").gameObject;
        BackButton = transform.Find("BackButton").gameObject;

        LetsPlayButton = transform.Find("LetsPlayButton").gameObject;

        intro00Story.SetActive(false);
        intro01Player.SetActive(false);
        intro02Package.SetActive(false);
        intro03Indicator.SetActive(false);
        intro04DeliveryZone.SetActive(false);
        intro05Bandit.SetActive(false);
        intro06Cop.SetActive(false);
        intro07Boost.SetActive(false);
        intro08Health.SetActive(false);
        intro09Controls.SetActive(false);   
        //NextButton.SetActive(false);
        //BackButton.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickNextButton()
    {
        showIndex ++;

        if (showIndex ==0)
        { 
            BackButton.SetActive(false);
            NextButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(NextButton);
        }
        else if((showIndex > 0) && (showIndex < 9))
        {
            BackButton.SetActive(true);
            NextButton.SetActive(true);
        }
        else if(showIndex == 9) 
        {
            BackButton.SetActive(true);
            NextButton.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(BackButton);
        }

        if ((showIndex >=0) && (showIndex <=9)) 
        {
            switch (showIndex)
            {
                case 1:
                    intro00Story.SetActive(false);
                    intro01Player.SetActive(true);
                    break;

                case 2:
                    intro01Player.SetActive(false);
                    intro02Package.SetActive(true);
                    break;

                case 3:
                    intro02Package.SetActive(false);
                    intro03Indicator.SetActive(true);
                    break;

                case 4:
                    intro03Indicator.SetActive(false);
                    intro04DeliveryZone.SetActive(true);
                    break;

                case 5:
                    intro04DeliveryZone.SetActive(false);
                    intro05Bandit.SetActive(true);
                    break;

                case 6:
                    intro05Bandit.SetActive(false);
                    intro06Cop.SetActive(true);
                    break;

                case 7:
                    intro06Cop.SetActive(false);
                    intro07Boost.SetActive(true);
                    break;

                case 8:
                    intro07Boost.SetActive(false);
                    intro08Health.SetActive(true);
                    break;

                case 9:
                    intro08Health.SetActive(false);
                    intro09Controls.SetActive(true);
                    //change button text, remind player the game will start
                    //nextButtonText = NextButton.GetComponentInChildren<TextMeshProUGUI>();
                    //nextButtonText.text = "END";
                    OverwriteBackButtonNavForFinalMenu();
                    OverWritePlayButtonForFinalMenu();
                    break;

            }
        }

    }

    public void ClickBackButton()
    {
        ResetBackButtonNav();
        ResetPlayButtonNav();
        showIndex--;

        if (showIndex == 0)
        {
            BackButton.SetActive(false);
            NextButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(NextButton);
        }
        else if ((showIndex > 0) && (showIndex < 9))
        {
            BackButton.SetActive(true);
            NextButton.SetActive(true);
        }
        else if (showIndex == 9)
        {
            BackButton.SetActive(true);
            NextButton.SetActive(false);
        }

        if ((showIndex >= 0) && (showIndex <= 9))
        {
            switch (showIndex)
            {
                case 0:
                    intro00Story.SetActive(true);
                    intro01Player.SetActive(false);
                    break;

                case 1:
                    intro01Player.SetActive(true);
                    intro02Package.SetActive(false);
                    break;

                case 2:
                    intro02Package.SetActive(true);
                    intro03Indicator.SetActive(false);
                    break;

                case 3:
                    intro03Indicator.SetActive(true);
                    intro04DeliveryZone.SetActive(false);
                    break;

                case 4:
                    intro04DeliveryZone.SetActive(true);
                    intro05Bandit.SetActive(false);
                    break;

                case 5:
                    intro05Bandit.SetActive(true);
                    intro06Cop.SetActive(false);
                    break;

                case 6:
                    intro06Cop.SetActive(true);
                    intro07Boost.SetActive(false);
                    break;

                case 7:
                    intro07Boost.SetActive(true);
                    intro08Health.SetActive(false);
                    break;

                case 8:
                    intro08Health.SetActive(true);
                    intro09Controls.SetActive(false);
                    break;


            }

        }

    }


    public void LoadLvl1()
    {
        SceneManager.LoadScene("Level1");
        Time.timeScale = 1.0f;
    }

    private void OverwriteBackButtonNavForFinalMenu()
    {
        Navigation newButtonNav = new Navigation();
        newButtonNav.mode = Navigation.Mode.Explicit;
        newButtonNav.selectOnRight = LetsPlayButton.GetComponent<Button>();
        newButtonNav.selectOnLeft = LetsPlayButton.GetComponent<Button>();
        newButtonNav.selectOnDown = LetsPlayButton.GetComponent<Button>();
        newButtonNav.selectOnUp = LetsPlayButton.GetComponent<Button>();
        BackButton.GetComponent<Button>().navigation = newButtonNav;
    }

    private void ResetBackButtonNav()
    {
        Navigation newButtonNav = new Navigation();
        newButtonNav.mode = Navigation.Mode.Explicit;
        newButtonNav.selectOnRight = NextButton.GetComponent<Button>();
        newButtonNav.selectOnUp = NextButton.GetComponent<Button>();
        BackButton.GetComponent<Button>().navigation = newButtonNav;
    }

    private void OverWritePlayButtonForFinalMenu()
    {
        Navigation newButtonNav = new Navigation();
        newButtonNav.mode = Navigation.Mode.Explicit;
        newButtonNav.selectOnRight = BackButton.GetComponent<Button>();
        newButtonNav.selectOnLeft = BackButton.GetComponent<Button>();
        newButtonNav.selectOnUp = BackButton.GetComponent<Button>();
        newButtonNav.selectOnDown = BackButton.GetComponent<Button>();
        LetsPlayButton.GetComponent<Button>().navigation = newButtonNav;
    }

    private void ResetPlayButtonNav()
    {
        Navigation newButtonNav = new Navigation();
        newButtonNav.mode = Navigation.Mode.Explicit;
        newButtonNav.selectOnLeft = NextButton.GetComponent<Button>();
        newButtonNav.selectOnDown = NextButton.GetComponent<Button>();
        LetsPlayButton.GetComponent<Button>().navigation = newButtonNav;
    }

}
