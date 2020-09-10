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

    [SerializeField] MessageWindow messageWindow;
    [SerializeField] Sprite loseIcon;
    [SerializeField] Sprite winIcon;
    [SerializeField] Sprite goalIcon;

    Board board;

    bool isWinner = false;
    bool isReadyToReaload = false;
    bool isReadyToBegin = false;

    bool isGameOver = false;
    public bool IsGameOver
    {
        get
        {
            return isGameOver;
        }
        set
        {
            isGameOver = value;
        }
    }

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

        // Wait for the board refill
        yield return StartCoroutine("WaitForBoardRoutine", 0.5f);
        yield return StartCoroutine("EndGameRoutine");
    }

    public void StartGame()
    {
        isReadyToBegin = true;
    }

    IEnumerator StartGameRoutine()
    {
        if (messageWindow != null)
        {
            messageWindow.GetComponent<RectTransformMover>().MoveOn();
            messageWindow.ShowMessage(goalIcon, "score goal\n" + scoreGoal.ToString(), "Start");
        }

        while (!isReadyToBegin)
        {
            yield return null;
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
            if (ScoreManager.Instance != null)
            {
                if (ScoreManager.Instance.CurrentScore >= scoreGoal)
                {
                    isGameOver = true;
                    isWinner = true;
                }
            }
            if (movesLeft <= 0)
            {
                isGameOver = true;
                isWinner = false;
            }
            yield return null;
        }
    }

    IEnumerator WaitForBoardRoutine(float delay = 0f)
    {
        if (board != null)
        {
            yield return new WaitForSeconds(board.swapTime);
            while (board.isRefilling)
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(delay);
    }

    IEnumerator EndGameRoutine()
    {
        isReadyToReaload = false;

        if (isWinner)
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectTransformMover>().MoveOn();
                messageWindow.ShowMessage(winIcon, "You Win!", "OK");
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayRandomWinSFX();
            }
        }
        else
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectTransformMover>().MoveOn();
                messageWindow.ShowMessage(loseIcon, "You Lose!", "OK");
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayRandomLoseSFX();
            }
        }

        yield return new WaitForSeconds(1f);

        if (fader != null)
        {
            fader.FadeOn();
        }

        while (!isReadyToReaload)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null;
    }

    public void ReloadScene()
    {
        isReadyToReaload = true;
    }
}
