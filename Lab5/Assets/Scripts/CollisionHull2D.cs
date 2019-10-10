using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionHull2D : MonoBehaviour
{
    public CollisionType HullType;
    static public CollisionInfo info;
    
    public class CollisionInfo
    {
        

        public struct Contact
        {
            public Vector2 point;
            public Vector2 normal;
            public float restitution;
            public float depth;
        }

        public CollisionHull2D a;
        public CollisionHull2D b;
        public Contact[] contacts = new Contact[4];
        public Vector2 closingVelocity;
        public bool status;

        public Contact contactData;

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
        info = new CollisionInfo();
    }

    // Update is called once per frame
    void Update()
    {

    }

    abstract public bool TestCollision(CollisionHull2D other);

    static protected bool CircleVSCircle(CircleHull circle1, CircleHull circle2)
    {
        float totalRadius = (circle1.radius + circle2.radius) * (circle1.radius + circle2.radius);
        Vector2 distance = (circle2.GetCenter() - circle1.GetCenter());

        if (Vector2.Dot(distance,distance) < totalRadius)
        {


            /*info.status = true;
            info.a = circle1;
            info.b = circle2;*/
            //resolve the circle overlap
            //float angle = Mathf.Atan2((circle2.GetCenter().y - circle1.GetCenter().y), (circle2.GetCenter().x - circle1.GetCenter().x));
            //float distanceBetweenCircles = Mathf.Sqrt((circle2.GetCenter().x - circle1.GetCenter().x) * (circle2.GetCenter().x - circle1.GetCenter().x) + 
            //(circle2.GetCenter().y - (circle1.GetCenter().y) * circle2.GetCenter().y - circle1.GetCenter().y));
            float penetration = Vector2.Dot(circle1.GetCenter(),circle2.GetCenter());
            float totalInverseMass = circle1.GetComponent<Particle2D>().GetInverseMass() + circle2.GetComponent<Particle2D>().GetInverseMass();
            Vector2 contact = (circle1.GetCenter() - circle2.GetCenter()).normalized;

            Vector2 closingVelocity = -(circle1.GetComponent<Particle2D>().velocity - circle2.GetComponent<Particle2D>().velocity) * contact;
            Vector2 separatingVelocity = (circle1.GetComponent<Particle2D>().velocity - circle2.GetComponent<Particle2D>().velocity) * contact;
            float restitution = 1f;

            Vector2 newSeparating = -separatingVelocity * restitution;

            Vector2 deltaVelocity = newSeparating - separatingVelocity;

            Vector2 impulse = deltaVelocity / totalInverseMass;

            Vector2 impulsePerMass = contact * impulse;

            circle1.GetComponent<Particle2D>().SetVelocity(circle1.GetComponent<Particle2D>().velocity + impulsePerMass * circle1.GetComponent<Particle2D>().GetInverseMass());
            circle2.GetComponent<Particle2D>().SetVelocity(circle2.GetComponent<Particle2D>().velocity + impulsePerMass * -circle2.GetComponent<Particle2D>().GetInverseMass());

            Vector2 movePerIMass = contact * (penetration / totalInverseMass);

            Vector2 circle1Move = movePerIMass * circle1.GetComponent<Particle2D>().GetInverseMass();

            Vector2 circle2Move = movePerIMass * -circle2.GetComponent<Particle2D>().GetInverseMass();

            circle1.transform.position = circle1.transform.position + new Vector3 (circle1Move.x,circle1Move.y,0);
            circle2.transform.position = circle2.transform.position + new Vector3(circle2Move.x, circle2Move.y, 0);

            //info.contactData.depth = distanceBetweenCircles;
            /*float distanceToMove = circle1.radius + circle2.radius - distanceBetweenCircles;
            float circle2X = circle2.GetCenter().x;
            float circle2Y = circle2.GetCenter().y;
            circle2X += (float)(Mathf.Cos(angle) * distanceBetweenCircles);
            circle2Y += (float)(Mathf.Cos(angle) * distanceBetweenCircles);
            circle2.setNewCenter(new Vector2(circle2X, circle2Y));
            //bounce time
            Vector2 tangentVector;
            tangentVector.y = -(circle2.GetCenter().x - circle1.GetCenter().x);
            tangentVector.x = (circle2.GetCenter().y - circle1.GetCenter().y);
            tangentVector.Normalize();
            Vector2 relativeVelocity = new Vector2((circle1.GetComponent<Particle2D>().velocity.x - circle2.GetComponent<Particle2D>().velocity.x), 
                (circle1.GetComponent<Particle2D>().velocity.y)- (circle2.GetComponent<Particle2D>().velocity.y));
            float length = Vector2.Dot(relativeVelocity, tangentVector);
            Vector2 velocityComponentOnTangent;
            velocityComponentOnTangent = tangentVector * length;
            Vector2 velocityComponentPerpendicularToTangent = relativeVelocity - velocityComponentOnTangent;
            //Move one circle
            circle2.GetComponent<Particle2D>().velocity.x -=  velocityComponentPerpendicularToTangent.x;
            circle2.GetComponent<Particle2D>().velocity.y -=  velocityComponentPerpendicularToTangent.y;*/
            return true;
        }
        else
        return false;
    }

    static protected bool CircleVSAABB(CircleHull circle, AABBHull AABB)
    {
        
        Vector2 circleBox = new Vector2(Mathf.Max(AABB.min.x + AABB.center.x, Mathf.Min(circle.GetCenter().x, AABB.max.x + AABB.center.x)),
            Mathf.Max(AABB.min.y + AABB.center.y, Mathf.Min(circle.GetCenter().y, AABB.max.y + AABB.center.y)));

        Vector2 distance = circle.GetCenter() - circleBox;
        float distanceSQ = Vector2.Dot(distance, distance);
        if (distanceSQ <= (circle.radius * circle.radius))
        {
            float penetration = 0f;
            Vector2 contact = new Vector2(0, 0);

            Vector2 n = AABB.center - circle.GetCenter();

            Vector2 closest = n;

            float x_extent = (AABB.max.x - AABB.min.x) / 2;
            float y_extent = (AABB.max.y -AABB.min.y) / 2;

            closest.x = Mathf.Clamp(closest.x, -x_extent, x_extent);
            closest.y = Mathf.Clamp(closest.y, -y_extent, y_extent);
            bool inside = false;
            if (n == closest)
            {
                inside = true;
                if (Mathf.Abs(n.x) > Mathf.Abs(n.y))
                {
                    if (closest.x > 0)
                        closest.x = x_extent;
                    else
                        closest.x = -x_extent;
                }
                else 
                {
                    if (closest.y > 0)
                        closest.y = y_extent;
                    else
                        closest.y = -y_extent;
                }
            }

            contact = n - closest;
            float d = contact.magnitude * contact.magnitude;
            float r = circle.radius;
            if (d > r && !inside)
                return false;

            d = Mathf.Sqrt(d);
            if (inside)
            {
                contact = -n;
                penetration = r - d;
            }
            else 
            {
                contact = n;
                penetration = r - d;
            }

            float totalInverseMass = circle.GetComponent<Particle2D>().GetInverseMass() + AABB.GetComponent<Particle2D>().GetInverseMass();
            Vector2 closingVelocity = -(circle.GetComponent<Particle2D>().velocity - AABB.GetComponent<Particle2D>().velocity) * contact;
            Vector2 separatingVelocity = (circle.GetComponent<Particle2D>().velocity - AABB.GetComponent<Particle2D>().velocity) * contact;
            float restitution = 1f;

            Vector2 newSeparating = -separatingVelocity * restitution;

            Vector2 deltaVelocity = newSeparating - separatingVelocity;

            Vector2 impulse = deltaVelocity / totalInverseMass;

            Vector2 impulsePerMass = contact * impulse;

            circle.GetComponent<Particle2D>().SetVelocity(circle.GetComponent<Particle2D>().velocity + impulsePerMass * circle.GetComponent<Particle2D>().GetInverseMass());
            Debug.Log(circle.GetComponent<Particle2D>().velocity);

            AABB.GetComponent<Particle2D>().SetVelocity(AABB.GetComponent<Particle2D>().velocity + impulsePerMass * -AABB.GetComponent<Particle2D>().GetInverseMass());



            Vector2 movePerIMass = contact * (penetration / totalInverseMass);

            Vector2 AABB1Move = movePerIMass * circle.GetComponent<Particle2D>().GetInverseMass();

            Vector2 AABB2Move = movePerIMass * -AABB.GetComponent<Particle2D>().GetInverseMass();

            circle.transform.position = circle.transform.position + new Vector3(AABB1Move.x, AABB1Move.y, 0);
            //Debug.Log(AABB1.transform.position);
            AABB.transform.position = AABB.transform.position + new Vector3(AABB2Move.x, AABB2Move.y, 0);
            return true;
        }

        return false;
    }

    static protected bool CircleVSOBB(CircleHull circle, OBBHull OBB)
    {
        
        Vector2 halfExtend = (OBB.max - OBB.min) / 2;
        Vector2 circleInOBB = OBB.transform.InverseTransformPoint(circle.GetCenter());
        Vector2 circleBox = new Vector2(Mathf.Max(-halfExtend.x, Mathf.Min(circleInOBB.x, halfExtend.x)),
            Mathf.Max(-halfExtend.y, Mathf.Min(circleInOBB.y, halfExtend.y)));

        Vector2 distance = circleInOBB - circleBox;
        float distanceSQ = Vector2.Dot(distance, distance);
        if (distanceSQ <= (circle.radius * circle.radius))
        {
            return true;
        }

        return false;
    }

    static protected bool AABBVSAABB(AABBHull AABB1 , AABBHull AABB2)
    {
        if (AABB1.max.x + AABB1.center.x >= AABB2.min.x + AABB2.center.x &&
            AABB1.max.y + AABB1.center.y >= AABB2.min.y + AABB2.center.y && 
            AABB2.max.x + AABB2.center.x >= AABB1.min.x + AABB1.center.x && 
            AABB2.max.y + AABB2.center.y >= AABB1.min.y + AABB1.center.y)
        {
            float penetration = 0f;
            Vector2 contact = new Vector2(0,0);

            Vector2 n = AABB2.center - AABB1.center;

            float a_extentX = (AABB1.max.x - AABB1.min.x) / 2;
            float b_extentX = (AABB2.max.x - AABB2.min.x) / 2;

            float a_extentY = (AABB1.max.y - AABB1.min.y) / 2;
            float b_extentY = (AABB2.max.y - AABB2.min.y) / 2;

            float x_overlap = a_extentX + b_extentX - Mathf.Abs(n.x);
            float y_overlap = a_extentY + b_extentY - Mathf.Abs(n.y);
            if (x_overlap > 0)
            {
                if (y_overlap > 0)
                {
                    if (x_overlap > y_overlap)
                    {
                        if (n.x < 0)
                        {
                            contact = new Vector2(-1, 0);
                        }
                        else
                        {
                            contact = new Vector2(1, 0);
                            
                        }
                        contact = n * contact.x;
                        penetration = x_overlap;
                    }
                    else
                    {
                        if (n.y < 0)
                        {
                            contact = new Vector2(0, -1);
                        }
                        else
                        {
                            contact = new Vector2(0, 1);
                            
                        }
                        contact = n * contact.y;
                        penetration = y_overlap;
                    }
                }
            }
            Debug.Log(penetration);

            float totalInverseMass = AABB1.GetComponent<Particle2D>().GetInverseMass() + AABB2.GetComponent<Particle2D>().GetInverseMass();
            Vector2 closingVelocity = -(AABB1.GetComponent<Particle2D>().velocity - AABB2.GetComponent<Particle2D>().velocity) * contact;
            Vector2 separatingVelocity = (AABB1.GetComponent<Particle2D>().velocity - AABB2.GetComponent<Particle2D>().velocity) * contact;
            float restitution = 1f;

            Vector2 newSeparating = -separatingVelocity * restitution;

            Vector2 deltaVelocity = newSeparating - separatingVelocity;

            Vector2 impulse = deltaVelocity / totalInverseMass;

            Vector2 impulsePerMass = contact * impulse;

            AABB1.GetComponent<Particle2D>().SetVelocity(AABB1.GetComponent<Particle2D>().velocity + impulsePerMass * AABB1.GetComponent<Particle2D>().GetInverseMass());
            Debug.Log(AABB1.GetComponent<Particle2D>().velocity);

            AABB2.GetComponent<Particle2D>().SetVelocity(AABB2.GetComponent<Particle2D>().velocity + impulsePerMass * -AABB2.GetComponent<Particle2D>().GetInverseMass());
            
           

            Vector2 movePerIMass = contact * (penetration / totalInverseMass);

            Vector2 AABB1Move = movePerIMass * AABB1.GetComponent<Particle2D>().GetInverseMass();

            Vector2 AABB2Move = movePerIMass * -AABB2.GetComponent<Particle2D>().GetInverseMass();

            AABB1.transform.position = AABB1.transform.position + new Vector3(AABB1Move.x, AABB1Move.y, 0);
            //Debug.Log(AABB1.transform.position);
            AABB2.transform.position = AABB2.transform.position + new Vector3(AABB2Move.x, AABB2Move.y, 0);
            
            return true;
        }
        return false;
    }

    static protected bool AABBVSOBB(AABBHull AABB, OBBHull OBB)
    {


        Vector2 AABBMinTransform = OBB.transform.InverseTransformPoint(AABB.min + AABB.center);
        Vector2 AABBMaxTransform = OBB.transform.InverseTransformPoint(AABB.max + AABB.center);

        //Debug.DrawLine(AABBMinTransform, AABBMaxTransform, Color.red);
        //Debug.DrawLine(OBB.min, OBB.max, Color.green);
        //Debug.DrawLine(AABB.center, OBB.center, Color.yellow);

        if (AABB.max.x + AABB.center.x >= OBB.min.x + OBB.center.x && OBB.max.x + OBB.center.x >= AABB.min.x + AABB.center.x)
        {
            if (AABB.max.y + AABB.center.y >= OBB.min.y + OBB.center.y && OBB.max.y + OBB.center.y >= AABB.min.y + AABB.center.y)
            {


                
                /*if (obbMinTransform.x > obbMaxTransform.x && obbMinTransform.y > obbMaxTransform.y)
                {
                    Vector2 temp = obbMinTransform;
                    obbMinTransform = obbMaxTransform;
                    obbMaxTransform = temp;
                }*/


                if (AABBMaxTransform.x + AABB.center.x >= OBB.min.x + OBB.center.x && OBB.max.x + OBB.center.x >= AABBMinTransform.x + AABB.center.x)
                {
                    if (AABBMaxTransform.y + AABB.center.y >= OBB.min.x + OBB.center.y && OBB.max.x + OBB.center.y >= AABBMinTransform.y + AABB.center.y)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    static protected bool OBBVSOBB(OBBHull OBB1, OBBHull OBB2)
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

            if (!(OBB1Max >= OBB2Min && OBB2Max >= OBB1Min))
            {
                return false;
            }
        }

        return true;
    }

  

    
}






