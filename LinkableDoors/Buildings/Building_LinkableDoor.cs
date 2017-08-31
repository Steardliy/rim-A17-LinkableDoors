using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using System.Reflection;
using System;

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
            if (this.linkable.LinkingFrom == 0)
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

            float num = Mathf.Clamp01((float)this.visualTicksOpen / (float)base.TicksToOpenNow);
            float d = 0f + 0.95f * num;
            Vector3 vector = default(Vector3);
            Vector3 offset = default(Vector3);
            Mesh mesh;

            if (this.linkable.DirectLinkCount >= 2)
            {
                /*if (base.DrawPos.sqrMagnitude < this.linkable.GroupParent.Center.sqrMagnitude)
                {

                }
                else if (base.DrawPos.sqrMagnitude > this.linkable.GroupParent.Center.sqrMagnitude)
                {

                }
                else
                {

                }*/
                mesh = LD_MeshPool.Plane10Fill;
                vector = new Vector3(0f, 0f, 1f);
                base.Rotation = (this.linkable.LinkingFrom & 0x01) == 0x01 ? Rot4.East : Rot4.North;
            }
            else
            {
                switch (this.linkable.LinkingFrom)
                {
                    case (int)LinkDirections.Up:
                        vector = new Vector3(0f, 0f, -1f);
                        offset = new Vector3(0f, 0f, -0.5f);
                        mesh = LD_MeshPool.Plane10Wide;
                        base.Rotation = Rot4.East;
                        break;
                    case (int)LinkDirections.Right:
                        vector = new Vector3(0f, 0f, 1f);
                        offset = new Vector3(-0.5f, 0f, 0f);
                        mesh = LD_MeshPool.Plane10FlipWide;
                        base.Rotation = Rot4.North;
                        break;
                    case (int)LinkDirections.Down:
                        vector = new Vector3(0f, 0f, 1f);
                        offset = new Vector3(0f, 0f, 0.5f);
                        mesh = LD_MeshPool.Plane10FlipWide;
                        base.Rotation = Rot4.East;
                        break;
                    case (int)LinkDirections.Left:
                        vector = new Vector3(0f, 0f, -1f);
                        offset = new Vector3(0.5f, 0f, 0f);
                        mesh = LD_MeshPool.Plane10Wide;
                        base.Rotation = Rot4.North;
                        break;
                    default:
                        Log.Warning("LinkableDoor:default.");
                        vector = new Vector3(0f, 0f, -1f);
                        mesh = LD_MeshPool.Plane10Wide;
                        break;
                }
            }
            
            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            vector = rotation.AsQuat * vector;
            Vector3 vector2 = this.DrawPos;
            vector2.y = Altitudes.AltitudeFor(AltitudeLayer.DoorMoveable);
            vector2 += offset + vector * d;
            Graphics.DrawMesh(mesh, vector2, base.Rotation.AsQuat, this.Graphic.MatAt(base.Rotation, null), 0);

            base.Comps_PostDraw();
        }
    }
}
