using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(RectTransformMover))]
public class MessageWindow : MonoBehaviour
{
    [SerializeField] Image messageIcon;
    [SerializeField] Text messageText;
    [SerializeField] Text buttonText;
    [SerializeField] Sprite loseIcon;
    [SerializeField] Sprite winIcon;
    [SerializeField] Sprite goalIcon;
    public Sprite goalCompleteIcon;
    public Sprite goalFailedIcon;
    [SerializeField] Sprite movesIcon;
    [SerializeField] Image infoImage;
    [SerializeField] Text infoText;

    public void ShowMessage(Sprite sprite = null, string message = "", string buttonMessage = "Start")
    {
        if (messageIcon != null)
        {
            messageIcon.sprite = sprite;
        }
        if (messageText != null)
        {
            messageText.text = message;
        }
        if (buttonText != null)
        {
            buttonText.text = buttonMessage;
        }
    }

    public void ShowScoreMessage(int scoreGoal)
    {
        string message = "score goal \n" + scoreGoal.ToString();
        ShowMessage(goalIcon, message, "start");
    }

    public void ShowWinMessage()
    {
        ShowMessage(winIcon, "level\ncomplete", "ok");
    }

    public void ShowLoseMessage()
    {
        ShowMessage(loseIcon, "level\nfailed", "ok");
    }

    public void ShowInfo(string caption = "", Sprite icon = null)
    {
        if (infoText != null && caption != "")
        {
            infoText.text = caption;
        }
        if (infoImage != null && icon != null)
        {
            infoImage.sprite = icon;
        }
    }

    public void ShowMoves(int moves)
    {
        string caption = moves.ToString() + " moves";
        ShowInfo(caption, movesIcon);
    }
}
