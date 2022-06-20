# Fugo Pişti Demo
This is a demo Unity project made for Fugo Games.
Please use 1080x1920 portrait aspect ratio to run the game.

## Classes
* GameMaster : MonoBehaviour
* Card : MonoBehaviour
* Player : MonoBehaviour
* Human : Player
* AI : Player
* In all documentation, "GM" refers to the GameMaster object, "Player" refers to Human and AI, "Human" refers to the object the user controls, and "AI" refers to the object the computer controls, "User" refers to the real-world person playing the game.

## Use Case
### Basic Flow:
1. Game starts
2. GM creates the deck and puts 4 cards on ground
3. GM gives the players 4 cards each
4. User decides which card to play and clicks on it
5. The card calls Human that it has been clicked
6. Human calls GM that a card has been chosen
7. GM puts the card on top of the ground, checks if it is a match
8. AI chooses a card and tells GM
9. GM puts the card on top of the ground, checks if it is a match
10. GM checks if Human has any cards left on its hand
11. GM checks if the deck has any cards left
### Alternative Flow:
* 7.a,9.a. It is not a match, continue
* 7.b,9.b. It is a match, GM gives all the cards on the ground to the last played player and tells them to adjust their score
* 10.a. Human has cards, jump to 4.
* 10.b. Human doesn't have cards, continue to 11.
* 11.a. Deck doesn't have cards, calculate who won and end the game
* 11.b. Deck has cards, jump to 3.
