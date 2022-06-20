// Mehmet Mert Buran, 2022

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// Class to control the game flow
public class GameMaster : MonoBehaviour
{
    private readonly string[] _cardTypes = { "club", "spade", "diamond", "heart" };

    // These two inherit from the class "Player"
    [SerializeField] private Human human;
    [SerializeField] private AI ai;
    
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject deck;
    [SerializeField] private Text deckCount;
    
    // Used to display a list of cards when hovered over the ground and the human winnings
    [SerializeField] private Text cardsOnGround;
    [SerializeField] private Text cardsOnWinnings;

    // USE CASE #1
    private void Start()
    {
        // This player pref is used to enable/disable clicking on the cards on the human's hand
        PlayerPrefs.SetInt("inputEnabled", 0);
        
        cardsOnGround.text = "";
        cardsOnWinnings.text = "";
        
        human.Initialize(this);
        ai.Initialize(this);
        
        CreateDeck();
    }

    // USE CASE #2
    // Method to create 52 cards and start the game with the human
    private void CreateDeck()
    {
        for (var i = 1; i < 14; i++)
        {
            foreach (var type in _cardTypes)
            {
                var newCard = Instantiate(cardPrefab, deck.transform.position, Quaternion.identity);
                newCard.GetComponent<Card>().SetCard(i, type);
                newCard.name = i + type;
                newCard.transform.SetParent(deck.transform);
            }
        }
        
        StartCoroutine(CO_PassPlayers(human, true));
    }

    // The main game loop
    // If it is ai's turn calls its ChooseCard method
    // If it is human's turn enables clicking on cards on human's hands
    private IEnumerator CO_PassPlayers(Object player, bool putCardsOnGround = false)
    {
        if (putCardsOnGround)
        {
            for (var i = 0; i < 3; i++)
            {
                yield return StartCoroutine(CO_DrawCardTo( hideCard : true));
            }
            yield return StartCoroutine(CO_DrawCardTo());
        }
        
        if (player == ai)
        {
            PlayerPrefs.SetInt("inputEnabled", 0);
            ai.ChooseCard(ground.transform);
        }
        else
        {
            if (human.Hand.transform.childCount == 0)
            {
                if (deck.transform.childCount == 0)
                {
                    HandleEndgame();
                }
                else
                {
                    yield return StartCoroutine(CO_DrawCards());
                }
            }
            PlayerPrefs.SetInt("inputEnabled", 1);
        }
    }

    // USE CASE #3
    // Method to give ai and human 4 cards each
    private IEnumerator CO_DrawCards()
    {
        for (var i = 0; i < 4; i++)
        {
            yield return StartCoroutine(CO_DrawCardTo(ai));
        }
        for (var i = 0; i < 4; i++)
        {
            yield return StartCoroutine(CO_DrawCardTo(human));
        }
    }

    // Method to give a random card from the deck to a player
    // If player is null the card is put on the ground
    // If player is human the card is flipped and enabled
    // If hideCard is true the card is not flipped, used for the initial 3 closed cards
    private IEnumerator CO_DrawCardTo(Player player = null, bool hideCard = false)
    {
        var drawnCardTransform = RandomCardFromDeck();
        var drawnCard = drawnCardTransform.GetComponent<Card>();
        
        var hand = ground;
        if (player)
        {
            hand = player.Hand;
        }
        
        // If the card is given to the human flip it and enable it
        if (player == human)
        {
            drawnCard.FlipCard();
            drawnCard.EnableCard(human);
        }
        // Else, if the card shouldn't be hidden, flip it and let the ai see it
        else if (!hideCard)
        {
            if (!player)
            {
                drawnCard.FlipCard();
                PlayerPrefs.SetInt("deckTopCard", drawnCard.CardValue);
                AddToCardList(cardsOnGround, drawnCard);
            }
            ai.SeeCard(drawnCard);
        }
        else
        {
            AddToCardList(cardsOnGround, drawnCard);
        }
        
        deckCount.text = (int.Parse(deckCount.text) - 1).ToString();

        yield return drawnCard.MoveCardTo(hand.transform, !player);
    }

    // USE CASE #7 and #9
    // Method to play a card from a player's hand to the ground
    // Gets called by the human class after a click on a card
    // And by the ai class after ai chooses a card to play
    // If the played card matches the one on top or is J, ClearGround is called
    public IEnumerator CO_PlayTheCard(Card card, Player player)
    {
        // Last card on top
        var groundTopValue = PlayerPrefs.GetInt("deckTopCard");
        
        // New card on top
        var playedCardValue = card.CardValue;
        PlayerPrefs.SetInt("deckTopCard", playedCardValue);
        AddToCardList(cardsOnGround, card);

        //AI sees the card if the player is human
        if (player == human)
        {
            ai.SeeCard(card);
        }
        else
        {
            yield return card.CardChosenAnimation();
        }
        
        yield return card.MoveCardTo(ground.transform, true);

        if (groundTopValue != 0 && (playedCardValue == 11 || playedCardValue == groundTopValue))
        {
            ClearGround(player);
            PlayerPrefs.SetInt("deckTopCard", 0);
            cardsOnGround.text = "";
        }

        StartCoroutine(CO_PassPlayers(GetNextPlayer(player)));
    }

    // Method to give every card on ground to the given player
    private void ClearGround(Player player)
    {
        var cardsWon = ground.transform.Cast<Transform>().ToList();

        // If the move was a pişti give the player 10 scores
        var isPisti = cardsWon.Count == 2;
        if(isPisti)
        {
            CalculateScore(player);
        }
        
        foreach (var child in cardsWon)
        {
            var card = child.GetComponent<Card>();

            // Lets ai see the card
            if (!card.IsSeen)
            {
                card.IsSeen = true;
                if(player == ai)
                    ai.SeeCard(card);
            }
            
            if(player == human)
            {
                AddToCardList(cardsOnWinnings, card);
            }
            
            // Tell the player to get the card
            player.WinCard(child);
            
            // If the move wasn't a pişti calculate the score of the card
            if(!isPisti)
            {
                CalculateScore(player, card);
            }
        }
    }

    // Method to calculate the score of a given card and give it to the given player
    private static void CalculateScore(Player player, Card card = null)
    {
        // Null card means pişti
        if (!card)
        {
            player.UpdateScore(10);
            return;
        }

        var cardValue = card.CardValue;
        var cardType = card.CardType;
        switch (cardValue)
        {
            case 1 or 11:
                player.UpdateScore(1);
                break;
            case 2 when cardType == "club":
                player.UpdateScore(2);
                break;
            case 10 when cardType == "diamond":
                player.UpdateScore(3);
                break;
        }
        
    }

    // Method to handle the ending
    // Determines who wins and activates the endGaöeScreem
    private void HandleEndgame()
    {
        PlayerPrefs.SetInt("inputEnabled", 0);
        StopAllCoroutines();
        cardsOnGround.text = "";
        cardsOnWinnings.text = "";

        var aiWinnings = ai.Winnings.transform.childCount;
        var humanWinnings = human.Winnings.transform.childCount;
        
        if(aiWinnings > humanWinnings)
        {
            ai.UpdateScore(3);
        }
        else if(aiWinnings < humanWinnings)
        {
            human.UpdateScore(3);
        }
        
        string gameOverText;
        if (ai.Score > human.Score)
        {
            gameOverText = "You Lost";
        }
        else if (ai.Score == human.Score)
        {
            gameOverText = "Draw";
        }
        else
        {
            gameOverText = "You Won";
        }
        deckCount.gameObject.SetActive(false);
        endGameScreen.transform.GetComponentInChildren<Text>().text = gameOverText;
        endGameScreen.SetActive(true);
    }

    // Method to restart the game
    // Destroys all cards, wipes the scores and ai's memory and creates the deck again 
    public void RestartGame()
    {
        var cards = human.Winnings.transform.Cast<Transform>().ToList();
        cards.AddRange(ai.Winnings.transform.Cast<Transform>());
        cards.AddRange(ground.transform.Cast<Transform>());

        foreach (var child in cards)
        {
            Destroy(child.gameObject);
        }
        human.UpdateScore(-1);
        ai.UpdateScore(-1);
        ai.WipeOutMemory();
        
        deckCount.gameObject.SetActive(true);
        deckCount.text = "52";

        CreateDeck();
    }

    private Transform RandomCardFromDeck()
    {
        return deck.transform.GetChild(Random.Range(0, deck.transform.childCount));
    }

    private Player GetNextPlayer(Object lastPlayed)
    {
        return lastPlayed == ai ? human : ai;
    }

    // Method to add a given card to the given text as a line of a list
    // The texts are visible on hover on the ground and the human's winnings
    private static void AddToCardList(Text list, Card card)
    {
        list.text += card.CardToString();
    }
}
