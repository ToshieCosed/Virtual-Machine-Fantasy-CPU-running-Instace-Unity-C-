using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTexture : MonoBehaviour {
    Texture2D text;
    Color[,] colors = new Color[160, 144];
	// Use this for initialization
	void Start () {


        
            Texture2D texture = new Texture2D(160, 144);
            GetComponent<Renderer>().material.mainTexture = texture;

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color color = ((x & y) != 0 ? Color.white : Color.gray);
                    texture.SetPixel(x, y, color);
                    colors[x, y] = color;
                }
            }
            texture.Apply();
        }
    
    // Update is called once per frame
    void Update()
    {
        Texture2D texture = new Texture2D(160, 144);
        GetComponent<Renderer>().material.mainTexture = texture;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                //Color b = new Color32( (byte) UnityEngine.Random.Range(0, 255), (byte) UnityEngine.Random.Range(0, 255), (byte) UnityEngine.Random.Range(0, 255), 255);
                // Color color = b;
                Color b = colors[x, y];
                    //brighten the color
                b  = brighten(b, 1.005f);
                    //Update the change in the framebuffer array
                changecolor(b, x, y);
                    //Display the pixel on the texture.
                texture.SetPixel(x, y, b);
            }
        }
        texture.Apply();
    }

    public void changecolor(Color c, int x, int y)
    {
        colors[x, y] = c;
    }

    public Color brighten(Color c, float power)
    {
        c.a = Mathf.Pow(c.a, power);
        c.b = Mathf.Pow(c.b, power);
        c.g = Mathf.Pow(c.g, power);
        c.r = Mathf.Pow(c.r, power);

        if (c.a > 256) { c.a = 256; }
        if (c.b > 256) { c.b = 256; }
        if (c.r > 256) { c.r = 256; }
        if (c.g > 256) { c.g = 256; }

        return c;

    }

}


	
	



