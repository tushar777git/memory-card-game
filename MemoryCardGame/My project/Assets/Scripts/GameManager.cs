using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    
    public static int gameSize = 4;

    
    [SerializeField]
    private GameObject prefab;

    
    [SerializeField]
    private GameObject cardList;

    
    [SerializeField]
    private Sprite cardBack;

    
    [SerializeField]
    private Sprite[] sprites;

   
    private _Card[] cards;

   
    [SerializeField]
    private GameObject panel;

    

    [SerializeField]
    private GameObject info;

   
    [SerializeField]
    private _Card spritePreload;

   
    [SerializeField]
    private TMP_Text timeLabel;

    private float time;

    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private bool gameStart;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameStart = false;
        panel.SetActive(false);
        
    }

    
    public void StartCardGame()
    {
        if (gameStart) return;

        gameStart = true;

        panel.SetActive(true);
        info.SetActive(false);

        SetGamePanel();

        cardSelected = -1;
        spriteSelected = -1;
        cardLeft = cards.Length;

        SpriteCardAllocation();

        StartCoroutine(HideFace());

        time = 0;
    }

    

    private void SetGamePanel()
    {
        int totalCards = gameSize * gameSize;
        cards = new _Card[totalCards];

        foreach (Transform child in cardList.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < totalCards; i++)
        {
            GameObject c = Instantiate(prefab, cardList.transform);

            cards[i] = c.GetComponent<_Card>();
            cards[i].ID = i;
        }
    }


    void ResetFace()
    {
        for (int i = 0; i < cards.Length; i++)
            cards[i].ResetRotation();
    }

    
    IEnumerator HideFace()
    {
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();

        yield return new WaitForSeconds(0.5f);
    }

   
    private void SpriteCardAllocation()
    {
        int[] selectedID = new int[cards.Length / 2];

        for (int i = 0; i < cards.Length / 2; i++)
        {
            int value = Random.Range(0, sprites.Length);

            for (int j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }

            selectedID[i] = value;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }

        for (int i = 0; i < cards.Length / 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length);

                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }
        }
    }

    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }

    public Sprite CardBack()
    {
        return cardBack;
    }

    public bool canClick()
    {
        return gameStart;
    }

    public void cardClicked(int spriteId, int cardId)
    {
        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
        }
        else
        {
            if (spriteSelected == spriteId)
            {
                cards[cardSelected].Inactive();
                cards[cardId].Inactive();
                cardLeft -= 2;
                CheckGameWin();
            }
            else
            {
                cards[cardSelected].Flip();
                cards[cardId].Flip();
            }

            cardSelected = -1;
            spriteSelected = -1;
        }
    }

    private void CheckGameWin()
    {
        if (cardLeft == 0)
        {
            EndGame();
            
        }
    }

    private void EndGame()
    {
        gameStart = false;
        panel.SetActive(false);
    }

    public void CloseGame()
    {
        EndGame();
    }

    public void DisplayInfo(bool i)
    {
        info.SetActive(i);
    }

    private void Update()
    {
        if (gameStart)
        {
            time += Time.deltaTime;
            timeLabel.text = "Time: " + time.ToString("F1") + "s";
        }
    }
}