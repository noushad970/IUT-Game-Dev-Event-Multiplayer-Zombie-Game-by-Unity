using System;

[Serializable]
public class PlayerProfile
{
    public string uid;
    public string username;
    public string email;

    public int coins;

    public int selectedCharacter;

    public int totalSingleKills;
    public int totalMultiKills;

    public int highestWave;
    public int gamesPlayed;

    public PlayerProfile()
    {
        uid = "";
        username = "";
        email = "";

        coins = 100;

        selectedCharacter = 1;

        totalSingleKills = 0;
        totalMultiKills = 0;

        highestWave = 0;
        gamesPlayed = 0;
    }
}