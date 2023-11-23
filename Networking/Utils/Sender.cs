﻿/******************************************************************************
 * Filename    = Utils/Sender.cs
 *
 * Author      = VM Sreeram
 *
 * Product     = Analyzer
 * 
 * Project     = Networking
 *
 * Description = The sending functionality of the communicators is implemented
 *               here.
 *****************************************************************************/

// TODO: Inherit this class in classes that use this and make the methods protected?

using System.Net.Sockets;
using Networking.Models;
using Networking.Queues;
using Networking.Serialization;

namespace Networking.Utils
{
    /// <summary>
    /// The class responsible for the sending functionality of the communicators
    /// </summary>
    public class Sender
    {
        private Queue _sendQueue = new();
        private Thread _sendThread;
        private bool _isClient;
        private Dictionary<string, NetworkStream> clientIDToStream;
        Dictionary<string, string> senderIDToClientID;
        private bool _stopThread;

        /// <summary>
        /// Constructor for the sender. Spawns thread that polls from <see cref="_sendQueue"/> and sends it
        /// </summary>
        /// <param name="clientIDToStream">The mapping of Client Id (internal) to Networksteam of the client</param>
        /// <param name="senderIDToClientID">The mapping of the Id of the communicator to the Client Id (internal)</param>
        /// <param name="isClient">Whether the communicator is Client</param>
        public Sender(Dictionary<string, NetworkStream> clientIDToStream, Dictionary<string, string> senderIDToClientID, bool isClient)
        {
            _stopThread = false;
            this.senderIDToClientID=senderIDToClientID;
            _isClient = isClient;
            Console.WriteLine("[Sender] Init");
            this.clientIDToStream = clientIDToStream;
            _sendThread = new Thread(SendLoop)
            {
                IsBackground = true
            };
            _sendThread.Start();
        }

        /// <summary>
        /// Stops the sender thread
        /// </summary>
        public void Stop()
        {

            Console.WriteLine("[Sender] Stop");
            _stopThread = true;
            _sendThread.Join();
        }

        /// <summary>
        /// Adds the <paramref name="message"/> to <see cref="_sendQueue"/> for it to be sent
        /// </summary>
        /// <param name="message">The message to be sent</param>
        public void Send(Message message)
        {
            // NOTE: destID should be in line with the dict passed 
            _sendQueue.Enqueue(message, Priority.GetPriority(message.ModuleName)/* TODO */);
        }
        private bool IfNetworkingMessage()
        {
            Message? message = _sendQueue.Peak();
            if (message == null)
            {
                return false;
            }
            else
            {
                if (message.ModuleName == ID.GetNetworkingID()||message.ModuleName==ID.GetNetworkingBroadcastID())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The function that polls from the <see cref="_sendQueue"/> and sends the message to appropriate destination
        /// </summary>
        public void SendLoop()
        {
            while (IfNetworkingMessage()||(!_stopThread)) 
            {
                if (!_sendQueue.canDequeue())
                {
                    // wait for some time
                    Thread.Sleep(500);
                    continue;
                }

                // Get the next message to send
                Message message = _sendQueue.Dequeue();

                string serStr = Serializer.Serialize(message);
                byte[] messagebytes = System.Text.Encoding.ASCII.GetBytes(serStr);
                int messageSize = messagebytes.Length;
                try
                {
                    if (_isClient == true)              // All messages from the client is sent to the Server. If the destination is not Server, the message will be sent to the right recipient from the Server
                    {
                        clientIDToStream[ID.GetServerID()].Write(BitConverter.GetBytes(messageSize), 0, sizeof(int));
                        clientIDToStream[ID.GetServerID()].Write(messagebytes);
                        clientIDToStream[ID.GetServerID()].Flush();
                    }
                    else
                    {
                        if (message.DestID == ID.GetBroadcastID())      // Broadcast the message to all clients
                        {
                            foreach (KeyValuePair<string, NetworkStream> pair in clientIDToStream)
                            {
                                pair.Value.Write(BitConverter.GetBytes(messageSize), 0, sizeof(int));
                                pair.Value.Write(messagebytes);
                                pair.Value.Flush();
                            }
                        }
                        else            // Send the message to the appropriate recipient
                        {
                            clientIDToStream[senderIDToClientID[message.DestID]].Write(BitConverter.GetBytes(messageSize), 0, sizeof(int));
                            clientIDToStream[senderIDToClientID[message.DestID]].Write(messagebytes);
                            clientIDToStream[senderIDToClientID[message.DestID]].Flush();
                        }
                    }
                } catch(Exception e) {
                    Console.WriteLine("Cannot send message to "+ message.SenderID);
                    Console.WriteLine(e.Message);    
                }

            }
        }
    }
}

