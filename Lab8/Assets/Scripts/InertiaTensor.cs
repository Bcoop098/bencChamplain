using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaTensor : MonoBehaviour
{
    public static InertiaTensor tensor;

    public float radius;

    public float mass;

    public float width;
    public float height;
    public float depth;

    private Matrix4x4 shape;
    // Start is called before the first frame update
    void Start()
    {
        tensor = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Matrix4x4 SolidSphere()
    {
        Vector4 column1 = new Vector4(0.2f * mass * (radius * radius), 0, 0, 0);
        Vector4 column2 = new Vector4(0, 0.2f * mass * (radius * radius), 0, 0);
        Vector4 column3 = new Vector4(0, 0, 0.2f * mass * (radius * radius), 0);
        Vector4 column4 = new Vector4(0, 0, 0, 1);

        shape = new Matrix4x4(column1, column2, column3, column4);
        return shape;
    }

    public Matrix4x4 HollowSphere()
    {
        Vector4 column1 = new Vector4((2f/3f) * mass * (radius * radius), 0, 0, 0);
        Vector4 column2 = new Vector4(0, (2f / 3f) * mass * (radius * radius), 0, 0);
        Vector4 column3 = new Vector4(0, 0, (2f / 3f) * mass * (radius * radius), 0);
        Vector4 column4 = new Vector4(0, 0, 0, 1);

        shape = new Matrix4x4(column1, column2, column3, column4);
        return shape;
    }

    public Matrix4x4 SolidBox()
    {
        Vector4 column1 = new Vector4((1f / 12f) * mass * ((height*height)+(depth*depth)), 0, 0, 0);
        Vector4 column2 = new Vector4(0, (1f / 12f) * mass * ((depth * depth) + (width * width)), 0, 0);
        Vector4 column3 = new Vector4(0, 0, (1f / 12f) * mass * ((width * width) + (height * height)), 0);
        Vector4 column4 = new Vector4(0, 0, 0, 1);

        shape = new Matrix4x4(column1, column2, column3, column4);
        return shape;
    }

    public Matrix4x4 HollowBox()
    {
        Vector4 column1 = new Vector4((5f / 3f) * mass * ((height * height) + (depth * depth)), 0, 0, 0);
        Vector4 column2 = new Vector4(0, (5f / 3f) * mass * ((depth * depth) + (width * width)), 0, 0);
        Vector4 column3 = new Vector4(0, 0, (5f / 3f) * mass * ((width * width) + (height * height)), 0);
        Vector4 column4 = new Vector4(0, 0, 0, 1);

        shape = new Matrix4x4(column1, column2, column3, column4);
        return shape;
    }

    public Matrix4x4 SolidCylinder()
    {
        Vector4 column1 = new Vector4((1f / 12f) * mass * ((3*(radius * radius)) + (height * height)), 0, 0, 0);
        Vector4 column2 = new Vector4(0, (1f / 12f) * mass * ((3 * (radius * radius)) + (height * height)), 0, 0);
        Vector4 column3 = new Vector4(0, 0, 0.5f * mass * (radius * radius), 0);
        Vector4 column4 = new Vector4(0, 0, 0, 1);

        shape = new Matrix4x4(column1, column2, column3, column4);
        return shape;
    }

    public Matrix4x4 SolidCone()
    {
        Vector4 column1 = new Vector4((0.6f * mass * (height * height)) + ((3f/20f) * mass *(radius*radius)), 0, 0, 0);
        Vector4 column2 = new Vector4(0, (0.6f * mass * (height * height)) + ((3f / 20f) * mass * (radius * radius)), 0, 0);
        Vector4 column3 = new Vector4(0, 0, 0.3f * mass * (radius * radius), 0);
        Vector4 column4 = new Vector4(0, 0, 0, 1);

        shape = new Matrix4x4(column1, column2, column3, column4);
        return shape;
    }

    public float getMass()
    {
        return mass;
    }
}
