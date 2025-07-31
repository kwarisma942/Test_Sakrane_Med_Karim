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
        StartCoroutine(RotateTo(transform, new Vector3(0, 0, 0), 2f, null));
    }

    // Reset the card's rotation
    public void ResetCard()
    {
        StartCoroutine(RotateTo(transform, new Vector3(0, 0, 0), 0.5f, null));
    }

    // Hide the card by moving it to the hide position and scaling it down
    public void HideCard(Transform hidePosition)
    {
        StartCoroutine(PunchRotation(transform, new Vector3(0, 0, 10f), 0.75f, 5));
        StartCoroutine(ScaleToZero(transform, 0.75f));
        StartCoroutine(MoveToPosition(transform, hidePosition.position, 0.75f));
        LevelManager.Instance.RemoveCard(Profile.Name);
    }

   

    #endregion

    #region Private Methods

    // Flip the card to reveal it
    private void FlipCard()
    {
        if (!canClick)
            return;
        SoundManager.Instance.PlaySoundFX(0);
        canClick = false; // Disable clicking while flipping
        FrontSprite.sprite = Profile.Sprite;
        Color color = FrontSprite.color;
        color.a = 1f; // Set alpha to fully visible
        FrontSprite.color = color;
        StartCoroutine(RotateTo(transform, new Vector3(0, 180, 0), 1, Check));

    }

    // Check if the card matches another card
    private void Check()
    {
        GameManager.Instance.CheckCard(this);
        canClick = true;
    }

    #endregion

    #region Motion Behaviour

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

    IEnumerator PunchRotation(Transform target, Vector3 punchAngle, float duration, int vibrato)
    {
        Quaternion originalRotation = target.rotation;
        float time = 0f;

        while (time < duration)
        {
            float normalizedTime = time / duration;
            float damper = 1f - normalizedTime; // decay over time
            float angle = Mathf.Sin(normalizedTime * vibrato * Mathf.PI * 2) * damper;

            target.rotation = originalRotation * Quaternion.Euler(punchAngle * angle);

            time += Time.deltaTime;
            yield return null;
        }

        target.rotation = originalRotation;
    }

    IEnumerator ScaleToZero(Transform target, float duration)
    {
        Vector3 startScale = target.localScale;
        Vector3 endScale = Vector3.zero;
        float time = 0f;

        while (time < duration)
        {
            target.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = endScale;
    }

    IEnumerator MoveToPosition(Transform target, Vector3 destination, float duration)
    {
        Vector3 startPos = target.position;
        float time = 0f;

        while (time < duration)
        {
            target.position = Vector3.Lerp(startPos, destination, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        target.position = destination;

        Destroy(this.gameObject);
    }
    #endregion

}
