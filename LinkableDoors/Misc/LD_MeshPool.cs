using UnityEngine;
using Verse;

namespace LinkableDoors
{
    [StaticConstructorOnStartup]
    public static class LD_MeshPool
    {
        public static readonly Mesh plane10Wide;
        public static readonly Mesh plane10FlipWide;
        public static readonly Mesh plane10Fill;
        public static readonly Mesh plane10FillHalf;

        static LD_MeshPool()
        {
            LD_MeshPool.plane10Wide = LD_MeshPool.NewPlaneMesh(new Vector2(2f, 1f), false);
            LD_MeshPool.plane10FlipWide = LD_MeshPool.NewPlaneMesh(new Vector2(2f, 1f), true);

            Vector2[] uvs = new Vector2[4]
            {
                new Vector2(0.1f, 0f),
                new Vector2(0.1f, 1f),
                new Vector2(0.2f, 1f),
                new Vector2(0.2f, 0f)
            };
            LD_MeshPool.plane10FillHalf = LD_MeshPool.NewPlaneMesh(new Vector2(0.5f, 1f), false, uvs);
            LD_MeshPool.plane10Fill = LD_MeshPool.NewPlaneMesh(new Vector2(1.1f, 1f), false, uvs);
        }

        static Mesh NewPlaneMesh(Vector2 size, bool flipped, Vector2[] uvs = null)
        {
            Vector3[] array = new Vector3[4];
            array[0] = new Vector3(-0.5f * size.x, 0f, -0.5f * size.y);
            array[1] = new Vector3(-0.5f * size.x, 0f, 0.5f * size.y);
            array[2] = new Vector3(0.5f * size.x, 0f, 0.5f * size.y);
            array[3] = new Vector3(0.5f * size.x, 0f, -0.5f * size.y);
            
            Vector2[] array2 = uvs;
            if (array2 == null)
            {
                array2 = new Vector2[4];
                if (!flipped)
                {
                    array2[0] = new Vector2(0f, 0f);
                    array2[1] = new Vector2(0f, 1f);
                    array2[2] = new Vector2(1f, 1f);
                    array2[3] = new Vector2(1f, 0f);
                }
                else
                {
                    array2[0] = new Vector2(1f, 0f);
                    array2[1] = new Vector2(1f, 1f);
                    array2[2] = new Vector2(0f, 1f);
                    array2[3] = new Vector2(0f, 0f);
                }
            }

            int[] array3 = new int[6];
            array3[0] = 0;
            array3[1] = 1;
            array3[2] = 2;
            array3[3] = 0;
            array3[4] = 2;
            array3[5] = 3;
            Mesh mesh = new Mesh();
            mesh.name = "NewPlaneMesh()";
            mesh.vertices = array;
            mesh.uv = array2;
            mesh.SetTriangles(array3, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
