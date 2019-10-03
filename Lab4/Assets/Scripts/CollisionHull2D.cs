﻿using UnityEngine;

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
        
        Vector2 AABBMinTransform = OBB.transform.InverseTransformPoint(AABB.min);
        Vector2 AABBMaxTransform = OBB.transform.InverseTransformPoint(AABB.max);

        Debug.DrawLine(AABBMinTransform, AABBMaxTransform, Color.red);
        Debug.DrawLine(OBB.min, OBB.max, Color.green);
        Debug.DrawLine(AABB.center, OBB.center, Color.yellow);

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
        //top left is min.x, max y
        //bottom right is max.x, min.y
        //project onto both axis, get new min and max for both, then aabb test
        /*Vector2 topLeftOBB1 = new Vector2(OBB1.min.x, OBB1.max.y);
        Vector2 bottomRightOBB1 = new Vector2(OBB1.max.x, OBB1.min.y);
        Vector2 topLeftOBB2 = new Vector2(OBB2.min.x, OBB2.max.y);
        Vector2 bottomRightOBB2 = new Vector2(OBB2.max.x, OBB2.min.y);*/

        Vector2 right1 = new Vector2(Mathf.Cos(OBB1.ZRotation), -Mathf.Sin(OBB1.ZRotation));
        Vector2 up1 = new Vector2(Mathf.Sin(OBB1.ZRotation), Mathf.Cos(OBB1.ZRotation));
        Vector2 right2 = new Vector2(Mathf.Cos(OBB2.ZRotation), -Mathf.Sin(OBB2.ZRotation));
        Vector2 up2 = new Vector2(Mathf.Sin(OBB2.ZRotation), Mathf.Cos(OBB2.ZRotation));

        if (OBBTests(right1,OBB1, OBB2))
        {
            if (OBBTests(up1,OBB1, OBB2))
            {
                if(OBBTests(right2, OBB2, OBB1))
                {
                    if (OBBTests(up2, OBB2, OBB1))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static private bool OBBTests(Vector2 norm,OBBHull ProjOBB, OBBHull MainOBB)
    {
        //top left is min.x, max y
        //bottom right is max.x, min.y

        Vector2 Max1;
        Vector2 Min1;
        Vector2 Max2;
        Vector2 Min2;


        Vector2 p1 = Vector2.Dot(ProjOBB.max, norm) * norm;
        Vector2 p2 = Vector2.Dot(ProjOBB.min, norm) * norm;
        Vector2 p3 = Vector2.Dot(new Vector2(ProjOBB.max.x, ProjOBB.min.y), norm) * norm;
        Vector2 p4 = Vector2.Dot(ProjOBB.min, norm) * norm;

        if (p1.x <= p3.x && p1.y <= p3.y)
        {
            p1 = p3;
        }
        if (p2.x >= p4.x && p2.y >= p4.y)
        {
            p2 = p4;
        }

        Max1 = p1;
        Min1 = p2;

        Max2 = Vector2.Dot(MainOBB.max, norm) * norm;
        Min2 = Vector2.Dot(MainOBB.min, norm) * norm;

        if (Max1.x>= Min2.x && Max1.y >= Min2.y && Max2.x >= Min1.x && Max2.y >= Min1.y)
        {
            return true;
        }
        return false;
    }

    
}






