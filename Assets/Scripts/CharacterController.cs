using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField, Foldout("Control")]
    float hSpeed = 1.2f;
    [SerializeField, Foldout("Control")]
    float vSpeed = 5.2f;
    [SerializeField, Foldout("Control")]
    float maxStepHeight = 0.3f;
    [SerializeField, Foldout("Control")]
    float targetDistanceToStop = 1;
    [SerializeField, Foldout("Control")]
    float targetMaxVerticalDistance = 4;
    [SerializeField, Foldout("Control")]
    Vector3 heightDetectorOffset = new Vector3(0, 0.4f, 0.3f);
    [SerializeField, Foldout("Control")]
    float climbableSurfaceAngle = 20;
    [SerializeField, Foldout("Control")]
    float groundedDistance = 0.3f;
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
    private Vector3 heightDetectorOrigin;
    private RaycastHit hDetectorHit;
    private RaycastHit groundHit;
    private RaycastHit lastValidPoint;

    private Vector3 target => MouseController.targetPoint;

    const int VERTICAL_ANGLE_DEGREES = 90;
    const float MAX_RAY_LENGTH = 5;

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
        if (!Game.Playing) return;
        bodyPositionsBuffer = Vector3.zero;

        IsGrounded();
        PlaceHeighDetectorOrigin();
        FollowMouse();

        WobbleBody();
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
    /// Calculate if player is grounded
    /// </summary>
    /// <returns></returns>
    bool IsGrounded()
    {
        Ray groundedRay = new Ray(transform.position, Vector3.down);
        Physics.Raycast(groundedRay,out groundHit, MAX_RAY_LENGTH, GlobalConstants.walkable_Mask);
        return Vector3.Distance(transform.position, groundHit.point) <= groundedDistance;
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
        body.LookAt(target + targetOffset);
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
        Ray heightRay = new Ray(heightDetectorOrigin, Vector3.down);
        //Y target is the result of a raycast downwards from Heigh Detector Origin.
        Physics.Raycast(heightRay, out hDetectorHit, MAX_RAY_LENGTH, GlobalConstants.walkable_Mask);

        if (isValidTarget())
        {
            lastValidPoint = hDetectorHit;
        }
        //Calculate xz and y separately to have control over vertical speed independantly
        Vector3 frameTarget;
        Vector3 HframeTarget = Vector3.MoveTowards(transform.position, lastValidPoint.point, hSpeed * Time.deltaTime);
        Vector3 VframeTarget = Vector3.MoveTowards(transform.position, lastValidPoint.point, vSpeed * Time.deltaTime);
        //Add a vertical offset to keep the body off the ground
        VframeTarget.y += bodyFloatHeight;

        frameTarget = HframeTarget;
        frameTarget.y = VframeTarget.y;
        transform.position = frameTarget;
       
    }

    /// <summary>
    /// Place the raycast origin to detect sudden height changes
    /// </summary>
    void PlaceHeighDetectorOrigin()
    {
        heightDetectorOrigin = transform.position + heightDetectorOffset;
        heightDetectorOrigin = VectorUtils.RotatePointAroundPivot(heightDetectorOrigin, transform.position, new Vector3(0, body.rotation.eulerAngles.y, 0));
    }

    [ShowNativeProperty]
    float distanceFromTarget => Vector3.Distance(transform.position, target);
    [ShowNativeProperty]
    bool targetNotReached => distanceFromTarget >= targetDistanceToStop;
    [ShowNativeProperty]
    float verticalDiffPlayerTarget => Mathf.Abs(target.y - transform.position.y);
    [ShowNativeProperty]
    bool targetInVerticalRange => verticalDiffPlayerTarget < targetMaxVerticalDistance;
    [ShowNativeProperty]
    float targetAngle => VERTICAL_ANGLE_DEGREES - (VERTICAL_ANGLE_DEGREES * Vector3.Dot(Vector3.up, MouseController.targetNormal));
    [ShowNativeProperty]
    bool targetIsFlat => targetAngle <= climbableSurfaceAngle;
    [ShowNativeProperty]
    float nextStepHeight => Mathf.Abs(transform.position.y - hDetectorHit.point.y);
    [ShowNativeProperty]
    bool stepInRange => nextStepHeight < maxStepHeight;
    /// <summary>
    /// Check if mouse target is valid
    /// </summary>
    /// <returns>if the target is valid</returns>
    bool isValidTarget()
    {
        return targetNotReached && targetInVerticalRange && targetIsFlat && stepInRange;
    }

    private void OnDrawGizmos()
    {
        //Height detector in front of the character
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(heightDetectorOrigin, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(hDetectorHit.point, heightDetectorOrigin);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundHit.point, hDetectorHit.point);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, groundHit.point);

    }
}
