# ABA Jam

A 2D retro-style basketball game inspired by NBA Jam, featuring the historical ABA teams from the 1970s. This game includes a 2v2 gameplay mode and a 3-point contest mode, all with authentic retro 1970s flair.

## Features

- **Team Selection**: Choose from 27 historical ABA teams, each with unique colors and players.
- **2v2 Gameplay**: Fast-paced 2v2 basketball with exaggerated player movements and arcade-style gameplay.
- **3-Point Contest**: Test your shooting skills in a timed 3-point shooting competition.
- **Retro Polish**: 16-bit pixel art style with animations, sound effects, and 1970s elements.

## Codebase Review (v2)

### Project Architecture Overview

ABAJAM follows a well-structured Unity architecture with clean separation of concerns:

#### Scene Flow
1. **Team Selection** → Choose from 27 ABA teams
2. **Game Mode Selection** → 2v2 or 3-Point Contest
3. **Game Scenes** → MainGameScene or ThreePointContestScene

#### Core Components

1. **GameManager.cs**
   - Singleton pattern for cross-scene persistence
   - Manages team selection and scene transitions
   - Stores references to selected team and opponent

2. **TeamManager.cs**
   - Defines Team and Player classes
   - Contains data for all 27 ABA teams with colors
   - Handles random opponent selection

3. **PlayerController.cs**
   - Controls player movement and actions (shooting, stealing)
   - Implements AI behavior for CPU-controlled players
   - Handles ball possession and passing logic

4. **BallController.cs**
   - Manages ball physics and visual effects
   - Implements shooting, passing, and scoring detection
   - Features ABA-themed visual effects (red/white/blue ball)

5. **GameController.cs**
   - Core game logic for 2v2 mode
   - Manages quarters, score, and game state
   - Handles "on fire" special abilities

6. **ThreePointContest.cs**
   - Specialized logic for 3-point contest
   - Manages shooting spots, ball racks, and timing
   - Implements money ball mechanics

### Code Quality Assessment

#### Strengths
- Clean separation of concerns between components
- Consistent naming conventions and good documentation
- Proper use of Unity's component architecture
- Well-organized project structure
- Good use of coroutines for timing-based events

#### Potential Improvements
1. **Code Refactoring**
   - Some lengthy methods in GameController and ThreePointContest could be refactored into smaller, more focused methods
   - Duplicate code exists between game modes that could be abstracted

2. **Data Management**
   - Team data is hardcoded rather than stored in external configuration files
   - Consider using ScriptableObjects for team data

3. **Player System**
   - Player stats generation is relatively basic
   - Could implement more complex player attributes and abilities

4. **Physics System**
   - Shot physics calculations could be more sophisticated
   - Could implement more realistic ball handling and collision

5. **UI System**
   - UI implementation is functional but could be enhanced with animations
   - Add more detailed stat tracking and display

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

## Future Development Roadmap

Based on the code review, potential future enhancements could include:

1. **Enhanced Player Stats System**
   - Implement more realistic player attributes
   - Add player fatigue and stamina mechanics

2. **Improved AI Behavior**
   - More sophisticated opponent AI with different play styles
   - Dynamic difficulty adjustment

3. **Additional Game Modes**
   - Season mode with standings and playoffs
   - Historical ABA challenges based on real events

4. **Visual Enhancements**
   - More detailed player sprites and animations
   - Additional court designs based on historical ABA venues

5. **Performance Optimizations**
   - Refactor code for better performance
   - Implement object pooling for frequently instantiated objects

## Credits

- Game Design: [Your Name]
- Programming: [Your Name]
- Art: [Your Name or Artist's Name]
- Historical Research: Based on ABA team archives and historical records
- Sound Effects & Music: [Source]

## License

[Specify your license here]