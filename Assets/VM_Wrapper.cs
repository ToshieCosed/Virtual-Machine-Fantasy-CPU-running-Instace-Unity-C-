using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class VM_Wrapper : MonoBehaviour
{
    public VM_Machine myVM;
    Texture2D text;
    //the public colors array, I suppose for this texture
    Color[,] colors = new Color[160, 144];
    List<string> loglist = new List<string>();

    // Use this for initialization
    void Start()
    {
        string loadname = @"C:\Users\Public\Documents\Unity Projects\Texture_Write_Test3D\Assets\out.bin";
        //change loadname to the filepath of the .bin file that you want the virtual machine to run
        //string loadname = @"";
        int[] ram = LoadPayLoad(loadname);
        int ramsize = ram.Length+500;
        myVM = new VM_Machine(2000, 5000, 0, ram, 20000);

        //Try to apply the texture here
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
            int op = myVM.Step();
            loglist.Add("Virtual machine was stepped in update call");
            loglist.Add("PC value is " + myVM.PC);
            loglist.Add("Opcode byte found was " + op);
            float red = (float)myVM.GetMemValue(100);
            loglist.Add("So it turns out that the red channel for the color is now actually " + red);

        //int value = myVM.GetMemValue(1);
        float x = myVM.registers[1];
            float y = myVM.ram[1];
            float z = myVM.ram[2];

            //The below code would move normally this object, based on the register in the VM but we don't need that now :)
        Vector3 pos = gameObject.transform.position;
        //float x = pos.x + 10;
        pos.x = x;
        gameObject.transform.position = pos;

        //Debug.Log(myVM.registers[1]);
        //Debug.Log(myVM.ram.Length + " is the size of ram in bytes");

        //texture change stuff

        
        //Debug.Log("The color is " + red);
        float green = (float) myVM.GetMemValue(104);
        float blue = (float) myVM.GetMemValue(108);
        //Color M = new Color(value, 0, 90);
        Color c = new Color(red, green, blue);
        
        for (int xx =10; xx<15; xx++)
        {
            for (int yy =10; yy<15; yy++)
            {
                colors[xx, yy] = c;
            }
        }
        texturechange_stuff();
        //System.Threading.Thread.Sleep(100);

        if (Input.GetKey(KeyCode.S))
        {
            try
            {
                using (var fs = new FileStream("RamDump.bin", FileMode.Create, FileAccess.Write))
                {
                    myVM.StoreValueToMem(250, 100, myVM.ram);
                    byte[] out_ram = new byte[myVM.ram.Length - 1];
                    for (int ii=0; ii<myVM.ram.Length -1; ii++)
                    {
                        out_ram[ii] = (byte) myVM.ram[ii];
                    }
                    

                    fs.Write(out_ram, 0, myVM.ram.Length -1);
                    Debug.Log("Wrote file presumably ");
                    //return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                //return false;
            }

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("LogFile.txt", true))
            {
               foreach(string i in loglist)
                {
                    file.WriteLine(i);
                }
            }


        }

    }

    public void texturechange_stuff()
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
                //b = brighten(b, 1.005f);
                //Update the change in the framebuffer array
                //changecolor(b, x, y);
                //Display the pixel on the texture.
                texture.SetPixel(x, y, b);
            }
        }
        texture.Apply();
    }

    //framebuffer specific for this object
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

    //Helpers for the VMLoader
    public int[] LoadPayLoad(string FileName)
    {
        byte[] file = System.IO.File.ReadAllBytes(FileName);
        int[] PayLoad = ConvertToInt(file);
        return PayLoad;

    }

    public int[] ConvertToInt(byte[] Source)
    {
        int length = Source.Length;
        int[] Converted = new int[length];
        for (int i = 0; i < length; i++)
        {
            Converted[i] = Source[i];
        }
        return Converted;
    }

}
