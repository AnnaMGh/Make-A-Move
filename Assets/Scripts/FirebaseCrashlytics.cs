using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCrashlytics : MonoBehaviour
{

    FirebaseApp appFirebase;
    int updatesBeforeException;

    // Start is called before the first frame update
    void Start()
    {
        updatesBeforeException = 0;
        StartCoroutine(ICheckFirebaseDependences((obj1) => {}));
    }

    // Update is called once per frame
    void Update()
    {
        //throwExceptionEvery60Updates();
       
    }

    public IEnumerator ICheckFirebaseDependences(Delegates.ObjectDelegate objDelegate)
    {
        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCanceled)
        {
            Debug.LogError("CheckAndFixDependenciesAsync was canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.LogError("CheckAndFixDependenciesAsync encountered an error: " + task.Exception);
        }
        else
        {
            appFirebase = FirebaseApp.DefaultInstance;
            Debug.Log("Connected to crashlytics: " + task.Exception);
           // throwExceptionEvery60Updates();
        }
    }

    void throwExceptionEvery60Updates()
    {
        if (updatesBeforeException > 0)
        {
            updatesBeforeException--;
        }
        else
        {
            // Set the counter to 60 updates
            updatesBeforeException = 60;

            // Throw an exception to test your Crashlytics implementation
            throw new System.Exception("test exception please ignore");
        }
    }
}
