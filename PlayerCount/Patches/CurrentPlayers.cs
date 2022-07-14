using HarmonyLib;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerCount.Patches;

[HarmonyPatch(typeof(PhotonZeepkist), "DrawNetworkGUI")]
public class PhotonZeepkistDrawNetworkGUIPatch
{
    static bool Prefix(PhotonZeepkist __instance)
    {
        if (PhotonNetwork.CurrentRoom == null || SceneManager.GetActiveScene().name != "GameScene" || __instance.manager.currentMaster == null || !__instance.GetDrawLeaderboard() || __instance.manager.photonGUI_leaderboardPanel.activeSelf) return true;
            
        GameObject leaderboardPanel = __instance.manager.photonGUI_leaderboardPanel.transform.Find("Panel").gameObject;
                
        if (leaderboardPanel.transform.Find("Player Count") == null)
        {
            GameObject timeLeft = leaderboardPanel.transform.Find("Time Left Leaderboard").gameObject;
                
            GameObject playerCount = Object.Instantiate(timeLeft, timeLeft.transform.parent);
            playerCount.name = "Player Count";
                
            RectTransform playerCountTransform = playerCount.GetComponent<RectTransform>();

            playerCountTransform.anchorMin = new Vector2(.05f, .91f);
            playerCountTransform.anchorMax = new Vector2(.18f, .96f);
        }
            
        leaderboardPanel.transform.Find("Player Count/Time Text").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + " Players";

        return true;
    }
}