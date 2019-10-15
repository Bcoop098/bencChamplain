using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionHull2D : MonoBehaviour
{
    public CollisionType HullType;
    //static public CollisionInfo info;
    
    public class CollisionInfo
    {
        

        public struct Contact
        {
            public Vector2 point;
            public Vector2 normal;
            public float restitution;
            public float penetration;
        }

        public CollisionInfo(CollisionHull2D shapeA, CollisionHull2D shapeB, Vector2 normal, float penetration)
        {
            RigidBodyA = shapeA.GetComponent<Particle2D>();
            ShapeA = shapeA;

            RigidBodyB = shapeB.GetComponent<Particle2D>();
            ShapeB = shapeB;

            RelativeVelocity = RigidBodyB.velocity - RigidBodyA.velocity;

            contacts[0].normal = normal;
            contacts[0].penetration = penetration;
            contacts[0].restitution = Mathf.Min(RigidBodyA.restitution, RigidBodyB.restitution);
        }

        public Particle2D RigidBodyA { get; }
        public CollisionHull2D ShapeA { get; }
        public Particle2D RigidBodyB { get; }
        public CollisionHull2D ShapeB { get; }

        public Vector2 RelativeVelocity { get; }
        public Contact[] contacts = new Contact[4];
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

    abstract public CollisionInfo TestCollision(CollisionHull2D other);

    static protected CollisionInfo CircleVSCircle(CircleHull circle1, CircleHull circle2)
    {
        float radiusSum = circle1.radius + circle2.radius;
        float totalRadius = (circle1.radius + circle2.radius) * (circle1.radius + circle2.radius);
        Vector2 centerDiff = (circle2.GetCenter() - circle1.GetCenter());
        float distanceSQ = Vector2.Dot(centerDiff, centerDiff);

        if (distanceSQ >totalRadius)
        {
            return null;
        }
        float distance = Mathf.Sqrt(distanceSQ);
        return new CollisionInfo(circle1, circle2, centerDiff / distance, radiusSum - distance);
    }

    static protected CollisionInfo CircleVSAABB(CircleHull circle, AABBHull AABB)
    {
        
        Vector2 circleBox = new Vector2(Mathf.Max(AABB.min.x + AABB.center.x, Mathf.Min(circle.GetCenter().x, AABB.max.x + AABB.center.x)),
            Mathf.Max(AABB.min.y + AABB.center.y, Mathf.Min(circle.GetCenter().y, AABB.max.y + AABB.center.y)));

        Vector2 distanceVec = circle.GetCenter() - circleBox;
        float distanceSQ = Vector2.Dot(distanceVec, distanceVec);
        if (distanceSQ > (circle.radius * circle.radius))
        {
            return null;
        }
        float distance = Mathf.Sqrt(distanceSQ);
        return new CollisionInfo(circle, AABB, -distanceVec.normalized, circle.radius - distance);

        
    }

    static protected CollisionInfo CircleVSOBB(CircleHull circle, OBBHull OBB)
    {
        
        Vector2 halfExtend = (OBB.max - OBB.min) / 2;
        Vector2 circleInOBB = OBB.transform.InverseTransformPoint(circle.GetCenter());
        Vector2 circleBox = new Vector2(Mathf.Max(-halfExtend.x, Mathf.Min(circleInOBB.x, halfExtend.x)),
            Mathf.Max(-halfExtend.y, Mathf.Min(circleInOBB.y, halfExtend.y)));

        Vector2 distanceVec = circleInOBB - circleBox;
        float distanceSQ = Vector2.Dot(distanceVec, distanceVec);
        if (distanceSQ > (circle.radius * circle.radius))
        {
            return null;
        }

        float distance = Mathf.Sqrt(distanceSQ);
        return new CollisionInfo(circle, OBB, OBB.transform.TransformVector(-distanceVec).normalized, circle.radius - distance);
    }

    static protected CollisionInfo AABBVSAABB(AABBHull AABB1 , AABBHull AABB2)
    {
        
        Vector2 AtoB = AABB2.center - AABB1.center;
        float x_overlap = AABB1.halfExtends.x + AABB2.halfExtends.x - Mathf.Abs(AtoB.x);

        if(x_overlap > 0.0f)
        {
            float y_overlap = AABB1.halfExtends.y + AABB2.halfExtends.y - Mathf.Abs(AtoB.y);
            if (y_overlap > 0.0f)
            {
                if (x_overlap < y_overlap)
                {
                    return new CollisionInfo(AABB1, AABB2, AtoB.x < 0.0f ? -Vector2.right : Vector2.right, x_overlap);
                }
                else 
                {
                    return new CollisionInfo(AABB1, AABB2, AtoB.y < 0.0f ? -Vector2.up : Vector2.up, y_overlap);
                }
            }
        }
        return null;
    }
    
    static protected CollisionInfo AABBVSOBB(AABBHull AABB, OBBHull OBB)
    {

        List<Vector2> allAxis = new List<Vector2>();
        allAxis.AddRange(AABB.NormalAxis);
        allAxis.AddRange(OBB.NormalAxis);

        foreach (var axis in allAxis)
        {
            float AABBMin = float.MaxValue;
            float AABBMax = float.MinValue;

            foreach (var vert in AABB.Vertices)
            {
                float dotValue = (vert.x * axis.x + vert.y * axis.y);
                if (dotValue < AABBMin)
                {
                    AABBMin = dotValue;
                }
                if (dotValue > AABBMax)
                {
                    AABBMax = dotValue;
                }
            }

            float OBBMin = float.MaxValue;
            float OBBMax = float.MinValue;
            foreach (var vert in OBB.Vertices)
            {
                float dotValue = (vert.x * axis.x + vert.y * axis.y);
                if (dotValue < OBBMin)
                {
                    OBBMin = dotValue;
                }
                if (dotValue > OBBMax)
                {
                    OBBMax = dotValue;
                }
            }

            if (!(AABBMax < OBBMin && OBBMax < AABBMin))
            {

                Vector2 OBBExtend = (OBB.RotMax - OBB.RotMin) / 2f;
                Vector2 AtoB = OBB.center - AABB.center;
                float x_overlap = AABB.halfExtends.x + OBBExtend.x- Mathf.Abs(AtoB.x);

                if (x_overlap > 0.0f)
                {
                    float y_overlap = AABB.halfExtends.y + OBBExtend.y - Mathf.Abs(AtoB.y);
                    if (y_overlap > 0.0f)
                    {
                        if (x_overlap < y_overlap)
                        {
                            return new CollisionInfo(AABB, OBB, AtoB.x < 0.0f ? -Vector2.right : Vector2.right, x_overlap);
                        }
                        else
                        {
                            return new CollisionInfo(AABB, OBB, AtoB.y < 0.0f ? -Vector2.up : Vector2.up, y_overlap);
                        }
                    }
                }
            }
        }

        return null;
    }

    static protected CollisionInfo OBBVSOBB(OBBHull OBB1, OBBHull OBB2)
    {
        List<Vector2> allAxis = new List<Vector2>();
        allAxis.AddRange(OBB1.NormalAxis);
        allAxis.AddRange(OBB2.NormalAxis);

        foreach (var axis in allAxis)
        {
            float OBB1Min = float.MaxValue;
            float OBB1Max = float.MinValue;

            foreach (var vert in OBB1.Vertices)
            {
                float dotValue = (vert.x * axis.x + vert.y * axis.y);
                if (dotValue < OBB1Min)
                {
                    OBB1Min = dotValue;
                }
                if (dotValue > OBB1Max)
                {
                    OBB1Max = dotValue;
                }
            }

            float OBB2Min = float.MaxValue;
            float OBB2Max = float.MinValue;
            foreach (var vert in OBB2.Vertices)
            {
                float dotValue = (vert.x * axis.x + vert.y * axis.y);
                if (dotValue < OBB2Min)
                {
                    OBB2Min = dotValue;
                }
                if (dotValue > OBB2Max)
                {
                    OBB2Max = dotValue;
                }
            }

            if (!(OBB1Max < OBB2Min && OBB2Max < OBB1Min))
            {
                Vector2 AtoB = OBB2.center - OBB1.center;


                
                float x_overlap = OBB1.halfExtends.x + OBB2.halfExtends.x- Mathf.Abs(AtoB.x);

                if (x_overlap > 0.0f)
                {
                    float y_overlap = OBB1.halfExtends.y + OBB2.halfExtends.y - Mathf.Abs(AtoB.y);
                    if (y_overlap > 0.0f)
                    {
                        if (x_overlap < y_overlap)
                        {
                            return new CollisionInfo(OBB1, OBB2, AtoB.x < 0.0f ? -Vector2.right : Vector2.right, x_overlap);
                        }
                        else
                        {
                            return new CollisionInfo(OBB1, OBB2, AtoB.y < 0.0f ? -Vector2.up : Vector2.up, y_overlap);
                        }
                    }
                }
            }
        }

        return null;
    }

}






