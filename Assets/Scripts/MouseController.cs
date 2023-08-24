using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    float rayMaxLength = 20f;
    static public Vector3 targetPoint;
    static public Vector3 lookTargetPoint;
    static public Vector3 targetNormal;
    static public Ray mouseRay;
    private void Update()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool rayHits = Physics.Raycast(mouseRay,out RaycastHit raycastHit, rayMaxLength, GlobalConstants.walkable_Mask);

        if (rayHits)
        {
            targetPoint = raycastHit.point;
            lookTargetPoint = raycastHit.point;
            targetNormal = raycastHit.normal;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition));

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(targetPoint, 0.05f);
    }
}
