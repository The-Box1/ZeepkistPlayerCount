using System;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlayerCount.Patches;

[HarmonyPatch(typeof(PhotonZeepkist), "CreateZeepkistRoom")]
public class PhotonZeepkistCreateZeepkistRoomPatch
{
    static bool Prefix(PhotonZeepkist __instance, string roomID, string roomName, bool isVisible, bool copyID)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = isVisible;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = Main.MaxPlayers;
        roomOptions.PublishUserId = true;
        __instance.copyIDonCreate = copyID;
        __instance.createRoomTempName = roomName;
        PhotonNetwork.CreateRoom(null, roomOptions);

        return false;
    }
}

[HarmonyPatch(typeof(LobbyManager), "CreateRoom")]
public class LobbyManagerCreateRoomPatch
{
    static bool Prefix(LobbyManager __instance)
    {
        string text = __instance.menu_createRoom.transform.Find("Players Input").GetComponent<TMP_InputField>().text;

        if (Byte.TryParse(text, out byte result)) Main.MaxPlayers = result;
        if (Main.MaxPlayers == 0 || Main.MaxPlayers > 16) Main.MaxPlayers = 16;


        __instance.GoToLobby();
        __instance.connectingToAnything = true;
        __instance.connectingState = 1;
        __instance.connectionFailedResult = 1;
        __instance.menu_connection.SetActive(value: true);
        __instance.menu_lobby.SetActive(value: false);
        __instance.connection_text.text = I2.Loc.LocalizationManager.GetTranslation("Online/Lobby/MSG_CreatingRoom");
        __instance.manager.photonManager.CreateZeepkistRoom("random ID", __instance.manager.steamAchiever.SteamFilterString(__instance.createRoom_roomInputName.text, __instance.manager.steamAchiever.GetPlayerSteamID()), __instance.isRoomVisible, !__instance.isRoomVisible);

        return false;
    }
}

[HarmonyPatch(typeof(LobbyManager), "GoToCreateRoom")]
public class LobbyManagerGoToCreateRoomPatch
{
    static void Postfix(LobbyManager __instance)
    {
        if (__instance.menu_createRoom.transform.Find("Players Input") != null) return;

        GameObject privateLabel = __instance.menu_createRoom.transform.Find("List In Lobby Label").gameObject;
        GameObject privateButton = __instance.menu_createRoom.transform.Find("Toggle Room Private").gameObject;
        GameObject roomName = __instance.menu_createRoom.transform.Find("RoomName InputField").gameObject;

        // Move existing option up to allow more space
        RectTransform privateLabelTransform = privateLabel.GetComponent<RectTransform>();
        RectTransform privateButtonTransform = privateButton.GetComponent<RectTransform>();

        privateLabelTransform.anchorMin = new Vector2(.05f, .5f);
        privateLabelTransform.anchorMax = new Vector2(.9f, .6f);

        privateButtonTransform.anchorMin = new Vector2(.8f, .48f);
        privateButtonTransform.anchorMax = new Vector2(.9f, .62f);

        // Create new option
        GameObject playersLabel = Object.Instantiate(privateLabel, privateLabel.transform.parent);
        GameObject playersInput = Object.Instantiate(roomName, privateLabel.transform.parent);

        playersLabel.name = "Players Label";
        playersInput.name = "Players Input";

        playersLabel.GetComponent<TextMeshProUGUI>().text = "Max Players";

        RectTransform playersLabelTransform = playersLabel.GetComponent<RectTransform>();
        RectTransform playersInputTransform = playersInput.GetComponent<RectTransform>();

        playersLabelTransform.anchorMin = new Vector2(.05f, .35f);
        playersLabelTransform.anchorMax = new Vector2(.9f, .45f);

        playersInputTransform.anchorMin = new Vector2(.8f, .34f);
        playersInputTransform.anchorMax = new Vector2(.9f, .46f);

        // Set text parameters
        TMP_InputField inputField = playersInput.GetComponent<TMP_InputField>();
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputField.pointSize = 20;

        playersInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>().text = "#";
    }
}