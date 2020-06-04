using UnityEngine;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;

//[IntegrationTest.Ignore]
[IntegrationTest.DynamicTest("TestScene")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.Timeout(5)]
public class TestObservableTriggers : MonoBehaviour
{
    public void Start()
    {
        // Get the plain object
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Add ObservableXxxTrigger for handle MonoBehaviour's event as Observable
        cube.AddComponent<ObservableUpdateTrigger>()
            .UpdateAsObservable()
            .SampleFrame(30)
            .TakeUntilDestroy(cube)
            .Subscribe(
            x =>
            {
                Debug.Log("Frame " + x);
            },
            () => IntegrationTest.Pass());

        // destroy after 3 second:)
        Object.Destroy(cube, 3f);
    }

}
