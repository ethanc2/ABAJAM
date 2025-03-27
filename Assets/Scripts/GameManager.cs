using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Team SelectedTeam { get; set; }
    public Team OpponentTeam { get; set; }
    private TeamManager teamManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            teamManager = new TeamManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedTeam(string teamName)
    {
        SelectedTeam = teamManager.Teams.Find(t => t.Name == teamName);
        OpponentTeam = teamManager.GetRandomOpponent(teamName);
        Debug.Log($"Selected team: {SelectedTeam.Name}, Opponent team: {OpponentTeam.Name}");
    }

    public void LoadTeamSelectionScene()
    {
        SceneManager.LoadScene("TeamSelectionScene");
    }

    public void LoadGameModeSelectionScene()
    {
        SceneManager.LoadScene("GameModeSelectionScene");
    }

    public void LoadMainGameScene()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void LoadThreePointContestScene()
    {
        SceneManager.LoadScene("ThreePointContestScene");
    }
}