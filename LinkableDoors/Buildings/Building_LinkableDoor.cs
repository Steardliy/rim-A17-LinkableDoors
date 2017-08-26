﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    public class Building_LinkableDoor : Building_Door
    {
        public class LinkData
        {
            public List<Building_LinkableDoor> linkedDoors = new List<Building_LinkableDoor>();
            public int linkType = -1;
            public bool Any()
            {
                return linkedDoors.Any();
            }
            public LinkData() { }
            public LinkData(Building_LinkableDoor other, int type)
            {
                this.linkedDoors.Add(other);
                this.linkType = type;
            }
        }

        private enum LinkDirection : int
        {
            Linking_Up,
            Linking_Right,
            Linking_Down,
            Linking_Left
        }

        protected LinkData linkData;

        public bool CanLinkFromOther => !this.linkData.Any();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.linkData = this.CheckAround();
        }
        public override void DeSpawn()
        {
            base.DeSpawn();
            foreach (var current in linkData.linkedDoors)
            {
                current.Nortify_UnLinked(this);
            }
            this.linkData.linkedDoors.Clear();
        }

        public override void Draw()
        {
            if (this.linkData.linkType < 0)
            {
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

        public void Nortify_Linked(Building_LinkableDoor other, int type)
        {
            if (!this.linkData.Any())
            {
                this.linkData.linkedDoors.Add(other);
                this.linkData.linkType = type;
            }
        }
        public void Nortify_UnLinked(Building_LinkableDoor other)
        {
            if (this.linkData.Any())
            {
                this.linkData = CheckAround();
            }
        }

        private LinkData CheckAround()
        {
            LinkData result = new LinkData();
            for(int i = 0; i < 4; i++)
            {
                IntVec3 pos = base.Position + GenAdj.CardinalDirections[i];
                Building_LinkableDoor door = base.Map.thingGrid.ThingAt(pos, base.def) as Building_LinkableDoor;
                if(door != null  && door.CanLinkFromOther)
                {;
                    result.linkType = ((i + 2) % 4);
                    result.linkedDoors.Add(door);
                    door.Nortify_Linked(this, i);
                    break;
                }
            }
            return result;
        }
    }
}
