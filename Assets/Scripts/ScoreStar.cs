﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreStar : MonoBehaviour
{
    [SerializeField] Image starImage;
    [SerializeField] float delay = 0.25f;
    [SerializeField] AudioClip starSFX;
    [SerializeField] bool isActivated = false;

    void Start()
    {
        SetActive(false);
        StartCoroutine(TestRoutine());
    }

    void SetActive(bool state)
    {
        if (starImage != null)
        {
            starImage.gameObject.SetActive(state);
        }
    }

    public void Activate()
    {
        if (isActivated)
        {
            return;
        }
        StartCoroutine(ActivateRoutine());
    }

    IEnumerator ActivateRoutine()
    {
        isActivated = true;

        if (starSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClipAtPoint(starSFX, Vector2.zero, AudioManager.Instance.sfxVolume);
        }

        yield return new WaitForSeconds(delay);
        SetActive(true);
    }

    IEnumerator TestRoutine()
    {
        yield return new WaitForSeconds(3f);
        Activate();
    }
}
