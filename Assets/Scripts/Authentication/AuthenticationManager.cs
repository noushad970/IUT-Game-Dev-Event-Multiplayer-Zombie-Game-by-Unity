using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;   // ← Added
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance;

    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore Firestore { get; private set; }   // ← Added

    public FirebaseUser CurrentUser => Auth?.CurrentUser;
    public bool IsInitialized { get; private set; }

    // Events
    public Action<FirebaseUser> OnLoginSuccess;
    public Action<string> OnLoginFailed;
    public Action<FirebaseUser> OnRegisterSuccess;
    public Action<string> OnRegisterFailed;
    public Action OnLogout;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            Auth = FirebaseAuth.DefaultInstance;
            Firestore = FirebaseFirestore.DefaultInstance;   // ← Initialize Firestore
            IsInitialized = true;
            Debug.Log("Firebase + Firestore Initialized.");
        }
        else
        {
            Debug.LogError("Firebase Dependency Error: " + dependencyStatus);
        }
    }

    public async Task<bool> Register(string email, string password, string username = "")
    {
        if (!IsInitialized)
        {
            OnRegisterFailed?.Invoke("Firebase not initialized.");
            return false;
        }

        try
        {
            AuthResult result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;

            Debug.Log($"Register Success: {user.Email}");

            // Create PlayerProfile in Firestore
            bool profileCreated = await CreatePlayerProfile(user, username);

            if (profileCreated)
            {
                OnRegisterSuccess?.Invoke(user);
                return true;
            }
            else
            {
                OnRegisterFailed?.Invoke("Failed to create player profile.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            OnRegisterFailed?.Invoke(ex.Message);
            return false;
        }
    }

    private async Task<bool> CreatePlayerProfile(FirebaseUser user, string username)
    {
        if (Firestore == null || user == null) return false;

        try
        {
            // Default values
            var profile = new PlayerProfile
            {
                uid = user.UserId,
                username = string.IsNullOrEmpty(username) ? user.Email.Split('@')[0] : username, // fallback username
                email = user.Email,
                coins = 100,                    // starting coins
                selectedCharacter = 0,
                totalSingleKills = 0,
                totalMultiKills = 0,
                highestWave = 0,
                gamesPlayed = 0
            };

            // Create document with UID as document ID (recommended)
            DocumentReference docRef = Firestore.Collection("players").Document(user.UserId);

            await docRef.SetAsync(profile);

            Debug.Log($"Player profile created in Firestore for UID: {user.UserId}");
            await LoadPlayerProfile();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create Firestore profile: {ex}");
            return false;
        }
    }

    public async Task<bool> Login(string email, string password)
    {
        if (!IsInitialized)
        {
            OnLoginFailed?.Invoke("Firebase not initialized.");
            return false;
        }

        try
        {
            AuthResult result = await Auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;

            Debug.Log($"Login Success: {user.Email}");
            OnLoginSuccess?.Invoke(user);
            await LoadPlayerProfile();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            OnLoginFailed?.Invoke(ex.Message);
            return false;
        }
    }

    public void Logout()
    {
        if (!IsInitialized) return;
        Auth.SignOut();
        Debug.Log("Logged Out");
        OnLogout?.Invoke();
    }

    public bool IsLoggedIn() => CurrentUser != null;

    public string GetUID() => CurrentUser?.UserId ?? "";
    public string GetEmail() => CurrentUser?.Email ?? "";
    public PlayerProfile CurrentProfile { get; private set; }
    private async Task LoadPlayerProfile()
    {
        if (CurrentUser == null)
            return;

        DocumentReference doc =
            Firestore.Collection("players").Document(CurrentUser.UserId);

        DocumentSnapshot snapshot = await doc.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            CurrentProfile = snapshot.ConvertTo<PlayerProfile>();

            Debug.Log("Profile Loaded");
            Debug.Log("Character = " + CurrentProfile.selectedCharacter);
        }
    }
    public int getSelectedCharacter()
    {
        return CurrentUser != null ? GetSelectedPlayer().Result : 0;
    }
    public async Task<int> GetSelectedPlayer()
    {
        if (!IsInitialized)
        {
            Debug.LogError("Firebase not initialized.");
            return 0;
        }

        if (CurrentUser == null)
        {
            Debug.LogError("No user is logged in.");
            return 0;
        }

        try
        {
            DocumentReference docRef =
                Firestore.Collection("players").Document(CurrentUser.UserId);

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Debug.LogError("Player profile not found.");
                return 0;
            }

            PlayerProfile profile = snapshot.ConvertTo<PlayerProfile>();

            Debug.Log("Selected Character : " + profile.selectedCharacter);

            return profile.selectedCharacter;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load selected character: " + ex.Message);
            return 0;
        }
    }
}