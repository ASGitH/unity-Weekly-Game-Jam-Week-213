using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
namespace UDP
{
    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        public static Dictionary<string, byte[]> packetQue = new Dictionary<string,byte[]>();
        public static Dictionary<string, EndPoint> clients = new Dictionary<string, EndPoint>();
        public static Dictionary<string, bool> hasRead = new Dictionary<string, bool>();

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }   

        public void Send(byte[] data)
        {
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                
            }, state);
        }
        public void SendServer(byte[] data)
        {
            
            for(int i = 0; i < clients.Count; i++)
            {
                _socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, clients.ElementAt(i).Value, (ar) =>
                {
                    State so = (State)ar.AsyncState;
                    int bytes = _socket.EndSend(ar);
                    
                }, state);
            }
        }


        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                
                if (epFrom != null && !clients.ContainsKey(((IPEndPoint)epFrom).Address.ToString()))
                {
                    try
                    {
                        clients.Add(((IPEndPoint)epFrom).Address.ToString(), epFrom);
                        packetQue.Add(((IPEndPoint)epFrom).Address.ToString(), so.buffer);
                        hasRead.Add(((IPEndPoint)epFrom).Address.ToString(), true);
                    }
                    catch
                    {

                    }
                }

                if (hasRead[((IPEndPoint)epFrom).Address.ToString()])
                {
                    if (so.buffer != null) packetQue[((IPEndPoint)epFrom).Address.ToString()] = so.buffer; //keeps track of each clients last packet
                    hasRead[((IPEndPoint)epFrom).Address.ToString()] = false;
                }
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
            }, state);
        }
    }
}