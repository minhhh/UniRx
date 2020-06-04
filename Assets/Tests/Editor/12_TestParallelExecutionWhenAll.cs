using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[IntegrationTest.DynamicTest("TestScene")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.Timeout(5)]
public class TestParallelExecutionWhenAll : MonoBehaviour
{
    void Start()
    {
        var subject1 = new Subject<int>();
        var subject2 = new Subject<int>();
        var subject3 = new Subject<int>();

        Observable.WhenAll(
            subject1,
            subject2,
            subject3
        ).
        Subscribe(xs =>
        {
            // only called once
            foreach (var i in xs)
            {
                Debug.Log(i);
            }
        }, () =>
        {
            Debug.Log("Complete");
            IntegrationTest.Pass();
        });
        subject1.OnNext(1);
        subject1.OnNext(2);
        subject1.OnNext(3);
        subject2.OnNext(13);
        subject2.OnNext(14);
        subject2.OnNext(15);
        subject3.OnNext(21);
        subject3.OnNext(25);
        subject3.OnNext(29);

        subject1.OnCompleted();
        subject2.OnCompleted();
        subject3.OnCompleted();
    }
}
