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
    void Start()
    {
        //GetDatabaseSnapshot();
        DontDestroyOnLoad(gameObject);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
