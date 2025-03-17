using System.Collections.Generic;
using UnityEngine;

namespace Kang
{
    public static class Core 
    {
        private static float[,] dist; // 거리 행렬
        private static int n;
        private static Dictionary<(int, int), float> dp;

        public static Color RandomColor()
        {
            return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
        public static bool RandomBool(float percent)
        {
            return Random.Range(0f, 1f) < percent;
        }

        public static float CalculateMeshArea(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            float totalArea = 0f;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector2 v1 = (Vector2)vertices[triangles[i]];
                Vector2 v2 = (Vector2)vertices[triangles[i + 1]];
                Vector2 v3 = (Vector2)vertices[triangles[i + 2]];

                // 삼각형 면적 공식 (벡터 크로스 곱)
                float area = Mathf.Abs((v2.x - v1.x) * (v3.y - v1.y) - (v3.x - v1.x) * (v2.y - v1.y)) * 0.5f;
                totalArea += area;
            }

            return totalArea;
        }
        public static Vector2[] Mutation(Vector2[] origin, float power, float ratio, float vertRatio)
        {
            List<Vector2> mutVec = new List<Vector2>(origin);

            if (RandomBool(ratio))
            {
                for (int i = 0; i < origin.Length; i++)
                {
                    mutVec[i] = SingleMotation(power, origin[i]);
                }
                if (RandomBool(vertRatio))
                {
                    int index = Random.Range(0, origin.Length);
                    if (RandomBool(0.5f))
                        mutVec.RemoveAt(index);
                    else 
                    {
                        Vector2 newPos = origin[index] + origin[(index + 1) % origin.Length];
                        newPos *= 0.5f;
                        mutVec.Insert(index, SingleMotation(power, newPos));
                    }
                }
            }
            return mutVec.ToArray();
        }
        private static Vector2 SingleMotation(float power, Vector2 vec)
        {
            vec += new Vector2(Random.Range(-power, power), Random.Range(-power, power));
            return vec;
        }
    }
}