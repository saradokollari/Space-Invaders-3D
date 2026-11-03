# Pantry Patrol

A cat-versus-mice take on Space Invaders, developed for CSCI 381: Video Game Design and Development, Fall 2026.

Protect the pantry as a ginger cat! Shoot yarn balls to chase away the mice while dodging the cheese they throw. Clear every mouse before they reach the row of fish.


## Controls

| Input | Action |
| --- | --- |
| Left / Right arrow | Move the cat |
| Space | Shoot yarn balls |
| Start Game | Begin playing |
| Play Again | Reload the game and return to the start screen |


## How to play

- You begin with three lives. Getting hit by cheese removes one life, followed by a brief period of protection from additional hits.
- Chase away all 30 mice to win.
- You lose if you run out of lives or the mice reach the fish defense line.
- The remaining mice move faster as more are defeated, with an extra speed increase near the end.


## Running the project

1. Download or clone this repository. If downloading a ZIP, extract it first.
2. In Unity Hub, choose **Add > Add project from disk** and select the project folder containing `Assets`, `Packages`, and `ProjectSettings`.
3. Open the project with Unity **6000.6.0f1**, the editor version used for development.
4. Open `Assets/Scenes/SampleScene.unity`.
5. Press **Play** in the Unity Editor, then click **Start Game**.


## Assignment requirements

### Required gameplay changes

- **Enemies accelerate as others are defeated:** The fleet's movement speed increases according to the proportion of mice destroyed, with an additional boost toward the end.
- **Detect when the enemies have descended too far:** Reaching the defense line triggers a game-over screen and stops gameplay. A row of golden fish marks this boundary.
- **Prevent the player from moving offscreen:** The cat's horizontal movement is restricted to the pantry's playable area.

### Four selected optional features

1. **Starting and ending screens:** A start menu explains the controls. Winning or losing displays an ending screen with a replay button. These screens are implemented within the gameplay scene.
2. **Lives:** The player has three lives, displayed in the HUD. Losing all three ends the game.
3. **Improved enemy and player graphics:** The original spaceship and aliens are represented by a ginger cat and alternating gray and caramel mouse sprites.
4. **New background:** A pixel-art pantry replaces the original background.

### Additional visual changes

- Blue yarn balls replace the player's darts and spin while traveling.
- Cheese pieces replace enemy projectiles.
- The menus and HUD use a pixel font, golden text, and wooden frames to match the pantry.


## Code improvements

### 1. Time-based enemy projectile movement
**File:** `Assets/AlienBullet.cs`

Originally, enemy bullets moved a fixed distance of 0.1 units each physics update. This made their actual speed depend on the configured physics timestep.

I changed movement to use `speed * Time.fixedDeltaTime`, with speed expressed in units per second. This preserves consistent movement if the physics timestep changes.

### 2. Collision handling based on object type
**File:** `Assets/AlienBullet.cs`

Originally, an enemy bullet destroyed itself and whatever object it collided with, without checking the object's type.

I added checks for the player and player projectiles. Hitting the player now calls `GameManager.TakeHit()` instead of destroying the player object. Hitting a player projectile destroys the projectiles. A `hasHit` flag also prevents the same bullet from processing multiple damaging collisions before it is destroyed.

### 3. Encapsulation of player shooting settings and state
**File:** `Assets/Spaceship.cs`

Originally, `shotCooldown` was public and static, while `lastShot` was also public. This exposed internal shooting state and made the cooldown shared between all instances.

I changed the cooldown to a private instance field with `[SerializeField]`, keeping it configurable in Unity's Inspector. I also made `lastShot` private because other scripts do not need to change it.


## Credits and AI disclosure

- **AI coding assistance:** ChatGPT assisted with revising code and troubleshooting. The project was developed through my prompts, and I tested and adjusted the resulting gameplay in Unity.
- **AI-generated artwork:** The pantry background, ginger cat, gray mouse, caramel mouse, blue yarn ball, cheese, and fish images were generated using ChatGPT's image-generation tools through my prompts and follow-up requests.
- **Font:** [Press Start 2P](https://fonts.google.com/specimen/Press+Start+2P) by CodeMan38.

The game uses 2D artwork within the instructor's 3D Unity project, retaining 3D collision components for gameplay.
