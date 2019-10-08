using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionHull2D : MonoBehaviour
{
    public CollisionType HullType;

    public class CollisionInfo
    {
        public struct Contact
        {
            Vector2 point;
            Vector2 normal;
            float restitution;
        }

        public CollisionHull2D a;
        public CollisionHull2D b;
        public Contact[] contacts = new Contact[4];
        public Vector2 closingVelocity;
        bool status;

    }


    public enum CollisionType
    {
        Circle,
        AABB,
        OBB
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    abstract public bool TestCollision(CollisionHull2D other);

    static protected bool CircleVSCircle(CircleHull circle1, CircleHull circle2)
    {
        float totalRadius = (circle1.radius + circle2.radius) * (circle1.radius + circle2.radius);
        Vector2 distance = (circle2.GetCenter() - circle1.GetCenter());

        if (Vector2.Dot(distance,distance) < totalRadius)
        {
            return true;
        }
        else
        return false;
    }

    static protected bool CircleVSAABB(CircleHull circle, AABBHull AABB)
    {
        
        Vector2 circleBox = new Vector2(Mathf.Max(AABB.min.x + AABB.center.x, Mathf.Min(circle.GetCenter().x, AABB.max.x + AABB.center.x)),
            Mathf.Max(AABB.min.y + AABB.center.y, Mathf.Min(circle.GetCenter().y, AABB.max.y + AABB.center.y)));

        Vector2 distance = circle.GetCenter() - circleBox;
        float distanceSQ = Vector2.Dot(distance, distance);
        if (distanceSQ <= (circle.radius * circle.radius))
        {
            return true;
        }

        return false;
    }

    static protected bool CircleVSOBB(CircleHull circle, OBBHull OBB)
    {
        
        Vector2 halfExtend = (OBB.max - OBB.min) / 2;
        Vector2 circleInOBB = OBB.transform.InverseTransformPoint(circle.GetCenter());
        Vector2 circleBox = new Vector2(Mathf.Max(-halfExtend.x, Mathf.Min(circleInOBB.x, halfExtend.x)),
            Mathf.Max(-halfExtend.y, Mathf.Min(circleInOBB.y, halfExtend.y)));

        Vector2 distance = circleInOBB - circleBox;
        float distanceSQ = Vector2.Dot(distance, distance);
        if (distanceSQ <= (circle.radius * circle.radius))
        {
            return true;
        }

        return false;
    }

    static protected bool AABBVSAABB(AABBHull AABB1 , AABBHull AABB2)
    {
        if (AABB1.max.x + AABB1.center.x >= AABB2.min.x + AABB2.center.x &&
            AABB1.max.y + AABB1.center.y >= AABB2.min.y + AABB2.center.y && 
            AABB2.max.x + AABB2.center.x >= AABB1.min.x + AABB1.center.x && 
            AABB2.max.y + AABB2.center.y >= AABB1.min.y + AABB1.center.y)
        {
            return true;
        }
        return false;
    }

    static protected bool AABBVSOBB(AABBHull AABB, OBBHull OBB)
    {


        Vector2 AABBMinTransform = OBB.transform.InverseTransformPoint(AABB.min + AABB.center);
        Vector2 AABBMaxTransform = OBB.transform.InverseTransformPoint(AABB.max + AABB.center);

        //Debug.DrawLine(AABBMinTransform, AABBMaxTransform, Color.red);
        //Debug.DrawLine(OBB.min, OBB.max, Color.green);
        //Debug.DrawLine(AABB.center, OBB.center, Color.yellow);

        if (AABB.max.x + AABB.center.x >= OBB.min.x + OBB.center.x && OBB.max.x + OBB.center.x >= AABB.min.x + AABB.center.x)
        {
            if (AABB.max.y + AABB.center.y >= OBB.min.y + OBB.center.y && OBB.max.y + OBB.center.y >= AABB.min.y + AABB.center.y)
            {


                
                /*if (obbMinTransform.x > obbMaxTransform.x && obbMinTransform.y > obbMaxTransform.y)
                {
                    Vector2 temp = obbMinTransform;
                    obbMinTransform = obbMaxTransform;
                    obbMaxTransform = temp;
                }*/


                if (AABBMaxTransform.x + AABB.center.x >= OBB.min.x + OBB.center.x && OBB.max.x + OBB.center.x >= AABBMinTransform.x + AABB.center.x)
                {
                    if (AABBMaxTransform.y + AABB.center.y >= OBB.min.x + OBB.center.y && OBB.max.x + OBB.center.y >= AABBMinTransform.y + AABB.center.y)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    static protected bool OBBVSOBB(OBBHull OBB1, OBBHull OBB2)
    {
        List<Vector2> allAxis = new List<Vector2>();
        allAxis.AddRange(OBB1.NormalAxis);
        allAxis.AddRange(OBB2.NormalAxis);

        foreach (var axis in allAxis)
        {
            float OBB1Min = float.MaxValue;
            float OBB1Max = float.MinValue;

            foreach (var vert in OBB1.Vertices)
            {
                float dotValue = (vert.x * axis.x + vert.y * axis.y);
                if (dotValue < OBB1Min)
                {
                    OBB1Min = dotValue;
                }
                if (dotValue > OBB1Max)
                {
                    OBB1Max = dotValue;
                }
            }

            float OBB2Min = float.MaxValue;
            float OBB2Max = float.MinValue;
            foreach (var vert in OBB2.Vertices)
            {
                float dotValue = (vert.x * axis.x + vert.y * axis.y);
                if (dotValue < OBB2Min)
                {
                    OBB2Min = dotValue;
                }
                if (dotValue > OBB2Max)
                {
                    OBB2Max = dotValue;
                }
            }

            if (!(OBB1Max >= OBB2Min && OBB2Max >= OBB1Min))
            {
                return false;
            }
        }

        return true;
    }

  

    
}






