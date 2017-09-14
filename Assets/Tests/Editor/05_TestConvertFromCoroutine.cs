using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;
using System;

[TestFixture]
public class TestConvertFromCoroutine
{

    [Test]
    public void ConvertFromCoroutine ()
    {
        GetWWW ("http://google.co.jp/").Subscribe (
            (x) => {
            },
            (ex) => Assert.Fail (),
            () => Assert.Pass ("ConvertFromCoroutine")
        );
    }

    [Test]
    public void ConvertFromCoroutineCancel ()
    {
        var cancelable = GetWWW ("http://google.co.jp/").Subscribe (
                             (x) => Assert.Fail (),
                             (ex) => Assert.Fail (),
                             () => Assert.Fail ()
                         );

        cancelable.Dispose ();
    }

    [Test]
    public void ConvertFromNestedCoroutine ()
    {
        Observable.FromCoroutine<string> ((observer, cancellationToken) => 
            NestedCoroutine ("", observer, cancellationToken))
            .Subscribe (
                (x) => {
                },
                (ex) => Assert.Fail (),
                () => Assert.Pass ("ConvertFromNestedCoroutine")
            );
    }

    IEnumerator NestedCoroutine (string url, IObserver<string> observer, CancellationToken cancellationToken)
    {
        yield return SecondLevelNestedCoroutine ();
        observer.OnNext ("Complete");
        observer.OnCompleted ();
    }

    IEnumerator SecondLevelNestedCoroutine ()
    {
        for (int i = 0; i < 10; i++) {
            yield return i;
        }
        throw new Exception ();
    }

    public static IObservable<string> GetWWW (string url)
    {
        // convert coroutine to IObservable
        return Observable.FromCoroutine<string> ((observer, cancellationToken) => GetWWWCore (url, observer, cancellationToken));
    }

    // IEnumerator with callback
    static IEnumerator GetWWWCore (string url, IObserver<string> observer, CancellationToken cancellationToken)
    {
        var www = new UnityEngine.WWW (url);
        while (!www.isDone && !cancellationToken.IsCancellationRequested) {
            yield return null;
        }

        if (cancellationToken.IsCancellationRequested)
            yield break;

        if (www.error != null) {
            observer.OnError (new Exception (www.error));
        } else {
            observer.OnNext (System.Text.Encoding.UTF8.GetString (www.bytes));
            observer.OnCompleted ();
        }
    }
}
