using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameModeSelection : MonoBehaviour
{
    public Button twoVsTwoButton;
    public Button threePointContestButton;
    public TextMeshProUGUI teamNameText;
    public Image teamLogoImage;

    void Start()
    {
        twoVsTwoButton.onClick.AddListener(OnTwoVsTwoClicked);
        threePointContestButton.onClick.AddListener(OnThreePointContestClicked);
        
        // Display selected team info
        if (GameManager.Instance != null && GameManager.Instance.SelectedTeam != null)
        {
            Team selectedTeam = GameManager.Instance.SelectedTeam;
            teamNameText.text = selectedTeam.Name;
            teamNameText.color = selectedTeam.PrimaryColor;
            
            // If we had a logo image for each team, we would set it here
            if (teamLogoImage != null)
            {
                teamLogoImage.color = selectedTeam.PrimaryColor;
            }
        }
        else
        {
            Debug.LogWarning("No team selected or GameManager not found!");
        }
        
        // Add some visual effects to buttons
        AddButtonEffects(twoVsTwoButton);
        AddButtonEffects(threePointContestButton);
    }
    
    void AddButtonEffects(Button button)
    {
        // Add hover animation
        button.transition = Selectable.Transition.ColorTint;
        
        // Add scale animation on hover
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        
        // This would normally use EventTrigger, but we'll use a simpler approach for now
        button.onClick.AddListener(() => {
            // Play sound effect
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
            
            // Visual feedback
            button.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            // Reset scale after a short delay
            Invoke("ResetButtonScale", 0.1f);
        });
    }
    
    void ResetButtonScale()
    {
        twoVsTwoButton.transform.localScale = Vector3.one;
        threePointContestButton.transform.localScale = Vector3.one;
    }

    void OnTwoVsTwoClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainGameScene();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }

    void OnThreePointContestClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadThreePointContestScene();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }
}