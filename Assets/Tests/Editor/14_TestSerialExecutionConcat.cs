using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[IntegrationTest.DynamicTest ("TestScene")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.Timeout (5)]
public class TestSerialExecutionConcat : MonoBehaviour
{
    void Start ()
    {
        Observable.FromCoroutine (AsyncA).Concat (
            Observable.FromCoroutine (AsyncB)
        ).Subscribe (xs => {
            Debug.Log (xs); // Called multiple times
        }, () => {
            IntegrationTest.Pass (); // OnComplete is called when everything finished
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
