using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    bool initialized = false;
    bool colliding = false;

    private void Start()
    {
        initialized = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!initialized || colliding) return;
        colliding = true;
        Game.Instance.UIVisible = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!initialized || !colliding) return;
        colliding = false;
        Game.Instance.UIVisible = false;
    }
}
