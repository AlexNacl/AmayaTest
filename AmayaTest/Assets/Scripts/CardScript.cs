using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    [SerializeField]
    public GameObject Letter;
    public Text taskText;

    public ParticleSystem stars;
    GameManager gameManager;
    [SerializeField]
    private string currentValue;

    Color lightBlue = new Color(0.3f, 0.4f, 0.6f, 0.3f);
    Color lightYelow = new Color(0.3f, 0.4f, 0.6f, 0.3f);
    Color lightGree = new Color(0.3f, 0.4f, 0.6f, 0.3f);
    Color lightRed = new Color(0.3f, 0.4f, 0.6f, 0.3f);
    private List<Color> colorList = new List<Color>();

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        taskText = GameObject.Find("TaskText").GetComponent<Text>();
        colorList.Add(lightBlue);
        colorList.Add(lightYelow);
        colorList.Add(lightGree);
        colorList.Add(lightRed);
    }

    public void SetCurrentValue(string value)
    {
        currentValue = value;
    }

    public void SetCurrentImage(Sprite value)
    {
        Letter.gameObject.GetComponent<Image>().sprite = value;
        Debug.Log("bum");
        //this.gameObject.GetComponent<Image>().color = colorList[Random.Range(0, 3)];
    }

    public string GetCurrentValue()
    {
        return currentValue;
    }

    public void IsRightAnswer()
    {
        if(currentValue == gameManager.GetRightAnswer())
        {
            stars.Play();
            StartCoroutine(RightAnswer());
        } 
        else 
        {
            this.transform.DOShakePosition(2.0f, strength: new Vector3(0, 10, 0), vibrato: 10, randomness: 5, snapping: false, fadeOut: true);
        }
    }

    IEnumerator RightAnswer()
    {
        Vector3 OriginalScale = Letter.transform.localScale; 
        DOTween.Sequence() 
            .Append(Letter.transform.DOScale(new Vector3(OriginalScale.x + 0.5f, OriginalScale.y + 0.5f, OriginalScale.z + 0.5f), 0.5f).SetEase(Ease.Linear)) 
            .Append(Letter.transform.DOScale(OriginalScale, 0.5f).SetEase(Ease.Linear));
        taskText.DOFade(0f, 2f);
        yield return new WaitForSeconds(2);
        gameManager.NextLevel(gameManager.GetCurrentLevel()+1);
    }
}
