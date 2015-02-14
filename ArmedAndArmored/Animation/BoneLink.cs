using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmedAndArmored.Animation
{
    public class BoneLink
    {
        private Bone parent;
        private Bone child;

        //I don't usually use this, but it's a simple class.
        public BoneLink(Bone parent, Bone child)
        {
            this.parent = parent;
            this.child = child;
        }

        public Bone Parent
        {
            get { return parent; }
        }

        public Bone Child
        {
            get { return child; }
        }
    }
}
