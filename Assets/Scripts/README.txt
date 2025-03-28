This directory contains the C# scripts for the ABA Jam game.

The following scripts are included:
1. GameManager.cs - Singleton that handles the global game state and persists across scenes
2. TeamManager.cs - Manages the 27 non-existent ABA teams, their colors, and player data
3. TeamSelectionMenu.cs - Handles the team selection UI and logic
4. GameModeSelection.cs - Manages the game mode selection UI and logic
5. PlayerController.cs - Controls player movement, AI behavior, and interactions with the ball
6. BallController.cs - Handles ball physics, shooting, and scoring
7. GameController.cs - Manages the 2v2 game logic, scoring, and game state
8. ShootingSpot.cs - Defines shooting spots for the 3-point contest
9. ThreePointContest.cs - Manages the 3-point contest mode
10. SceneSetupHelper.cs - Helps with setting up scenes and transitions

These scripts work together to create the complete game experience. The GameManager and TeamManager are particularly important as they maintain state across different scenes.