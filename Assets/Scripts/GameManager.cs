using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] int moveLeft = 30;
    [SerializeField] int scoreGoal = 10000;
    [SerializeField] Fader fader;
    [SerializeField] Text levelNameText;

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

        StartCoroutine("ExecuteGameLoop");
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
    }

    IEnumerator PlayGameRoutine()
    {
        while (!isGameOver)
        {
            yield return null;
        }
    }

    IEnumerator EndGameRoutine()
    {
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
