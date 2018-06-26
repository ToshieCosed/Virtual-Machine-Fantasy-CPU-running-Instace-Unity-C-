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
        int ramsize = ram.Length + 500;
        myVM = new VM_Machine(2000, 5000, 0, ram, 400000);

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

    public int[] convert_to4bytes(int value)
    {
        int n = value;
        int[] bytes = new int[3];
        bytes[0] = (n >> 24) & 0xFF;
        bytes[1] = (n >> 16) & 0xFF;
        bytes[2] = (n >> 8) & 0xFF;
        bytes[3] = n & 0xFF;
        return bytes;
    }

    public void getcolors(int startoffset)
    {
        Color lastcolor = new Color();
        int colorindex = startoffset;
        for (int x = 0; x < 160; x++)
            for (int y = 0; y < 144; y++) {

                //trying to get the color here
                float redvalue = (byte) myVM.ram[startoffset];
                float greenvalue = (byte) myVM.ram[startoffset + 1];
                float bluevalue = (byte) myVM.ram[startoffset + 2];
                float alphavalue = 1.0f;
                colors[x, y] = new Color(redvalue, greenvalue, bluevalue, alphavalue);
                startoffset += 4;
                if (redvalue == 90) { lastcolor = colors[x, y]; }
                
            }

        for (int x=0; x<10; x++)
        {
            for (int y=0; y<10; y++)
            {
                colors[x, y] = lastcolor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int[] b = new int[10];
        b[0] = 255; //All red
        b[1] = 0; // No blue
        b[2] = 255; // all green!
        int value = myVM.ReadNum(b, 3);
        string add_string = "";
        loglist.Add("The byte 0 was " + b[0] + "  and the byte 1 was " + b[1] + " , and the byte 2 was " + b[2] + "  and the byte 3 was " + b[3] + " and the value was " + value);
        int seekingramvalue = myVM.ram[20000];
        if (seekingramvalue == 90) { loglist.Add("The value 90 was found in plain decimal at ram address " + 20000); }
        int seekingramvalue2 = myVM.ram[20001];
        byte newvalue = (byte)seekingramvalue2;
        //{ loglist.Add("The value " + seekingramvalue2 + "  was found in decimal at ram address" + 20001); }
       // { loglist.Add("The value " + newvalue + " was sign unextended from the above"); }
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
        //float green = (float) myVM.GetMemValue(104);
        //float blue = (float) myVM.GetMemValue(108);
        //Color M = new Color(value, 0, 90);
        //Color c = new Color(red, green, blue);

        //We will start copying the buffer at ram 20, 000 into the color array
        //This will fetch 3 byte sets out of 32 bit integers, ignoring the last byte of the 32 bit integer always (they are signed I do believe)
        //copybuffer(20000);

        //for (int xx =10; xx<15; xx++)
        //{
        // for (int yy =10; yy<15; yy++)
        {
            //colors[xx, yy] = c;
            //}
            //}
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
                        for (int ii = 0; ii < myVM.ram.Length - 1; ii++)
                        {
                            out_ram[ii] = (byte)myVM.ram[ii];
                        }


                        fs.Write(out_ram, 0, myVM.ram.Length - 1);
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
                    foreach (string i in loglist)
                    {
                        file.WriteLine(i);
                    }
                }


            }

        }
    }

    public void texturechange_stuff()
    {
        Texture2D texture = new Texture2D(160, 144);
        GetComponent<Renderer>().material.mainTexture = texture;
            //copy the colors from the vm ram buffer directly into the renderbuffer
        getcolors(20000);
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
