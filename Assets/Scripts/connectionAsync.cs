using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System;

public class connectionAsync : MonoBehaviour {
	// Constants for socket operations.
	private const Int32 ReceiveOperation = 1, SendOperation = 0;

	// The socket used to send/receive messages.
	private Socket clientSocket;

	// Flag for connected socket.
	private Boolean connected = false; 

	// Listener endpoint.
	private IPEndPoint hostEndPoint;

	String Host = "192.168.1.81";
	Int32 Port = 9999;
	bool socketReady = false;
	bool waiting = false;

	// Use this for initialization
	void Start () {
		setupSocket();
		Connect();
	}
	
	// Update is called once per frame
	void Update () {
		// print(readSocket());
		if (!waiting && connected)
		{
			waitForData();
		}
	}

	public void setupSocket() {
		try {
			// Get host related information.
			// IPHostEntry host = Dns.GetHostEntry(Host);
			IPAddress ipAddress = IPAddress.Parse(Host);

			// Instantiates the endpoint and socket.
			hostEndPoint = new IPEndPoint(ipAddress, Port);
			clientSocket = new Socket(hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socketReady = true;
		}
		catch (Exception e) {
			Debug.Log("Socket error: " + e);
		}
	}

	// Connect to the host.
	internal void Connect()
	{
		Debug.Log("Connecting...");
		SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();

		connectArgs.UserToken = clientSocket;
		connectArgs.RemoteEndPoint = hostEndPoint;
		connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);

		clientSocket.ConnectAsync(connectArgs);

		SocketError errorCode = connectArgs.SocketError;
		if (errorCode != SocketError.Success)
		{
			throw new SocketException((Int32)errorCode);
		}
	}

	// Calback for connect operation
	private void OnConnect(object sender, SocketAsyncEventArgs e)
	{
		// Set the flag for socket connected.
		connected = (e.SocketError == SocketError.Success);

		// clientSocket = e.AcceptSocket;
		Debug.Log("Connected? " + connected);
	}

	/*

	public void writeSocket(string theLine) {
		if (!socketReady)
			return;
		String foo = theLine + "\r\n";
		theWriter.Write(foo);
		theWriter.Flush();
	}

	public String readSocket() {
		if (!socketReady)
		{
			return "Not ready";
		}
		if (theStream.DataAvailable)
			return theReader.ReadLine();
		return "?";
	}
	*/

	internal void waitForData()
	{
		Debug.Log("Waiting...");
		if (connected)
		{			
			SocketAsyncEventArgs e = new SocketAsyncEventArgs();
			byte[] receiveBuffer = new byte[255];
			e.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
			e.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceive);
			clientSocket.ReceiveAsync(e);
			waiting = true;
		}
		else
		{
			throw new SocketException((Int32)SocketError.NotConnected);
		}
	} 

	// Calback for receive operation
	private void OnReceive(object sender, SocketAsyncEventArgs e)
	{
		Debug.Log("Received...");
		// check if the remote host closed the connection
		Socket token = (Socket)e.UserToken;
		if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
		{
			//echo the data received back to the client
			string recievedMessage = Encoding.ASCII.GetString(e.Buffer, 0, e.BytesTransferred);
			Debug.Log("Data: " + recievedMessage);
		}
		else
		{
			closeSocket();
		}
		waiting = false;
	}

	public void closeSocket() {
		Debug.Log("Closing");
		if (!socketReady)
			return;
	
		clientSocket.Shutdown(SocketShutdown.Both);
		clientSocket.Close();
		socketReady = false;
		connected = false;
	}

	void Stop()
	{
		closeSocket();
	}
}
