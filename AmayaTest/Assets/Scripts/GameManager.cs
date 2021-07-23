using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image Curtain;
    public Image FaderImg;
    public Image restartBtn;
    public Text btnText;
    public Text taskText;
    public GameObject Slots;
    public GameObject Card;
    private string currentRightAnswer;
    private int currentLevel;
    private bool FirstLoad = true;

    public TextAsset dataText;
    public Sprite[] cardImgs;
    public List<string> namesList = new List<string>();
    public List<string> usedNameList = new List<string>();
    public List<string> answerPool = new List<string>();
    public List<string> usedAnswerPool = new List<string>();
    public List<int> IDlist = new List<int>();
    [SerializeField]
    int PresentId;
    int presetCount;

    void Awake() {
        IDlist.Add(0);
        presetCount = Convert.ToInt32(dataText.text);
        for (int i = 1; i < presetCount+1; i++)
        {
            string curName = i.ToString();
            cardImgs = Resources.LoadAll<Sprite>(curName);
        } 
        Debug.Log(cardImgs.Length);    
        for(int i = 0; i < cardImgs.Length; i++)
        {
            namesList.Add(cardImgs[i].name);            
        } 
        IDlist.Add(namesList.Count);
    }

    void Start()
    {
        currentLevel = 1;
        FaderImg.DOFade(0, 1f);
        LoadLevel(1);
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    IEnumerator LoadFader(int Index){
        FaderImg.DOFade(1, 2f);
        restartBtn.DOFade(0, 2f);
        restartBtn.gameObject.SetActive(false);
        FaderImg.raycastTarget = true;
        yield return new WaitForSeconds(2);
        Curtain.DOFade(0, 2f);
        LoadLevel(Index);
    }

    void LoadLevel(int Index){
        ClearLevel();
        PresentId = UnityEngine.Random.Range(0, presetCount-1);
        Vector2 newHeight = new Vector2(800, 265*Index + 15);
        Slots.GetComponent<RectTransform>().DOSizeDelta(newHeight, 0f, false);
        for (int i = 0; i < Index*3; i++)
        {
            GameObject NewCard = Instantiate(Card, new Vector2(0, 0), Quaternion.identity);
            SetValue(NewCard.GetComponent<CardScript>(), PresentId);
            NewCard.transform.SetParent(Slots.transform, false);
            if (FirstLoad)
            {
                Vector3 OriginalScale = NewCard.transform.localScale; 
                NewCard.GetComponent<RectTransform>().localScale = new Vector2(0.1f, 0.1f);
                DOTween.Sequence() 
                    .Append(NewCard.transform.DOScale(new Vector3(OriginalScale.x + 0.3f, OriginalScale.y + 0.3f, OriginalScale.z + 0.3f), 1f).SetEase(Ease.Linear)) 
                    .Append(NewCard.transform.DOScale(OriginalScale, 0.5f).SetEase(Ease.Linear));
            }
        }
        SetRightAnswer();
        taskText.DOFade(1f, 2f);
        FaderImg.DOFade(0, 2f);
        FaderImg.raycastTarget = false;
        Curtain.raycastTarget = false;
        restartBtn.raycastTarget = false;
        FirstLoad = false;  
    }

    public void ClearLevel()
    {
        foreach (Transform child in Slots.transform)
            Destroy(child.gameObject);
    }

    public void Restart()
    {
        StartCoroutine(LoadFader(currentLevel));
        usedAnswerPool.Clear();
    }

    public void NextLevel(int level)
    {
        usedNameList.Clear();
        if (level > 3)
        {
            currentLevel = 1;
            FirstLoad = true;
            Curtain.DOFade(1, 1);
            Curtain.raycastTarget = true;
            restartBtn.DOFade(1, 2f);
            restartBtn.raycastTarget = true;
            restartBtn.gameObject.SetActive(true);
            btnText.DOText("Restart", 2f);
        } 
        else 
        {
            currentLevel = level;
            StartCoroutine(LoadFader(currentLevel));
        }
        
    }

    /////////////////////////////////////////////////
    //Only the Get and Set functions below this line
    /////////////////////////////////////////////////

    private void SetValue(CardScript card, int Id)
    {
        int rand = UnityEngine.Random.Range(IDlist[Id], IDlist[Id+1]);
        string applyValue = namesList[rand];
        while (!IfUsed(applyValue, usedNameList))
        {
            rand = UnityEngine.Random.Range(IDlist[Id], IDlist[Id+1]);
            applyValue = namesList[rand];
        }
        card.SetCurrentValue(applyValue);
        foreach (Sprite sprite in cardImgs)
        {
            if (sprite.name == applyValue)
            {
                card.SetCurrentImage(sprite);
            }
        }
        usedNameList.Add(applyValue);
    }

    private bool IfUsed(string value, List<string> list)
    {
        foreach (string name in list)
        {
            if (name == value)
            {
                return false;
            }
        }
        return true;
    }

    private void SetRightAnswer()
    {
        foreach (Transform child in Slots.transform)
        {
            answerPool.Add(child.gameObject.GetComponent<CardScript>().GetCurrentValue());
        }
        string displayAnswer = answerPool[UnityEngine.Random.Range(0, answerPool.Count-1)];
        while (!IfUsed(displayAnswer, usedAnswerPool))
        {
            displayAnswer = answerPool[UnityEngine.Random.Range(0, answerPool.Count-1)];
        }
        usedAnswerPool.Add(displayAnswer);
        currentRightAnswer = displayAnswer;
        taskText.text = "Find " + currentRightAnswer;
        answerPool.Clear();
    }

    public string GetRightAnswer()
    {
        return currentRightAnswer;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

}
