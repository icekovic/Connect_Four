using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flags : MonoBehaviour
{
    private bool isPlayersTurn = true;
    private bool isLoading = true;
    private bool isDropping = false;
    private bool mouseButtonPressed = false;
    private bool gameOver = false;
    private bool isCheckingForWinner = false;   

    public void SetIsPlayersTurnTrue()
    {
        isPlayersTurn = true;
    }

    public void SetIsPlayersTurnFalse()
    {
        isPlayersTurn = false;
    }

    public void ConvertIsPlayersTurnToBoolean()
    {
        isPlayersTurn = Convert.ToBoolean(UnityEngine.Random.Range(0, 1));
    }

    public void InvertIsPlayersTurn()
    {
        isPlayersTurn = !isPlayersTurn;
    }

    public void SetIsLoadingTrue()
    {
        isLoading = true;
    }

    public void SetIsLoadingFalse()
    {
        isLoading = false;
    }

    public void SetIsDroppingTrue()
    {
        isDropping = true;
    }

    public void SetIsDroppingFalse()
    {
        isDropping = false;
    }

    public void SetIsMouseButtonPressedTrue()
    {
        mouseButtonPressed = true;
    }

    public void SetIsMouseButtonPressedFalse()
    {
        mouseButtonPressed = false;
    }

    public void SetGameOverTrue()
    {
        gameOver = true;
    }

    public void SetGameOverFalse()
    {
        gameOver = false;
    }

    public void SetIsCheckingForWinnerTrue()
    {
        isCheckingForWinner = true;
    }

    public void SetIsCheckingForWinnerFalse()
    {
        isCheckingForWinner = false;
    }

    public bool GetIsPlayersTurn()
    {
        return isPlayersTurn;
    }

    public bool GetIsLoading()
    {
        return isLoading;
    }

    public bool GetIsDropping()
    {
        return isDropping;
    }

    public bool GetMouseButtonPressed()
    {
        return mouseButtonPressed;
    }

    public bool GetGameOver()
    {
        return gameOver;
    }

    public bool GetIsCheckingForWinner()
    {
        return isCheckingForWinner;
    }
}
