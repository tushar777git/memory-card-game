using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid Settings")]
    [SerializeField] private int gridSize = 4;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;

    [Header("Card Sprites")]
    [SerializeField] private Sprite cardBack;
    [SerializeField] private Sprite[] frontSprites;

    [Header("UI")]
    [SerializeField] private GameObject gamePanel;
    
    [SerializeField] private TMP_Text timerText;

    private CardItem[] cards;

    private int firstCardID = -1;
    private int firstSpriteID = -1;
    private int remainingCards;

    private float gameTimer;
    private bool gameRunning;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameRunning = false;
        gamePanel.SetActive(false);
    }

    #region GAME FLOW

    public void StartGame()
    {
        if (gameRunning) return;

        gameRunning = true;
        gameTimer = 0f;

        gamePanel.SetActive(true);
       

        CreateBoard();
        AssignSprites();

        remainingCards = cards.Length;
        firstCardID = -1;
        firstSpriteID = -1;

        StartCoroutine(PreviewPhase());
    }

    public void StopGame()
    {
        gameRunning = false;
        gamePanel.SetActive(false);
    }

    #endregion

    #region BOARD CREATION

    private void CreateBoard()
    {
        int total = gridSize * gridSize;
        cards = new CardItem[total];

        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        for (int i = 0; i < total; i++)
        {
            GameObject obj = Instantiate(cardPrefab, cardParent);
            CardItem card = obj.GetComponent<CardItem>();

            card.ID = i;   // ✅ matches your script
            cards[i] = card;
        }
    }

    private void AssignSprites()
    {
        int pairCount = cards.Length / 2;
        int[] spriteIndexes = new int[pairCount];

        for (int i = 0; i < pairCount; i++)
            spriteIndexes[i] = Random.Range(0, frontSprites.Length);

        foreach (CardItem card in cards)
        {
            card.Active();
            card.SpriteID = -1;
            card.ResetRotation();
        }

        for (int i = 0; i < pairCount; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int randomIndex = Random.Range(0, cards.Length);

                while (cards[randomIndex].SpriteID != -1)
                    randomIndex = (randomIndex + 1) % cards.Length;

                cards[randomIndex].SpriteID = spriteIndexes[i];
            }
        }
    }

    private IEnumerator PreviewPhase()
    {
        yield return new WaitForSeconds(0.4f);

        foreach (CardItem card in cards)
            card.Flip();

        yield return new WaitForSeconds(0.6f);
    }

    #endregion

    #region MATCH LOGIC

    public void OnCardSelected(int spriteID, int cardID)
    {
        if (firstSpriteID == -1)
        {
            firstSpriteID = spriteID;
            firstCardID = cardID;
            return;
        }

        if (firstSpriteID == spriteID)
        {
            cards[firstCardID].Inactive();
            cards[cardID].Inactive();
            remainingCards -= 2;

            if (remainingCards <= 0)
                StopGame();
        }
        else
        {
            cards[firstCardID].Flip();
            cards[cardID].Flip();
        }

        firstSpriteID = -1;
        firstCardID = -1;
    }

    #endregion

    #region HELPERS

    public Sprite GetSprite(int id)
    {
        return frontSprites[id];
    }

    public Sprite GetBackSprite()
    {
        return cardBack;
    }

    public bool CanClick()
    {
        return gameRunning;
    }

   

    #endregion

    private void Update()
    {
        if (!gameRunning) return;

        gameTimer += Time.deltaTime;
        timerText.text = $"Time: {gameTimer:F1}s";
    }
}