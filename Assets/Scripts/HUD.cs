using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class HUD : MonoBehaviour
{
    public Rect boxRect = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
    public Texture boxImage = new Texture();
    public GUISkin guiSkin = new GUISkin();

    private GUIText myGuiText;

	// Use this for initialization
	void Start()
	{
        myGuiText = GetComponent<GUIText>();

        float boxW = guiSkin.button.normal.background.width;
        float boxH = guiSkin.button.normal.background.height;
        boxRect = new Rect(Screen.width - boxW - 15, 10.0f, boxW, boxH);
	}

    void Update()
    {
        //myGuiText.text = string.Format("Image Height: {0}\r\nImage Width: {1}", boxImage.height, boxImage.width);
        //boxRect = new Rect(Screen.width - boxRect.width, 10.0f, 145.0f, 92.5f);
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        //GUI.Box(boxRect, boxImage, "");

        GUI.Button(boxRect, boxImage);
    }
}