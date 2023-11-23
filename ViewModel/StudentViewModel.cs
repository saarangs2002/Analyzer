﻿/******************************************************************************
 * Filename    = StudentViewModel.cs
 *
 * Author      = Prayag Krishna
 *
 * Product     = Analyzer
 * 
 * Project     = ViewModel
 *
 * Description = Defines the Student viewmodel.
 *****************************************************************************/
using Networking.Communicator;
using Networking.Events;
using SessionState;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Networking.Utils;
using Networking.Models;

namespace ViewModel
{
    public class StudentViewModel : INotifyPropertyChanged , IEventHandler
    {
        private readonly ICommunicator _client; // Communicator used to send and receive messages.

        public StudentViewModel( string name , string id, ICommunicator? communicator = null)
        {
            _client = communicator ?? CommunicationFactory.GetClient();
            StudentName = name;
            StudentRoll = id;
            IpAddress = GetPrivateIp();

        }

        /// <summary>
        /// Gets the receive port.
        /// </summary>
        public string? ReceivePort { get; private set; }

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        public string? IpAddress { get; private set; }

        /// <summary>
        /// Gets the instructor's IP address.
        /// </summary>
        public string? InstructorIp { get; private set; }

        /// <summary>
        /// Gets the instructor's ip
        /// </summary>
        public string? InstructorPort { get; private set; }

        /// <summary>
        /// Gets the instructor port
        /// </summary>
        /// 

        private bool _isConnected = false;

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }

            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        public ICommunicator Communicator
        {
            get 
            {
                return _client;
            }
            
            private set
            {

            }
        }

        /// <summary>
        /// Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        public string StudentName { get; private set; }

        /// <summary>
        /// Gets the instructor's port.
        /// </summary>
        public string StudentRoll { get; private set; }

        /// <summary>
        /// Gets the private IP address of the host machine.
        /// </summary>
        /// <param name="property">The name of the property</param>
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private string? GetPrivateIp()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (address.ToString().Length >= 3 && address.ToString().Substring(0, 3) == "10.")
                    {
                        return address.ToString();
                    }
                }
            }
            return null;
        }

        private static string SerializeStudnetInfo(string? name, string? rollNo, string? ip, string? port, int connect)
        {
            return $"{rollNo}|{name}|{ip}|{port}|{connect}";
        }

        private static (int, string?, string?, int) DeserializeStudnetInfo(string data)
        {
            string[] parts = data.Split('|');
            if (parts.Length == 4)
            {
                try
                {
                    return
                    (
                        int.Parse(parts[0]),
                        parts[1],
                        parts[2],
                        int.Parse(parts[3])
                    );
                }
                catch { }

            }
            return (0, null, null, 0);
        }

        private void HandleMessage(string message)
        {
            if (message == "1")
            {
                IsConnected = true;
                Debug.WriteLine("Connected to Instructor");
            }
            else if (message == "0")
            {
                IsConnected = false;
                _client.Stop();
                Debug.WriteLine("Disconnected from Instructor");
            }
        }

        public void DisconnectInstructor()
        {
            string message = SerializeStudnetInfo(StudentName, StudentRoll, IpAddress, ReceivePort, 0);
            
            if (InstructorIp != null && InstructorPort != null)
            {
                _client.Send(message, "server");
            }
        }

        public bool ConnectInstructor()
        {
            if (InstructorIp != null && InstructorPort != null && StudentRoll!=null)
            {
                string ipPort = _client.Start( InstructorIp , int.Parse( InstructorPort ) , StudentRoll , "Dashboard" );
                if(ipPort == "failed")
                {
                    return false;
                }
                _client.Subscribe(this, "Dashboard");
                Debug.WriteLine(ipPort);
                string[] parts = ipPort.Split(':');
                try
                {
                    IpAddress = parts[0];
                    ReceivePort = parts[1];
                    OnPropertyChanged(nameof(IpAddress));
                    OnPropertyChanged(nameof(ReceivePort));

                    string message = SerializeStudnetInfo(StudentName, StudentRoll, IpAddress, ReceivePort, 1);
                    _client.Send(message, "server");
                    return true;
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Sets the instructor's IP address and port.
        /// </summary>
        /// <param name="ip">The instructor's IP address.</param>
        /// <param name="port">The instructor's port.</param>
        public void SetInstructorAddress(string ip, string port)
        {
            InstructorIp = ip;
            InstructorPort = port;
        }

        public void SetStudentInfo(string name, string roll)
        {
            StudentName = name;
            StudentRoll = roll;
        }

        public string HandleMessageRecv(Networking.Models.Message data)
        {
            HandleMessage(data.Data);
            return "";
        }
    }
}
