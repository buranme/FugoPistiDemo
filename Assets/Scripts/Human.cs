public class Human : Player
{
    // USE CASE #6
    // Method that gets called when a card on human's hand is clicked
    public void ChooseCard(Card card)
    {
        card.DisableCard();
        StartCoroutine(GameMaster.CO_PlayTheCard(card, this));
    }
}