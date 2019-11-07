using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHull : CollisionHull3D
{
    [SerializeField]
    Vector3 center;
    public float radius;

    public override CollisionInfo TestCollision(CollisionHull3D other)
    {
        switch (other.HullType)
        {
            case CollisionHull3D.CollisionType.Circle:
                return CollisionHull3D.CircleVSCircle(this, other as CircleHull);
            case CollisionHull3D.CollisionType.AABB:
                return CollisionHull3D.CircleVSAABB(this, other as AABBHull);
            case CollisionHull3D.CollisionType.OBB:
                return CollisionHull3D.CircleVSOBB(this, other as OBBHull);


            default:
                break;
        }

        return null;
    }

    private void Start()
    {
        center = transform.position;
    }

    private void FixedUpdate()
    {
        center = transform.position;
    }

    public Vector3 GetCenter()
    {
        return transform.position;
    }

    public void setNewCenter(Vector3 newCenter)
    {
        transform.position = newCenter;
    }



}
