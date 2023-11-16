using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using Networking.Models;
using Networking.Queues;

namespace Networking.Utils
{
    public class Priority
    {
        //public static int GetPriority(string eventName)
        //{
        //    /*
        //    lets have 3 priority levels
        //    */
        //    if (eventName == EventType.ClientDeregister())
        //    {
        //        return 0;
        //    }
        //    if (eventName == EventType.NewClientJoined())
        //    {
        //        return 1;
        //    }
        //    if (eventName == EventType.ClientLeft())
        //    {
        //        return 1;
        //    }
        //    if (eventName == EventType.ClientRegister())
        //    {
        //        return 1;
        //    }
        //    if (eventName == EventType.AnalyseFile())
        //    {
        //        return 2;
        //    }
        //    if (eventName == EventType.AnalyserResult())
        //    {
        //        return 3;
        //    }
        //    if (eventName == EventType.ChatMessage())
        //    {
        //        return 3;
        //    }
        //    Trace.WriteLine("Event type:" + eventName + " doesn't exists");
        //    return 1000;
        //}
        public static int GetPriority(string moduleName)
        {
            var priority = 0;
            if (moduleName == ID.GetNetworkingID())
            {
                priority= 1;
            }
            else
            {
                priority = 2;
            }
            return priority;
        }
    }
}

