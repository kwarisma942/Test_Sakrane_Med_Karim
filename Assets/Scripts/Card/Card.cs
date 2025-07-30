using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    #region Public Variables

    public CardProfile Profile; // Profile of the card, including sprite and other properties
    public SpriteRenderer FrontSprite;

    #endregion

    #region Private Variables

    bool canClick = true; // Flag to check if the card can be clicked

    #endregion

    #region MonoBehaviour Methods

    private void Start()
    {
        Init();
    }

    public void OnMouseDown()
    {
        FlipCard();
    }
    #endregion

    #region Public Methods

    // Initialize the card with its profile and set up its visual appearance
    public void Init()
    {
        FrontSprite.sprite = Profile.Sprite;
        Color color = FrontSprite.color;
        color.a = 1f; // Set alpha to fully visible
        FrontSprite.color = color;
        StartCoroutine(PunchScale(transform, new Vector3(1.5f, 1.5f, 1.5f), 1f));
        StartCoroutine(RotateTo(transform, new Vector3(0, 0, 0), 2f,null));
    }

    #endregion

    #region Private Methods

    // Flip the card to reveal it
    private void FlipCard()
    {
        if (!canClick)
            return;

        canClick = false; // Disable clicking while flipping
        FrontSprite.sprite = Profile.Sprite;
        Color color = FrontSprite.color;
        color.a = 1f; 
        FrontSprite.color = color;
        StartCoroutine(RotateTo(transform, new Vector3(0, 180, 0), 1, null));

    }

    #endregion

    #region Animation Methods

    IEnumerator PunchScale(Transform target, Vector3 punchAmount, float duration)
    {
        Vector3 startScale = target.localScale;
        float time = 0f;

        while (time < duration)
        {
            float strength = Mathf.Sin((time / duration) * Mathf.PI); // creates punch curve
            target.localScale = startScale + punchAmount * strength;
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = startScale; // reset to original
    }

    IEnumerator RotateTo(Transform target, Vector3 targetEulerAngles, float duration, Action action)
    {
        Quaternion startRotation = target.rotation;
        Quaternion endRotation = Quaternion.Euler(targetEulerAngles);
        float time = 0f;

        while (time < duration)
        {
            target.rotation = Quaternion.Lerp(startRotation, endRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        target.rotation = endRotation;
        if (action != null)
            action();
    }

  
    #endregion

}
