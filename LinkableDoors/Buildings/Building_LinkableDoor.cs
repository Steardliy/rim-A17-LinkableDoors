using RimWorld;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    public class Building_LinkableDoor : Building_Door
    {
        private ILinkData linkable;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.linkable = base.GetComp<CompLinkable>();
            if(this.linkable == null)
            {
                Log.Error("[LinkableDoors] This class does not have a component of Linkable.");
            }
        }
        public override void Draw()
        {
            if (this.linkable == null || this.linkable.LinkingFrom == 0)
            {
                base.Draw();
                return;
            }

            if (LinkGroupUtility.ShouldSingleDoor(base.Position, base.Map))
            {
                LinkGroupUtility.DetachGroupAround(this.linkable);
                base.Draw();
                return;
            }

            base.Rotation = ((this.linkable.LinkingFrom & 0x01) | (this.linkable.LinkingFrom >> 2 & 0x01)) == 0x01 ? Rot4.West : Rot4.North;
            float num = Mathf.Clamp01((float)this.visualTicksOpen / (float)base.TicksToOpenNow);
            float d = 0f + 0.95f * num;
            Vector3[] offset = { default(Vector3), default(Vector3) };
            Vector3[] vector = { default(Vector3), default(Vector3) };
            Mesh[] mesh = { null, null };
            
            switch (this.linkable.PosFlag)
            {
                case PositionFlag.LeftSide:
                    vector[1] = new Vector3(0f, 0f, -1f);
                    offset[1] = new Vector3(0f, 0f, 0.1f);
                    mesh[1] = LD_MeshPool.plane10Fill;
                    break;
                case PositionFlag.RightSide:
                    vector[1] = new Vector3(0f, 0f, 1f);
                    offset[1] = new Vector3(0f, 0f, -0.1f);
                    mesh[1] = LD_MeshPool.plane10Fill;
                    break;
                case PositionFlag.LeftBorder:
                    vector[0] = new Vector3(0f, 0f, -1f);
                    vector[1] = new Vector3(0f, 0f, -1f);
                    offset[0] = new Vector3(0f, 0f, 0.5f);
                    offset[1] = new Vector3(0f, 0f, -0.23f);
                    mesh[0] = MeshPool.plane10;
                    mesh[1] = LD_MeshPool.plane10FillHalf;
                    break;
                case PositionFlag.RightBorder:
                    vector[0] = new Vector3(0f, 0f, 1f);
                    vector[1] = new Vector3(0f, 0f, 1f);
                    offset[0] = new Vector3(0f, 0f, -0.5f);
                    offset[1] = new Vector3(0f, 0f, 0.23f);
                    mesh[0] = MeshPool.plane10Flip;
                    mesh[1] = LD_MeshPool.plane10FillHalf;
                    break;
                case PositionFlag.Center:
                    vector[0] = new Vector3(0f, 0f, -1f);
                    vector[1] = new Vector3(0f, 0f, 1f);
                    mesh[0] = MeshPool.plane10;
                    mesh[1] = MeshPool.plane10Flip;
                    break;
                default:
                    Log.Error("default");
                    vector[0] = new Vector3(0f, 0f, 1f);
                    mesh[0] = LD_MeshPool.plane10FlipWide;
                    break;
            }

            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);

            for (int i = 0; i < 2; i++)
            {
                if (mesh[i] != null)
                {
                    vector[i] = rotation.AsQuat * vector[i];
                    offset[i] = rotation.AsQuat * offset[i];
                    Vector3 vector2 = this.DrawPos;
                    vector2.y = Altitudes.AltitudeFor(AltitudeLayer.DoorMoveable) + (0.1f * i);
                    vector2 += offset[i] + vector[i] * d;
                    Graphics.DrawMesh(mesh[i], vector2, base.Rotation.AsQuat, this.Graphic.MatAt(base.Rotation, null), 0);
                }
            }
            base.Comps_PostDraw();
        }
    }
}
