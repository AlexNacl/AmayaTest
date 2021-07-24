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
    public List<Sprite> cardImgs = new List<Sprite>();
    public List<string> namesList = new List<string>();
    public List<string> usedNameList = new List<string>();
    public List<string> answerPool = new List<string>();
    public List<string> usedAnswerPool = new List<string>();
    public List<int> IDlist = new List<int>();
    int PresentId;
    int presetCount;

    void Awake()    //Loading preset images and their names
    { 
        IDlist.Add(0);
        presetCount = Convert.ToInt32(dataText.text);
        for (int i = 1; i < presetCount+1; i++)
        {
            string curName = i.ToString();
            Sprite[] cardImgsLocal;
            cardImgsLocal = Resources.LoadAll<Sprite>(curName);
            for (int j = 0; j < cardImgsLocal.Length; j++)
            {
                cardImgs.Add(cardImgsLocal[j]);
            }
            IDlist.Add(cardImgs.Count);
        } 
        Debug.Log(cardImgs.Count);    
        for(int i = 0; i < cardImgs.Count; i++)
        {
            namesList.Add(cardImgs[i].name);            
        } 
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

    ////////////////////////////////
    ////////Level operations////////
    ////////////////////////////////

    void LoadLevel(int Index){
        ClearLevel();
        PresentId = UnityEngine.Random.Range(0, presetCount);
        Vector2 newHeight = new Vector2(800, 265*Index + 15);
        Slots.GetComponent<RectTransform>().DOSizeDelta(newHeight, 0f, false);
        for (int i = 0; i < Index*3; i++) //Initiating cards
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
        taskText.DOFade(1f, 2f);
        FaderImg.DOFade(0, 2f);
        FaderImg.raycastTarget = false;
        Curtain.raycastTarget = false;
        restartBtn.raycastTarget = false;
        FirstLoad = false;  
        SetRightAnswer();
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

    public void ClearLevel()
    {
        foreach (Transform child in Slots.transform)
            Destroy(child.gameObject);
    }

    public void Restart()
    {
        usedAnswerPool.Clear();
        StartCoroutine(LoadFader(currentLevel));
        btnText.text = "";
    }

    /////////////////////////////////////////////////
    ////////Card and Answer value controllers////////
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


    private void SetRightAnswer()
    {
        foreach (string child in usedNameList)
        {
            answerPool.Add(child);
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


    //////////////////////////////////////////////////
    //Only the Get and Set functions below this line//
    //////////////////////////////////////////////////

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

    public string GetRightAnswer()
    {
        return currentRightAnswer;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

}
