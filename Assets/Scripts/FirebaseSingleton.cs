using Firebase;
using Firebase.Crashlytics;
using Firebase.Analytics;
using System;
using System.Collections;
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


            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // When this property is set to true, Crashlytics will report all
                // uncaught exceptions as fatal events. This is the recommended behavior.
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;

                // Set a flag here for indicating that your project is ready to use Firebase.
            }
            else
            {
                Debug.LogError(String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }

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
