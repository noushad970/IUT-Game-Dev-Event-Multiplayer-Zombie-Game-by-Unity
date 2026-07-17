using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Photon.Pun;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance;

    public string PlayFabId { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsLoggedIn() => !string.IsNullOrEmpty(PlayFabId);

    // Events
    public Action<string> OnLoginSuccess;           // PlayFabId or user info
    public Action<string> OnLoginFailed;
    public Action<string> OnRegisterSuccess;
    public Action<string> OnRegisterFailed;
    public Action OnLogout;

    public PlayerProfile CurrentProfile { get; private set; }

    private void Awake()
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

        InitializePlayFab();
    }

    private void InitializePlayFab()
    {
        // PlayFab is typically initialized automatically via PlayFabSettings.TitleId
        // Make sure TitleId is set in PlayFabSettings or via code
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            Debug.LogError("PlayFab TitleId is not set! Set it in PlayFabSettings or via code.");
            return;
        }

        IsInitialized = true;
        Debug.Log("PlayFab Initialized.");
    }

    public async Task<bool> Register(string email, string password, string username = "")
    {
        if (!IsInitialized)
        {
            OnRegisterFailed?.Invoke("PlayFab not initialized.");
            return false;
        }

        var tcs = new TaskCompletionSource<bool>();

        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = string.IsNullOrEmpty(username) ? email.Split('@')[0] : username,
            DisplayName = string.IsNullOrEmpty(username) ? email.Split('@')[0] : username,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(
            request,
            async result =>
            {
                PlayFabId = result.PlayFabId;

                Debug.Log($"Register Success : {PlayFabId}");

                bool profileCreated = await CreatePlayerProfile(username);

                if (profileCreated)
                {
                    OnRegisterSuccess?.Invoke(PlayFabId);
                    tcs.SetResult(true);
                    PlayerPrefs.SetString("EMAIL", email);
                    PlayerPrefs.SetString("PASSWORD", password);
                    PlayerPrefs.Save();
                }
                else
                {
                    OnRegisterFailed?.Invoke("Failed to create profile.");
                    tcs.SetResult(false);
                }
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
                OnRegisterFailed?.Invoke(error.ErrorMessage);
                tcs.SetResult(false);
            });

        return await tcs.Task;
    }
    private async Task<bool> CreatePlayerProfile(string username)
    {
        var tcs = new TaskCompletionSource<bool>();

        PlayerProfile profile = new PlayerProfile
        {
            uid = PlayFabId,
            username = string.IsNullOrEmpty(username) ? "Player" : username,
            email = "",
            coins = 100,
            selectedCharacter = 0,
            totalSingleKills = 0,
            totalMultiKills = 0,
            highestWave = 0,
            gamesPlayed = 0
        };

        string json = JsonUtility.ToJson(profile);

        UpdateUserDataRequest request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { "PlayerProfile", json }
        },
            Permission = UserDataPermission.Public
        };

        PlayFabClientAPI.UpdateUserData(
            request,
            async result =>
            {
                Debug.Log("Profile Created.");

              await LoadPlayerProfile();

                tcs.SetResult(true);
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
                tcs.SetResult(false);
            });

        return await tcs.Task;
    }

    public async Task<bool> Login(string email, string password)
    {
        if (!IsInitialized)
        {
            OnLoginFailed?.Invoke("PlayFab not initialized.");
            return false;
        }

        var tcs = new TaskCompletionSource<bool>();

        LoginWithEmailAddressRequest request =
            new LoginWithEmailAddressRequest
            {
                Email = email,
                Password = password
            };

        PlayFabClientAPI.LoginWithEmailAddress(
            request,
            async result =>
            {
                PlayFabId = result.PlayFabId;

                Debug.Log("Login Success : " + PlayFabId);
                PlayerPrefs.SetString("EMAIL", email);
                PlayerPrefs.SetString("PASSWORD", password);
                PlayerPrefs.Save();
                await LoadPlayerProfile();

                OnLoginSuccess?.Invoke(PlayFabId);

                tcs.SetResult(true);
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());

                OnLoginFailed?.Invoke(error.ErrorMessage);

                tcs.SetResult(false);
            });

        return await tcs.Task;
    }

    public void Logout()
    {
        // PlayFab doesn't have a direct "SignOut" like Firebase, but you can clear local session
        PlayFabId = null;
        CurrentProfile = null;
        Debug.Log("Logged Out");
        OnLogout?.Invoke();
    }

    public string GetUID() => PlayFabId ?? "";
    public string GetEmail() => ""; // Not directly stored; retrieve from profile if needed

    private async Task<bool> LoadPlayerProfile()
    {
        if (string.IsNullOrEmpty(PlayFabId))
            return false;

        var tcs = new TaskCompletionSource<bool>();

        GetUserDataRequest request = new GetUserDataRequest
        {
            Keys = new List<string> { "PlayerProfile" }
        };

        PlayFabClientAPI.GetUserData(
            request,
            result =>
            {
                if (result.Data != null &&
                    result.Data.ContainsKey("PlayerProfile"))
                {
                    string json = result.Data["PlayerProfile"].Value;

                    CurrentProfile = JsonUtility.FromJson<PlayerProfile>(json);

                    PhotonNetwork.NickName = CurrentProfile.username;

                    Debug.Log("Profile Loaded");
                    Debug.Log("Username : " + CurrentProfile.username);
                    Debug.Log("Photon Nickname : " + PhotonNetwork.NickName);

                    tcs.SetResult(true);
                }
                else
                {
                    Debug.LogWarning("Player profile not found.");
                    tcs.SetResult(false);
                }
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
                tcs.SetResult(false);
            });

        return await tcs.Task;
    }

    public int getSelectedCharacter()
    {
        return CurrentProfile?.selectedCharacter ?? 0;
    }

    public async Task<int> GetSelectedPlayer()
    {
        if (!IsInitialized || string.IsNullOrEmpty(PlayFabId))
        {
            Debug.LogError("PlayFab not initialized.");
            return 0;
        }

        if (CurrentProfile == null)
        {
            bool loaded = await LoadPlayerProfile();

            if (!loaded)
                return 0;
        }

        return CurrentProfile.selectedCharacter;
    }

    // Optional: Save profile updates (call this whenever you change stats)
    public void SavePlayerProfile()
    {
        if (CurrentProfile == null || string.IsNullOrEmpty(PlayFabId))
            return;

        string json = JsonUtility.ToJson(CurrentProfile);

        UpdateUserDataRequest request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { "PlayerProfile", json }
        }
        };

        PlayFabClientAPI.UpdateUserData(
            request,
            result =>
            {
                Debug.Log("Player profile saved to PlayFab.");
            },
            error =>
            {
                Debug.LogError("Failed to save profile: " + error.GenerateErrorReport());
            });
    }
}

//using System;
//using System.Threading.Tasks;
//using Firebase;
//using Firebase.Auth;
//using Firebase.Firestore;   // ← Added
//using UnityEngine;
//using Photon.Pun;
//public class AuthenticationManager : MonoBehaviour
//{
//    public static AuthenticationManager Instance;

//    public FirebaseAuth Auth { get; private set; }
//    public FirebaseFirestore Firestore { get; private set; }   // ← Added

//    public FirebaseUser CurrentUser => Auth?.CurrentUser;
//    public bool IsInitialized { get; private set; }

//    // Events
//    public Action<FirebaseUser> OnLoginSuccess;
//    public Action<string> OnLoginFailed;
//    public Action<FirebaseUser> OnRegisterSuccess;
//    public Action<string> OnRegisterFailed;
//    public Action OnLogout;

//    private async void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }

//        await InitializeFirebase();
//    }

//    private async Task InitializeFirebase()
//    {
//        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
//        if (dependencyStatus == DependencyStatus.Available)
//        {
//            Auth = FirebaseAuth.DefaultInstance;
//            Firestore = FirebaseFirestore.DefaultInstance;   // ← Initialize Firestore
//            IsInitialized = true;
//            Debug.Log("Firebase + Firestore Initialized.");
//        }
//        else
//        {
//            Debug.LogError("Firebase Dependency Error: " + dependencyStatus);
//        }
//    }

//    public async Task<bool> Register(string email, string password, string username = "")
//    {
//        if (!IsInitialized)
//        {
//            OnRegisterFailed?.Invoke("Firebase not initialized.");
//            return false;
//        }

//        try
//        {
//            AuthResult result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
//            FirebaseUser user = result.User;

//            Debug.Log($"Register Success: {user.Email}");

//            // Create PlayerProfile in Firestore
//            bool profileCreated = await CreatePlayerProfile(user, username);

//            if (profileCreated)
//            {
//                OnRegisterSuccess?.Invoke(user);
//                return true;
//            }
//            else
//            {
//                OnRegisterFailed?.Invoke("Failed to create player profile.");
//                return false;
//            }
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError(ex);
//            OnRegisterFailed?.Invoke(ex.Message);
//            return false;
//        }
//    }

//    private async Task<bool> CreatePlayerProfile(FirebaseUser user, string username)
//    {
//        if (Firestore == null || user == null) return false;

//        try
//        {
//            // Default values
//            var profile = new PlayerProfile
//            {
//                uid = user.UserId,
//                username = string.IsNullOrEmpty(username) ? user.Email.Split('@')[0] : username, // fallback username
//                email = user.Email,
//                coins = 100,                    // starting coins
//                selectedCharacter = 0,
//                totalSingleKills = 0,
//                totalMultiKills = 0,
//                highestWave = 0,
//                gamesPlayed = 0
//            };

//            // Create document with UID as document ID (recommended)
//            DocumentReference docRef = Firestore.Collection("players").Document(user.UserId);

//            await docRef.SetAsync(profile);

//            Debug.Log($"Player profile created in Firestore for UID: {user.UserId}");
//            await LoadPlayerProfile();
//            return true;
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError($"Failed to create Firestore profile: {ex}");
//            return false;
//        }
//    }

//    public async Task<bool> Login(string email, string password)
//    {
//        if (!IsInitialized)
//        {
//            OnLoginFailed?.Invoke("Firebase not initialized.");
//            return false;
//        }

//        try
//        {
//            AuthResult result = await Auth.SignInWithEmailAndPasswordAsync(email, password);
//            FirebaseUser user = result.User;

//            Debug.Log($"Login Success: {user.Email}");
//            OnLoginSuccess?.Invoke(user);
//            await LoadPlayerProfile();
//            return true;
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError(ex);
//            OnLoginFailed?.Invoke(ex.Message);
//            return false;
//        }
//    }

//    public void Logout()
//    {
//        if (!IsInitialized) return;
//        Auth.SignOut();
//        Debug.Log("Logged Out");
//        OnLogout?.Invoke();
//    }

//    public bool IsLoggedIn() => CurrentUser != null;

//    public string GetUID() => CurrentUser?.UserId ?? "";
//    public string GetEmail() => CurrentUser?.Email ?? "";
//    public PlayerProfile CurrentProfile { get; private set; }
//    private async Task LoadPlayerProfile()
//    {
//        if (CurrentUser == null)
//            return;

//        DocumentReference doc =
//            Firestore.Collection("players").Document(CurrentUser.UserId);

//        DocumentSnapshot snapshot = await doc.GetSnapshotAsync();

//        if (snapshot.Exists)
//        {
//            CurrentProfile = snapshot.ConvertTo<PlayerProfile>();

//            // Set Photon nickname
//            Photon.Pun.PhotonNetwork.NickName = CurrentProfile.username;

//            Debug.Log("Profile Loaded");
//            Debug.Log("Username : " + CurrentProfile.username);
//            Debug.Log("Photon Nickname : " + Photon.Pun.PhotonNetwork.NickName);
//        }
//    }
//    public int getSelectedCharacter()
//    {
//        return CurrentUser != null ? GetSelectedPlayer().Result : 0;
//    }
//    public async Task<int> GetSelectedPlayer()
//    {
//        if (!IsInitialized)
//        {
//            Debug.LogError("Firebase not initialized.");
//            return 0;
//        }

//        if (CurrentUser == null)
//        {
//            Debug.LogError("No user is logged in.");
//            return 0;
//        }

//        try
//        {
//            DocumentReference docRef =
//                Firestore.Collection("players").Document(CurrentUser.UserId);

//            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

//            if (!snapshot.Exists)
//            {
//                Debug.LogError("Player profile not found.");
//                return 0;
//            }

//            PlayerProfile profile = snapshot.ConvertTo<PlayerProfile>();

//            Debug.Log("Selected Character : " + profile.selectedCharacter);

//            return profile.selectedCharacter;
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError("Failed to load selected character: " + ex.Message);
//            return 0;
//        }
//    }
//}