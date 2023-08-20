using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField, Foldout("Control")]
    float speed = 1.2f;
    [SerializeField, Foldout("Control")]
    float targetDistanceToStop = 1;
    [SerializeField, Foldout("Head")]
    Vector3 targetOffset;
    [SerializeField, Foldout("Body")]
    Transform body;
    [SerializeField, Foldout("Body")]
    AnimationCurve bodyWobble;
    [SerializeField, Foldout("Body")]
    float bodyLeanAmout = 5;
    [SerializeField, Foldout("Body")]
    float bodyLeanDamp = 0.2f;
    [SerializeField, Foldout("Body")]
    float bodyFloatHeight = 0.2f;
    [SerializeField, Foldout("Limbs")]
    LimbMotion[] limbMotions;

    private Vector3 velocity;
    private Vector3 lastBodyPos;
    private Quaternion lastBodyRot;

    private Vector3 target => MouseController.targetPoint;

    /// <summary>
    /// Stores all the compounding changes of the body position in the current frame.
    /// </summary>
    private Vector3 bodyPositionsBuffer;

    private void Awake()
    {
        lastBodyPos = body.transform.position;
        lastBodyRot = body.transform.rotation;
    }

    void FixedUpdate()
    {
        bodyPositionsBuffer = Vector3.zero;
        velocity = transform.position - lastBodyPos;
        FollowMouse();
        WobbleBody();
        FloatBody();
        LeanBody();
        RotateBody();
        body.position = lastBodyPos + bodyPositionsBuffer;
        lastBodyPos = transform.position;
    }

    /// <summary>
    /// Wobble Body up and down based on an Animation Curve
    /// </summary>
    void WobbleBody()
    {
        var loopedTime = Mathf.Repeat(Time.time, 1);
        bodyPositionsBuffer.y += bodyWobble.Evaluate(loopedTime);
    }

    /// <summary>
    /// Lean body towards walking direction
    /// </summary>
    void LeanBody()
    {
        Quaternion newBodyRotation = body.rotation;
        newBodyRotation.eulerAngles = lastBodyRot.eulerAngles + (velocity * 100 * bodyLeanAmout);
        body.rotation = Quaternion.Slerp(body.rotation, newBodyRotation, bodyLeanDamp);
    }

    /// <summary>
    /// Rotate Body towards walking direction
    /// </summary>
    void RotateBody()
    {
        body.LookAt(target + targetOffset);
    }

    /// <summary>
    /// Keep Body afloat from the group level
    /// </summary>
    void FloatBody()
    {
        //Add an extra offset
        bodyPositionsBuffer.y += bodyFloatHeight;
    }

    /// <summary>
    /// Follow mouse position
    /// </summary>
    void FollowMouse()
    {
        if(Vector3.Distance(transform.position, target) >= targetDistanceToStop)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }
}
