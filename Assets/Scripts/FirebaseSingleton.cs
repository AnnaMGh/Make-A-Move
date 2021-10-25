using Firebase;
using Firebase.Crashlytics;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseSingleton
{
    public FirebaseApp appFirebase;
    public bool areDependencesChecked;

    private static FirebaseSingleton instance;

    public static FirebaseSingleton GetInstance()
    {
        if (instance == null)
        {
            instance = new FirebaseSingleton();
        }
        return instance;
    }


    #region Dependences

    public IEnumerator ICheckFirebaseDependences(Delegates.ObjectDelegate objDelegate)
    {
        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCanceled)
        {
            Debug.LogError("CheckAndFixDependenciesAsync was canceled.");
            objDelegate(null);
        }
        else if (task.IsFaulted)
        {
            Debug.LogError("CheckAndFixDependenciesAsync encountered an error: " + task.Exception);
            objDelegate(null);
        }
        else
        {
            Debug.Log("Connected to Firebase: " + task.Exception);

            appFirebase = FirebaseApp.DefaultInstance;
            FirebaseApp.LogLevel = LogLevel.Debug;
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            Crashlytics.IsCrashlyticsCollectionEnabled = true;

            areDependencesChecked = true;
            objDelegate(null);
        }
    }

    public void SendEvents(string title, Parameter[] receivedParams)
    {
        FirebaseAnalytics.LogEvent(title, receivedParams);
    }

    public void CrashApp() {
        Crashlytics.LogException(new Exception("Test crashlytics exception please ignore. "));
        throw new System.Exception("Test system exception please ignore.");
    }

    #endregion
}
