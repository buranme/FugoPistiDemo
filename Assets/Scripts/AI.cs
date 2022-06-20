using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AI : Player
{
    // Array for the AI to keep track of how many times it had seen each card
    // I chose the size to be 14 to not bother with index conversion since card values start from 1
    // Which means index 0 is not used
    private readonly int[] _cardCounts = new int[14];

    private void Awake()
    {
        WipeOutMemory();
    }
    
    // USE CASE #8
    // Method to choose the best fitting card in AI's hand
    // The AI checks the card on top of the ground, cards on its hand and refers to _cardCounts to decide what card to play
    // The behaviour according to how many cards are on the ground:
    // 0: picks the most seen card (to not get pişti'd)
    // 2-5: tries matching a card, if cannot picks the most seen (saves J -if it has any- for getting more cards)
    // 1, 5+: tries matching a card, if cannot tries using J, if cannot picks the most seen
    public void ChooseCard(Transform ground)
    {
        var cardsOnGroundCount = ground.childCount;
        var cardsOnHand = (from Transform child in Hand.transform select child.GetComponent<Card>()).ToList();

        var pickedCard = TryMatchingCardWithGround(cardsOnHand);
        
        if (cardsOnGroundCount is 1 or > 5 && !pickedCard)
        {
            pickedCard = TryGettingJ(cardsOnHand);
        }
        
        if(!pickedCard)
        {
            pickedCard = PickMostSeenCard(cardsOnHand);
        }

        pickedCard.FlipCard();
        StartCoroutine(GameMaster.CO_PlayTheCard(pickedCard, this));
    }

    // Method to pick the most seen card in the given hand.
    // To do this checks the values of the cards in _cardCounts, the one with the biggest value is the preferred choice
    private Card PickMostSeenCard(List<Card> cards) //Doesn't use J
    {
        var rarest = cards[0];
        foreach (var elem in cards)
        {
            var elemValue = elem.CardValue;
            if (rarest.CardValue == 11 || (elemValue != 11 && _cardCounts[elemValue] > _cardCounts[rarest.CardValue]))
            {
                rarest = elem;
            }
        }
        
        return rarest;
    }

    // Method to try matching one of the cards on hand with the one on the top of the ground.
    // If there is no matching card found, returns null
    private Card TryMatchingCardWithGround(List<Card> cards) //Doesn't use J
    {
        var cardOnGround = PlayerPrefs.GetInt("deckTopCard");
        Card match = null;
        foreach (var elem in cards)
        {
            var elemValue = elem.CardValue;
            if (elemValue != cardOnGround || elemValue == 11) continue;
            match = elem;
            break;
        }
        return match;
    }

    // Method to try finding a J in the given cards on hand
    // If there is no matching card found, returns null
    private Card TryGettingJ(List<Card> cards)
    {
        Card match = null;
        foreach (var elem in cards)
        {
            if (elem.CardValue != 11) continue;
            match = elem;
            break;
        }
        return match;
    }

    // Method for the ai to "see" a class so that it can update the matching value in _cardCounts
    public void SeeCard(Card card)
    {
        _cardCounts[card.CardValue]++;
    }

    // Method to clear _cardCounts
    public void WipeOutMemory()
    {
        Array.Fill(_cardCounts, 0);
    }
}
