using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class GameManager : MonoBehaviour
{
    #region Public Variables

    public static GameManager Instance;
    public bool InGame;

    public Text ScoreTxt, ComboTxt, WinScoreTxt, MaxComboTxt, TimerTxt;
    public Image TimerBar;

    public int Level;
    public int Score, Combo, MaxCombo;
    public float ComboValue, TimerValue = 60;
    public Transform ComboBar;
    public GameObject MainScreen, WinScreen, LoseScreen, GameplayScreen;
    public Animation TimerMotion;

    #endregion

    #region Private Variables

    [SerializeField]
    private Transform hideCardPosition;
    private int valideCardCount;
    private Card SelectCard;
    private StringBuilder sb = new StringBuilder();
    private Vector3 comboBarOriginalScale;
    private Vector3 scoreTxtOriginalScale;
    private Vector3 comboTxtOriginalScale;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        comboBarOriginalScale = ComboBar.parent.localScale;
        scoreTxtOriginalScale = ScoreTxt.transform.localScale;
        comboTxtOriginalScale = ComboTxt.transform.localScale;
    }

    private void Update()
    {
        if (!InGame) return;

        if (TimerValue >= 0)
        {
            TimerValue -= Time.deltaTime;
            PlayerPrefs.SetFloat("TimerValue_" + Level, TimerValue);
            sb.Clear().AppendFormat("{0:00}", TimerValue);
            TimerTxt.text = sb.ToString();
            TimerBar.fillAmount = Mathf.Clamp01(TimerValue / 60);
        }
        else
        {
            Lose();
        }

        if (ComboValue >= 0)
        {
            ComboValue -= Time.deltaTime * 0.05f;
            PlayerPrefs.SetFloat("ComboValue" + Level, ComboValue);
            Vector3 scale = ComboBar.localScale;
            scale.x = ComboValue;
            ComboBar.localScale = scale;
        }
        else
        {
            RemoveComboStrike();
        }
    }

    #endregion

    #region Public Methods

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    public void ContinueLevel(int level)
    {
        Level = level;
        InitValue();
        LevelManager.Instance.ContinueLevel();
        MainScreen.SetActive(false);
        GameplayScreen.SetActive(true);
    }

    public void GenerateLevel(int level)
    {
        Level = level;
        InitValue();
        valideCardCount = PlayerPrefs.GetInt("valideCardCount_" + Level, 0);
        LevelManager.Instance.GenerateLevel();
        MainScreen.SetActive(false);
        GameplayScreen.SetActive(true);
    }

    public IEnumerator CheckLevel()
    {
        yield return new WaitForSeconds(1);
        if (LevelManager.Instance.IsVide())
            Win();
    }

    public void CheckCard(Card card)
    {
        if (SelectCard)
        {
            if (SelectCard.Profile == card.Profile && SelectCard != card)
            {
                SelectCard.HideCard(hideCardPosition);
                card.HideCard(hideCardPosition);
                valideCardCount += 2;
                PlayerPrefs.SetInt("valideCardCount_" + Level, valideCardCount);
                AddScore();
                SelectCard = null;
                SoundManager.Instance.PlaySoundFX(1);
            }
            else
            {
                SelectCard.ResetCard();
                card.ResetCard();
                SelectCard = null;
                SoundManager.Instance.PlaySoundFX(2);
            }
            StartCoroutine(CheckLevel());
        }
        else
        {
            SelectCard = card;
        }
    }

    #endregion

    #region Private Methods

    private void Lose()
    {
        GameplayScreen.SetActive(false);
        WinScoreTxt.text = Score.ToString();
        MaxComboTxt.text = "X " + MaxCombo.ToString();
        LoseScreen.SetActive(true);
        InGame = false;
        ResetValue();
    }

    private void Win()
    {
        GameplayScreen.SetActive(false);
        WinScoreTxt.text = Score.ToString();
        MaxComboTxt.text = "X " + MaxCombo.ToString();
        WinScreen.SetActive(true);
        InGame = false;
        ResetValue();
    }

    private void AddScore()
    {
        Score += (1 * Combo);
        TimerValue += 5;
        TimerMotion.Play();
        ScoreTxt.text = Score.ToString();
        PlayerPrefs.SetInt("Score_" + Level, Score);

        StartCoroutine(PunchScale(ScoreTxt.transform, scoreTxtOriginalScale));
        AddComboStrike();
    }

    private void AddComboStrike()
    {
        ComboValue += 0.4f;
        Vector3 scale = ComboBar.localScale;
        scale.x = ComboValue;
        ComboBar.localScale = scale;

        StartCoroutine(PunchScale(ComboBar.parent, comboBarOriginalScale));

        if (ComboValue >= 1)
        {
            ComboValue = 0.3f;
            PlayerPrefs.SetFloat("ComboValue" + Level, ComboValue);

            scale.x = ComboValue;
            ComboBar.localScale = scale;

            Combo++;
            PlayerPrefs.SetInt("Combo_" + Level, Combo);
            ComboTxt.text = Combo.ToString();

            StartCoroutine(PunchScale(ComboTxt.transform, comboTxtOriginalScale));

            if (MaxCombo <= Combo)
            {
                MaxCombo = Combo;
                PlayerPrefs.SetInt("MaxCombo" + Level, MaxCombo);
            }
        }
    }

    private void RemoveComboStrike()
    {
        if (Combo > 1)
        {
            ComboValue = 1;
            Combo--;
            ComboTxt.text = Combo.ToString();
            StartCoroutine(PunchScale(ComboTxt.transform, comboTxtOriginalScale));
        }
    }

    private void InitValue()
    {
        Combo = PlayerPrefs.GetInt("Combo_" + Level, 1);
        ComboTxt.text = Combo.ToString();

        TimerValue = PlayerPrefs.GetFloat("TimerValue_" + Level, 60);
        sb.Clear().AppendFormat("{0:00}", TimerValue);
        TimerTxt.text = sb.ToString();

        MaxCombo = PlayerPrefs.GetInt("MaxCombo" + Level, 1);

        Score = PlayerPrefs.GetInt("Score_" + Level, 0);
        ScoreTxt.text = Score.ToString();

        ComboValue = PlayerPrefs.GetFloat("ComboValue_" + Level, 0);
    }

    private void ResetValue()
    {
        PlayerPrefs.DeleteKey("TotalRestCard" + Level);
        PlayerPrefs.DeleteKey("valideCardCount_" + Level);
        PlayerPrefs.DeleteKey("Combo_" + Level);
        PlayerPrefs.DeleteKey("MaxCombo_" + Level);
        PlayerPrefs.DeleteKey("Score_" + Level);
        PlayerPrefs.DeleteKey("ComboValue_" + Level);
        PlayerPrefs.DeleteKey("TimerValue_" + Level);
        LevelManager.Instance.ResetStage();

    }

    private IEnumerator PunchScale(Transform target, Vector3 originalScale)
    {
        float time = 0f;
        float duration = 0.3f;
        Vector3 peak = originalScale * 1.5f;

        while (time < duration)
        {
            float t = time / duration;
            target.localScale = Vector3.Lerp(peak, originalScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = originalScale;
    }

    #endregion
}

[System.Serializable]
public class LevelStatus
{
    public int Index;
    public Transform Pivot;
    public int Width, Length;
    public int TotalRestCard;
    public float PaddingVertical, PaddingHorizontal;
    public Card CardPrefab;
}

