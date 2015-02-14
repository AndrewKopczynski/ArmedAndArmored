using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Window;

namespace ArmedAndArmored.Animation
{
    public class KeyFrame
    {
        Bone skeletonRoot;
        List<BoneLink> skeleton;

        public KeyFrame(Bone root)
        {
            skeletonRoot = root;
            skeleton = new List<BoneLink>();
        }

        public bool addBones(BoneLink bone)
        {
            skeleton.Add(bone);

            if (skeleton.Contains(bone))
            { 
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool removeBones(BoneLink bone)
        {
            return skeleton.Remove(bone);
        }

        public List<BoneLink> Skeleton
        {
            get { return skeleton; }
        }
    }
}
