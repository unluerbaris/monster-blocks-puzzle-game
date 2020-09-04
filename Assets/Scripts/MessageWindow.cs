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
}
