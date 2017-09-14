using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;
using System;

[TestFixture]
public class TestDetectDoubleClick
{
    [Test]
    // Try to double click the screen
    public void DetectDoubleClick ()
    {
        var timerStream = Observable.Timer (TimeSpan.FromSeconds (3f));

        var clickStream = Observable.EveryUpdate ().TakeUntil (timerStream)
            .Where (_ => Input.GetMouseButtonDown (0));

        clickStream.Buffer (clickStream.Throttle (TimeSpan.FromMilliseconds (250)))
            .Where (xs => xs.Count >= 2)
            .Subscribe (xs => Debug.Log ("DoubleClick Detected! Count:" + xs.Count));
    }
}
