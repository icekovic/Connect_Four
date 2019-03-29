using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasMessageManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerWon;

    [SerializeField]
    private GameObject computerWon;

    [SerializeField]
    private GameObject draw;

    private void Awake()
    {
        playerWon.SetActive(false);
        computerWon.SetActive(false);
        draw.SetActive(false);
    }

    public void ShowPlayerWonMessage()
    {
        playerWon.SetActive(true);
    }

    public void ShowComputerWonMessage()
    {
        computerWon.SetActive(true);
    }

    public void ShowDrawMessage()
    {
        draw.SetActive(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("ConnectFour");
    }
}
