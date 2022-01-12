using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace CustomAvatarFramework.Editor.Items
{
    public class SliderToText: MonoBehaviour
    {
        public Slider sliderUI;
        private Text textSliderValue;

        void Start (){
            textSliderValue = GetComponent<Text>();
            ShowSliderValue();
        }

        public void ShowSliderValue () {
            textSliderValue.text = sliderUI.value.ToString(CultureInfo.InvariantCulture);
        }
    }
}