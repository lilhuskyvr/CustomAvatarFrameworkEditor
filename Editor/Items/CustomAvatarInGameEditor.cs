using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomAvatarFramework.Editor.Items
{
    public class CustomAvatarInGameEditor: MonoBehaviour
    {
        public Button spawnMannequinButton;
        public Button spawnCreatureButton;
        public Button copyBonesButton;

        public Text creatureIdText;
        public Dropdown bonesSelector;
        public InputField extraRotationXInputField;
        public InputField extraRotationYInputField;
        public InputField extraRotationZInputField;
        public Slider extraRotationXSlider;
        public Slider extraRotationYSlider;
        public Slider extraRotationZSlider;
        
        //grips
        public Dropdown gripsSelector;
        public Button testGripButton;

        public Button showHideCreatureButton;
        public Slider avatarHeightSlider;
        public Button copyAvatarHeightButton;       
        
        //hipsCalibration
        public Slider extraHipPositionXSlider;
        public Slider extraHipPositionYSlider;
        public Button copyExtraHipPositionButton;   

        //extra dimension
        public Slider extraDimensionXSlider;
        public Slider extraDimensionYSlider;
        public Slider extraDimensionZSlider;
        public Button copyExtraDimensionButton;             
    }
}