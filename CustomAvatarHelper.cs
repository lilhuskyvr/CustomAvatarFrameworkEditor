using System.Collections;
using System.Collections.Generic;
using CustomAvatarFramework.Editor.Items;
using EasyButtons;
using UnityEngine;

public class CustomAvatarHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button("Map Bones")]
    void MapBones()
    {
        GetComponent<CustomAvatar>().MapBones();
    }
    
    [Button("Calibrate Character")]
    void Resize3dModel()
    {
        GetComponent<CustomAvatar>().CalibrateCharacter();
    }
    
    [Button("Generate JSON files")]
    void GenerateJSONFiles()
    {
        GetComponent<CustomAvatar>().GenerateJSONFiles();
    }
}
