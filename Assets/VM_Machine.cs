using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VM_Machine
{
    public int PC;
    public int SP;
    public int[] ram;
    public int[] registers;
    public List<int> callstack = new List<int>();
    


    //Initialize the Vm upon construction
    public VM_Machine(int ram_amount, int reg_amount, int entrypoint, int[] payload, int ram_size)
    {
        ram = new int[ram_amount];
        registers = new int[reg_amount];
        PC = entrypoint;
        ram = new int[ram_size];
        for (int i = 0; i < payload.Length; i++)
        {
            Debug.Log("Length is" + i);
            ram[entrypoint + i] = payload[i];
        }
        //set the stack pointer to I guess nothing. for now.
        SP = callstack.Count;
    }

    public void Step()
    {
        int OpcodeByte = FetchByte(PC);
        Debug.Log("Register 0 is " + registers[0]);
        switch (OpcodeByte) {
            case 0:
                NOP();
                break;
            case 1:
                LDREG_VALUE();
                break;
            case 2:
                LdRegister_Register();
                break;
            case 3:
                LdRegister_RegisterPTR();
                break;
            case 4:
                STVALUE_REGISTER();
                break;
            case 5:
                StValue_RegisterPTR();
                break;
            case 6:
                ADDREGISTER_REGISTER();
                break;
            case 7:
                AddRegister_Value();
                break;
            case 8:
                AddRegister_Value();
                break;
            case 9:
                SubRegister_Register();
                break;
            case 10:
                SUBREGISTER_VALUE();
                break;
            case 11:
                SubRegister_RegisterPTR();
                break;
            case 12:
                ST8VALUE_REGISTER();
                break;
            case 13:
                ST8VALUE_REGISTERPTR();
                break;
            case 14:
                ST16VALUE_REGISTER();
                break;
            case 15:
                St16Value_RegisterPTR();
                break;
            case 16:
                Ld8_Register_AddressPTR();
                break;
            case 17:
                Ld8Register_Register();
                break;
            case 18:
                LD16REGISTER_ADDRESSPTR();
                break;
            case 19:
                Ld16Register_Register();
                break;
            case 20:
                CmpRegister_Register();
                break;
            case 21:
                CmpRegister_RegisterPTR();
                break;
            case 22:
                CmpRegister_Value();
                break;
            case 23:
                Cmp16Register_RegisterPTR();
                break;
            case 24:
                Cmp8Register_RegisterPTR();
                break;
            case 25:
                Beq_Address();
                break;
            case 26:
                Beq_RegisterPTR();
                break;
            case 27:
                Bneq_AddressPTR();
                break;
            case 28:
                Bneq_Address();
                break;
            case 29:
                Sub_Address();
                break;
            case 30:
                Sub_RegisterPTR();
                break;
            case 31:
                Ret();
                break;
            case 32:
                JMP_RegisterPTR();
                break;
            case 33:
                Jmp_Address();
                break;
            case 34:
                Or_Register_Value();
                break;
            case 35:
                Or_Register_RegisterPTR();
                break;
            case 36:
                And_Register_Value();
                break;
            case 37:
                And_Register_RegisterPTR();
                break;
            case 38:
                XOR_Register_Value();
                break;
            case 39:
                Xor_Register_RegisterPTR();
                break;
            case 40:
                StRegister_RegisterPTR();
                break;

                
        }
       

    }

 // Use this for initialization
    void Start()
    {
       

    }

    // Update is called once per frame
    void Update()
    {
      
    }


    //ALL THE OPCODES GO HERE

    //0x00
    public void NOP()
    {
        return;
    }

    //0x01
    public void LDREG_VALUE()
    {
        int[] bytes = new int[5];
        //Get register parameter
        int REGNUM = FetchByte(PC);
        //Get next 4 bytes
        Get4Bytes(bytes);
        //Now the storing part
        int num = ReadNum(bytes, 3);
        //Saves the value to target register
        StoreValueToReg(num, REGNUM);

    }

    //0x02
    public void LdRegister_Register()
    {
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        registers[Reg1] = registers[Reg2];
    }

    //0X03
    public void LdRegister_RegisterPTR()
    {
        //GET TWO REGISTERS FROM OPCODE
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);

        //LOAD FROM RAM INTO REGISTER1
        int POINTER = registers[Reg2];
        int VALUE = GetMemValue(POINTER);
        registers[Reg1] = VALUE;
    }


    //0X04
    public void STVALUE_REGISTER() {
        int[] bytes = new int[5];
        //GET 4 BYTES FROM OPCODE
        bytes[0] = FetchByte(PC);
        bytes[1] = FetchByte(PC);
        bytes[2] = FetchByte(PC);
        bytes[3] = FetchByte(PC);

        //GET REGISTER NUMBER
        int Regnum = FetchByte(PC);
        int num = ReadNum(bytes, 3);
        //SAVES THE VALUE TO TARGET REGISTER
        StoreValueToReg(num, Regnum);
        Debug.Log("Storing Value" + num + " in Register # " + Regnum);
    }

    //0X05
    public void StValue_RegisterPTR() {
        int[] bytes = new int[5];
        //GET VALUE BYTES FROM OPCODE
        Get4Bytes(bytes);
        //GET THE REGISTER
        int Regnum = FetchByte(PC);
        int Num = ReadNum(bytes, 3);
        //CALL MEMORY WRITE FUNCTION
        Debug.Log(Regnum);
        Debug.Log("Storing Value" + Num + " in Ram Address " + registers[Regnum] + " pointed to by Register #" + Regnum ) ;
        StoreValueToMem(Num, registers[Regnum], ram);
    }

    //0X06
    public void ADDREGISTER_REGISTER() {
        //GET TWO REGISTER BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //ADD THE REFERENCED REGISTERS INTO REG1
        registers[Reg1] = registers[Reg1] + registers[Reg2];
    }

    //0X07 ADDS A VALUE TO A REGISTER
    public void AddRegister_Value() {
        //MAKE BYTE ARRAY FOR 32BIT INT
        int[] bytes = new int[5];
        //GET THE REGISTER REF
        int Reg = FetchByte(PC);
        //GRAB THE VALUE
        int Value = FetchByte(Reg);
        Get4Bytes(bytes);
        //CONVERT BYTES TO 32BIT INT
        int Num = ReadNum(bytes, 3);
        //ADD TWO VALUES
        registers[Reg] = Value + Num;
        //DONE
    }


    //0X09
    public void SubRegister_Register()
    {
        //GET TWO REGISTER BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //SET THE VALUES FOR SUBTRACTION
        //THEN RE ASSIGN RESULT TO REG1
        int Value1 = registers[Reg1];
        int Value2 = registers[Reg2];
        registers[Reg1] = Value1 - Value2;
    }

    //0X0A
    public void SUBREGISTER_VALUE() {
        //CREATE BYTES ARRAY
        int[] bytes = new int[5];
        //GET REGISTER FROM OPCODE
        int Reg = FetchByte(PC);
        //GET 4 BYTES FROM ROM
         Get4Bytes(bytes);
        //CONVERT TO 32 BIT INT
        int num = ReadNum(bytes, 3);
        //ASSIGN THE TEMP VALUES
        int Value1 = registers[Reg];
        int Value2 = num;
        //PERFORM SUBTRACTION AMD RE ASSIGNMEMT
        registers[Reg] = Value1 - Value2;
    }

    //0X0B
    public void SubRegister_RegisterPTR() {
        //GET TWO REGISTER BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        int Pointer = registers[Reg1];
        //GET THE VALUE REG2 POINTS TO
        int Value = GetMemValue(Pointer);
        //SETUP TEMP VALUES
        int Value1 = registers[Reg1];
        int Value2 = Value;
        //HANDLE SUBTRACTION AND RE ASSIGN REG1
        registers[Reg1] = Value1 - Value2;
    }

    //0X0C
    public void ST8VALUE_REGISTER() {
        //GET THE VALUE BYTE
        int Num = FetchByte(PC);
        //GET REGISTER REFERENCE
        int Reg = FetchByte(PC);
        //SIGN THE VALUE
        Num = Num << 24 >> 24;
        //STORE IT
        registers[Reg] = Num;
    }

    //0X0D
    public void ST8VALUE_REGISTERPTR() {
        //STORE THE VALUE IN A MEMORY ADDRESS
        int Num = FetchByte(PC);
        //GET REGISTER BYTE
        int Reg = FetchByte(PC);
        //WE HAVE TO STILL SIGN THE VALUE--OOPS
        //ALMOST FORGOT HEHEã€œ
        int Value8Bit = Convert8BitByteToSigned(Num);
        //STORE IN RAM ADDR REG POINTS TO
        ram[registers[Reg]] = Value8Bit;
        //DONE
    }

    //0X0E
    public void ST16VALUE_REGISTER() {
        //STORE THE 16BIT VALUE IN A REGISTER
        //FIRST GET TWO VALUE BYTES
        int ByteHigh = FetchByte(PC);
        int ByteLow = FetchByte(PC);
        //NOW THEVREGISTER BYTE
        int Reg = FetchByte(PC);
        //CONVERT TO SIGNED
        int Num = Convert16BitBytesToSigned(ByteHigh, ByteLow);
        //NOW ASSIGN
        registers[Reg] = Num;
        //BOOM

    }

    //0X0F
    public void St16Value_RegisterPTR() {
        //SO THIS IS POOR OPTIMIZING
        //IT NEEDS TO SIGN IR FIRST
        //THEN UNSIGN IT IN THE WRITETOMEM
        //FUNCTION DUE TO WRITING 2 BYTES TO RAM
        //SEQUENTIALLY TO READ BACK PROPERLY
        //STORE THE 16BIT VALUE IN A REGISTER
        //FIRST GET TWO VALUE BYTES
        int ByteHigh = FetchByte(PC);
        int ByteLow = FetchByte(PC);
        //NOW THE REGISTER BYTE
        int Reg = FetchByte(PC);
        //CONVERT TO SIGNED
        int Num = Convert16BitBytesToSigned(ByteHigh, ByteLow);
        //NOW STORE TO RAM
        Write16BitValueToRam(Num, registers[Reg], ram);
    }

    //0X10
    public void Ld8_Register_AddressPTR() {
        //SO THIS OPCODE GETS THE REGISTER
        //BYTE THEN 4 VALUE BYES
        //THEN IT CONVERTS THOSE 4 BYTES
        //TO A 32 BIT INT POINTER
        //AFTER THIS IT DOES
        //AN EXCLUSIVE 8 BIT SIGNED READ
        //FROM A SINGLE BYTE IN RAM AT POINTER
        //THEN STORES THE SIGNED 8BIT RESULT
        //INTO THE REGISTER

        //MAKE THE BYTES ARRAY
        int[] bytes = new int[5]; //JUST BEING SAFE
        //GET THE REGISTER BYTE NEXT
        int Reg = FetchByte(PC);
        //GET 4 BYTE ADDRESS
        Get4Bytes(bytes);
        //GET THE VALUE OF THE MEMORY POINTER
        int Pointer = ReadNum(bytes, 3);
        //READ THE 8 BIT VALUE FROM RAM
        int Value = Read8BitvalueFromRam(Pointer, ram);
        //IT AUTO-SIGN EXTENDS FOR US =P

        //NEXT WE NEED TO STORE THE VALUE
        //INTO THE REGISTER
        registers[Reg] = Value;
        //DONE WHEW THAT WAS ALMOST CONFUSING!
    }


    //0X11
    public void Ld8Register_Register() {
        //LOAD A VALUE AS 8BIT FROM R2 INTO R1

        //1)FETCH 2 PC BYTES
        //2) AND THE VALUE
        //3) CAST BACK TO SIGNED
        //4)STORE NEW VALUE IN REG1
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //GET THE REG2 VALUE
        int ValueRaw = registers[Reg2];
        //DO SOME TWOS COMPLIMENT MAGIC =P
        int Value_Changed = ValueRaw & 255;
        int Value8 = SignExtend8Bit(Value_Changed);
        //*APPLAUD* *CONFETIIã€œãƒ¼ãƒ¼ãƒ¼*
        registers[Reg1] = Value8;
        //TADA!
        //AND THATS WHY HELPER FUNCTIONS R GUD =O
    }

    //0X12
    public void LD16REGISTER_ADDRESSPTR()
    {
        //LOAD A 16 BIT VALUE FROM RAM
        //THAT ADDRESS POINTS TO
        //INTO REGISTER
        //============================='
        //CREATE THE 4 BYTE ARRAY
        int[] bytes = new int[5]; //JUST BEING SAFER

        //FETCH REGISTER BYTE
        int Reg = FetchByte(PC);
        //NEXT GET 4 BYTES FOR POINTER ADDRESS
        Get4Bytes(bytes);
        //NEXT WE NEED  TO CONVERT THAT
        //TO 32BIT VALUE
        int Pointer = ReadNum(bytes, 3);
        //NEXT GET THE SIGNED 16BIT VALUE FROM RAM
        int Num = Read16BitValueFromRam(Pointer, ram);
        //NEXT ASSIGN TO REG
        registers[Reg] = Num;
        //EASY *RAISES ARMS* WOOO!! <3
    }

    //0X13
    public void Ld16Register_Register() {
        //LOAD REG2 INTO REG1 FORCED AS 16BIT
        //1) GET 2 REG BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //2) GET VALUE OF REG2
        int RawValue = registers[Reg2];
        //3) DO SOME TWOS COMPLIMENT MAGIC =P
        int NewValue = RawValue & 65535;
        int Value16 = SignExtend16Bit(NewValue);
        //ASSIGN TO REG1 PARAMETER
        registers[Reg1] = Value16;
        //CONFETTI TIME ãƒ¼ãƒ»ã€‚î‰¿
    }

    //0X14
    public void CmpRegister_Register() {
        //THIS IS SIMPLER THAN THE REST
        //FIRST GET THE ONLY 2 OPCODE BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //NEXT DO A COMPARE
        bool CmpResult = (Reg1 == Reg2);
        if (CmpResult) {
            //IF TRUE SET THE LAST F BIT
            int Value = registers[5];
            Value = SetBit(Value, 0);
            registers[5] = Value;
        }

    }


    //0X15
    public void CmpRegister_RegisterPTR() {
        //FIRST GET TWO REGISTER BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //NOW GET THE POINTER FROM REG2
        int Pointer = registers[Reg2];
        //NOW READ FROM RAM
        int Value = GetMemValue(Pointer);

        //FINALLY COMPARE
        int Value1 = registers[Reg1];
        int Value2 = Value;
        bool CmpResult = (Value1 == Value2);
        //IF RESULT IS 1 THEN SET 1ST BIT OF REG5
        if (CmpResult) {
            int RegValue = registers[5];
            RegValue = SetBit(RegValue, 0);
            registers[5] = RegValue;
             
        }
        //WHEW SUPPORT FUNCTIONS MAKE IT SO EASY
    }

    //0X16
    public void CmpRegister_Value() {
        //THIS COMPARES AN IMMEDIATE 32BIT NUMBER
        //TO THE REGISTER
        //BIT 0 OF REG5 IS SET IF TRUE ALA (0->7)

        //FIRST MAKE A BYTE ARRAY
        int[] bytes = new int[5];
        //READ REGISTER BYTE
        int Reg = FetchByte(PC);
        //GET 4 BYTES NEXT
        Get4Bytes(bytes);
        //CONVERT TO VALUE
        int Num = ReadNum(bytes, 3);
        //GET TWO VALUES FOR COMPARE
        int Value1 = registers[Reg];
        int Value2 = Num;
        //FINALLY RUN COMPARISON
        bool CmpResult = (Value1 == Value2);
        //IF RESULT IS 1 THEN SET 1ST BIT OF REG5
        if (CmpResult) {
            int RegValue = registers[5];
            RegValue = SetBit(RegValue, 0);
            registers[5] = RegValue;
        }
        //DONE. BOOM!!
    }

    //0X17
    public void Cmp16Register_RegisterPTR() {
        //COMPARE THE 16BIT VALUE IN RAM
        //TO THE VALUE OF A REGISTER
        //FIRST GET THE TWO BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //NEXT GET THE POINTER
        int Pointer = registers[Reg2];
        //NEXT GET THE 16BIT VALUE FROM RAM
        int Num = Read16BitValueFromRam(Pointer, ram);
        //FINALLY GET VALUE 1 OF REG1
        int Value1 = registers[Reg1];
        int Value2 = Num;
        //AND WOW EASY LETS COMPARE!!
        bool CmpResult = (Value1 == Value2);
        //IF RESULT IS 1 THEN SET 1ST BIT OF REG5
        if (CmpResult) {
            int RegValue = registers[5];
            RegValue = SetBit(RegValue, 0);
            registers[5] = RegValue;
                }


    }

    //0X18
    public void Cmp8Register_RegisterPTR() {
        //COMPARE THE VALUE IN R1 WITH 8BIT VALUE
        //IN RAM THAT R2 POINTS TO
        //FIRST FETCH 2 BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //NEXT GET THE POINTER FROM REG2
        int Pointer = registers[Reg2]; //In the original source it said R2 instead of Reg2 and I need to fix that
                                       //NEXT GET THE TWO VALUES
        int Num = Read8BitvalueFromRam(Pointer, ram);
        int Value1 = registers[Reg1];
        int Value2 = Num;
        bool CmpResult = (Value1 == Value2);
        //IF RESULT IS 1 THEN SET 1ST BIT OF REG5
        if (CmpResult) {
            int RegValue = registers[5];
            RegValue = SetBit(RegValue, 0);
            registers[5] = RegValue;
        }
        // *FALLS OVER*
        //ETA FROM OPCODE #0X0C->0X18 IS LIKE 7 HRS
        //NEARLY DONE ONLY LIKE... 16 MORE YAYY X_X
        //TIME TO SLEEP.
    }

    //0X19
    public void Beq_Address() {
        //JUMPS TO ADDRESS IF LAST CMP WAS EQUAL
        //FIRST GET THE ADDRESS
        int[] bytes = new int[5]; //AGAIN 5 TO BE SAFE
        //GET ADDRESS BYTES FOR JUMP
        Get4Bytes(bytes);
        //CONVERT TO 32BIT INT
        int Pointer = ReadNum(bytes, 3);
        //CHECK REGISTER 5 BIT
        int R5 = registers[5];
        int CmpBit = GetBit(R5, 0); //IN the original source all instances of CHECKBIT should be changed to GETBIT 
                                    //My errors are astounding.
                                    //IF 1 CLEAR BIT AND SET PC TO POINTER

        if (CmpBit == 1) {
            //CLEAR BIT 0 BEFORE JUMP
            R5 = ClearBit(R5, 0);
            //OVERWRITE REGISTER
            registers[5] = R5;
            //PERFORM BRANCH
            PC = Pointer;
        //BOOM DONE
        
        }
    }

    //0X1A
    public void Beq_RegisterPTR()
    {
        //BRANCH TO ADDRESS OF REG IF EQUAL
        //STEP 1 GET A REGISTER BYTE
        int Reg = FetchByte(PC);
        //STEP TWO GET REG2 POINTER
        int Pointer = registers[Reg];
        //AND THE ADDRESS IN RAM THE REGISTER
        //POINTS TO
        int Address = GetMemValue(Pointer);

        //STEP 3 CHECK COMPARE FLAG
        int R5 = registers[5];
        int CmpBit = GetBit(R5, 0);

        if (CmpBit == 1)
        {
            //CLEAR BIT 0 BEFORE JUMP
            R5 = ClearBit(R5, 0);
            //OVERWRITE REGISTER
            registers[5] = R5;
            //PERFORM BRANCH
            PC = Address;
        }

    }

    //0X1B
    public void Bneq_AddressPTR() {
        //STEP 1 GET REGISTER BYTE
        int Reg = FetchByte(PC);
        //STEP 2 GET REG POINTER
        int Pointer = registers[Reg];
        //STEP 3 GET ADDRESS FROM RAM OF POINTER
        int Address = GetMemValue(Pointer);
        //STEP 4 CHECK COMPARE FLAG
        int R5 = registers[5];
        int CmpBit = GetBit(R5, 0);

        if (CmpBit == 0) {
            //PERFORM BRANCH
            PC = Address;
 
        //DONE
        }
    }

    //0X1C
    public void Bneq_Address() {
        //JUMPS TO ADDRESS IF LAST CMP WASNT EQUAL
        //FIRST GET THE ADDRESS
        int[] bytes = new int[5];
        //GET ADDRESS BYTES FOR JUMP
        Get4Bytes(bytes);
        //CONVERT TO 32BIT INT
        int Pointer = ReadNum(bytes, 3);
        //CHECK REGISTER 5 BIT
        int R5 = registers[5];
        int CmpBit = GetBit(R5, 0);
        //IF 0 SET PC TO POINTER

        if (CmpBit == 0) {
            //PERFORM BRANCH
            PC = Pointer;
        }
    }


    //0X1D
    //GOES TO THE SUBROUTINE OF THE ADDRESS
    //GIVEN
    public void Sub_Address() {
        //GET THE ADDRESS BYTES NEXT
        int[] bytes = new int[5]; //AGAIN 5 TO BE SAFE
        //GET ADDRESS BYTES FOR JUMP
        Get4Bytes(bytes);
        //CONVERT TO 32BIT INT
        int Pointer = ReadNum(bytes, 3);
        //PUT THE PC ONTO THE CALLSTACK
        callstack.Add(PC);
        SP++; //INCREMENT STACK POINTER
        //PERFORM THE JUMP
        PC = Pointer;
    }

    //0X1E
    public void Sub_RegisterPTR() {
        //GOES TO THE SUBROUTINE THE REGISTER
        //POINTS TO IN RAM
        //FIRST GET THE REGISTER
        int Reg = FetchByte(PC);
        //POINTER TIME
        int Pointer = registers[Reg];
        //GET ADDRESS
        int Address = GetMemValue(Pointer);
        //PUT THE PC ONTO THE CALLSTACK
        callstack.Add(PC);
        SP++;
        //PERFORM THE JUMP
        PC = Address;
    }

    //0X1F
        //POP A VALUE OFF THE STACK AND GO
        //TO THAT MEMORY ADDRESS
        public void Ret() {
        //DECREMENT THE STACK POINTER
        int Address = callstack[SP];
        callstack.Remove(SP);
        SP--;
        PC = Address;
        //DONE
    }

    //0X20
        public void JMP_RegisterPTR() {
        //JUMPS TO AN ADDRESS STORED AT REGISTER$
        //GET REGISTER BYTE
        int Reg = FetchByte(PC);
        //GET POINTER
        int Pointer = registers[Reg];
        //GET ADDRESS FROM RAM
        int Address = GetMemValue(Pointer);
        //PERFORM JUMP, CALLSTACK IS
        //UN-EFFECTED
        PC = Address;
        //DONE
    }

    //0X21
    public void Jmp_Address() {
        //PERFORM A JUMP TO AN ADDRESS
        int[] bytes = new int[5];
        //GET ADDRESS BYTES FOR JUMP
        Get4Bytes(bytes);
        //CONVERT TO 32BIT INT
        int Pointer = ReadNum(bytes, 3);
        //PERFORM THE JUMP WITH EXTREMESPEED
        //GOTTA GO FAST
        PC = Pointer;
        //DONE
    }

    //0X22
    public void Or_Register_Value() {
        //GET REGISTER BYTE
        int Reg = FetchByte(PC);
        //GET VALUE 1
        int Value1 = registers[Reg];

        int[] bytes = new int[5]; //AGAIN 5 TO BE SAFE
        //GET VALUE BYTES
        Get4Bytes(bytes);
        //CONVERT TO 32BIT INT
        int Num = ReadNum(bytes, 3);
        //SET VALUE 2
        int Value2 = Num;
        //RESULT
        int Rslt = (Value1 | Value2);
        //STORE IT BACK
        registers[Reg] = Rslt;
        //DONE!
    }

    //0X23
    public void Or_Register_RegisterPTR()
    {
        //PERFORMS OR ON REG1 AND VALUE
        //REG2 POINTS TO IN RAM. RESULT IS IN REG1
        //GET 2 REGISTER BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //GET REG2 POINTER
        int Pointer = registers[Reg2];
        //SET VALUE 1
        int Value1 = registers[Reg1];
        //GET VALUE 2
        int Value2 = GetMemValue(Pointer);
        //PERFORM OR
        int Rslt = (Value1 | Value2);
        //STORE BACK TO REG1
        registers[Reg1] = Rslt;
        //DONE
    }

    //0X24
    public void And_Register_Value() {
        //PERFORM AND ON REG AND VALUE
        //RESULT IN REG
        //GET REGISTER BYTE
        int Reg = FetchByte(PC);
        //GET VALUE1
        int Value1 = registers[Reg];
        //GET VALUE2
        int[] bytes = new int[5]; //AGAIN 5 TO BE SAFE
        //GET VALUE BYTES
        Get4Bytes(bytes);
        //CONVERT TO 32BIT INT
        int Value2 = ReadNum(bytes, 3);
        //RESULT TIME
        int Rslt = (Value1 & Value2);
        //SAVE RESULT
        registers[Reg] = Rslt;
        //DONE AGAIN
    }

    //0X25
    public void And_Register_RegisterPTR() {
        //PERFORM AND ON REG1 AND VALUE IN RAM
        //THAT REG2 POINTS TO
        //GET TWO REGISTER BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //GET REG1 VALUE
        int Value1 = registers[Reg1];
        //GET REG2 POINTER
        int Pointer = registers[Reg2];
        //GET VALUE2
        int Value2 = GetMemValue(Pointer);
        //DO AND
        int Rslt = (Value1 & Value2);
        //STORE BACK INTO REG1
        registers[Reg1] = Rslt;
        //NAILED IT!
    }

    //26
    public void XOR_Register_Value() {
        //PERFORM XOR ON REGISTER AND VALUE
        //STORE RESULT IN REGISTER
        //GET REGISTER BYTE
        int Reg = FetchByte(PC);
        //GET VALUE1
        int Value1 = registers[Reg];

        int[] bytes = new int[5]; //AGAIN 5 TO BE SAFE
        //GET VALUE BYTES
        Get4Bytes(bytes);
        //CONVERT TO 32BIT INT
        int Value2 = ReadNum(bytes, 3);
        //PERFORM XOR
        int Rslt = (Value1 ^ Value2);
        //STORE BACK INTO REG
        registers[Reg] = Rslt;
        //FINISH!!!
    }

    //0X27
    public void Xor_Register_RegisterPTR()
    {
        //PERFORM XOR ON REG1 AND VALUE IN RAM
        //THAT REG2 POINTS TO
        //GET TWO REGISTER BYTES
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //GET REG1 VALUE
        int Value1 = registers[Reg1];
        //GET REG2 POINTER
        int Pointer = registers[Reg2];
        //GET VALUE2
        int Value2 = GetMemValue(Pointer);
        //DO AND
        int Rslt = (Value1 ^ Value2);
        //STORE BACK INTO REG1
        registers[Reg1] = Rslt;
        //ALL OPCODES DONE NOW BY COINZ
        //MAYBE TIME TO BATONPASS TO 12ME21 :D
    }

    //0x28
    public void StRegister_RegisterPTR()
    {
        int[] bytes = new int[5];
        //GET VALUE BYTES FROM OPCODE
        int Reg1 = FetchByte(PC);
        int Reg2 = FetchByte(PC);
        //GET THE POINTER
        int Pointer = registers[Reg2]; 
        
        //CALL MEMORY WRITE FUNCTION
        StoreValueToMem(Reg1, registers[Reg2], ram);
    }


    public int FetchByte(int PC_)
    {
        int value = ram[PC_];
        PC = PC_ + 1;
        Debug.Log("PC is " + PC);
        Debug.Log("Fetched " + value);
        return value;
    }

    //'====HELPER FUNCTION===='
    public void StoreValueToReg(int Value, int Register)
    {
        registers[Register] = Value;
    }
    //'======================='

    //==STORE VALUE TO MEM==
    public void StoreValueToMem(int Value, int Pointer, int[] ram) {
        //CREATE THE BYTES ARRAY
        int[] bytes = new int[32]; //this is overflow I guess just to be safe? I can't remember.
        Debug.Log("Value is " + Value);
        get(Value, 8, 0, bytes);
        for (int i = 0; i < 3; i++) {
            ram[Pointer] = bytes[i];
            Pointer = Pointer + 1;

        }
    }

    public void Get4Bytes(int[] bytes)
    {
        //HELPER FUNCTION TO GET 4 BYTES FROM PC

        for (int i = 0; i < 3; i++)
            bytes[i] = FetchByte(PC);
    }
            
        
       
    
         //==HELPER FUNC GET VALUE OF 32BIT INT==
        public int ReadNum(int[] bytes, int length)
         {


            int Num = 0;
            for (int i = 0; i < length; i++)        {
                Num = Num | bytes[i] << 8 * i;
             }
            return Num;

        }
    
        //=====================================


//GETS A 32 BIT VALUE FROM MEMORY
public int GetMemValue(int POINTER) {
        //CREATE BYTES ARRAY
        int[] bytes = new int[5];
        //GET 4 BYTES GENERIC
        //THIS FUNCTION BELOW DIRECTLY POINTS TO RAM
        int POINT = GET4RAMBYTES(POINTER, bytes);
        //THIS SHOULD PUT THE BYTES TOGETHER
        int NUM = ReadNum(bytes, 3);
        int VALUE = NUM;

        return VALUE;
    }

    //HELPER FUNCTION GET4BYTES GENERIC
    public int GET4RAMBYTES(int Pointer, int[] array) {
        for (int i = 0; i < 3; i++)
        {
            int point = FetchRamByte(Pointer, array);
            Pointer = point;
        }
        return Pointer;

}

    //GET A BYTE FROM ANY RAM
    public int FetchRamByte(int pointer, int[] ar) {
    int byte_ = ar[pointer];
    pointer++;
    return pointer;
    }


    //THIS IS USED TO GET THE BYTES OF A VALUE
    public void get(int X, int SIZE, int NUM, int[] ARRAY) {
    for (int i=0; i<SIZE; i++)
        {
            ARRAY[i] = X >> SIZE * i & (1 << SIZE) - 1;
        }
        //ARRAY[I] = X >> SIZE * I AND(1 << SIZE) - 1
        //NEXT
    }


    //CONVERT A RAW UNSIGNED 8BIT BYTE TO SIGNED
    public int Convert8BitByteToSigned(int byte_) {
    int Value8Bit = byte_ << 24 >> 24;
        return Value8Bit;
    }

    //CONVERT TWO 16BIT BYTES TO SIGNED VALUE
    public int Convert16BitBytesToSigned(int ByteHigh, int ByteLow) {
        int Value16Bit = (ByteHigh << 8) + ByteLow;
        return Value16Bit;
    }

    //WRITE TWO BYTES OF A 16BIT VALUE TO RAM
    //AND STORE AS UNSIGNED
    public void Write16BitValueToRam(int Value, int Pointer, int[] Mem) {
        //MAKE THE TEMPARRAY FOR 2 BYTES
        int[] Bytes2 = new int[3];
        //CALL CONVERSION FUNCTIONS
        int Value16Bit = ConvertValue16Bit(Value);
        //CONVERT ALREADY ANDED VALUE TO BYTES
        CONVERT16BITVALUETOBYTES(Value16Bit, Bytes2);
        //STORE TO RAM
        Mem[Pointer] = Bytes2[0];
        Mem[Pointer + 1] = Bytes2[1];
    }


    //CONVERT VALUE TO 16BIT
    public int ConvertValue16Bit(int VALUE) {
        int Value16Bit = VALUE & 65535;
        return Value16Bit;
    }


    //CONVERT A 16BIT VALUE THATS ALREADY ANDED TO BYTES
    public void CONVERT16BITVALUETOBYTES(int Value, int[] Bytes) {
        int ByteHigh= (Value >> 8) & 255;
        int ByteLow = Value & 255;
        Bytes[0] = ByteHigh;
        Bytes[1] = ByteLow;
    }

    //READ ONE BYTE OF 8BITVALUE FROM RAM
    //AND CONVERT TO SIGNED
    public int Read8BitvalueFromRam(int Pointer, int[] Mem) {
        //READ UNSIGNED BYTE FROM RAM
        int UnsignedValue = Mem[Pointer];
        //SIGN THE VALU IN 8BITMODE
        int Num = Convert8BitByteToSigned(UnsignedValue); //WERE DONE. BOOM!
        return Num;
    }

    //SIGN EXTEND A 8 BIT VALUE
    public int SignExtend8Bit(int Value) {
        Value = Value << 24 >> 24;
        int Value8 = Value;
        return Value8;
    }

    //READ TWO BYTES OF A 16BIT VALUE FROM RAM
    //AND CONVERT TO SIGNED 16BIT VALUE
    public int Read16BitValueFromRam(int Pointer, int[] Mem) {
        int ByteHigh = Mem[Pointer];
        int ByteLow = Mem[Pointer + 1];
        //PERFORM SURGERY!
        int Num = Convert16BitBytesToSigned(ByteHigh, ByteLow);
        return Num;
        //WERE DONE
    }

    //SIGN EXTEND A 16 BIT VALUE
    public int SignExtend16Bit(int Value) {
        Value = Value << 16 >> 16;
        int Value16 = Value;
        return Value16;
    }


    //THIS WILL SELECT A BIT AND SET IT
    public int SetBit(int Value, int Bitnum) {
        int Rslt = Value | 1 << Bitnum;
        return Rslt;
    }

    //THIS WILL GET IF THE BIT IS SET OR NOT
    public int GetBit(int Value, int BitNum) {
        int Bit = (Value >> BitNum);
        Bit = Value & 1;
        int RSLT = Bit;
        return RSLT;

    }

    public int ClearBit(int Value, int Bitnum) {
        int Rslt = Value | 0 << Bitnum;
        return Rslt;
    }


}


