using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHull : CollisionHull2D
{
    [SerializeField]
    Vector2 center;
    public float radius;

    public override bool TestCollision(CollisionHull2D other)
    {
        switch (other.HullType)
        {
            case CollisionHull2D.CollisionType.Circle:
                return CollisionHull2D.CircleVSCircle(this, other as CircleHull);
            case CollisionHull2D.CollisionType.AABB:
                return CollisionHull2D.CircleVSAABB(this, other as AABBHull);
            case CollisionHull2D.CollisionType.OBB:
                return CollisionHull2D.CircleVSOBB(this, other as OBBHull);


            default:
                break;
        }

        return false;
    }

    /*private void Start()
    {
        center = transform.position;
    }

    private void FixedUpdate()
    {
        center = transform.position;
    }*/

    public Vector2 GetCenter()
    {
        return transform.position;
    }

    public void setNewCenter(Vector2 newCenter)
    {
        transform.position = newCenter;
    }



}
