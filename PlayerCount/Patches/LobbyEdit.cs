using System;
using HarmonyLib;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlayerCount.Patches;

[HarmonyPatch(typeof(HostControlsMenu), "GoFromRoomSettingToHome")]
public class HostControlsMenuGoFromRoomSettingToHomePatch
{
    static void Postfix(HostControlsMenu __instance)
    {
        string text = __instance.roomSettings_holder.transform.Find("Players Input").GetComponent<TMP_InputField>().text;

        if (!Byte.TryParse(text, out byte result)) result = 16;
        if (result == 0 || result > 16) result = 16;

        PhotonNetwork.CurrentRoom.MaxPlayers = result;
    }
}

[HarmonyPatch(typeof(HostControlsMenu), "GoFromHomeToRoomSettings")]
public class HostControlsMenuGoFromHomeToRoomSettingsPatch
{
    static void Postfix(HostControlsMenu __instance)
    {
        if (__instance.roomSettings_holder.transform.Find("Players Input") == null)
        {
            GameObject privateLabel = __instance.roomSettings_holder.transform.Find("Room Private Toggle/RoomName Title").gameObject;
            GameObject backButton = __instance.roomSettings_holder.transform.Find("Room Settings GoBack").gameObject;
            GameObject roomName = __instance.roomSettings_holder.transform.Find("RoomName Panel/RoomName InputField").gameObject;

            GameObject playersLabel = Object.Instantiate(privateLabel, backButton.transform.parent);
            GameObject playersInput = Object.Instantiate(roomName, backButton.transform.parent);

            playersLabel.name = "Players Label";
            playersInput.name = "Players Input";

            RectTransform playersLabelTransform = playersLabel.GetComponent<RectTransform>();
            RectTransform playersInputTransform = playersInput.GetComponent<RectTransform>();

            playersLabelTransform.anchorMin = new Vector2(.05f, .045f);
            playersLabelTransform.anchorMax = new Vector2(.45f, .135f);

            playersInputTransform.anchorMin = new Vector2(.425f, .03f);
            playersInputTransform.anchorMax = new Vector2(.565f, .15f);

            playersLabel.GetComponent<TextMeshProUGUI>().text = "Max Players";

            TMP_InputField inputField = playersInput.GetComponent<TMP_InputField>();
            inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputField.pointSize = 20;

            playersInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>().text = "#";
        }

        __instance.roomSettings_holder.transform.Find("Players Input").GetComponent<TMP_InputField>().text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
    }
}