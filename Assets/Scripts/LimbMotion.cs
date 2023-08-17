using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Simulate limb movement by moving the end effector once the tollerance distance is surpassed
/// </summary>
public class LimbMotion : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    float tollerance = 0.15f;
    [SerializeField]
    Vector3 offset = Vector3.zero;

    //Target that the limb will try to reach
    [SerializeField]
    Transform endEffectorTarget;
    private Vector3 WorldTarget => endEffectorTarget.position;
    //Current position of the Limb
    private Vector3 worldEndPosition;

    int layer_mask;

    private void Awake()
    {
        //Get last point in the line renderer and save the world position. Assuming the last is the end effector.
        worldEndPosition = WorldTarget;
        layer_mask = LayerMask.GetMask("Walkable");
    }

    private void Update()
    {
        Ray ray = new Ray(WorldTarget, Vector3.down);
        //Keep end effector on the last position
        var newLocalPosition = transform.InverseTransformPoint(worldEndPosition);

        //Calculate plane distance
        Vector3 planeTarget = new Vector3(WorldTarget.x, 0, WorldTarget.z) + offset;
        Vector3 planeNewPosition = new Vector3(worldEndPosition.x, 0, worldEndPosition.z);

        float distance = Vector3.Distance(planeTarget, planeNewPosition);
        //When the distance threshold is surpassed, move the limb to the target
        if (distance > tollerance)
        {
            newLocalPosition = transform.InverseTransformPoint(WorldTarget);

            //Raycast to find the contact point for the limbs
            if(Physics.Raycast(ray,out RaycastHit hit, 5, layer_mask))
            {
                worldEndPosition = hit.point;
            }
            else
            {
                worldEndPosition = WorldTarget;
            }
        }
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newLocalPosition);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(WorldTarget + offset, 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(worldEndPosition, tollerance);
    }
}
