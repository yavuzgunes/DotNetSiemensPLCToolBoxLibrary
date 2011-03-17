/*
 This implements a "glue" layer between libnodave.dll and applications written
 in MS .Net languages.
 
 Part of Libnodave, a free communication libray for Siemens S7 200/300/400 via
 the MPI adapter 6ES7 972-0CA22-0XAC
 or  MPI adapter 6ES7 972-0CA23-0XAC
 or  TS adapter 6ES7 972-0CA33-0XAC
 or  MPI adapter 6ES7 972-0CA11-0XAC,
 IBH/MHJ-NetLink or CPs 243, 343 and 443
 or VIPA Speed7 with builtin ethernet support.
  
 (C) Thomas Hergenhahn (thomas.hergenhahn@web.de) 2002..2005

 Libnodave is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 Libnodave is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary
{
    public class libnodave {
/*
    This struct contains whatever your Operating System uses to hold an in and outgoing 
    connection to external devices.
*/
        public struct daveOSserialType {
            public int rfd;
            public int wfd;
        }
/*
    Protocol types to be used with new daveInterface:
*/
        public static readonly int daveProtoMPI = 0;	/* MPI for S7 300/400 */    
        public static readonly int daveProtoMPI2 = 1;	/* MPI for S7 300/400, "Andrew's version" */
        public static readonly int daveProtoMPI3 = 2;	/* MPI for S7 300/400, Step 7 Version, experimental */
        public static readonly int daveProtoMPI4 = 3;	/* MPI for S7 300/400, "Andrew's version" with STX */
        public static readonly int daveProtoPPI = 10;	/* PPI for S7 200 */
    
        public static readonly int daveProtoAS511 = 20;	/* S5 via programming interface */
        public static readonly int daveProtoS7online = 50;	/* use s7onlinx.dll for transport */

        public static readonly int daveProtoISOTCP = 122;	/* ISO over TCP */
        public static readonly int daveProtoISOTCP243 = 123;	/* ISO over TCP with CP243 */

        public static readonly int daveProtoMPI_IBH = 223;	/* MPI with IBH NetLink MPI to ethernet gateway */
        public static readonly int daveProtoPPI_IBH = 224;	/* PPI with IBH NetLink PPI to ethernet gateway */

        public static readonly int daveProtoUserTransport = 255;	/* Libnodave will pass the PDUs of */
        /* S7 Communication to user defined */
        /* call back functions. */
/*
 *    ProfiBus speed constants. This is the baudrate on MPI network, NOT between adapter and PC:
*/
        public static readonly int daveSpeed9k = 0;
        public static readonly int daveSpeed19k =   1;
        public static readonly int daveSpeed187k =   2;
        public static readonly int daveSpeed500k =  3;
        public static readonly int daveSpeed1500k =  4;
        public static readonly int daveSpeed45k =    5;
        public static readonly int daveSpeed93k =   6;
    
/*
    Some function codes (yet unused ones may be incorrect).
*/
        public static readonly int daveFuncOpenS7Connection	= 0xF0;
        public static readonly int daveFuncRead		= 0x04;
        public static readonly int daveFuncWrite		= 0x05;
        public static readonly int daveFuncRequestDownload	= 0x1A;
        public static readonly int daveFuncDownloadBlock	= 0x1B;
        public static readonly int daveFuncDownloadEnded	= 0x1C;
        public static readonly int daveFuncStartUpload	= 0x1D;
        public static readonly int daveFuncUpload		= 0x1E;
        public static readonly int daveFuncEndUpload	= 0x1F;
        public static readonly int daveFuncInsertBlock	= 0x28;
/*
    S7 specific constants:
*/
        public static readonly int daveBlockType_OB  = '8';
        public static readonly int daveBlockType_DB  = 'A';
        public static readonly int daveBlockType_SDB = 'B';
        public static readonly int daveBlockType_FC  = 'C';
        public static readonly int daveBlockType_SFC = 'D';
        public static readonly int daveBlockType_FB  = 'E';
        public static readonly int daveBlockType_SFB = 'F';
/*
    Use these constants for parameter "area" in daveReadBytes and daveWriteBytes
*/    
        public static readonly int daveSysInfo = 0x3;	/* System info of 200 family */
        public static readonly int daveSysFlags =  0x5;	/* System flags of 200 family */
        public static readonly int daveAnaIn =  0x6;	/* analog inputs of 200 family */
        public static readonly int daveAnaOut =  0x7;	/* analog outputs of 200 family */
        public static readonly int daveP = 0x80;    	/* direct peripheral access */
        public static readonly int daveInputs = 0x81;   
        public static readonly int daveOutputs = 0x82;    
        public static readonly int daveFlags = 0x83;
        public static readonly int daveDB = 0x84;		/* data blocks */
        public static readonly int daveDI = 0x85;	/* instance data blocks */
        public static readonly int daveLocal = 0x86; 	/* not tested */
        public static readonly int daveV = 0x87;	/* don't know what it is */
        public static readonly int daveCounter = 28;	/* S7 counters */
        public static readonly int daveTimer = 29;	/* S7 timers */
        public static readonly int daveCounter200 = 30;	/* IEC counters (200 family) */
        public static readonly int daveTimer200 = 31;	/* IEC timers (200 family) */
/**
    Library specific:
**/
/*
    Result codes. Genarally, 0 means ok, 
    >0 are results (also errors) reported by the PLC
    <0 means error reported by library code.
*/
        public static readonly int daveResOK = 0;			/* means all ok */
        public static readonly int daveResNoPeripheralAtAddress = 1;	/* CPU tells there is no peripheral at address */
        public static readonly int daveResMultipleBitsNotSupported = 6; /* CPU tells it does not support to read a bit block with a */
        /* length other than 1 bit. */
        public static readonly int daveResItemNotAvailable200 = 3;	/* means a a piece of data is not available in the CPU, e.g. */
        /* when trying to read a non existing DB or bit bloc of length<>1 */
        /* This code seems to be specific to 200 family. */
					    
        public static readonly int daveResItemNotAvailable = 10;	/* means a a piece of data is not available in the CPU, e.g. */
        /* when trying to read a non existing DB */

        public static readonly int daveAddressOutOfRange = 5;		/* means the data address is beyond the CPUs address range */
        public static readonly int daveWriteDataSizeMismatch = 7;	/* means the write data size doesn't fit item size */
        public static readonly int daveResCannotEvaluatePDU = -123;     /* PDU is not understood by libnodave */
        public static readonly int daveResCPUNoData = -124; 
        public static readonly int daveUnknownError = -125; 
        public static readonly int daveEmptyResultError = -126;
        public static readonly int daveEmptyResultSetError = -127;
        public static readonly int daveResUnexpectedFunc = -128;
        public static readonly int daveResUnknownDataUnitSize = -129;

        public static readonly int daveResShortPacket = -1024;
        public static readonly int daveResTimeout = -1025;
/*
    Error code to message string conversion:
    Call this function to get an explanation for error codes returned by other functions.
*/    
/*
    [DllImport("libnodave.dll")]
    public static extern string 
    daveStrerror(int res);
*/

#if !IPHONE	
        [DllImport("libnodave.dll", EntryPoint="daveStrerror" )]
#else
	[DllImport ("__Internal", EntryPoint="daveStrerror")]
#endif
        public static extern IntPtr
            _daveStrerror(int res);
        public static string daveStrerror(int res) {
            return Marshal.PtrToStringAnsi(_daveStrerror(res));
        }

        //Get's an Error for the Errors from S7 Online!
        public static string daveStrS7onlineError()
        {
            int err = SCP_get_errno();
            switch (err)
            {
                case 202:
                    return "S7Online: Ressourcenengpa� im Treiber oder in der Library";
                case 203:
                    return "S7Online: Konfigurationsfehler";
                case 205:
                    return "S7Online: Auftrag zur Zeit nicht erlaubt";
                case 206:
                    return "S7Online: Parameterfehler";
                case 207:
                    return "S7Online: Ger�t bereits/noch nicht ge�ffnet.";
                case 208:
                    return "S7Online: CP reagiert nicht";
                case 209:
                    return "S7Online: Fehler in der Firmware";
                case 210:
                    return "S7Online: Speicherengpa� im Treiber";
                case 215:
                    return "S7Online: Keine Nachricht vorhanden";
                case 216:
                    return "S7Online: Fehler bei Zugriff auf Anwendungspuffer";
                case 219:
                    return "S7Online: Timeout abgelaufen";
                case 225:
                    return "S7Online: Die maximale Anzahl an Anmeldungen ist �berschritten";
                case 226:
                    return "S7Online: Der Auftrag wurde abgebrochen";
                case 233:
                    return "S7Online: Ein Hilfsprogramm konnte nicht gestartet werden";
                case 234:
                    return "S7Online: Keine Autorisierung f�r diese Funktion vorhanden";
                case 304:
                    return "S7Online: Initialisierung noch nicht abgeschlossen";
                case 305:
                    return "S7Online: Funktion nicht implementiert";
                case 4865:
                    return "S7Online: CP-Name nicht vorhanden";
                case 4866:
                    return "S7Online: CP-Name nicht konfiguriert";
                case 4867:
                    return "S7Online: Kanalname nicht vorhanden";
                case 4868:
                    return "S7Online: Kanalname nicht konfiguriert";
            }
            if (err != 0)
                return "Fehler nicht definiert, Code: " + err.ToString();
            else
                return "Kein Fehler";
        }

    
/*
    Copy an internal String into an external string buffer. This is needed to interface
    with Visual Basic. Maybe it is helpful elsewhere, too.
    C# can well work with C strings.
*/
//EXPORTSPEC void DECL2 daveStringCopy(char * intString, char * extString);
    
/* 
    Max number of bytes in a single message. 
*/
        public static readonly int daveMaxRawLen = 2048;

/*
    Some definitions for debugging:
*/
        public static readonly int daveDebugRawRead = 0x01;	/* Show the single bytes received */
        public static readonly int daveDebugSpecialChars = 0x02;	/* Show when special chars are read */
        public static readonly int daveDebugRawWrite = 0x04;	/* Show the single bytes written */
        public static readonly int daveDebugListReachables = 0x08;	/* Show the steps when determine devices in MPI net */
        public static readonly int daveDebugInitAdapter = 0x10;	/* Show the steps when Initilizing the MPI adapter */
        public static readonly int daveDebugConnect = 0x20;	/* Show the steps when connecting a PLC */
        public static readonly int daveDebugPacket = 0x40;
        public static readonly int daveDebugByte = 0x80;
        public static readonly int daveDebugCompare = 0x100;
        public static readonly int daveDebugExchange = 0x200;
        public static readonly int daveDebugPDU = 0x400;	/* debug PDU handling */
        public static readonly int daveDebugUpload = 0x800;	/* debug PDU loading program blocks from PLC */
        public static readonly int daveDebugMPI = 0x1000;
        public static readonly int daveDebugPrintErrors = 0x2000;	/* Print error messages */
        public static readonly int daveDebugPassive = 0x4000;

        public static readonly int daveDebugErrorReporting = 0x8000;
        public static readonly int daveDebugOpen = 0x10000;

        public static readonly int daveDebugAll = 0x1ffff;
/*
    set and read debug level:
*/
	
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern void daveSetDebug(int newDebugLevel);
    
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern int daveGetDebug();

        public static int  daveMPIReachable = 0x30;
        public static int  daveMPIunused = 0x10;
        public static int  davePartnerListSize = 126;
    
/*
    This wrapper class is used to avoid dealing with "unsafe" pointers to libnodave
    internal structures. More wrapper classes are derived from this for the different 
    structures. Constructors of derived classes will call functions in libnodave that 
    allocate internal structures via malloc. The functions used return integers by 
    declaration. These integers are stored in "pointer" In fact, these integers contain 
    the "bit patterns" of the pointers. The compiler is deceived about the real nature of 
    the return values. This is ok as long as the pointers are only used in libnodave, 
    because libnodave routines are assumed to know what they may do with them.
    The destructor here passes the pointers back to libnodave's daveFree to release memory
    when the C# object is destructed.
*/    
        public class pseudoPointer {
            public IntPtr pointer;
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveFree(IntPtr p);
	
            ~pseudoPointer(){
//	    Console.WriteLine("~pseudoPointer()"+pointer);
                daveFree(pointer);
            }
	
        }

        public class daveInterface: pseudoPointer  {
	
//	[DllImport("libnodave.dll"//, PreserveSig=false)]
/*
	I cannot say why, but when I recompiled the existing code with latest libnodave.dll
	(after using stdcall so that VC++ producs these "decorated names", I got a runtime
	error about not finding daveNewInterface. When I state full name entry point explicitly,
	(like below) it runs. The most strange thing is that all other functions work well...
*/
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif	
            static extern IntPtr daveNewInterface(
                daveOSserialType fd,
                string name,
                int localMPI,
                int  useProto,
                int speed
                );
            public daveInterface(daveOSserialType fd,
                                 string name,
                                 int localMPI,
                                 int  useProto,
                                 int speed) {
                pointer=daveNewInterface(fd, name, localMPI, useProto, speed);
                                 }
	
/*
This was just here to check inheritance	
	~daveInterface(){
	    Console.WriteLine("destructor("+daveGetName(pointer)+")");
	    Console.WriteLine("~daveInterface()"+pointer);
	    daveFree(pointer);
	}
*/	
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveInitAdapter(IntPtr di);
            public int initAdapter() {
                return daveInitAdapter(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected  static extern int daveListReachablePartners(IntPtr di, byte[] buffer);
            public int listReachablePartners(byte[] buffer) {
                return daveListReachablePartners(pointer,buffer);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern void daveSetTimeout(IntPtr di, int time);
            public void setTimeout(int time) {
                daveSetTimeout(pointer, time);
            }    

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetTimeout(IntPtr di);
            public int getTimeout() {
                return daveGetTimeout(pointer);
            }    
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern IntPtr daveDisconnectAdapter(IntPtr di);
            public IntPtr disconnectAdapter() {
                return daveDisconnectAdapter(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern string daveGetName(IntPtr di);
            public string getName() {
                return daveGetName(pointer);
            }
	
        }
    
        public class daveConnection:pseudoPointer {
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern IntPtr daveNewConnection(
                IntPtr di,
                int MPI,
                int rack,
                int slot
                );
       
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern IntPtr daveNewExtendedConnection(
                IntPtr di,
                byte[] destination,
                int DestinationIsIP,
                int rack,
                int slot,
                int routing,
                int routingSubnetFirst,
                int routingSubnetSecond,
                int routingRack,
                int routingSlot,
                byte[] routingDestination,
                int routingDestinationIsIP,
                int ConnectionType,
                int routingConnectionType
                );

            public daveConnection(
                daveInterface di,
                int MPI,
                int rack,
                int slot
                ) {
                pointer=daveNewConnection(
                    di.pointer, MPI, rack, slot
                    );
                }
            public daveConnection(
                daveInterface di,
                int MPI,
                string IP,
                bool DestinationIsIP,
                int rack,
                int slot,
                bool routing,
                int routingSubnetFirst,
                int routingSubnetSecond,
                int routingRack,
                int routingSlot,
                string routingDestination,
                int PLCConnectionType,
                int routingPLCConnectionType
                )
            {
                string[] ip = IP.Split('.');
                byte[] myDestination;
                int myDestinationIsIP = 0;
                if (ip.Length < 4 || !DestinationIsIP)
                {
                    myDestination = new byte[] { (byte)Convert.ToInt32(MPI) };
                }
                else
                {
                    myDestinationIsIP = 1;
                    myDestination = new byte[]
                                        {
                                            (byte) Convert.ToInt32(ip[0]), (byte) Convert.ToInt32(ip[1]),
                                            (byte) Convert.ToInt32(ip[2]),
                                            (byte) Convert.ToInt32(ip[3])
                                        };
                }
                ip = routingDestination.Split('.');
                byte[] myRoutingDestination;
                int routingDestinationIsIP = 0;
                if (ip.Length < 4)
                {
                    myRoutingDestination = new byte[] { (byte)Convert.ToInt32(routingDestination) };
                }
                else
                {
                    routingDestinationIsIP = 1;
                    myRoutingDestination = new byte[]
                                               {
                                                   (byte) Convert.ToInt32(ip[0]), (byte) Convert.ToInt32(ip[1]),
                                                   (byte) Convert.ToInt32(ip[2]),
                                                   (byte) Convert.ToInt32(ip[3])
                                               };
                }
                pointer = daveNewExtendedConnection(
                    di.pointer, myDestination, myDestinationIsIP, rack, slot, Convert.ToInt32(routing), routingSubnetFirst, routingSubnetSecond,
                    routingRack, routingSlot,
                    myRoutingDestination, routingDestinationIsIP, PLCConnectionType, routingPLCConnectionType);

            }

/* This wa here to test inheritance
	~daveConnection(){
	    Console.WriteLine("~daveConnection()"+pointer);
	    daveFree(pointer);
	    daveFree(pointer);
	}
*/
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveConnectPLC(IntPtr dc);
            public int connectPLC(){
                return daveConnectPLC(pointer);
            }
    
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveDisconnectPLC(IntPtr dc);
            public int disconnectPLC() {
                return daveDisconnectPLC(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveReadBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int readBytes(int area, int DBnumber, int start, int len, byte[] buffer) {
                return daveReadBytes(pointer, area, DBnumber, start, len, buffer);
            }

    
            //[DllImport("libnodave.dll"/*, PreserveSig=false */ )]
            //    protected static extern int daveReadManyBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            //public int readManyBytes(int area, int DBnumber, int start, int len, byte[] buffer) {
            //    return daveReadManyBytes(pointer, area, DBnumber, start, len, buffer);
            //}

            //Inserted a .NET implementation of readManyBytes, because the libNoDave one did not work!
            public int readManyBytes(int area, int DBnumber, int start, int len, ref byte[] buffer)
            {
                int res, readLen;
                int pos = 0;        

                while (len > 0)
                {
                    if (len > getMaxPDULen() - 18) readLen = getMaxPDULen() - 18; else readLen = len;

                    byte[] tmp = new byte[readLen];

                    res = daveReadBytes(pointer, area, DBnumber, start + pos, readLen, tmp);
                    if (res != 0) return res;

                    tmp.CopyTo(buffer, pos);

                    len -= readLen;
                    pos += readLen;
                }

                return 0;
            }    

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveReadBits(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int readBits(int area, int DBnumber, int start, int len, byte[] buffer) {
                return daveReadBits(pointer, area, DBnumber, start, len, buffer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveWriteBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int writeBytes(int area, int DBnumber, int start, int len, byte[] buffer) {
                return daveWriteBytes(pointer, area, DBnumber, start, len, buffer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveWriteManyBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int writeManyBytes(int area, int DBnumber, int start, int len, byte[] buffer) {
                return daveWriteManyBytes(pointer, area, DBnumber, start, len, buffer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveWriteBits(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
            public int writeBits(int area, int DBnumber, int start, int len, byte[] buffer) {
                return daveWriteBits(pointer, area, DBnumber, start, len, buffer);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveBuildAndSendPDU(IntPtr dc, IntPtr p, byte[] b1, int l1, byte[] b2, int l2);
            public int daveBuildAndSendPDU(PDU myPDU, byte[] Parameter, byte[] Data)
            {        
                int res=daveBuildAndSendPDU(pointer, myPDU.pointer, Parameter, Parameter.Length, Data, Data.Length);
                //return p;
                return res;
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetS32(IntPtr dc);
            public int getS32() {
                return daveGetS32(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetU32(IntPtr dc);
            public int getU32() {
                return daveGetU32(pointer);
            }	
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetS16(IntPtr dc);
            public int getS16() {
                return daveGetS16(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetU16(IntPtr dc);
            public int getU16() {
                return daveGetU16(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetS8(IntPtr dc);
            public int getS8() {
                return daveGetS8(pointer);
            }
    
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetU8(IntPtr dc);
            public int getU8() {
                return daveGetU8(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern float daveGetFloat(IntPtr dc);
            public float getFloat() {
                return daveGetFloat(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetCounterValue(IntPtr dc);
            public int getCounterValue() {
                return daveGetCounterValue(pointer);
            }
    
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern float daveGetSeconds(IntPtr dc);
            public float getSeconds() {
                return daveGetSeconds(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetS32At(IntPtr dc,int pos);
            public int getS32At(int pos) {
                return daveGetS32At(pointer, pos);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetU32At(IntPtr dc, int pos);
            public int getU32At(int pos) {
                return daveGetU32At(pointer, pos);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetS16At(IntPtr dc, int pos);
            public int getS16At(int pos) {
                return daveGetS16At(pointer, pos);
            }
    	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetU16At(IntPtr dc, int pos);
            public int getU16At(int pos) {
                return daveGetU16At(pointer, pos);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetS8At(IntPtr dc, int pos);
            public int getS8At(int pos) {
                return daveGetS8At(pointer, pos);
            }
    	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetU8At(IntPtr dc, int pos);
            public int getU8At(int pos) {
                return daveGetU8At(pointer, pos);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern float daveGetFloatAt(IntPtr dc, int pos);
            public float getFloatAt(int pos) {
                return daveGetFloatAt(pointer, pos);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetCounterValueAt(IntPtr dc, int pos);
            public int getCounterValueAt(int pos) {
                return daveGetCounterValueAt(pointer, pos);
            }
    
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern float daveGetSecondsAt(IntPtr dc, int pos);
            public float getSecondsAt(int pos) {
                return daveGetSecondsAt(pointer, pos);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetAnswLen(IntPtr dc);
            public int getAnswLen() {
                return daveGetAnswLen(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetMaxPDULen(IntPtr dc);
            public int getMaxPDULen() {
                return daveGetMaxPDULen(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int davePrepareReadRequest(IntPtr dc, IntPtr p);
            public PDU prepareReadRequest() {
                PDU p=new PDU();
                davePrepareReadRequest(pointer, p.pointer);
                return p;
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int davePrepareWriteRequest(IntPtr dc, IntPtr p);
            public PDU prepareWriteRequest() {
                PDU p=new PDU();
                davePrepareWriteRequest(pointer, p.pointer);
                return p;
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveExecReadRequest(IntPtr dc, IntPtr p, IntPtr rl);
            public int execReadRequest(PDU p, resultSet rl) {
                return daveExecReadRequest(pointer, p.pointer, rl.pointer);
            }
    
    
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveExecWriteRequest(IntPtr dc, IntPtr p, IntPtr rl);
            public int execWriteRequest(PDU p, resultSet rl) {
                return daveExecWriteRequest(pointer, p.pointer, rl.pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveUseResult(IntPtr dc, IntPtr rs, int number, byte[] buffer);
            public int useResult(resultSet rs, int number, byte[] buffer)
            {
                return daveUseResult(pointer, rs.pointer, number, buffer);
            }

	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveReadSZL(IntPtr dc,int id,int index,byte[] buffer, int len);
            public int readSZL(int id, int index, byte[] buffer)
            {
                return daveReadSZL(pointer, id, index, buffer, buffer.Length);
            }	
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveStart(IntPtr dc);
            public int start() {
                return daveStart(pointer);
            }
    
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveStop(IntPtr dc);
            public int stop() {
                return daveStop(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveForce200(IntPtr dc, int area, int start, int val);
            public int force200(int area, int start, int val) {
                return daveForce200(pointer, area, start, val);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveForceDisconnectIBH(IntPtr dc, int src, int dest, int MPI);
            public int forceDisconnectIBH(int src, int dest, int MPI) {
                return daveForceDisconnectIBH(pointer, src, dest, MPI);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveResetIBH(IntPtr dc);
            public int resetIBH() {
                return daveResetIBH(pointer);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetResponse(IntPtr dc);
            public int getGetResponse() {
                return daveGetResponse(pointer);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveSendMessage(IntPtr dc, IntPtr p);
            public int getMessage(PDU p) {
                return daveSendMessage(pointer, p.pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetProgramBlock(IntPtr dc, int blockType, int number, byte[] buffer, ref int length);
            public int getProgramBlock(int blockType, int number, byte[] buffer, ref int length) {
                int a=daveGetProgramBlock(pointer, blockType, number, buffer, ref length);
                return a;
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int davePutProgramBlock(IntPtr dc, int blockType, int number, byte[] buffer, ref int length);
            public int putProgramBlock(int blockType, int number, byte[] buffer, ref int length)
            {        
                int a = davePutProgramBlock(pointer, blockType, number, buffer, ref length);     
                return a;
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveDeleteProgramBlock(IntPtr dc, int blockType, int number);
            public int deleteProgramBlock(int blockType, int number)
            {
                int a = daveDeleteProgramBlock(pointer, blockType, number);
                return a;
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveListBlocksOfType(IntPtr dc, int blockType, byte[] buffer);
            public int ListBlocksOfType(int blockType, byte[] buffer) {
                return daveListBlocksOfType(pointer, blockType, buffer);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveSetPLCTime(IntPtr dc, byte[] buffer);
            public int daveSetPLCTime(DateTime tm)
            {
                byte[] buffer = new byte[] {0x00, 0x19, 0x05, 0x08, 0x23, 0x04, 0x10, 0x23, 0x67, 0x83,};
                putBCD8at(buffer, 2, tm.Year%100);
                putBCD8at(buffer, 3, tm.Month);
                putBCD8at(buffer, 4, tm.Day);
                putBCD8at(buffer, 5, tm.Hour);
                putBCD8at(buffer, 6, tm.Minute);
                putBCD8at(buffer, 7, tm.Second);
                putBCD8at(buffer, 8, tm.Millisecond/100);
                putBCD8at(buffer, 9, (tm.Millisecond%100) << 4);
      
                return daveSetPLCTime(pointer, buffer);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveReadPLCTime(IntPtr dc);
            public DateTime daveReadPLCTime()
            {

                int res = daveReadPLCTime(pointer);
                int year, month, day, hour, minute, second, millisecond;
                getU8();
                getU8();
                byte[] tmp = new byte[1];
                tmp[0] = Convert.ToByte(getU8());
                year = getBCD8from(tmp, 0);
                year += year >= 90 ? 1900 : 2000;
                tmp[0] = Convert.ToByte(getU8());
                month = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                day = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                hour = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                minute = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());
                second = getBCD8from(tmp, 0);
                tmp[0] = Convert.ToByte(getU8());      
                millisecond = getBCD8from(tmp, 0)*10;
                tmp[0] = Convert.ToByte(getU8());
                tmp[0] = Convert.ToByte(tmp[0] >> 4);
                millisecond += getBCD8from(tmp, 0);
                DateTime ret = new DateTime(year, month, day, hour, minute, second, millisecond);
        
                return ret;
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetPDUData(IntPtr dc, IntPtr p, byte[] data, ref int ldata, byte[] param, ref int lparam);
            public int daveGetPDUData(PDU myPDU, out byte[] data, out byte[] param)
            {
                byte[] tmp1=new byte[65536];
                byte[] tmp2=new byte[65536];
                int ltmp1 = 0;
                int ltmp2 = 0;
                int res = daveGetPDUData(pointer, myPDU.pointer, tmp1, ref ltmp1, tmp2, ref ltmp2);
                data = new byte[ltmp1];
                param = new byte[ltmp2];
                Array.Copy(tmp1, data, ltmp1);
                Array.Copy(tmp2, param, ltmp2);
                return res;
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            private static extern int _daveSetupReceivedPDU(IntPtr dc, IntPtr p);
            public int daveRecieveData(out byte[] data, out byte[] param)
            {
                int res = daveGetResponse(pointer);
                PDU myPDU = new PDU();
                _daveSetupReceivedPDU(pointer, myPDU.pointer);
                byte[] tmp1 = new byte[65536];
                byte[] tmp2 = new byte[65536];
                int ltmp1 = 0;
                int ltmp2 = 0;
                res = daveGetPDUData(pointer, myPDU.pointer, tmp1, ref ltmp1, tmp2, ref ltmp2);
                data = new byte[ltmp1];
                param = new byte[ltmp2];
                Array.Copy(tmp1, data, ltmp1);
                Array.Copy(tmp2, param, ltmp2);
                return res;
            }

        }

        public class PDU : pseudoPointer
        {
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
		[DllImport ("__Internal")]
#endif
            protected static extern IntPtr daveNewPDU();

            public PDU()
            {
                pointer = daveNewPDU();
            }

            /*	~PDU(){
                Console.WriteLine("~PDU()");
                daveFree(pointer);
            }
        */
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
		[DllImport ("__Internal")]
#endif
            protected static extern void daveAddVarToReadRequest(IntPtr p, int area, int DBnum, int start, int bytes);
            public void addVarToReadRequest(int area, int DBnum, int start, int bytes)
            {
                daveAddVarToReadRequest(pointer, area, DBnum, start, bytes);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
		[DllImport ("__Internal")]
#endif
            protected static extern void daveAddBitVarToReadRequest(IntPtr p, int area, int DBnum, int start, int bytes);
            public void addBitVarToReadRequest(int area, int DBnum, int start, int bytes)
            {
                daveAddBitVarToReadRequest(pointer, area, DBnum, start, bytes);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
		[DllImport ("__Internal")]
#endif
            protected static extern void daveAddVarToWriteRequest(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);
            public void addVarToWriteRequest(int area, int DBnum, int start, int bytes, byte[] buffer)
            {
                daveAddVarToWriteRequest(pointer, area, DBnum, start, bytes, buffer);
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
		[DllImport ("__Internal")]
#endif
            protected static extern void daveAddBitVarToWriteRequest(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);
            public void addBitVarToWriteRequest(int area, int DBnum, int start, int bytes, byte[] buffer)
            {
                daveAddBitVarToWriteRequest(pointer, area, DBnum, start, bytes, buffer);
            }    
        } // class PDU

        public class resultSet:pseudoPointer {
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern IntPtr daveNewResultSet();
            public resultSet() {
                pointer=daveNewResultSet();
            }

#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern void daveFreeResults(IntPtr rs);
            ~resultSet(){
//	    Console.WriteLine("~resultSet(1)");
                daveFreeResults(pointer);
//	    Console.WriteLine("~resultSet(2)");
//	    daveFree(pointer);
            }
	
#if !IPHONE	
            [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
            protected static extern int daveGetErrorOfResult(IntPtr rs, int number);
            public int getErrorOfResult(int number) {
                return daveGetErrorOfResult(pointer, number);
            }	
    
        }

#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern int setPort(
            [MarshalAs(UnmanagedType.LPStr)] string portName,
            [MarshalAs(UnmanagedType.LPStr)] string baud,
            int parity
            );
    
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern int openSocket(
            int port,
            [MarshalAs(UnmanagedType.LPStr)] string portName
            );
        
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        public static extern int openS7online(
            [MarshalAs(UnmanagedType.LPStr)] string portName,
            int hwnd
            );
#else
	public static int openS7online(string portName, int hwnd   ) {return 0; }
#endif
    
#if !IPHONE
        [DllImport("S7onlinx.dll" /*, PreserveSig=false */ )]
        private static extern int SCP_get_errno();
#else
	private static int SCP_get_errno() { return 0; }
#endif
    

#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern int closePort(
            int port
            );
    
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern int closeSocket(
            int port
            );

#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        public static extern int closeS7online(
            int port
            );
#else
	public static int closeS7online(int port   ) {return 0; }
#endif

        
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern float toPLCfloat(float f);
    
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern int daveToPLCfloat(float f);
    
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern int daveSwapIed_32(int i);
    
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        public static extern int daveSwapIed_16(int i);


        public static byte getU8from(byte[] b, int pos)
        {
            return Convert.ToByte(b[pos]);
        }

        public static sbyte getS8from(byte[] b, int pos)
        {

            if (b[pos] > 127)
                return Convert.ToSByte((256 - b[pos])*-1);
            else
                return Convert.ToSByte(b[pos]);
        }

        public static short getS16from(byte[] b, int pos) {
            if (BitConverter.IsLittleEndian) {
                byte[] b1=new byte[2];
                b1[1]=b[pos+0];
                b1[0]=b[pos+1];
                return BitConverter.ToInt16(b1, 0);
            }    
            else 
                return BitConverter.ToInt16(b, pos);
        }

        public static void putS16at(byte[] b, int pos, short value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 1] = bytes[0];
                b[pos] = bytes[1];
            }
            else
                Array.Copy(bytes, 0, b, pos, 2);
        }
    
        public static ushort getU16from(byte[] b, int pos) {
            if (BitConverter.IsLittleEndian) {
                byte[] b1=new byte[2];
                b1[1]=b[pos+0];
                b1[0]=b[pos+1];
                return BitConverter.ToUInt16(b1, 0);
            }    
            else 
                return BitConverter.ToUInt16(b, pos);
        }

        public static void putU16at(byte[] b, int pos, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 1] = bytes[0];
                b[pos] = bytes[1];
            }
            else
                Array.Copy(bytes, 0, b, pos, 2);
        }
        
        public static int getS32from(byte[] b, int pos) {
            if (BitConverter.IsLittleEndian) {
                byte[] b1=new byte[4];
                b1[3]=b[pos];
                b1[2]=b[pos+1];
                b1[1]=b[pos+2];
                b1[0]=b[pos+3];
                return BitConverter.ToInt32(b1, 0);
            }    
            else 
                return BitConverter.ToInt32(b, pos);
        }

        public static void putS32at(byte[] b, int pos, int value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[3];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }
    
        public static uint getU32from(byte[] b, int pos) {
            if (BitConverter.IsLittleEndian) {
                byte[] b1=new byte[4];
                b1[3]=b[pos];
                b1[2]=b[pos+1];
                b1[1]=b[pos+2];
                b1[0]=b[pos+3];
                return BitConverter.ToUInt32(b1, 0);
            }    
            else 
                return BitConverter.ToUInt32(b, pos);
        }

        public static void putU32at(byte[] b, int pos, uint value)
        {
            byte[] bytes = BitConverter.GetBytes((value));
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[3];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }

        public static void putDateTimeat(byte[] b, int pos, DateTime mydatetime)
        {
            int tmp;
        
            tmp = mydatetime.Year/100;
            tmp = tmp*100;
            tmp = mydatetime.Year - tmp;
            b[pos] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Month;
            b[pos + 1] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Day;
            b[pos + 2] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Hour;
            b[pos + 3] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Minute;
            b[pos + 4] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Second;
            b[pos + 5] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = mydatetime.Millisecond;
            b[pos + 6] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = (int) mydatetime.DayOfWeek;
            b[pos + 7] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);
        }

        public static void putS5Timeat(byte[] b, int pos, TimeSpan value)
        {
            byte basis;
            int wert;
            if (value.TotalMilliseconds <= 999*10)
            {
                basis = 0;
                wert = Convert.ToInt32(value.TotalMilliseconds)/10;
            }
            else if (value.TotalMilliseconds <= 999*100)
            {
                basis = 1;
                wert = Convert.ToInt32(value.TotalMilliseconds)/100;
            }
            else if (value.TotalMilliseconds <= 999*1000)
            {
                basis = 2;
                wert = Convert.ToInt32(value.TotalMilliseconds)/1000;
            }
            else if (value.TotalMilliseconds <= 999*10000)
            {
                basis = 3;
                wert = Convert.ToInt32(value.TotalMilliseconds)/10000;
            }
            else
            {
                basis = 3;
                wert = 999;
            }

            int p1, p2, p3;

            p3 = (wert/100);
            p2 = ((wert - p3*100)/10);
            p1 = (wert - p3*100 - p2*10);

            b[pos] = Convert.ToByte(basis << 4 | p3);
            b[pos + 1] = Convert.ToByte((p2 << 4 | p1));
        }

        public static void putTimeat(byte[] b, int pos, TimeSpan value)
        {
            putU32at(b, pos, Convert.ToUInt32(value.TotalMilliseconds));
        }

        public static void putTimeOfDayat(byte[] b, int pos, DateTime value)
        {
            var tmp = new TimeSpan(0, value.Hour, value.Minute, value.Second, value.Millisecond);
            putU32at(b, pos, Convert.ToUInt32(tmp.Milliseconds));
        }

        public static void putDateat(byte[] b, int pos, DateTime value)
        {
            DateTime tmp = new DateTime(1990, 1, 1);
            var tmp2 = value.Subtract(tmp);
            putU16at(b, pos, Convert.ToUInt16(tmp2.Days));
        }

        public static DateTime getDatefrom(byte[] b, int pos)
        {
            DateTime tmp = new DateTime(1990, 1, 1);
            var tmp2 = TimeSpan.FromDays(getU16from(b, pos));
            tmp = tmp.Add(tmp2);
            return tmp;
        }

        public static float getFloatfrom(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToSingle(b1, 0);
            }
            else
                return BitConverter.ToSingle(b, pos);
        }

        /// <summary>
        /// This put's a String as a S7 String to the PLC
        /// </summary>
        /// <param name="b"></param>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        public static void putS7Stringat(byte[] b, int pos, string value, int length)
        {
            b[pos] = (byte) length;
            b[pos + 1] = length > value.Length ? (byte) value.Length : (byte) length;
            Array.Copy(Encoding.ASCII.GetBytes(value), 0, b, pos + 2, value.Length > length ? length : value.Length);
        }

        /// <summary>
        /// This put's a String as a Char-Array to the PLC
        /// </summary>
        /// <param name="b"></param>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        public static void putStringat(byte[] b, int pos, string value, int length)
        {
            Array.Copy(Encoding.ASCII.GetBytes(value), 0, b, pos, value.Length > length ? length : value.Length);
        }

        public static void putFloatat(byte[] b, int pos, Single value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                b[pos + 3] = bytes[0];
                b[pos + 2] = bytes[1];
                b[pos + 1] = bytes[2];
                b[pos] = bytes[3];
            }
            else
                Array.Copy(bytes, 0, b, pos, 4);
        }

        public static int getBCD8from(byte[] b, int pos)
        {
            int bt1 = b[pos];
            bt1 = (bt1 / 0x10) * 10 + (bt1 % 0x10);
            return bt1;
        }

        public static void putBCD8at(byte[] b, int pos, int value)
        {
            int b0 = 0, b1 = 0;
            string chars = Convert.ToString(value);
            if (chars.Length > 1)
            {
                b0 = Convert.ToInt32(chars[1].ToString());
                b1 = Convert.ToInt32(chars[0].ToString());
            }
            else if (chars.Length > 0)
                b0 = Convert.ToInt32(chars[0].ToString());

            b[pos] = (byte) (b0 + b1*16);                   
        }

        public static void putBCD16at(byte[] b, int pos, int value)
        {
            int b0 = 0, b1 = 0, b2 = 0, b3 = 0;
            string chars = Convert.ToString(value);
            if (chars.Length > 3)
            {
                b0 = Convert.ToInt32(chars[3].ToString());
                b1 = Convert.ToInt32(chars[2].ToString());
                b2 = Convert.ToInt32(chars[1].ToString());
                b3 = Convert.ToInt32(chars[0].ToString());
            }
            else if (chars.Length > 2)
            {
                b0 = Convert.ToInt32(chars[2].ToString());
                b1 = Convert.ToInt32(chars[1].ToString());
                b2 = Convert.ToInt32(chars[0].ToString());
            }
            else if (chars.Length > 1)
            {
                b0 = Convert.ToInt32(chars[1].ToString());
                b1 = Convert.ToInt32(chars[0].ToString());
            }
            else if (chars.Length > 0)
                b0 = Convert.ToInt32(chars[0].ToString());

            b[pos] = (byte) (b2 + b3*16);
            b[pos + 1] = (byte) (b0 + b1*16);
        }

        public static int getBCD16from(byte[] b, int pos)
        {
            int bt1 = b[pos];
            int bt2 = b[pos + 1];
            bt1 = (bt1/0x10)*10 + (bt1 %0x10);
            bt2 = (bt2 / 0x10)*10 + (bt2 % 0x10);
            return bt1*100 + bt2;
        }

        public static DateTime getDateTimefrom(byte[] b, int pos)
        {
            int jahr, monat, tag, stunde, minute, sekunde, mili;
            int bt = b[pos];
            //BCD Umwandlung
            bt = (((bt >> 4)) * 10) + ((bt & 0x0f));
            if (bt < 90)
                jahr = 2000;
            else
                jahr = 1900;
            jahr += bt;

            //Monat
            bt = b[pos + 1];
            monat = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Tag
            bt = b[pos + 2];
            tag = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Stunde
            bt = b[pos + 3];
            stunde = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Minute
            bt = b[pos + 4];
            minute = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Sekunde
            bt = b[pos + 5];
            sekunde = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Milisekunden (werden noch nicht verarbeitet...)
            bt = b[pos + 6];
            mili = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //Wochentag
            bt = b[pos + 7];
            try
            {
                return new DateTime(jahr, monat, tag, stunde, minute, sekunde, mili);
            }
            catch(Exception ex)
            {
                return new DateTime(1900, 01, 01, 00, 00, 00);
            }
        }

        public static DateTime getTimeOfDayfrom(byte[] b, int pos)
        {
            long msval = getU32from(b, pos);
            return new DateTime(msval * 10000);
        }

        public static TimeSpan getTimefrom(byte[] b, int pos)
        {
            long msval = getS32from(b, pos);
            return new TimeSpan(msval * 10000);
        }

        public static TimeSpan getS5Timefrom(byte[] b, int pos)
        {
            int w1 = getBCD8from(b, pos+1);
            int w2 = ((b[pos] & 0x0f));

            long zahl = w2*100 + w1;

            int basis = (b[pos] >> 4) & 0x03;

            switch (basis)
            {
                case 0:
                    zahl = zahl * 100000;
                    break;
                case 1:
                    zahl = zahl * 1000000;
                    break;
                case 2:
                    zahl = zahl * 10000000;
                    break;
                case 3:
                    zahl = zahl * 100000000;
                    break;
                
            }
            return new TimeSpan(zahl);
        }

        public static bool getBit(int Byte, int Bit)
        {
            int wrt = System.Convert.ToInt32(System.Math.Pow(2, Bit));
            return ((Byte & wrt) > 0);
        }

        public static string dec2bin(byte Bytewert)
        {
            byte[] bitwert = { 128, 64, 32, 16, 8, 4, 2, 1 };
            byte[] bits = new byte[8];

            string bitstring = string.Empty; for (int Counter = 0; Counter < 8; Counter++)
            {
                if (Bytewert >= bitwert[Counter])
                {
                    bits[Counter] = 1; Bytewert -= bitwert[Counter];
                }
                bitstring += Convert.ToString(bits[Counter]);
            }
            return bitstring;
        }
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        private static extern int daveAreaName(int area);
    
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        private static extern int daveBlockName(int blockType);
#if !IPHONE	
        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
#else
	[DllImport ("__Internal")]
#endif
        private static extern void daveStringCopy(int i, byte[] c);
    
        public static string blockName(int blockType) {
            byte[] s=new byte[255];
            int i=daveBlockName(blockType);
            daveStringCopy(i, s);
            string st="";
            i=0;
            while (s[i]!=0) {
                st=st+(char)s[i];
                i++;
            }
            return st;
        }
    
        public static string areaName(int blockType) {
            byte[] s=new byte[255];
            int i=daveAreaName(blockType);
            daveStringCopy(i, s);
            string st="";
            i=0;
            while (s[i]!=0) {
                st=st+(char)s[i];
                i++;
            }
            return st;
        }
    
    }
}