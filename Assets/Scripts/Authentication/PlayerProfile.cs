using Firebase.Firestore;   // ← Important

[FirestoreData]   // ← Add this attribute
public class PlayerProfile
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string username { get; set; }
    [FirestoreProperty] public string email { get; set; }
    [FirestoreProperty] public int coins { get; set; }
    [FirestoreProperty] public int selectedCharacter { get; set; }
    [FirestoreProperty] public int totalSingleKills { get; set; }
    [FirestoreProperty] public int totalMultiKills { get; set; }
    [FirestoreProperty] public int highestWave { get; set; }
    [FirestoreProperty] public int gamesPlayed { get; set; }

    // Parameterless constructor is required by Firestore
    public PlayerProfile() { }
}