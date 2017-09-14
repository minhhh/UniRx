using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;
using System;

//[IntegrationTest.Ignore]
[IntegrationTest.DynamicTest ("TestScene")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.Timeout (5)]
public class TestEventHandling : MonoBehaviour
{
    public void Start ()
    {
        var o = new GameObject ();
        var comp = o.AddComponent<SampleEventHandling> ();

        UnityEngine.Object.Destroy (o, 3f);

        StartCoroutine (IntegrationTestPass ());
    }

    IEnumerator IntegrationTestPass ()
    {
        yield return new WaitForSeconds (4);
        IntegrationTest.Pass ();
    }
}

public class SampleEventHandling : MonoBehaviour
{
    public class MyEventArgs : EventArgs
    {
        public int MyProperty { get; set; }
    }

    public event EventHandler<MyEventArgs> FooBar;
    public event Action<int> FooFoo;

    CompositeDisposable disposables = new CompositeDisposable ();

    // Subject is Rx's native event expression and recommend way for use Rx as event.
    // Subject.OnNext as fire event,
    // expose IObserver is subscibable for external source, it's no need convert.
    Subject<int> onBarBar = new Subject<int> ();

    public IObservable<int> OnBarBar { get { return onBarBar; } }

    void Start ()
    {
        // convert to IO<EventPattern> as (sender, eventArgs)
        Observable.FromEventPattern<EventHandler<MyEventArgs>, MyEventArgs> (
            h => h.Invoke, h => FooBar += h, h => FooBar -= h)
            .Subscribe ()
            .AddTo (disposables); // IDisposable can add to collection easily by AddTo

        // convert to IO<EventArgs>, many situation this is useful than FromEventPattern
        Observable.FromEvent<EventHandler<MyEventArgs>, MyEventArgs> (
            h => (sender, e) => h (e), h => FooBar += h, h => FooBar -= h)
            .Subscribe ()
            .AddTo (disposables);

        // You can convert Action like event.
        Observable.FromEvent<int> (
            h => FooFoo += h, h => FooFoo -= h)
            .Subscribe ()
            .AddTo (disposables);

        // AOT Safe EventHandling, use dummy capture, see:https://github.com/neuecc/UniRx/wiki/AOT-Exception-Patterns-and-Hacks
        var capture = 0;
        Observable.FromEventPattern<EventHandler<MyEventArgs>, MyEventArgs> (h => {
            capture.GetHashCode (); // dummy for AOT
            return new EventHandler<MyEventArgs> (h);
        }, h => FooBar += h, h => FooBar -= h)
            .Subscribe ()
            .AddTo (disposables);

        // Subject as like event.
        OnBarBar.Subscribe ().AddTo (disposables);
        onBarBar.OnNext (1); // fire event
    }

    void OnDestroy ()
    {
        // manage subscription lifecycle
        disposables.Dispose ();
    }
}
