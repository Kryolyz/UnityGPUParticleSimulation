using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [HideInInspector]
    public int avgFrameRate;
    [SerializeField]
    public Text display_Text;
    private int frameCount = 0;
    private float dt = 0.0f;
    [HideInInspector]
    public float fps = 0.0f;
    [SerializeField]
    private float updateRate = 4.0f;  // 4 updates per sec.

    void Start()
    {
        display_Text = GetComponent<Text>();
    }

    void Update() 
    {
        frameCount++;
        dt += Time.unscaledDeltaTime;
        if (dt > 1.0 / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
            avgFrameRate = (int)fps;
            display_Text.text = string.Format("{0} FPS", avgFrameRate);
        }
    }
}
