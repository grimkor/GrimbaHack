using UnityEngine;

namespace GrimbaHack;

public class Online : MonoBehaviour
{
    private Rect _windowRect = new Rect(20, 20, 300, 150);

    private void SomeFunc(int windowID)
    {
        GUI.Label(new Rect(25, 25, 100, 30), "Attack:");
        GUI.Label(new Rect(135, 25, 100, 30), "light_high_1_0");
        GUI.Label(new Rect(25, 50, 100, 30), "Base Damage:");
        GUI.Label(new Rect(135, 50, 100, 30), "100");
        GUI.Label(new Rect(25, 75, 100, 30), "Blockstun:");
        GUI.Label(new Rect(135, 75, 100, 30), "25");
        GUI.Label(new Rect(25, 100, 100, 30), "Hitstun:");
        GUI.Label(new Rect(135, 100, 100, 30), "95");
        GUI.Label(new Rect(25, 125, 100, 30), "Advantage:");
        GUI.Label(new Rect(135, 125, 100, 30), "-12");
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    private void OnGUI()
    {
        // GUI.color = Color.black;
        GUI.backgroundColor = Color.black;

        var currentStyle = new GUIStyle(GUI.skin.box)
        {
            normal =
            {
                background = MakeTex(2, 2, new Color(0f, 1f, 0f, .8f))
            }
        };
        _windowRect = GUI.Window(0, _windowRect, (GUI.WindowFunction)SomeFunc, "Frame Data", currentStyle);
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}