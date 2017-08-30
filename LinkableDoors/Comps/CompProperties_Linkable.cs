using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace LinkableDoors
{
    class CompProperties_Linkable : CompProperties
    {
        public int linkableLimit = 9999;

        public CompProperties_Linkable()
        {
            base.compClass = typeof(CompLinkable);
        }
    }
}
