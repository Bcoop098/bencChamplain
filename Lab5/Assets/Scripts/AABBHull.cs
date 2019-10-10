using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBHull : CollisionHull2D
{
    public Vector2 min;
    public Vector2 max;
    public Vector2 center;
    private float ZRotation = 0f;
    public Vector2 halfExtends;

    public override CollisionInfo TestCollision(CollisionHull2D other)
    {
        switch (other.HullType)
        {
            case CollisionHull2D.CollisionType.Circle:
                return CollisionHull2D.CircleVSAABB(other as CircleHull, this);
            case CollisionHull2D.CollisionType.AABB:
                return CollisionHull2D.AABBVSAABB(this, other as AABBHull);
            /*case CollisionHull2D.CollisionType.OBB:
                return CollisionHull2D.AABBVSOBB(this, other as OBBHull);*/


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
        transform.eulerAngles = new Vector3(0, 0, ZRotation);
    }
    public IEnumerable<Vector2> Vertices
    {
        get
        {
            List<Vector2> vert = new List<Vector2>();

            float[,] rotoMat = new float[,] { { Mathf.Cos(ZRotation * Mathf.Deg2Rad), -Mathf.Sin(ZRotation * Mathf.Deg2Rad) },
                                              { Mathf.Sin(ZRotation * Mathf.Deg2Rad) , Mathf.Cos(ZRotation * Mathf.Deg2Rad) } };

            vert.Add(new Vector2(min.x * rotoMat[0, 0] + min.y * rotoMat[1, 0],
                                 min.x * rotoMat[0, 1] + min.y * rotoMat[1, 1]) + center);
            vert.Add(new Vector2(max.x * rotoMat[0, 0] + max.y * rotoMat[1, 0],
                                 max.x * rotoMat[0, 1] + max.y * rotoMat[1, 1]) + center);
            vert.Add(new Vector2(min.x * rotoMat[0, 0] + max.y * rotoMat[1, 0],
                                 min.x * rotoMat[0, 1] + max.y * rotoMat[1, 1]) + center);
            vert.Add(new Vector2(max.x * rotoMat[0, 0] + min.y * rotoMat[1, 0],
                                 max.x * rotoMat[0, 1] + min.y * rotoMat[1, 1]) + center);

            return vert;
        }
    }

    private Vector2 RightAxis
    {
        get
        {
            return new Vector2(Mathf.Cos(ZRotation * Mathf.Deg2Rad), -Mathf.Sin(ZRotation * Mathf.Deg2Rad));
        }
    }

    private Vector2 UpAxis
    {
        get
        {
            return new Vector2(Mathf.Sin(ZRotation * Mathf.Deg2Rad), Mathf.Cos(ZRotation * Mathf.Deg2Rad));
        }
    }

    public IEnumerable<Vector2> NormalAxis
    {
        get
        {
            List<Vector2> axis = new List<Vector2>();
            axis.Add(RightAxis);
            axis.Add(UpAxis);
            return axis;
        }
    }


}
