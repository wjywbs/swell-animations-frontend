using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindClosestIntersect
{
    public static Vector3 search(IList<Vector3> curve, Vector3 point)
    {
        
        Vector3 closestPoint;
        Vector3 ret = curve[curve.Count - 1];
        float closestDistance = Vector3.Distance(ret, point);
        for (int x = 0; x < curve.Count - 1; x++)
        {
            Vector3 pointA = curve[x];
            Vector3 pointB = curve[x + 1];
            closestPoint = GetClosestPointOnLineSegment(pointA, pointB, point);
            float distance = Vector3.Distance(closestPoint, point);
            Debug.Log(distance);
            if (distance < closestDistance)
            {
                ret = closestPoint;
                closestDistance = distance;
                if (distance == 0)
                {
                    break;
                }
            }
        }
        return ret;
    }


    private static Vector3 GetClosestPointOnLineSegment(Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 AP = P - A;       //Vector from A to P   
        Vector3 AB = B - A;       //Vector from A to B  

        float magnitudeAB = AB.magnitude * AB.magnitude;     //Magnitude of AB vector (it's length squared)     
        float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  
        if (distance < 0)
        {     //Check if P projection is over vectorAB     
            return A;

        }
        else if (distance > 1)
        {
            return B;
        }
        else
        {
            return A + AB * distance;
        }

    }
}
