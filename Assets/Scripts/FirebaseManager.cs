using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore DB { get; private set; }

    public bool IsFirebaseReady { get; private set; } = false;

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

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;

            if (status == DependencyStatus.Available)
            {
                Auth = FirebaseAuth.DefaultInstance;
                DB = FirebaseFirestore.DefaultInstance;

                IsFirebaseReady = true;

                Debug.Log("Firebase Initialized Successfully.");
            }
            else
            {
                Debug.LogError("Firebase Dependency Error : " + status);
            }
        });
    }
}