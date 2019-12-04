using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionHull3D : MonoBehaviour
{
    // Start is called before the first frame update
    public CollisionType HullType;
    //static public CollisionInfo info;

    public class CollisionInfo
    {


        public struct Contact
        {
            public Vector3 point;
            public Vector3 normal;
            public float restitution;
            public float penetration;
        }

        public CollisionInfo(CollisionHull3D shapeA, CollisionHull3D shapeB, Vector3 normal, float penetration, Vector3 point)
        {
            RigidBodyA = shapeA.GetComponent<Particle3D>();
            ShapeA = shapeA;

            RigidBodyB = shapeB.GetComponent<Particle3D>();
            ShapeB = shapeB;

            RelativeVelocity = RigidBodyB.velocity - RigidBodyA.velocity;

            contacts[0].normal = normal;
            contacts[0].penetration = penetration;
            contacts[0].restitution = Mathf.Min(RigidBodyA.restitution, RigidBodyB.restitution);
            contacts[0].point = point;
        }

        public Particle3D RigidBodyA { get; }
        public CollisionHull3D ShapeA { get; }
        public Particle3D RigidBodyB { get; }
        public CollisionHull3D ShapeB { get; }

        public Vector3 RelativeVelocity { get; }
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

    abstract public CollisionInfo TestCollision(CollisionHull3D other);

    static protected CollisionInfo CircleVSCircle(CircleHull circle1, CircleHull circle2)
    {
        float radiusSum = circle1.radius + circle2.radius;
        float totalRadius = (circle1.radius + circle2.radius) * (circle1.radius + circle2.radius);
        Vector3 centerDiff = (circle2.GetCenter() - circle1.GetCenter());
        float distanceSQ = Vector3.Dot(centerDiff, centerDiff);

        if (distanceSQ > totalRadius)
        {
            return null;
        }
        float distance = Mathf.Sqrt(distanceSQ);
        return new CollisionInfo(circle1, circle2, centerDiff / distance, radiusSum - distance,(circle1.GetCenter() + centerDiff * 0.5f));
    }

    static protected CollisionInfo CircleVSAABB(CircleHull circle, AABBHull AABB)
    {

        Vector3 circleBox = new Vector3(Mathf.Max(AABB.min.x + AABB.center.x, Mathf.Min(circle.GetCenter().x, AABB.max.x + AABB.center.x)),
            Mathf.Max(AABB.min.y + AABB.center.y, Mathf.Min(circle.GetCenter().y, AABB.max.y + AABB.center.y)), 
            Mathf.Max(AABB.min.z + AABB.center.z, Mathf.Min(circle.GetCenter().z, AABB.max.z + AABB.center.z)));

        Vector3 distanceVec = circle.GetCenter() - circleBox;
        float distanceSQ = Vector3.Dot(distanceVec, distanceVec);
        if (distanceSQ > (circle.radius * circle.radius))
        {
            return null;
        }
        float distance = Mathf.Sqrt(distanceSQ);
        return new CollisionInfo(circle, AABB, -distanceVec.normalized, circle.radius - distance, circleBox);


    }

    static protected CollisionInfo CircleVSOBB(CircleHull circle, OBBHull OBB)
    {
        Vector3 halfExtend = OBB.halfExtends;
        Vector3 circleInOBB = OBB.GetComponent<Particle3D>().getWorldToObject().MultiplyPoint(circle.GetCenter());
        Vector3 circleBox = new Vector3(Mathf.Max(-halfExtend.x, Mathf.Min(circleInOBB.x, halfExtend.x)),
            Mathf.Max(-halfExtend.y, Mathf.Min(circleInOBB.y, halfExtend.y)),
            Mathf.Max(-halfExtend.z, Mathf.Min(circleInOBB.z, halfExtend.z)));

        Vector3 distanceVec = circleInOBB - circleBox;
        float distanceSQ = Vector3.Dot(distanceVec, distanceVec);
        if (distanceSQ > (circle.radius * circle.radius))
        {
            return null;
        }
        Vector3 closestPt = new Vector3(0f,0f,0f);
        float dist;
        // Clamp each coordinate to the box.
        dist = circleInOBB.x;
        if (dist > OBB.halfExtends.x) dist = OBB.halfExtends.x;
        if (dist < -OBB.halfExtends.x) dist = -OBB.halfExtends.x;
        closestPt.x = dist;
        dist = circleInOBB.y;
        if (dist > OBB.halfExtends.y) dist = OBB.halfExtends.y;
        if (dist < -OBB.halfExtends.y) dist = -OBB.halfExtends.y;
        closestPt.y = dist;
        dist = circleInOBB.z;
        if (dist > OBB.halfExtends.z) dist = OBB.halfExtends.z;
        if (dist < -OBB.halfExtends.z) dist = -OBB.halfExtends.z;
        closestPt.z = dist;
        // Check to see if we’re in contact.
        dist = (closestPt - circleInOBB).sqrMagnitude;
        if (dist > circle.radius * circle.radius)
            return null;
        // Compile the contact.
        Vector3 closestPtWorld = OBB.GetComponent<Particle3D>().getObjectToWorld().MultiplyPoint(closestPt);
        //place contact point in world space
        float distance = Mathf.Sqrt(distanceSQ);
        //Debug.LogError(closestPtWorld);
        return new CollisionInfo(circle, OBB, (-distanceVec).normalized, (circle.radius - distance),closestPtWorld);
        //OBB.GetComponent<Particle3D>().getObjectToWorld().MultiplyPoint(-distanceVec).normalized this failed to work, removing it fixed it
    }

    static protected CollisionInfo AABBVSAABB(AABBHull AABB1, AABBHull AABB2)
    {

        Vector3 AtoB = AABB2.center - AABB1.center;
        float x_overlap = AABB1.halfExtends.x + AABB2.halfExtends.x - Mathf.Abs(AtoB.x);

        if (x_overlap > 0.0f)
        {
            float y_overlap = AABB1.halfExtends.y + AABB2.halfExtends.y - Mathf.Abs(AtoB.y);
            if (y_overlap > 0.0f)
            {
                float z_overlap = AABB1.halfExtends.z + AABB2.halfExtends.z - Mathf.Abs(AtoB.z);
                {
                    if (z_overlap > 0.0f)
                    {
                        float minOverlap = Mathf.Min(x_overlap, y_overlap, z_overlap);
                        if (minOverlap == x_overlap)
                        {
                            return new CollisionInfo(AABB1, AABB2, AtoB.x < 0.0f ? -Vector3.right : Vector3.right, x_overlap, AABB1.center + AtoB / 2);
                        }
                        else if(minOverlap == y_overlap)
                        {
                            Debug.Log(AtoB / 2f);
                            return new CollisionInfo(AABB1, AABB2, AtoB.y < 0.0f ? -Vector3.up : Vector3.up, y_overlap, AABB1.center + AtoB / 2);
                        }
                        else if(minOverlap == z_overlap)
                        {
                            Debug.Log(AtoB / 2f);
                            return new CollisionInfo(AABB1, AABB2, AtoB.y < 0.0f ? -Vector3.forward : Vector3.forward, z_overlap, AABB1.center + AtoB / 2);
                        }
                    }
                }
                
            }
        }
        return null;
    }

    static protected CollisionInfo AABBVSOBB(AABBHull AABB, OBBHull OBB)
    {
        Vector3 toCenter = AABB.center - OBB.center;
        return boxTest(AABB, OBB, toCenter, AABB.halfExtends, OBB.halfExtends);
    }

    static protected CollisionInfo OBBVSOBB(OBBHull OBB1, OBBHull OBB2)
    {
        Vector3 toCenter = OBB1.center - OBB2.center;
        return boxTest(OBB1,OBB2, toCenter,OBB1.halfExtends,OBB2.halfExtends);
      
    }

    static private float penetrationOnAxis(CollisionHull3D box1, CollisionHull3D box2, Vector3 axis, Vector3 toCenter, Vector3 box1HalfExtends, Vector3 box2HalfExtends)
    {
        float projectOne = transformToAxis(box1HalfExtends, box1.GetComponent<Particle3D>().getObjectToWorld(), axis);
        float projectTwo = transformToAxis(box2HalfExtends, box2.GetComponent<Particle3D>().getObjectToWorld(), axis);

        float distance = Mathf.Abs(Vector3.Dot(toCenter, axis));

        return projectOne + projectTwo - distance;


    }

    static private float transformToAxis(Vector3 halfExtends, Matrix4x4 objectToWorld, Vector3 axis)
    {
        return halfExtends.x * Mathf.Abs(Vector3.Dot(axis, objectToWorld.GetColumn(0))) +
            halfExtends.y * Mathf.Abs(Vector3.Dot(axis, objectToWorld.GetColumn(1))) +
            halfExtends.z * Mathf.Abs(Vector3.Dot(axis, objectToWorld.GetColumn(2)));
    }

    static protected CollisionInfo boxTest(CollisionHull3D box1, CollisionHull3D box2, Vector3 toCenter, Vector3 box1HalfExtends, Vector3 box2HalfExtends)
    {
        List<Vector3> allAxis = new List<Vector3>();
        allAxis.Add(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0));
        allAxis.Add(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1));
        allAxis.Add(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2));

        allAxis.Add(box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0));
        allAxis.Add(box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1));
        allAxis.Add(box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2));

        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0)));
        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1)));
        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2)));
        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0)));
        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1)));
        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2)));
        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0)));
        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1)));
        allAxis.Add(Vector3.Cross(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2), box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2)));

        float bestOverlap = float.MaxValue;
        int bestCase = 0; //greater than 6, edge
        Vector3 bestAxis = Vector3.one;

        for (int i = 0; i < 15; i++)
        {
            Vector3 axis = allAxis[i];
            if (axis.sqrMagnitude < 0.001) continue;
            axis = axis.normalized;
            float overlap = penetrationOnAxis(box1, box2, axis, toCenter, box1HalfExtends, box2HalfExtends);
            if (overlap < 0)
                return null;
            if (overlap < bestOverlap)
            {
                bestOverlap = overlap;
                bestCase = i;
                //bestAxis = allAxis[bestCase];
            }
        }
        if (bestCase < 3)
        {
            return resolveVertexFaceBox(box1, box2, toCenter, bestCase, bestOverlap, box2HalfExtends);
        }
        else if (bestCase < 6)
        {
            return resolveVertexFaceBox(box2, box1, toCenter * -1.0f, bestCase - 3, bestOverlap, box1HalfExtends);
        }
        else
        {
            int oneAxisIndex = bestCase / 3;
            int twoAxixIndex = bestCase % 3;

            Vector3 oneAxis = box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(oneAxisIndex);
            Vector3 twoAxis = box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(twoAxixIndex);
            Vector3 axis2 = Vector3.Cross(oneAxis, twoAxis);
            axis2.Normalize();

            return new CollisionInfo(box1, box2, allAxis[bestCase], bestOverlap, Vector3.one);
        }

        
    }

    static private CollisionInfo resolveVertexFaceBox(CollisionHull3D box1, CollisionHull3D box2, Vector3 toCenter, int bestCase, float bestOverlap, Vector3 halfExtends)
    {
        Vector3 normal = box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(bestCase);
        Vector4 tempCenter = new Vector4(toCenter.x,toCenter.y,toCenter.z);
        if(Vector4.Dot(box1.GetComponent<Particle3D>().getObjectToWorld().GetColumn(bestCase),tempCenter) > 0)
        {
            normal = normal * -1.0f;
        }
        Vector3 vertex = halfExtends;
        if (Vector4.Dot(box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(0), normal) < 0)
        {
            vertex.x = -vertex.x;
        }
        if (Vector4.Dot(box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(1), normal) < 0)
        {
            vertex.y = -vertex.y;
        }
        if (Vector4.Dot(box2.GetComponent<Particle3D>().getObjectToWorld().GetColumn(2), normal) < 0)
        {
            vertex.z = -vertex.z;
        }
        Debug.Log(vertex);
        Vector3 vertexPoint = box1.GetComponent<Particle3D>().getObjectToWorld().MultiplyPoint(vertex);
        return new CollisionInfo(box1, box2, normal, bestOverlap, vertexPoint);
    }
}
