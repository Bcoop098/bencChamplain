using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBBHull : CollisionHull2D
{
    public Vector2 min;
    public Vector2 max;
    public float ZRotation;

    public override bool TestCollision(CollisionHull2D other)
    {
        switch (other.HullType)
        {
            case CollisionHull2D.CollisionType.Circle:
                return CollisionHull2D.CircleVSOBB(other as CircleHull, this);
            case CollisionHull2D.CollisionType.AABB:
                return CollisionHull2D.AABBVSOBB(other as AABBHull, this);
            case CollisionHull2D.CollisionType.OBB:
                return CollisionHull2D.OBBVSOBB(this, other as OBBHull);



            default:
                break;
        }

        return false;
    }


    //top left is min.x, max y
    //bottom right is max.x, min.y
    Vector2 GetRightAxis()
    {
        return new Vector2(Mathf.Cos(ZRotation), Mathf.Sin(ZRotation));
    }

    Vector2 GetUpAxis()
    {
        return new Vector2(-Mathf.Sin(ZRotation), Mathf.Cos(ZRotation));
    }

    public Vector2 ProjectRight(Vector2 vectorToTest)
    {
        Vector2 ProjectRight = Vector2.Dot(vectorToTest, GetRightAxis()) * GetRightAxis();
        return ProjectRight;
    }

    public Vector2 ProjectUp(Vector2 vectorToTest)
    {
        Vector2 ProjectUp = Vector2.Dot(vectorToTest, GetUpAxis()) * GetUpAxis();
        return ProjectUp;
    }
}