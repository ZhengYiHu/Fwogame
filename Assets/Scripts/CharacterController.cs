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
    [SerializeField, Foldout("Control")]
    float targetMaxVerticalDistance = 4;
    [SerializeField, Foldout("Control")]
    float climbableSurfaceAngle = 20;
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
    [SerializeField]
    ParticleSystem walkParticles;

    [ShowNonSerializedField]
    private Vector3 velocity;
    private Vector3 lastBodyPos;
    private Vector3 lastPos;
    private Quaternion lastBodyRot;

    private Vector3 target => MouseController.targetPoint;
    private Vector3 lookTarget => MouseController.lookTargetPoint;

    const int VERTICAL_ANGLE_DEGREES = 90;

    /// <summary>
    /// Stores all the compounding changes of the body position in the current frame.
    /// </summary>
    private Vector3 bodyPositionsBuffer;

    private void Awake()
    {
        lastBodyPos = body.transform.position;
        lastBodyRot = body.transform.rotation;
        lastPos = transform.position;
    }

    void FixedUpdate()
    {
        bodyPositionsBuffer = Vector3.zero;
        
        FollowMouse();
        WobbleBody();
        FloatBody();
        LeanBody();
        RotateBody();
        ShowWalkParticles();

        //body position is in relation to the transform position. All buffered movements are summed up
        body.position = lastBodyPos + bodyPositionsBuffer;
        lastBodyPos = transform.position;
        if(transform.position != lastPos)
        {
            velocity = (transform.position - lastPos);
            lastPos = transform.position;
        }
        else
        {
            velocity = Vector3.zero;
        }
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
    /// Rotate Body towards target direction
    /// </summary>
    void RotateBody()
    {
        body.LookAt(lookTarget + targetOffset);
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
    /// Show walking particles if character is moving
    /// </summary>
    void ShowWalkParticles()
    {
        bool shouldPlayParticles = velocity.magnitude/Time.deltaTime > 0;
        if (walkParticles.isPlaying == shouldPlayParticles) return;
        if(shouldPlayParticles)
        {
            walkParticles.Play();
        }
        else
        {
            walkParticles.Stop();
        }
    }

    /// <summary>
    /// Follow mouse position
    /// </summary>
    void FollowMouse()
    {
        if(isValidTarget())
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Check if mouse target is valid
    /// </summary>
    /// <returns>if the target is valid</returns>
    bool isValidTarget()
    {
        bool targetNotReached = Vector3.Distance(transform.position, target) >= targetDistanceToStop;
        bool targetInRange = Mathf.Abs(target.y - transform.position.y) < targetMaxVerticalDistance;
        bool targetIsFlat = VERTICAL_ANGLE_DEGREES - (VERTICAL_ANGLE_DEGREES*Vector3.Dot(Vector3.up, MouseController.targetNormal)) <= climbableSurfaceAngle;
        return targetNotReached && targetInRange && targetIsFlat;
    }
}
