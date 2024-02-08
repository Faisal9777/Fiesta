using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpecialProperties 
{
    public static void MoveToPoint(Transform obj, float velocity)
    {
        obj.Translate(0.0f, 0.0f, velocity * Time.deltaTime);
    }

    public static void MoveToPoint(Transform obj, float velocity, Vector3 targetPoint)
    {
        //Debug.Log("Im moving to the point with x " + targetPoint.x + " and y " + targetPoint.y);
        //Debug.Log("Im currently at point with x " + obj.position.x + " and y " + obj.position.y);
        var distanceVector = targetPoint - obj.position;
        if(distanceVector.magnitude > velocity*Time.fixedDeltaTime)
        {
            obj.position += Time.fixedDeltaTime * velocity * distanceVector.normalized;
        }
        else
        {
            Debug.Log("i have reached my destination");
            obj.position = targetPoint;
            Debug.Log("my position right now is " + obj.position);
        }
        //obj.Translate(0.0f, 0.0f, velocity * Time.deltaTime);
    }
}
