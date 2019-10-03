using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBHull : CollisionHull2D
{
    public Vector2 min;
    public Vector2 max;
    public Vector2 center;

    public override bool TestCollision(CollisionHull2D other)
    {
        switch (other.HullType)
        {
            case CollisionHull2D.CollisionType.Circle:
                return CollisionHull2D.CircleVSAABB(other as CircleHull, this);
            case CollisionHull2D.CollisionType.AABB:
                return CollisionHull2D.AABBVSAABB(this, other as AABBHull);
            case CollisionHull2D.CollisionType.OBB:
                return CollisionHull2D.AABBVSOBB(this, other as OBBHull);


            default:
                break;
        }

        return false;
    }

    void Update()
    {
        center.x = transform.position.x;
        center.y = transform.position.y;
    }
}
