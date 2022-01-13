using ThunderRoad;
using UnityEditor;
using UnityEngine;

namespace CustomAvatarFramework.Editor
{
    public static class ItemExtension
    {
        public static void AutoGenerateParams(this Item item)
        {
            IconManager.SetIcon(item.gameObject, null);

            if (!item.gameObject.activeInHierarchy) return;

            Transform holderPoint = null;

            holderPoint = item.transform.Find("HolderPoint");

            if (!holderPoint)
            {
                holderPoint = new GameObject("HolderPoint").transform;
                holderPoint.SetParent(item.transform, false);
            }

            item.parryPoint = item.transform.Find("ParryPoint");
            if (!item.parryPoint)
            {
                item.parryPoint = new GameObject("ParryPoint").transform;
                item.parryPoint.SetParent(item.transform, false);
            }

            item.preview = item.GetComponentInChildren<Preview>();
            if (!item.preview && item.transform.Find("Preview"))
                item.preview = item.transform.Find("Preview").gameObject.AddComponent<Preview>();
            if (!item.preview)
            {
                item.preview = new GameObject("Preview").AddComponent<Preview>();
                item.preview.transform.SetParent(item.transform, false);
            }

            Transform whoosh = item.transform.Find("Whoosh");
            if (whoosh && !whoosh.GetComponent<WhooshPoint>())
            {
                whoosh.gameObject.AddComponent<WhooshPoint>();
            }

            if (!item.mainHandleRight)
            {
                foreach (Handle handle in item.GetComponentsInChildren<Handle>())
                {
                    if (handle.IsAllowed(Side.Right))
                    {
                        item.mainHandleRight = handle;
                        break;
                    }
                }
            }

            if (!item.mainHandleLeft)
            {
                foreach (Handle handle in item.GetComponentsInChildren<Handle>())
                {
                    if (handle.IsAllowed(Side.Left))
                    {
                        item.mainHandleLeft = handle;
                        break;
                    }
                }
            }

            if (!item.mainHandleRight)
            {
                item.mainHandleRight = item.GetComponentInChildren<Handle>();
            }

            if (item.useCustomCenterOfMass)
            {
                item.GetComponent<Rigidbody>().centerOfMass = item.customCenterOfMass;
            }
            else
            {
                item.GetComponent<Rigidbody>().ResetCenterOfMass();
            }

            if (item.customInertiaTensor)
            {
                if (item.customInertiaTensorCollider == null)
                {
                    item.customInertiaTensorCollider =
                        new GameObject("InertiaTensorCollider").AddComponent<CapsuleCollider>();
                    item.customInertiaTensorCollider.transform.SetParent(item.transform, false);
                    item.customInertiaTensorCollider.radius = 0.05f;
                    item.customInertiaTensorCollider.direction = 2;
                }

                item.customInertiaTensorCollider.enabled = false;
                item.customInertiaTensorCollider.isTrigger = true;
                item.customInertiaTensorCollider.gameObject.layer = 2;
            }
        }
    }
}