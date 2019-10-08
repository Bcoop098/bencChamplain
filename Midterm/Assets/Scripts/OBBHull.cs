using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBBHull : CollisionHull2D
{
    public Vector2 min = new Vector2(0f, 0f);
    public Vector2 max = new Vector2(0f, 0f);
    [Range(0f, 360f)]
    public float ZRotation = 0f;
    public Vector2 center = new Vector2(0f, 0f);

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

    void Update()
    {
        center.x = transform.position.x;
        center.y = transform.position.y;
        transform.eulerAngles = new Vector3(0, 0, ZRotation);
    }
}