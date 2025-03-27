using UnityEngine;
using System.Collections.Generic;

public class Team
{
    public string Name { get; set; }
    public Color PrimaryColor { get; set; }
    public Color SecondaryColor { get; set; }
    public List<Player> Players { get; set; }
}

public class Player
{
    public string Name { get; set; }
    public float Speed { get; set; }
    public float Shooting { get; set; }
    public float Dunking { get; set; }
}

public class TeamManager
{
    public List<Team> Teams { get; private set; }

    public TeamManager()
    {
        Teams = new List<Team>
        {
            new Team { Name = "Anaheim Amigos", PrimaryColor = new Color(1.0f, 0.5f, 0.0f), SecondaryColor = Color.black }, // Orange and Black
            new Team { Name = "Baltimore Claws", PrimaryColor = new Color(0.0f, 0.5f, 0.0f), SecondaryColor = Color.yellow }, // Green and Yellow
            new Team { Name = "Carolina Cougars", PrimaryColor = Color.blue, SecondaryColor = new Color(0.0f, 0.5f, 0.0f) }, // Blue and Green
            new Team { Name = "Dallas Chaparrals", PrimaryColor = Color.red, SecondaryColor = Color.blue }, // Red, White, and Blue
            new Team { Name = "The Floridians", PrimaryColor = Color.black, SecondaryColor = new Color(1.0f, 0.3f, 0.0f) }, // Black, Hot Orange, Magenta
            new Team { Name = "Houston Mavericks", PrimaryColor = Color.red, SecondaryColor = Color.blue }, // Red, White, and Blue
            new Team { Name = "Kentucky Colonels", PrimaryColor = new Color(0.7f, 1.0f, 0.0f), SecondaryColor = Color.white }, // Chartreuse Green and White
            new Team { Name = "Los Angeles Stars", PrimaryColor = new Color(0.5f, 0.7f, 1.0f), SecondaryColor = Color.white }, // Powder Blue
            new Team { Name = "Memphis Pros", PrimaryColor = Color.red, SecondaryColor = Color.white },
            new Team { Name = "Memphis Sounds", PrimaryColor = Color.red, SecondaryColor = Color.white },
            new Team { Name = "Memphis Tams", PrimaryColor = Color.red, SecondaryColor = Color.white },
            new Team { Name = "Miami Floridians", PrimaryColor = Color.orange, SecondaryColor = Color.white },
            new Team { Name = "Minnesota Muskies", PrimaryColor = Color.blue, SecondaryColor = Color.green },
            new Team { Name = "Minnesota Pipers", PrimaryColor = Color.red, SecondaryColor = Color.white },
            new Team { Name = "New Jersey Americans", PrimaryColor = Color.red, SecondaryColor = Color.white },
            new Team { Name = "New Orleans Buccaneers", PrimaryColor = Color.blue, SecondaryColor = Color.white },
            new Team { Name = "Oakland Oaks", PrimaryColor = Color.green, SecondaryColor = Color.white },
            new Team { Name = "Pittsburgh Condors", PrimaryColor = Color.yellow, SecondaryColor = Color.red },
            new Team { Name = "Pittsburgh Pipers", PrimaryColor = Color.red, SecondaryColor = Color.white },
            new Team { Name = "San Diego Conquistadors", PrimaryColor = Color.blue, SecondaryColor = Color.green },
            new Team { Name = "San Diego Sails", PrimaryColor = new Color(0.0f, 0.3f, 0.6f), SecondaryColor = new Color(1.0f, 0.5f, 0.0f) }, // Navy Blue and Orange
            new Team { Name = "Spirits of St. Louis", PrimaryColor = new Color(0.8f, 0.4f, 0.0f), SecondaryColor = Color.black }, // Burnt Orange, Black, Silver
            new Team { Name = "Texas Chaparrals", PrimaryColor = Color.red, SecondaryColor = Color.blue }, // Same as Dallas Chaparrals
            new Team { Name = "Utah Stars", PrimaryColor = Color.yellow, SecondaryColor = Color.blue },
            new Team { Name = "Virginia Squires", PrimaryColor = Color.red, SecondaryColor = Color.blue },
            new Team { Name = "Washington Caps", PrimaryColor = Color.red, SecondaryColor = Color.white }
        };

        foreach (var team in Teams)
        {
            team.Players = new List<Player>
            {
                new Player { Name = team.Name + " Player 1", Speed = Random.Range(5f, 10f), Shooting = Random.Range(5f, 10f), Dunking = Random.Range(5f, 10f) },
                new Player { Name = team.Name + " Player 2", Speed = Random.Range(5f, 10f), Shooting = Random.Range(5f, 10f), Dunking = Random.Range(5f, 10f) }
            };
        }
    }

    public Team GetRandomOpponent(string selectedTeamName)
    {
        List<Team> availableTeams = Teams.FindAll(t => t.Name != selectedTeamName);
        return availableTeams[Random.Range(0, availableTeams.Count)];
    }
}