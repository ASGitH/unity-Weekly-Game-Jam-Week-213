using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mono.Nat; //i don't know why this dosn't work
using System.Net;
using System;
using System.Net.Sockets;
using System.Text;
using UDP;
public class Networking
{
    static INatDevice device;
    static byte[] data;
    static bool isServer = true;
    public static bool isReady = false;
    static UDPSocket socket;
    static UDPSocket serverSocket;
    public static GridContainer gc;

    public static IEnumerator Update()
    {
        //Debug.Log("Network init");
        //NatUtility.DeviceFound += DeviceFound;
        //NatUtility.DeviceLost += DeviceLost;
        //NatUtility.StartDiscovery();
        //
        //data = new byte[2048];
        //while (!portReady) yield return null;

        // determin if we are a server or a client some how
        
        if (isServer)
        {
            if (serverSocket == null)
            {
                serverSocket = new UDPSocket();
                serverSocket.Server("127.0.0.1", 2700);
            }
        }
        else
        {
            if (socket == null)
            {
                socket = new UDPSocket();
                socket.Client("127.0.0.1", 2700);
            }
        }
        while (true)
        {
            if (isServer)
            {
                serverSocket.SendServer(gc.destroyedGrid);
            } else
            {
                socket.Send(gc.destroyedGrid);
            }
            yield return null;
        }
    }
    private static void DeviceFound(object sender, DeviceEventArgs args)
    {
        device = args.Device;
        device.CreatePortMap(new Mapping(Protocol.Udp, 2700, 2700));
        Debug.Log(device.GetExternalIP().ToString());
        var mapings = device.GetAllMappings();
        foreach(var maping in mapings)
        {
            Debug.Log(maping);
        }
        // on device found code
    }
    private static void DeviceLost(object sender, DeviceEventArgs args)
    {
        Debug.Log("dead");
        //device = args.Device;
        //device.DeletePortMap(new Mapping(Protocol.Udp, 2700, 2700));
        // on device disconnect code
    }
}
