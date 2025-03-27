using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectionMenu : MonoBehaviour
{
    public GameObject teamButtonPrefab;
    public Transform teamListContainer;
    public Button confirmButton;
    private TeamManager teamManager;
    private string selectedTeamName;

    void Start()
    {
        teamManager = new TeamManager();
        PopulateTeamList();
        confirmButton.onClick.AddListener(OnConfirmClicked);
        confirmButton.interactable = false;
    }

    void PopulateTeamList()
    {
        foreach (var team in teamManager.Teams)
        {
            GameObject buttonObj = Instantiate(teamButtonPrefab, teamListContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Image buttonImage = buttonObj.GetComponent<Image>();

            buttonText.text = team.Name;
            buttonImage.color = team.PrimaryColor;
            
            // Set button colors for normal and highlighted states
            ColorBlock colors = button.colors;
            colors.normalColor = team.PrimaryColor;
            colors.highlightedColor = team.SecondaryColor;
            button.colors = colors;
            
            button.onClick.AddListener(() => SelectTeam(team.Name));
        }
    }

    void SelectTeam(string teamName)
    {
        selectedTeamName = teamName;
        confirmButton.interactable = true;
        
        // Visual feedback for selection
        foreach (Transform child in teamListContainer)
        {
            Button button = child.GetComponent<Button>();
            TextMeshProUGUI text = child.GetComponentInChildren<TextMeshProUGUI>();
            
            if (text.text == teamName)
            {
                // Highlight selected team
                button.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                text.fontStyle = FontStyles.Bold;
            }
            else
            {
                // Reset other teams
                button.transform.localScale = Vector3.one;
                text.fontStyle = FontStyles.Normal;
            }
        }
        
        Debug.Log($"Selected Team: {teamName}");
    }

    void OnConfirmClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSelectedTeam(selectedTeamName);
            GameManager.Instance.LoadGameModeSelectionScene();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }
}