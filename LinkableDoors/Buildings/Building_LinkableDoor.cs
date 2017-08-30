﻿using RimWorld;
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
        public override void Draw()
        {
            if (this.linkData.linkType == 0)
            {
                base.Draw();
                return;
            }

            if (this.ShouldSingleDoor())
            {
                foreach (var current in linkData.linkedDoors)
                {
                    current.Nortify_UnLinked(this);
                }
                this.linkData.Reset();
                base.Draw();
                return;
            }

            float num = Mathf.Clamp01((float)this.visualTicksOpen / (float)base.TicksToOpenNow);
            float d = 0f + 0.95f * num;

            Vector3 vector = default(Vector3);
            Vector3 offset = default(Vector3);
            Mesh mesh;
            switch (this.linkData.linkType) {
                case (int)LinkDirection.Linking_Up:
                    vector = new Vector3(0f, 0f, -1f);
                    offset = new Vector3(0f, 0f, -0.5f);
                    mesh = LD_MeshPool.Plane10Wide;
                    base.Rotation = Rot4.East;
                    break;
                case (int)LinkDirection.Linking_Right:
                    vector = new Vector3(0f, 0f, 1f);
                    offset = new Vector3(-0.5f, 0f, 0f);
                    mesh = LD_MeshPool.Plane10FlipWide;
                    base.Rotation = Rot4.North;
                    break;
                case (int)LinkDirection.Linking_Down:
                    vector = new Vector3(0f, 0f, 1f);
                    offset = new Vector3(0f, 0f, 0.5f);
                    mesh = LD_MeshPool.Plane10FlipWide;
                    base.Rotation = Rot4.East;
                    break;
                case (int)LinkDirection.Linking_Left:
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
                
            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            vector = rotation.AsQuat * vector;
            Vector3 vector2 = this.DrawPos;
            vector2.y = Altitudes.AltitudeFor(AltitudeLayer.DoorMoveable);
            vector2 += offset + vector * d;
            Graphics.DrawMesh(mesh, vector2, base.Rotation.AsQuat, this.Graphic.MatAt(base.Rotation, null), 0);

            base.Comps_PostDraw();
        }
        
        private bool ShouldSingleDoor()
        {
            IntVec3 pos = base.Position;
            int num = this.AlignQualityAgainst(pos + IntVec3.East, base.Map);
            num += this.AlignQualityAgainst(pos + IntVec3.West, base.Map);
            if (num == 2)
            {
                return true;
            }
            
            num = this.AlignQualityAgainst(pos + IntVec3.North, base.Map);
            num += this.AlignQualityAgainst(pos + IntVec3.South, base.Map);
            if (num == 2)
            {
                return true;
            }
            return false;
        }

        private int AlignQualityAgainst(IntVec3 c, Map map)
        {
            if (!c.InBounds(map))
            {
                return 0;
            }
            if (!c.Walkable(map))
            {
                return 1;
            }
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i] as Blueprint;
                if (thing != null)
                {
                    if (thing.def.entityDefToBuild.passability == Traversability.Impassable)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }
    }
}
