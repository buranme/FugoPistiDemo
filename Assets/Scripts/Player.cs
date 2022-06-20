using UnityEngine;
using UnityEngine.UI;

// Parent class for the AI and the Human
public class Player : MonoBehaviour
{
    [SerializeField] private GameObject hand;
    [SerializeField] private GameObject winnings;
    [SerializeField] private Text scoreText;
    
    protected GameMaster GameMaster;
    
    public GameObject Hand => hand;
    public GameObject Winnings => winnings;
    public int Score { get; private set; }
    
    public void Initialize(GameMaster givenGameMaster)
    {
        Score = 0;
        GameMaster = givenGameMaster;
    }

    public void WinCard(Transform card)
    {
        card.SetParent(Winnings.transform);
        card.position = Winnings.transform.position;
    }

    public void UpdateScore(int scoreAdded)
    {
        // The value -1 is used to reset the score
        if (scoreAdded == -1)
        {
            Score = 0;
        }
        else
        {
            Score += scoreAdded;
        }
        scoreText.text = $"Score: {Score}";
    }
}
