using RimWorld;
using System;
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
                return;
            }
            this.linkable.CallBack = this.CallBack;
        }
        public override void Tick()
        {
            base.Tick();
            if (base.Open)
            {
                foreach (var a in this.linkable.GroupParent.GetTagGroup(this.linkable.PosTag & (PositionTag.RightSide | PositionTag.LeftSide)))
                {
                    if (a.DistFromCenter < this.linkable.DistFromCenter)
                    {
                        a.CallBack(base.ticksUntilClose);
                        a.synchronize = true;
                    }
                }
                if (this.linkable.synchronize && base.ticksUntilClose <= 0)
                {
                    base.DoorTryClose();
                    this.linkable.synchronize = false;
                }
            }
        }
        public void CallBack(int param)
        {
            if (!base.Open)
            {
                base.DoorOpen(param);
            }
            //base.FriendlyTouched();
            base.ticksUntilClose = param;
        }

        public override void Draw()
        {
            if (this.linkable == null || this.linkable.IsSingle)
            {
                base.Draw();
                return;
            }

            if (LinkGroupUtility.ShouldSingle(base.Position, base.Map))
            {
                LinkGroupUtility.DetachGroupAround(this.linkable);
                base.Draw();
                return;
            }

            base.Rotation = this.linkable.LineDirection;
            float num = Mathf.Clamp01((float)this.visualTicksOpen / (float)base.TicksToOpenNow);

            float[] move = { 0, 0 };
            Vector3[] offset = { default(Vector3), default(Vector3) };
            Vector3[] vector = { default(Vector3), default(Vector3) };
            Mesh[] mesh = { null, null };

            switch (this.linkable.PosTag)
            {
                case PositionTag.LeftSide:
                    this.linkable.commonField = 1f * num;
                    move[0] = this.linkable.GroupParent.GetCommonFieldSum(PositionTag.LeftSide);
                    vector[0] = new Vector3(0f, 0f, -1f);
                    offset[0] = new Vector3(0f, 0.1f, 0.1f);
                    mesh[0] = LD_MeshPool.plane10Fill;
                    break;
                case PositionTag.RightSide:
                    this.linkable.commonField = 1f * num;
                    move[0] = this.linkable.GroupParent.GetCommonFieldSum(PositionTag.RightSide);
                    vector[0] = new Vector3(0f, 0f, 1f);
                    offset[0] = new Vector3(0f, 0.1f, -0.1f);
                    mesh[0] = LD_MeshPool.plane10Fill;
                    break;
                case PositionTag.LeftBorder | PositionTag.LeftSide:
                    this.linkable.commonField = 1f * num;
                    move[0] = this.linkable.GroupParent.GetCommonFieldSum(PositionTag.LeftSide);
                    move[1] = move[0];
                    vector[0] = new Vector3(0f, 0f, -1f);
                    vector[1] = vector[0];
                    offset[0] = new Vector3(0f, 0f, 0.5f);
                    offset[1] = new Vector3(0f, 0.1f, -0.23f);
                    mesh[0] = MeshPool.plane10;
                    mesh[1] = LD_MeshPool.plane10FillHalf;
                    break;
                case PositionTag.RightBorder | PositionTag.RightSide:
                    this.linkable.commonField = 1f * num;
                    move[0] = this.linkable.GroupParent.GetCommonFieldSum(PositionTag.RightSide);
                    move[1] = move[0];
                    vector[0] = new Vector3(0f, 0f, 1f);
                    vector[1] = vector[0];
                    offset[0] = new Vector3(0f, 0f, -0.5f);
                    offset[1] = new Vector3(0f, 0.1f, 0.23f);
                    mesh[0] = MeshPool.plane10Flip;
                    mesh[1] = LD_MeshPool.plane10FillHalf;
                    break;
                case PositionTag.Center:
                    this.linkable.commonField = 0.45f * num;
                    move[0] = this.linkable.GroupParent.GetCommonFieldSum(PositionTag.LeftSide);
                    move[1] = this.linkable.GroupParent.GetCommonFieldSum(PositionTag.RightSide);
                    vector[0] = new Vector3(0f, 0f, -1f);
                    vector[1] = new Vector3(0f, 0f, 1f);
                    mesh[0] = MeshPool.plane10;
                    mesh[1] = MeshPool.plane10Flip;
                    break;
                default:
                    Log.Error("[LinkableDoors] default");
                    vector[0] = new Vector3(0f, 0f, 1f);
                    mesh[0] = LD_MeshPool.plane10FlipWide;
                    break;
            }

            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            Quaternion quat = rotation.AsQuat;

            for (int i = 0; i < 2; i++)
            {
                if (mesh[i] != null)
                {
                    vector[i] = quat * vector[i];
                    Vector3 vector2 = this.DrawPos;
                    vector2.y = Altitudes.AltitudeFor(AltitudeLayer.DoorMoveable) + offset[i].y;
                    offset[i] = quat * offset[i];
                    vector2 += offset[i] + vector[i] * move[i];
                    Graphics.DrawMesh(mesh[i], vector2, base.Rotation.AsQuat, this.Graphic.MatAt(base.Rotation, null), 0);
                }
            }
            base.Comps_PostDraw();
        }
    }
}
