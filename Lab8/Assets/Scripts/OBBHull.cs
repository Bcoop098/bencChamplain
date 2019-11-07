using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBBHull : CollisionHull3D
{
    public Vector3 min = new Vector2(0f, 0f);
    public Vector3 max = new Vector2(0f, 0f);
    [Range(0f, 360f)]
    public float ZRotation = 0f;
    public Vector3 center = new Vector2(0f, 0f);
    public Vector3 halfExtends;
    public Vector3 rotExtends;
    public override CollisionInfo TestCollision(CollisionHull3D other)
    {
        switch (other.HullType)
        {
            case CollisionHull3D.CollisionType.Circle:
                return CollisionHull3D.CircleVSOBB(other as CircleHull, this);
            case CollisionHull3D.CollisionType.AABB:
                return CollisionHull3D.AABBVSOBB(other as AABBHull, this);
            case CollisionHull3D.CollisionType.OBB:
                return CollisionHull3D.OBBVSOBB(this, other as OBBHull);

            default:
                break;
        }

        return null;
    }

    private void Start()
    {
        center = transform.position;
        halfExtends = (max - min) / 2f;
        //rotExtends = (RotMax - RotMin) / 2f;
    }
    void Update()
    {
        center.x = transform.position.x;
        center.y = transform.position.y;
        transform.eulerAngles = new Vector3(0, 0, ZRotation);
    }

   
}