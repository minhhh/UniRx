using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;

[TestFixture]
public class TestObservableWWW : MonoBehaviour
{
    [Test]
    public void DownloadFromGoogle ()
    {
        ObservableWWW.Get ("http://google.co.jp/")
            .Subscribe (
            x => {
            },
            ex => Assert.Fail (),
            () => Assert.Pass ("DownloadFromGoogle ")
        );
    }

    [Test]
    public void CallDisposeStopDownloading ()
    {
        var cancel = 
            ObservableWWW.Get ("http://google.co.jp/")
            .Subscribe (
                x => Assert.Fail (),
                ex => Assert.Fail ());
        cancel.Dispose ();
    }

    [Test]
    public void DownloadInSeries ()
    {
        ObservableWWW.Get ("http://google.co.jp/")
            .Concat (ObservableWWW.Get ("http://bing.com/"))
            .Subscribe (
            x => {
            },
            ex => Assert.Fail (),
            () => Assert.Pass ("DownloadInSeries")
        );
    }

    [Test]
    public void DownloadInParallel ()
    {
        var parallel = Observable.WhenAll (
                           ObservableWWW.Get ("http://google.com/"),
                           ObservableWWW.Get ("http://bing.com/"),
                           ObservableWWW.Get ("http://unity3d.com/"));

        parallel.Subscribe (
            xs => {
            },
            ex => Assert.Fail (),
            () => Assert.Pass ("DownloadInParallel"));
    }

    [Test]
    public void DownloadWithProgress ()
    {
        // notifier for progress
        var progressNotifier = new ScheduledNotifier<float> ();
        progressNotifier.Subscribe (
            x => {
            },
            ex => Assert.Fail ());

        ObservableWWW.Get ("http://google.com/", progress: progressNotifier).Subscribe ();
    }

    [Test]
    public void ErrorHandling ()
    {
        ObservableWWW.Get ("http://www.google.com/404")
            .CatchIgnore (
            (WWWErrorException ex) => {
                Assert.IsTrue (ex.HasResponse);
                if (ex.HasResponse) {
                    Assert.AreEqual (404, (int)ex.StatusCode);
                }
            }).Subscribe (
            x => {
            },
            ex => Assert.Fail (ex.Message),
            () => Assert.Pass ("ErrorHandling"));
    }


}
