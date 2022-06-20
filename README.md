# Libra Grid Demo
This is a demo Unity project made for Libra Softworks.
Made in Unity version 2019.4.16f1 per instructions.

## Comments
* I made the whole game in a single scene. The canvas has two children, one for the main menu and one for the grid. At the start the menu is active and the grid is inactive. After the user hits the "Start" button the activity changes.
* I added additional buttons to be able to change between the menu and the game.

### Menu
* For the size input area, I used an integer input with 2 character limit. Since the game doesn't make sense for a size smaller than 2x2, I check the input text after the Start button is hit to make sure the given integer is bigger than 1, if not, the text changes to notify the user.

### Grid
* In addition to the box count which is asked from the user to input, I added a grid size. In default it is set to 900px and this value can be changed under the GridClass component of the grid. No matter what the value is, the grid instantiates the boxes to fit the given size and puts it in the middle of the screen. It is assumed that the value will be set in between 0 and 1080px.
* The grid has an additional button to go back to the menu, which turns on an initially hidden "Continue" button under the "Start" button to go back to the ongoing game.

### Box
* Since it already has a built-in on/off switch, I decided to make each box out of a Toggle preset.
* The way the game works is that each box has a list of boxes to keep track of the immidiate neighbors which are active.
* With each click, the clicked and the neighboring four boxes manipulate their lists.
* If at any point, a box has a neighbors list of size bigger than two -which means at least three boxes are touching each other side by side- that blob of neighboring boxes get recursively deactivated and their lists wiped clean.
* To get the neighbors of a box I used raycasting that starts from the center of the box and goes towards each cardinal direction to check all four direct neighbors. Initially I used Physics2D.Raycast, but the raycast hits the starting box and stops. Online I found that self-hitting raycasting can be turned off from the project preferences, I tried exactly that and it worked, but I thought I could achieve the same in code, albeit a bit less efficient. Instead of Physics2D.Raycast I used Physics2D.RaycastAll and took the second element of the resulting array to get the immediate neighbor towards the given direction.
