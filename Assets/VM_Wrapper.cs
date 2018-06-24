using UnityEngine;
using System.Collections;

public class VM_Wrapper : MonoBehaviour
{
    public VM_Machine myVM;

    // Use this for initialization
    void Start()
    {
        //string loadname = @"C:\Users\Public\Documents\Unity Projects\Texture_Write_Test3D\Assets\out.bin";
        //change loadname to the filepath of the .bin file that you want the virtual machine to run
        string loadname = @"";
        int[] ram = LoadPayLoad(loadname);
        int ramsize = ram.Length;
        myVM = new VM_Machine(2000, 5000, 0, ram, 2000);
    }

    // Update is called once per frame
    void Update()
    {
            myVM.Step();
            float x = myVM.registers[1];
            float y = myVM.ram[1];
            float z = myVM.ram[2];
        Vector3 pos = gameObject.transform.position;
        pos.x = x;
        gameObject.transform.position = pos;
        Debug.Log("X is " + x);
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
