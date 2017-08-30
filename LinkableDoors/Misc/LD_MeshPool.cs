using UnityEngine;
using Verse;

namespace LinkableDoors
{
    [StaticConstructorOnStartup]
    public static class LD_MeshPool
    {
        public static readonly Mesh Plane10Wide;
        public static readonly Mesh Plane10FlipWide;

        static LD_MeshPool()
        {
            LD_MeshPool.Plane10Wide = LD_MeshPool.NewPlaneMesh(new Vector2(2f, 1f), false);
            LD_MeshPool.Plane10FlipWide = LD_MeshPool.NewPlaneMesh(new Vector2(2f, 1f), true);
        }

        static Mesh NewPlaneMesh(Vector2 size, bool flipped)
        {
            Vector3[] array = new Vector3[4];
            Vector2[] array2 = new Vector2[4];
            int[] array3 = new int[6];
            array[0] = new Vector3(-0.5f * size.x, 0f, -0.5f * size.y);
            array[1] = new Vector3(-0.5f * size.x, 0f, 0.5f * size.y);
            array[2] = new Vector3(0.5f * size.x, 0f, 0.5f * size.y);
            array[3] = new Vector3(0.5f * size.x, 0f, -0.5f * size.y);

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
