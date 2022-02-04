using EasyButtons;
using UnityEngine;

namespace CustomAvatarFramework.Editor
{
    public class CustomAvatarHeadAdder: MonoBehaviour
    {
        public GameObject inputGameObject;

        [Button]
        public void AddCustomAvatarHeads()
        {
            inputGameObject.AddCustomAvatarHeads();
        }
    }
}