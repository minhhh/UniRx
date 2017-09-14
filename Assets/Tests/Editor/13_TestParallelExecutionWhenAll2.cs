using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[IntegrationTest.DynamicTest ("TestScene")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.Timeout (5)]
public class TestParallelExecutionWhenAll2 : MonoBehaviour
{
    void Start ()
    {
        Observable.WhenAll (
            Observable.FromCoroutine(AsyncA),
            Observable.FromCoroutine(AsyncB)
        ).Subscribe (xs => {
            // only called once
            Debug.Log (xs);
            IntegrationTest.Pass ();
        });

        Observable.Merge (
            Observable.FromCoroutine (AsyncA),
            Observable.FromCoroutine (AsyncB)
        ).Subscribe (xs => {
            // only called once
            Debug.Log (xs);
//            IntegrationTest.Pass ();
        });

    }

    IEnumerator AsyncA ()
    {
        Debug.Log ("a start");
        yield return new WaitForSeconds (1);
        Debug.Log ("a end");
    }

    IEnumerator AsyncB ()
    {
        Debug.Log ("b start");
        yield return new WaitForSeconds (2);
        Debug.Log ("b end");
    }
}
