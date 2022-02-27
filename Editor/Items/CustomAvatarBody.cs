using System.Collections.Generic;
using UnityEngine;

namespace CustomAvatarFramework.Editor.Items
{
    /// <summary>
    /// used in Custom Avatar Prefab Builder to indicate the body. the body contains blend shapes 
    /// </summary>
    public class CustomAvatarBody : MonoBehaviour
    {
        [HideInInspector]
        public List<string> filledSlotNames = new List<string>();
    }
}