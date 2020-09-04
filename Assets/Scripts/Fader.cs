using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class Fader : MonoBehaviour
{
    [SerializeField] float solidAlpha = 1f;
    [SerializeField] float clearAlpha = 0f;
    [SerializeField] float delay = 0f;
    [SerializeField] float timeToFade = 1f;

    MaskableGraphic maskableGraphic;

    void Start()
    {
        maskableGraphic = GetComponent<MaskableGraphic>();
        //FadeOff();
    }

    IEnumerator FadeRoutine(float alpha)
    {
        yield return new WaitForSeconds(delay);
        maskableGraphic.CrossFadeAlpha(alpha, timeToFade, true);
    }

    public void FadeOn()
    {
        StartCoroutine(FadeRoutine(solidAlpha));
    }

    public void FadeOff()
    {
        StartCoroutine(FadeRoutine(clearAlpha));
    }
}
