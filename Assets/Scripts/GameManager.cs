using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int movesLeft = 30;
    [SerializeField] int scoreGoal = 10000;
    [SerializeField] Fader fader;
    [SerializeField] Text levelNameText;
    [SerializeField] Text movesLeftText;

    Board board;

    bool isReadyToBegin = false;
    bool isGameOver = false;
    bool isWinner = false;

    private void Start()
    {
        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
        Scene scene = SceneManager.GetActiveScene();

        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }

        UpdateMoves();
        StartCoroutine("ExecuteGameLoop");
    }

    public void UpdateMoves()
    {
        if (movesLeftText != null)
        {
            movesLeftText.text = movesLeft.ToString();
        }
    }

    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");
        yield return StartCoroutine("EndGameRoutine");
    }

    IEnumerator StartGameRoutine()
    {
        while (!isReadyToBegin)
        {
            yield return null;
            yield return new WaitForSeconds(2f);
            isReadyToBegin = true;
        }
        if (fader != null)
        {
            fader.FadeOff();
        }

        yield return new WaitForSeconds(1f);

        if (board != null)
        {
            board.SetupBoard();
        }
    }

    IEnumerator PlayGameRoutine()
    {
        while (!isGameOver)
        {
            if (movesLeft <= 0)
            {
                isGameOver = true;
                isWinner = false;
            }
            yield return null;
        }
    }

    IEnumerator EndGameRoutine()
    {
        if (fader != null)
        {
            fader.FadeOn();
        }
        if (isWinner)
        {
            Debug.Log("You win!");
        }
        else
        {
            Debug.Log("Game over");
        }
        yield return null;
    }
}
