using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;
using System;

[TestFixture]
public class TestConvertToCoroutine
{

    [Test]
    public void ConvertToCoroutine ()
    {
        var o = new GameObject ();
        var comp = o.AddComponent<EmptyMonoBehaviour> ();
        comp.StartCoroutine (ComplexCoroutineTest ());

        comp.StartCoroutine (comp.TestNewCustomYieldInstruction ());
    }

    IEnumerator ComplexCoroutineTest ()
    {
        var v = default(int);
        yield return Observable.Range (1, 10).StartAsCoroutine (x => v = x);

        yield return new WaitForSeconds (3);

        yield return Observable.Return (100).StartAsCoroutine (x => v = x);
    }

}

public class EmptyMonoBehaviour: MonoBehaviour
{
    // Note:ToAwaitableEnumerator/StartAsCoroutine/LazyTask are obsolete way on Unity 5.3
    // You can use ToYieldInstruction.
    public IEnumerator TestNewCustomYieldInstruction ()
    {
        // wait Rx Observable.
        yield return Observable.Timer (TimeSpan.FromSeconds (1)).ToYieldInstruction ();

        // you can change the scheduler(this is ignore Time.scale)
        yield return Observable.Timer (TimeSpan.FromSeconds (1), Scheduler.MainThreadIgnoreTimeScale).ToYieldInstruction ();

        // get return value from ObservableYieldInstruction
        var o = ObservableWWW.Get ("http://unity3d.com/").ToYieldInstruction (throwOnError: false);
        yield return o;

        if (o.HasError) {
            Debug.Log (o.Error.ToString ());
        }
        if (o.HasResult) {
//            Debug.Log (o.Result);
        }

        // other sample(wait until transform.position.y >= 100) 
        yield return this.ObserveEveryValueChanged (x => x.transform).FirstOrDefault (x => x.position.y >= 100).ToYieldInstruction ();

        UnityEngine.Object.Destroy (gameObject);
    }

    void Update ()
    {
        transform.position = transform.position + Vector3.up;
    }
}