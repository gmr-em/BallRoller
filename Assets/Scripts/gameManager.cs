using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class gameManager : MonoBehaviour {

	public GameObject[] rooms;                                 //Array of floor prefabs.

	private GameObject[] ARooms;                                 //Array of floor prefabs.
	private GameObject[] BRooms;                                 //Array of floor prefabs.
	private List<string> previousRooms;
	private int currentRoomIndex;

	// the current room being used
	private GameObject currentRoom;

	void OnEnable()
	{
		PlayerController.OnForwardsTrigger += NextRoom;
		PlayerController.OnBackwardsTrigger += PreviousRoom;
	}
		
	void OnDisable()
	{
		PlayerController.OnForwardsTrigger -= NextRoom;
		PlayerController.OnBackwardsTrigger -= PreviousRoom;
	}

	void NextRoom()
	{
		if(currentRoomIndex < previousRooms.Count - 1)
		{
			currentRoomIndex++;
			Destroy(currentRoom);
			currentRoom = getRoomWithName(previousRooms[currentRoomIndex]);
		}
		else
		{
			currentRoomIndex++;

			GameObject newRoom;
			if (currentRoom.tag == "roomTypeA")
			{
				newRoom = getRandomRoom (BRooms);
			} 
			else 
			{
				newRoom = getRandomRoom (ARooms);
			}			

			Destroy(currentRoom);

			currentRoom = newRoom;
		}
	}

	void PreviousRoom()
	{
		if (currentRoomIndex > 0)
		{
			currentRoomIndex--;
			Destroy(currentRoom);
			currentRoom = getRoomWithName(previousRooms[currentRoomIndex]);
		}

		// Destroy(currentRoom);
	}

	// Use this for initialization
	void Start () {
		currentRoomIndex = 0;
		previousRooms = new List<string>();
		ARooms = rooms.Where (x => x.tag == "roomTypeA").ToArray();
		BRooms = rooms.Where (x => x.tag == "roomTypeB").ToArray();

		currentRoom	= getRoomWithName("RoomA1");
		previousRooms.Add("RoomA1");
	}

	GameObject getRandomRoom(GameObject[] roomArray)
	{
		// pick a random room from the list
		GameObject toInstantiate = roomArray[Random.Range (0,roomArray.Length)];
		previousRooms.Add(toInstantiate.name);
		// instantiate it at the position of the old one
		GameObject instance =
			Instantiate (toInstantiate, new Vector3 (0f, -15f, 5f), Quaternion.identity) as GameObject;

		return instance;
	}

	GameObject getRoomWithName(string roomName)
	{
		GameObject toInstantiate = rooms.First (X => X.name == roomName);

		GameObject instance =
			Instantiate (toInstantiate, new Vector3 (0f, -15f, 5f), Quaternion.identity) as GameObject;

		return instance;
	}
}
