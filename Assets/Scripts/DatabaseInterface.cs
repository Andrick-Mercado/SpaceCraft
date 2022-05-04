using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using TMPro;
public class DatabaseInterface : MonoBehaviour
{
    public User loggedInUser;
    string snapshot="";
    public TMP_InputField registerInput;
    public TMP_InputField loginInput;
    public GameObject MenuCover;
    public static DatabaseInterface db;
    bool looped = false;
    private void Start()
    {
        if (db)
        {
            
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        db = this;
    }
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(looped&&scene.buildIndex==0)
            GameObject.Find("LoginCover").SetActive(false);
        looped = true;
    }

  

    // called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        save();
    }
    void GetDatabaseSnapshot()
    {
        string result = "error";
        FirebaseDatabase.GetInstance("https://spacecraft-46d0b-default-rtdb.firebaseio.com/")
        .GetReference("default")
        .GetValueAsync().ContinueWithOnMainThread(task => {
        if (task.IsFaulted)
        {
              // Handle the error...
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            result=snapshot.ToString();
            print(result.ToString());
               
                // Do something with snapshot...
            }
        });
        
    }
    public void TriggerRegisterAttempt()
    {
        RegisterUser(registerInput.text);
        print(registerInput.text);
    }
    public void RegisterUser(string name)
    {
        
        User user = new User(name);
        string json = JsonUtility.ToJson(user);

        FirebaseDatabase.GetInstance("https://spacecraft-46d0b-default-rtdb.firebaseio.com/")
        .RootReference.Child(name).SetRawJsonValueAsync(json);
        
    }
    public void TriggerLoginAttempt()
    {
        LoginUser(loginInput.text);
        
    }
    public void LoginUser(string name)
    {

        FirebaseDatabase.GetInstance("https://spacecraft-46d0b-default-rtdb.firebaseio.com/").RootReference.
            Child(name).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                print("login unsuccessful");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string result = snapshot.ToString();
                User user = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
                print("login successful"+user.username);
                loggedInUser = user;
                    LoadNextScene();
            }
        });

    }
    public void LoadNextScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        MenuCover.SetActive(false);
    }
    void OnApplicationQuit()
    {
        save();
    }
    public void save()
    {
        InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem)
        {
            loggedInUser.inventory = inventorySystem.inventory;
            string json = JsonUtility.ToJson(loggedInUser);
            FirebaseDatabase.GetInstance("https://spacecraft-46d0b-default-rtdb.firebaseio.com/")
        .RootReference.Child(loggedInUser.username).SetRawJsonValueAsync(json);
        }
    }
    [System.Serializable]
    public class User
    {
        public string username;
        public  List<InventoryItem> inventory;

        public User()
        {
        }

        public User(string username)
        {
            this.username = username;
            this.inventory = new List<InventoryItem>();
        }
    }
}
