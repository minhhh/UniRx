using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;
using System;

[TestFixture]
public class TestOrchestratIEnumerator
{
    [Test]
    public void OrchestratIEnumerator ()
    {
        var o = new GameObject ();
        var comp = o.AddComponent<OrchestratIEnumeratorMB> ();
        var cancel =
            comp
            .StartOrchestra ()
            .Subscribe (
                (x) => {
                },
                () => {
                    UnityEngine.Object.Destroy (o);
                    Assert.Pass ("OrchestratIEnumerator");
                }
            );
    }
}

public class OrchestratIEnumeratorMB: MonoBehaviour
{
    // two coroutines
    IEnumerator AsyncA ()
    {
//        Debug.Log ("a start");
        yield return new WaitForSeconds (0f);
//        Debug.Log ("a end");
    }

    IEnumerator AsyncB ()
    {
//        Debug.Log ("b start");
        yield return new WaitForEndOfFrame ();
//        Debug.Log ("b end");
    }

    public IObservable<Unit> StartOrchestra ()
    {
        return Observable.FromCoroutine (AsyncA)
            .SelectMany (AsyncB);
    }
}