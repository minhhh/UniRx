using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;

[TestFixture]
public class TestConvertFromUnityCallback
{
    public static event Application.LogCallback logEvent;

//    [Test]
    public void ConvertFromUnityCallback ()
    {
        logEvent += DummyHandler;

        // method is separatable and composable
        int warningCount = 0;
        var observableW = 
            LogHelper
            .LogCallbackAsObservable ()
            .Where (x => x.LogType == LogType.Warning)
            .Subscribe (
                (x) => {
                    warningCount++;
                    Assert.AreEqual (1, warningCount);
                },
                (ex) => {
                    Assert.Fail ();
                },
                () => {
                    Assert.Fail (); // never called
                }
            );

        int errorCount = 0;
        var observableE = 
            LogHelper
                .LogCallbackAsObservable ()
                .Where (x => x.LogType == LogType.Error)
                .Subscribe (
                    (x) => {
                        errorCount++;
                        Assert.AreEqual (1, errorCount);
                    },
                    (ex) => {
                        Assert.Fail ();
                    },
                    () => {
                        Assert.Fail (); // never called
                    }
                );

        if (logEvent != null) {
            logEvent ("Cond", "Warn", LogType.Warning);
            observableW.Dispose ();
            logEvent ("Cond", "Warn", LogType.Warning);

            logEvent ("Cond", "Error", LogType.Error);
            observableE.Dispose ();
            logEvent ("Cond", "Error", LogType.Error);
        }
    }

    public void DummyHandler (string condition, string stackTrace, LogType type)
    {
    }
}


class LogCallback
{
    public string Condition;
    public string StackTrace;
    public UnityEngine.LogType LogType;
}

static class LogHelper
{
    public static IObservable<LogCallback> LogCallbackAsObservable ()
    {
        return Observable.FromEvent<Application.LogCallback, LogCallback> (
            h => (condition, stackTrace, type) => h (new LogCallback {
                Condition = condition,
                StackTrace = stackTrace,
                LogType = type
            }),
            h => TestConvertFromUnityCallback.logEvent += h,
            h => TestConvertFromUnityCallback.logEvent -= h);
    }
}