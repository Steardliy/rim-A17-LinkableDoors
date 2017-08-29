using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkableDoors
{
    public class LinkData
    {
        public List<Building_LinkableDoor> linkedDoors = new List<Building_LinkableDoor>();
        public int linkType = -1;
        public bool Any()
        {
            return linkedDoors.Any();
        }
        public void Reset()
        {
            this.linkedDoors.Clear();
            this.linkType = -1;
        }
        public LinkData() { }
        public LinkData(Building_LinkableDoor other, int type)
        {
            this.linkedDoors.Add(other);
            this.linkType = type;
        }
    }

    public enum LinkDirection : int
    {
        Linking_Up,
        Linking_Right,
        Linking_Down,
        Linking_Left
    }
}
