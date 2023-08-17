using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField, Foldout("Body")]
    Transform body;
    [SerializeField, Foldout("Body")]
    AnimationCurve bodyWobble;
    [SerializeField, Foldout("Body")]
    float bodyLeanAmout;
    [SerializeField, Foldout("Body")]
    float bodyLeanDamp;
    [Space]

    private Vector3 velocity;
    private Vector3 lastBodyPos;
    private Quaternion lastBodyRot;


    private void Awake()
    {
        lastBodyPos = body.transform.position;
        lastBodyRot = body.transform.rotation;
    }

    void FixedUpdate()
    {
        velocity = transform.position - lastBodyPos;

        WobbleBody();
        LeanBody();
        lastBodyPos = transform.position;
    }

    //Wobble Body up and down based on an Animation Curve
    void WobbleBody()
    {
        var loopedTime = Mathf.Repeat(Time.time, 1);
        Vector3 newBodyPosition = body.position;
        newBodyPosition.y = lastBodyPos.y + bodyWobble.Evaluate(loopedTime);
        body.position = newBodyPosition;
    }

    //Lean body towards walking direction
    void LeanBody()
    {
        Quaternion newBodyRotation = body.rotation;
        newBodyRotation.eulerAngles = lastBodyRot.eulerAngles + (velocity * 100 * bodyLeanAmout);
        body.rotation = Quaternion.Slerp(body.rotation, newBodyRotation, bodyLeanDamp);
    }
}
