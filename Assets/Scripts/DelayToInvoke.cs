using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayToInvoke : MonoBehaviour
{
    public static IEnumerator DelayToInvokeDo(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
}