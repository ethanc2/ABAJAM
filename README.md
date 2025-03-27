# ABA Jam

A 2D retro-style basketball game inspired by NBA Jam, featuring the historical ABA teams from the 1970s. This game includes a 2v2 gameplay mode and a 3-point contest mode, all with authentic retro 1970s flair.

## Features

- **Team Selection**: Choose from 27 non-existent ABA teams, each with unique colors and players.
- **2v2 Gameplay**: Fast-paced 2v2 basketball with exaggerated player movements and arcade-style gameplay.
- **3-Point Contest**: Test your shooting skills in a timed 3-point shooting competition.
- **Retro Polish**: 16-bit pixel art style with animations, sound effects, and 1970s elements.

## Historical Team Colors

The game features historically accurate team colors based on research of the original ABA teams:

- **Anaheim Amigos**: Orange and Black
- **Baltimore Claws**: Green and Yellow
- **Carolina Cougars**: Blue and Green
- **Dallas Chaparrals**: Red, White, and Blue
- **The Floridians**: Black, Hot Orange, and Magenta
- **Houston Mavericks**: Red, White, and Blue
- **Kentucky Colonels**: Chartreuse Green and White (early years)
- **Los Angeles Stars**: Powder Blue
- **Spirits of St. Louis**: Burnt Orange, Black, and Silver
- **San Diego Sails**: Navy Blue and Orange

Some teams changed colors over time, such as the Kentucky Colonels who started with chartreuse green and white, later switched to blue and white, and finally to red, white, and blue. The game uses their early chartreuse green and white color scheme.

For teams with limited historical color information, we've made educated guesses based on available resources:

- Memphis teams (Pros, Sounds, Tams)
- Minnesota teams (Muskies, Pipers)
- New Jersey Americans
- Pittsburgh teams (Condors, Pipers)

Sources for historical team colors include [Remember the ABA](http://www.remembertheaba.com/), [Sports Logo History](https://sportslogohistory.com/), and other historical basketball archives.

## Project Structure

The project is organized into the following structure:

```
ABAJam/
├── Assets/
│   ├── Scenes/
│   │   ├── TeamSelectionScene.unity
│   │   ├── GameModeSelectionScene.unity
│   │   ├── MainGameScene.unity
│   │   └── ThreePointContestScene.unity
│   ├── Scripts/
│   │   ├── TeamManager.cs
│   │   ├── GameManager.cs
│   │   ├── TeamSelectionMenu.cs
│   │   ├── GameModeSelection.cs
│   │   ├── PlayerController.cs
│   │   ├── BallController.cs
│   │   ├── GameController.cs
│   │   ├── ShootingSpot.cs
│   │   └── ThreePointContest.cs
│   ├── Sprites/
│   ├── Audio/
│   ├── Animations/
│   └── Prefabs/
└── README.md
```

## Installation

1. Clone this repository or download the ZIP file.
2. Open the project in Unity 2022.3 or later.
3. Open the `TeamSelectionScene` from the `Assets/Scenes` folder.
4. Press Play to start the game.

## Controls

### Team Selection
- Use the mouse to select a team
- Click "Confirm" to proceed

### Game Mode Selection
- Click "2v2 Match" or "3-Point Contest" to choose a mode

### 2v2 Gameplay
- Arrow keys: Move player
- Space: Shoot/Dunk
- Left Control: Steal attempt
- P or Escape: Pause game

### 3-Point Contest
- Space: Shoot
- Arrow keys: Move between shooting spots
- P or Escape: Pause game

## Core Scripts

### TeamManager.cs
Manages the 27 non-existent ABA teams, their colors, and player data.

### GameManager.cs
Singleton that handles the global game state and persists across scenes.

### TeamSelectionMenu.cs
Handles the team selection UI and logic.

### GameModeSelection.cs
Manages the game mode selection UI and logic.

### PlayerController.cs
Controls player movement, AI behavior, and interactions with the ball.

### BallController.cs
Handles ball physics, shooting, and scoring.

### GameController.cs
Manages the 2v2 game logic, scoring, and game state.

### ShootingSpot.cs
Defines shooting spots for the 3-point contest.

### ThreePointContest.cs
Manages the 3-point contest mode.

## Game Mechanics

### 2v2 Gameplay
- Players can move, shoot, dunk, and steal
- Score points by shooting the ball through the opponent's basket
- 2 points for regular shots, 3 points for shots beyond the 3-point line
- Score 3 consecutive baskets to go "on fire" for enhanced abilities
- Game consists of 4 quarters with halftime

### 3-Point Contest
- Shoot from 5 different spots around the 3-point line
- 5 balls per rack, with the last ball being a "money ball" worth 2 points
- Regular shots are worth 1 point
- 60-second time limit to score as many points as possible

## Development

This project was created using Unity and C#. The game features a modular architecture with separate scenes for each mode and a centralized GameManager for data persistence.

## Credits

- Game Design: [Your Name]
- Programming: [Your Name]
- Art: [Your Name or Artist's Name]
- Historical Research: Based on ABA team archives and historical records
- Sound Effects & Music: [Source]

## License

[Specify your license here]