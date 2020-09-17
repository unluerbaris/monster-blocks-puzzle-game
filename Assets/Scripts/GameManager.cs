using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{
    //public int movesLeft = 30;
    //[SerializeField] int scoreGoal = 10000;
    [SerializeField] Fader fader;
    [SerializeField] Text levelNameText;
    [SerializeField] Text movesLeftText;

    [SerializeField] MessageWindow messageWindow;
    [SerializeField] ScoreMeter scoreMeter;

    Board board;
    LevelGoal levelGoal;

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

    public override void Awake()
    {
        base.Awake();
        levelGoal = GetComponent<LevelGoal>();
        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
    }

    private void Start()
    {
        if (scoreMeter != null)
        {
            scoreMeter.SetupStars(levelGoal);
        }

        Scene scene = SceneManager.GetActiveScene();

        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }

        levelGoal.movesLeft++;
        UpdateMoves();
        StartCoroutine("ExecuteGameLoop");
    }

    public void UpdateMoves()
    {
        levelGoal.movesLeft--;

        if (movesLeftText != null)
        {
            movesLeftText.text = levelGoal.movesLeft.ToString();
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
            int maxGoal = levelGoal.scoreGoals.Length - 1;
            messageWindow.ShowScoreMessage(levelGoal.scoreGoals[0]);
            messageWindow.ShowMoves(levelGoal.movesLeft);
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
            isGameOver = levelGoal.IsGameOver();
            isWinner = levelGoal.IsWinner();
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
        isWinner = levelGoal.IsWinner();

        if (isWinner)
        {
            ShowWinScreen();
        }
        else
        {
            ShowLoseScreen();
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

    private void ShowLoseScreen()
    {
        if (messageWindow != null)
        {
            messageWindow.GetComponent<RectTransformMover>().MoveOn();
            messageWindow.ShowLoseMessage();

            if (messageWindow.goalFailedIcon != null)
            {
                string caption = "out of moves!";
                messageWindow.ShowInfo(caption, messageWindow.goalFailedIcon);
            }
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRandomLoseSFX();
        }
    }

    private void ShowWinScreen()
    {
        if (messageWindow != null)
        {
            messageWindow.GetComponent<RectTransformMover>().MoveOn();
            messageWindow.ShowWinMessage();

            if (ScoreManager.Instance != null)
            {
                string scoreString = "you scored\n" + ScoreManager.Instance.CurrentScore.ToString() + " points!";
                messageWindow.ShowInfo(scoreString, messageWindow.goalCompleteIcon);
            }
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRandomWinSFX();
        }
    }

    public void ReloadScene()
    {
        isReadyToReaload = true;
    }

    public void ScorePoints(Block block, int multiplier = 1, int bonus = 0)
    {
        if (block == null) return;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(block.scoreValue * multiplier + bonus);
            levelGoal.UpdateScoreStars(ScoreManager.Instance.CurrentScore);

            if (scoreMeter != null)
            {
                scoreMeter.UpdateScoreMeter(ScoreManager.Instance.CurrentScore, levelGoal.scoreStars);
            }
        }

        if (AudioManager.Instance != null && block.clearSound != null)
        {
            AudioManager.Instance.PlayClipAtPoint(block.clearSound, Vector2.zero, AudioManager.Instance.sfxVolume);
        }
    }
}
