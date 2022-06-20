using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

// Class to handle all things card related
public class Card : MonoBehaviour
{
    private Text _valueText;
    private Image _cardImage;
    private Human _human;

    public int CardValue { get; private set; }
    public string CardType { get; private set; }
    public bool IsSeen { get; set; }

    private void Awake()
    {
        _valueText = gameObject.GetComponentInChildren<Text>();
        _cardImage = gameObject.GetComponent<Image>();
        IsSeen = false;
    }

    // USE CASE #5
    public void OnClicked()
    {
        if (PlayerPrefs.GetInt("inputEnabled") != 1) return;
        _human.ChooseCard(this);
    }

    public void SetCard(int cardNumber, string cardType)
    {
        CardValue = cardNumber;
        CardType = cardType;
    }

    // Method used to enable card for click
    public void EnableCard(Human human)
    {
        transform.GetComponent<BoxCollider2D>().enabled = true;
        _human = human;
    }

    public void DisableCard()
    {
        transform.GetComponent<BoxCollider2D>().enabled = false;
        _human = null;
    }

    // Method to flip a card by applying the correct image and value text
    public void FlipCard()
    {
        IsSeen = true;
        var imageName = "";

        switch (CardValue)
        {
            case 13:
                imageName += "king_";
                break;
            case 12:
                imageName += "queen_";
                break;
            case 11:
                imageName += "jack_";
                break;
            default:
                _valueText.text = CardValue.ToString();
                if (_valueText.text == "1")
                    _valueText.text = "A";
                if (CardType is "club" or "spade")
                    _valueText.color = new Color32(53, 56, 89, 255);
                break;
        }

        imageName += CardType;

        _cardImage.sprite = Resources.Load<Sprite>("Cards/" + imageName);
    }

    // Method to set the card's parent to the given transform and move the card
    public IEnumerator MoveCardTo(Transform parent, bool randomize = false)
    {
        transform.position = parent.position;
        transform.SetParent(parent);
        if (randomize)
        {
            transform.position += new Vector3(Random.Range(0, 50), Random.Range(0, 50), 0);
        }

        for (var i = 1.25f; i > 1f; i -= 0.025f)
        {
            transform.localScale = new Vector3(i, i, 1f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Method to visualize AI picking a card
    public IEnumerator CardChosenAnimation()
    {
        for (var i = 1f; i < 1.15f; i += 0.025f)
        {
            transform.localScale = new Vector3(i, i, 1f);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    // Method to convert the card into a string
    // Used to list the cards on hover on the ground and the human's winnings
    public string CardToString()
    {
        if (!IsSeen)
        {
            return "-------\n";
        }

        var textColor = CardType is "club" or "spade" ? "<color=#353859>" : "<color=#a4235d>";

        var value = CardValue.ToString();
        value = value switch
        {
            "1" => "A",
            "11" => "J",
            "12" => "Q",
            "13" => "K",
            _ => value
        };

        return $"{textColor}{GetTypeCharacter()}\t{value}</color>\n";
    }

    private string GetTypeCharacter()
    {
        return CardType switch
        {
            "club" => "♣",
            "spade" => "♠",
            "diamond" => "♦",
            _ => "♥"
        };
    }
}