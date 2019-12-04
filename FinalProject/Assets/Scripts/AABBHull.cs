using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBHull : CollisionHull3D
{
    public Vector3 min;
    public Vector3 max;
    public Vector3 center;
    private float ZRotation = 0f;
    public Vector3 halfExtends;

    public override CollisionInfo TestCollision(CollisionHull3D other)
    {
        switch (other.HullType)
        {
            case CollisionHull3D.CollisionType.Circle:
                return CollisionHull3D.CircleVSAABB(other as CircleHull, this);
            case CollisionHull3D.CollisionType.AABB:
                return CollisionHull3D.AABBVSAABB(this, other as AABBHull);
            case CollisionHull3D.CollisionType.OBB:
                return CollisionHull3D.AABBVSOBB(this, other as OBBHull);


            default:
                break;
        }

        return null;
    }

    private void Start()
    {
        center = transform.position;
        halfExtends = (max - min) / 2f;
    }

    void FixedUpdate()
    {
        center.x = transform.position.x;
        center.y = transform.position.y;
        center.z = transform.position.z;
        transform.eulerAngles = new Vector3(0, 0, ZRotation);
    }
    


}
