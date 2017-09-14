using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[IntegrationTest.DynamicTest ("TestScene")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.Timeout (10)]
public class TestSerialExecutionSelectMany : MonoBehaviour
{
    void Start ()
    {
        Observable
            .FromCoroutine (AsyncA)
            .SelectMany (AsyncB)
            .SelectMany (AsyncC)
            .Subscribe 
            (
            xs => {
                Debug.Log (xs); // Only called once
            }, 
            () => {
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

    IEnumerator AsyncC ()
    {
        Debug.Log ("c start");
        yield return new WaitForSeconds (3);
        Debug.Log ("c end");
    }
}
