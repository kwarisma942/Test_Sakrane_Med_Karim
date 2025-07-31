using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    #region Public Variable

    public static LevelManager Instance;

    public LevelStatus LevelStatus;
    #endregion

    #region Private Variable

    public GameObject ContinueBtn;

    [SerializeField]
    private Slider WitdhSlider;
    [SerializeField]
    private Text WitdhValueTxt;

    [SerializeField]
    private Slider LengthSlider;
    [SerializeField]
    private Text LengthValueTxt;

    [SerializeField]
    private Transform cardParent;

    [SerializeField]
    private List<CardProfile> cardProfiles;



    private WaitForSeconds cardSpawnDelay = new WaitForSeconds(0.1f);
    private static readonly StringBuilder sb = new StringBuilder();

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Stage_" + LevelStatus.Index))
        {
            ContinueBtn.SetActive(true);
        }
        else
        {
            ContinueBtn.SetActive(false);
        }
    }
    #endregion

    #region Public Methode

    public void ContinueLevel()
    {
        if (PlayerPrefs.HasKey("Stage_" + LevelStatus.Index))
        {
            StartCoroutine(InitCard(LoadCard(), false));
        }
        else
        {
            GenerateLevel();
        }
    }

    public void GenerateLevel()
    {
        StartCoroutine(InitCard(GetRandomCard(), true));
    }

    public void ResetStage()
    {
        PlayerPrefs.DeleteKey("Stage_" + LevelStatus.Index);
        ContinueBtn.SetActive(false);
    }

    public bool IsVide()
    {
        return cardParent.childCount == 0;
    }

    public void RemoveCard(string cardName)
    {
        string loadCardString = PlayerPrefs.GetString("Stage_" + LevelStatus.Index, "");
        int index = loadCardString.IndexOf(cardName);
        if (index >= 0)
        {
            loadCardString = loadCardString.Remove(index, cardName.Length);
            loadCardString = loadCardString.Insert(index, "null");
            PlayerPrefs.SetString("Stage_" + LevelStatus.Index, loadCardString);
        }
    }

    public void ChangeWitdh()
    {
        LevelStatus.Width =(int)WitdhSlider.value;
        WitdhValueTxt.text= WitdhSlider.value.ToString();
    }

    public void ChangeLength()
    {
        LevelStatus.Length = (int)LengthSlider.value;
        LengthValueTxt.text = LengthSlider.value.ToString();
    }

    #endregion

    #region Private Methode

    private IEnumerator InitCard(List<CardProfile> currentProfiles, bool useSave)
    {
        int index = 0;
        float x = LevelStatus.Pivot.position.x;
        float y = LevelStatus.Pivot.position.y;

        for (int i = 0; i < LevelStatus.Width; i++)
        {
            x = LevelStatus.Pivot.position.x;
            for (int j = 0; j < LevelStatus.Length; j++)
            {
                if (currentProfiles[index] != null)
                {
                    CreateCard(new Vector3(x, y, 1), currentProfiles[index]);
                    if (useSave)
                    {
                        SaveCard(currentProfiles[index].Name);
                    }
                    yield return cardSpawnDelay;
                }
                x += LevelStatus.PaddingHorizontal;
                index++;
            }
            y -= LevelStatus.PaddingVertical;
        }

        GameManager.Instance.InGame = true;
    }

    private void CreateCard(Vector3 cardPosition, CardProfile profile)
    {
        Card newCard = Instantiate(LevelStatus.CardPrefab, cardPosition, Quaternion.identity);
        newCard.Profile = profile;
        newCard.Init();
        newCard.transform.SetParent(cardParent, true);
    }

    private void SaveCard(string cardName)
    {
        sb.Clear();
        sb.Append(PlayerPrefs.GetString("Stage_" + LevelStatus.Index, ""));
        sb.Append(cardName);
        sb.Append('+');
        PlayerPrefs.SetString("Stage_" + LevelStatus.Index, sb.ToString());
    }

    private List<CardProfile> LoadCard()
    {
        List<string> cardStrings = PlayerPrefs.GetString("Stage_" + LevelStatus.Index, "").Split('+').ToList();
        List<CardProfile> res = new List<CardProfile>(cardStrings.Count);

        foreach (string cardString in cardStrings)
        {
            if (cardString == "null")
            {
                res.Add(null);
            }
            else
            {
                res.Add(cardProfiles.Find(cp => cp.Name == cardString));
            }
        }

        return res;
    }

    private List<CardProfile> GetRandomCard()
    {
        List<CardProfile> randomCard = new List<CardProfile>();
        List<CardProfile> res = new List<CardProfile>();

        List<CardProfile> availableProfiles = new List<CardProfile>(cardProfiles);

        for (int i = 0; i < (LevelStatus.Width * LevelStatus.Length) / 2; i++)
        {
            int index = Random.Range(0, availableProfiles.Count);
            CardProfile selected = availableProfiles[index];
            randomCard.Add(selected);
            res.Add(selected);
            availableProfiles.RemoveAt(index);
        }

        while (randomCard.Count > 0)
        {
            int index = Random.Range(0, randomCard.Count);
            res.Add(randomCard[index]);
            randomCard.RemoveAt(index);
        }

        return res;
    }

    #endregion
}