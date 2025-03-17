using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonTriangulation : MonoBehaviour
{
    public int numOfTriangle; 

    public GameObject[] goArray;
    List<Vector3> dotList = new List<Vector3>();
    LineRenderer lr;

    LineRenderer[] triangles;

    float CCWby2D(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 p = b - a;
        Vector3 q = c - b;

        return Vector3.Cross(p, q).y;
    }

    void makeTriangle(LineRenderer lr, Vector3 a, Vector3 b, Vector3 c)
    {
        lr.startWidth = lr.endWidth = 1.0f;
        lr.material.color = Color.red;

        lr.positionCount = 3;

        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        lr.SetPosition(2, c);

        lr.loop = true;
    }

    bool CheckDotInLine(Vector3 a, Vector3 b, Vector3 dot)
    {
        float epsilon = 0.00001f;
        float dAB = Vector3.Distance(a, b);
        float dADot = Vector3.Distance(a, dot);
        float dBDot = Vector3.Distance(b, dot);

        return ((dAB + epsilon) >= (dADot + dBDot));
    }

    bool CrossCheck2D(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        // (x, 0, z)
        float x1, x2, x3, x4, z1, z2, z3, z4, X, Z;

        x1 = a.x; z1 = a.z;
        x2 = b.x; z2 = b.z;
        x3 = c.x; z3 = c.z;
        x4 = d.x; z4 = d.z;

        float cross = ((x1 - x2) * (z3 - z4) - (z1 - z2) * (x3 - x4));

        if (cross == 0 /* parallel */) return false;

        X = ((x1 * z2 - z1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * z4 - z3 * x4)) / cross;
        Z = ((x1 * z2 - z1 * x2) * (z3 - z4) - (z1 - z2) * (x3 * z4 - z3 * x4)) / cross;

        return CheckDotInLine(a, b, new Vector3(X, 0, Z)) && CheckDotInLine(c, d, new Vector3(X, 0, Z));
    }

    float getAreaOfTriangle(Vector3 dot1, Vector3 dot2, Vector3 dot3)
    {
        Vector3 a = dot2 - dot1;
        Vector3 b = dot3 - dot1;
        Vector3 cross = Vector3.Cross(a, b);

        return cross.magnitude / 2.0f;
    }

    bool checkTriangleInPoint(Vector3 dot1, Vector3 dot2, Vector3 dot3, Vector3 checkPoint)
    {
        float area = getAreaOfTriangle(dot1, dot2, dot3);
        float dot12 = getAreaOfTriangle(dot1, dot2, checkPoint);
        float dot23 = getAreaOfTriangle(dot2, dot3, checkPoint);
        float dot31 = getAreaOfTriangle(dot3, dot1, checkPoint);

        return (dot12 + dot23 + dot31) <= area + 0.1f /* 오차 허용 */;
    }

    bool CrossCheckAll(List<Vector3> list, int index)
    {
        Vector3 a = list[index];
        Vector3 b = list[index + 2];

        // 마지막 삼각형은 삼각형 내부의 점으로 판단
        if (list.Count == 4 && index == 0)
        {
            Vector3 c = list[index + 1];
            Vector3 d = list[index + 3];

            return checkTriangleInPoint(a, b, c, d);
        }

        for (int i = index + 3; i < list.Count - 1; i++)
        {
            Vector3 c = list[i];
            Vector3 d = list[i + 1];

            bool check = CrossCheck2D(a, b, c, d);
            if (check == true) return true;
        }

        return false;
    }

    void triangluation(int count)
    {
        dotList.Clear();
        foreach (GameObject go in goArray) // init
            dotList.Add(go.transform.position);

        triangles = this.GetComponentsInChildren<LineRenderer>();

        for (int i = 1; i < triangles.Length; i++) // init
            triangles[i].positionCount = 0;

        //int numOfTriangle = dotList.Count - 2;
        if (count > dotList.Count - 2) count = dotList.Count - 2;
        for (int i = 0; i < count; i++)
        {
            List<Vector3> copy = new List<Vector3>(dotList);

            for (int k = 0; k < copy.Count - 2; k++)
            {
                bool ccw = (CCWby2D(copy[k], copy[k + 1], copy[k + 2]) > 0);
                bool cross = CrossCheckAll(copy, k);

                if (ccw == true && cross == false)
                {
                    /* triangle[0]은 부모의 LineRenderer */
                    makeTriangle(triangles[i + 1], copy[k], copy[k + 1], copy[k + 2]);
                    copy.RemoveAt(k + 1);
                    dotList = new List<Vector3>(copy);

                    break;
                }
            }
        }
    }

    void OnValidate()
    {
        if(numOfTriangle > 0)
        {
            triangluation(numOfTriangle);
        }
    }

    void Start()
    {
        foreach (GameObject go in goArray)
            dotList.Add(go.transform.position);

        lr = this.GetComponent<LineRenderer>();
        lr.startWidth = lr.endWidth = 1.0f;
        lr.material.color = Color.blue;

        lr.positionCount = dotList.Count;

        for (int i = 0; i < dotList.Count; i++) 
            lr.SetPosition(i, dotList[i]);

        lr.loop = true; 
    }
}
