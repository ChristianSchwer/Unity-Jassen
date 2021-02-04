using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
    #region Private Fields

    [SerializeField]
    private Transform _content;
    [SerializeField]
    private RoomListing _roomListing;

    private RoomsCanvases _roomsCanvases;

    Dictionary<string, RoomListing> roomListing = new Dictionary<string, RoomListing>();

    #endregion

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnJoinedRoom()
    {
        _roomsCanvases.CurrentRoomCanvas.Show();
        _content.DestroyChildren();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (roomListing.ContainsKey(info.Name))
            {
                if (info.RemovedFromList)
                {
                    //Remove from list.
                    roomListing[info.Name].RemoveFromList();
                    roomListing.Remove(info.Name);
                }
                else
                {
                    //Update list
                    roomListing[info.Name].SetRoomInfo(info);
                }
            }
            else
            {
                //Added to list.
                roomListing[info.Name] = Instantiate(_roomListing, _content);
                roomListing[info.Name].gameObject.SetActive(true);
                roomListing[info.Name].AddToList(info);
            }
        }
    }

    #endregion
}
