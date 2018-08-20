#define NETFX_CORE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if NETFX_CORE
//using Windows.Networking.Sockets;
//using Windows.Storage.Streams;
//using Windows.Networking;
//using Windows.Networking.Connectivity;
using System.Net;
using System.Net.Sockets;
using System.Text;
#else
using System.Net.Sockets;
#endif
using System.IO;
using System;

public class DataReceiver : MonoBehaviour {
	internal Boolean socketReady = false;
	#if NETFX_CORE
	Socket socket;
	private byte[] _recieveBuffer = new byte[8142];
	String receivedString = "";
	#else
	TcpClient mySocket;
	NetworkStream theStream;
	NetworkClient client = new NetworkClient();
	StreamWriter theWriter;
	StreamReader theReader;
	#endif
	String Host = "192.168.1.81";
	SocketAsyncEventArgs readWriteEventArg;
	Int32 Port = 9999;

	// Use this for initialization
	void Start () {
		setupSocket();
	}

	// Update is called once per frame
	void Update () {
		if (!socketReady)
			return;
		
		#if NETFX_CORE


		#endif
			
		//Debug.Log("update> " + client.isConnected);
		print(readSocket());
	}

	public void setupSocket() {
		try {
			/*
			//Network.Connect(Host, Port);
			Debug.Log ("Connecting...");

			//client.RegisterHandler(MsgType.Connect, OnConnected);
			//client.Connect(Host, Port);

			NetworkTransport.Init();
			ConnectionConfig config = new ConnectionConfig();
			int channelId = config.AddChannel(QosType.Reliable);
			HostTopology topology = new HostTopology(config, 10);
			int hostId = NetworkTransport.AddHost(topology, 9999, null);

			byte error;
			int connectionId = NetworkTransport.Connect(hostId, Host, 9999, 0, out error);

			Debug.Log ("Connecting done?");
			*/



			#if NETFX_CORE
			IPAddress ipAddress = IPAddress.Parse(Host);
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);

			// Create a TCP/IP  socket.  
			socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			//socket.Connect(remoteEP);
			//socket.ConnectAsync();
			Connect();
			//Debug.LogFormat("Socket connected to {0}", socket.RemoteEndPoint.ToString());
			#else
			mySocket = new TcpClient(Host, Port);
			theStream = mySocket.GetStream();
			theWriter = new StreamWriter(theStream);
			theReader = new StreamReader(theStream);
			#endif
			socketReady = true;
			//socket.BeginReceive(_recieveBuffer,0,_recieveBuffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),null);
		}
		catch (Exception e) {
			Debug.Log("Socket error: " + e);
		}
	}

	public void OnConnected(NetworkMessage netMsg)
	{
		Debug.Log ("Connected!");
	}
	/*
	public void writeSocket(string theLine) {
		if (!socketReady)
			return;
		String foo = theLine + "\r\n";
		theWriter.Write(foo);
		theWriter.Flush();
	}
	*/
	public String readSocket() {
		if (!socketReady)
		{
			return "Not ready";
		}
		#if NETFX_CORE
		if (receivedString.Length > 0)
			return receivedString;
		else
			return "";

		/*
		byte[] msg = new byte[0];
		// Receive the response from the remote device
		int bytesRec = socket.BeginReceive(msg);

		string recievedMessage = Encoding.ASCII.GetString(msg, 0, bytesRec);

		Debug.LogFormat("Got {1}, {2}, Echoed test = \n|{0}|", recievedMessage, bytesRec, socket.Connected);
		if (bytesRec > 0)
			return recievedMessage;
		*/
		#else
		if (theStream.DataAvailable)
			return theReader.ReadLine();
		#endif
		return "";
	}
	#if NETFX_CORE
	/*
	private void ReceiveCallback(IAsyncResult AR)
	{
		if (!socketReady)
			return;
		//Check how much bytes are recieved and call EndRecieve to finalize handshake
		int recieved = socket.EndReceive(AR);

		if(recieved <= 0)
			return;

		//Copy the recieved data into new buffer , to avoid null bytes
		byte[] recData = new byte[recieved];
		Buffer.BlockCopy(_recieveBuffer,0,recData,0,recieved);

		//Process data here the way you want , all your bytes will be stored in recData
		string recievedMessage = Encoding.ASCII.GetString(recData, 0, recieved);

		// Debug.LogFormat("Got {1}, {2}, Echoed test = \n|{0}|", recievedMessage, recieved, socket.Connected);
		if (recieved > 0)
			receivedString = recievedMessage;
		else
			receivedString = "";
		//Start receiving again
		socket.BeginReceive(_recieveBuffer,0,_recieveBuffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),null);
	}
	*/
	#endif

	public void closeSocket() {
		if (!socketReady)
			return;
		#if NETFX_CORE
		socket.Shutdown(SocketShutdown.Both);
		//socket.Close();
		#else
		theWriter.Close();
		theReader.Close();
		mySocket.Close();
		#endif
		socketReady = false;
	}

	void Stop()
	{
		closeSocket();
	}

	private void Connect()
	{
		//Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		SocketAsyncEventArgs e = new SocketAsyncEventArgs();
		IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(Host), Port);
		e.RemoteEndPoint = ipEnd;
		e.UserToken = socket;
		e.Completed += new EventHandler<SocketAsyncEventArgs>(e_Completed);
		Debug.Log("Trying to connect to : " + ipEnd);
		socket.ConnectAsync(e);
	}

	private void e_Completed(object sender, SocketAsyncEventArgs e)
	{
		Debug.Log("Donenenen");
		if (e != null)
		{
			//StreamReader sr = new StreamReader(new NetworkStream(e.ConnectSocket));
			Debug.Log("Connection Established : " + e.RemoteEndPoint + " PC NAME : " );
		}
		else
		{
			Debug.Log("Connection Failed : " + e.RemoteEndPoint);
		}
	}

}

