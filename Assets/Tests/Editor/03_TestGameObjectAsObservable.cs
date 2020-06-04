using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;

[IntegrationTest.DynamicTest("TestScene")]
[IntegrationTest.SucceedWithAssertions]
[IntegrationTest.Timeout(3)]
public class TestGameObjectAsObservable : MonoBehaviour
{
    public void Start()
    {
        #if !(UNITY_IPHONE || UNITY_ANDROID)
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // click this cube to destroy it
        var comp = cube.AddComponent<OMonoBehaviour>();
        comp.OnMouseDownAsObservable()
            .SelectMany(_ => cube.gameObject.UpdateAsObservable())
            .TakeUntil(comp.OnMouseUpAsObservable())
            .Select(_ => Input.mousePosition)
            .RepeatUntilDestroy(cube)
            .Subscribe(
            x =>
            {
                Debug.Log("Mouse position " + x);
                Object.Destroy(cube);
            },
            () => IntegrationTest.Pass());

        Object.Destroy(cube, 5f);
        #else
        IntegrationTest.Pass();
        #endif
    }
}

public class OMonoBehaviour : MonoBehaviour
{

}