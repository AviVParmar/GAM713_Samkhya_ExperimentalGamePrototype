using UnityEngine;
using UnityEngine.UI;

public class PointSystem : MonoBehaviour
{
    
    public Slider slider;

    public void SetBrahman(int brahman) {
        slider.value = brahman;
    }


}
