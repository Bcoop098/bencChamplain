using UnityEngine;

public class CollisionHull2D : MonoBehaviour
{

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

    static public bool CircleVSCircle(CircleHull circle1, CircleHull circle2)
    {
        float totalRadius = (circle1.radius + circle2.radius); //* (circle1.radius + circle2.radius);
        Vector2 distance = (circle2.GetCenter() - circle1.GetCenter());

        if (Vector2.Dot(distance,distance) <= totalRadius)
        {
            return true;
        }
        else
        return false;
    }

    static public bool CircleVSAABB()
    {
        return false;
    }

    static public bool CircleVSOBB()
    {
        return false;
    }

    static public bool AABBVSAABB(AABBHull AABB1 , AABBHull AABB2)
    {
        if (AABB1.max.x >= AABB2.min.x && AABB1.max.y >= AABB2.min.y && AABB2.max.x >= AABB1.min.x && AABB2.max.x >= AABB1.min.y)
        {
            return true;
        }
        return false;
    }

    static public bool AABBVSOBB()
    {
        return false;
    }

    static public bool OBBVSOBB(OBBHull OBB1, OBBHull OBB2)
    {
        //project onto both axis, get new min and max for both, then aabb test
        return false;
    }


}


public class CircleHull : CollisionHull2D
{
    public float radius;

    public Vector2 GetCenter()
    {
        return transform.position;
    }

    
}

public class AABBHull : CollisionHull2D
{
    public Vector2 min;
    public Vector2 max;
}

public class OBBHull : CollisionHull2D
{
    public Vector2 min;
    public Vector2 max;
    public float ZRotation;


    //top left is min.x, max y
    //bottom right is max.x, min.y
    Vector2 GetRightAxis()
    {
        return new Vector2(Mathf.Cos(ZRotation), Mathf.Sin(ZRotation));
    }

    Vector2 GetUpAxis()
    {
        return new Vector2(- Mathf.Sin(ZRotation), Mathf.Cos(ZRotation));
    }

    public Vector2 ProjectRight(Vector2 vectorToTest)
    {
        Vector2 ProjectRight = Vector2.Dot(vectorToTest,GetRightAxis()) * GetRightAxis();
        return ProjectRight;
    }

    public Vector2 ProjectUp(Vector2 vectorToTest)
    {
        Vector2 ProjectUp = Vector2.Dot(vectorToTest, GetUpAxis()) * GetUpAxis();
        return ProjectUp;
    }
}

