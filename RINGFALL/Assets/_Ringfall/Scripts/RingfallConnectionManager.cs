using UnityEngine;
using Unity.Netcode;

namespace Ringfall.Networking
{
    public class RingfallConnectionManager : MonoBehaviour
    {
        // GDD kuralı: Maksimum 9 oyuncu (1 Villain + 8 İnsan)
        private const int MAX_PLAYERS = 9;

        private void Start()
        {
            if (NetworkManager.Singleton != null)
            {
                // Oyuna biri bağlanırken bizim kontrol fonksiyonumuzu çalıştır
                NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            }
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            int currentPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;

            // İçeride 9 kişi varsa 10. kişiyi reddet (Çökme ve taşma önlemi)
            if (currentPlayers >= MAX_PLAYERS)
            {
                response.Approved = false;
                response.Reason = "Oda dolu! Maksimum 9 oyuncu katilabilir.";
                Debug.LogWarning("10. oyuncu girmeye calisti ve engellendi!");
                return;
            }

            // Yer varsa girişe izin ver
            response.Approved = true;
            response.CreatePlayerObject = true;
            Debug.Log($"Oyuncu katildi! Mevcut oyuncu sayisi: {currentPlayers + 1}/{MAX_PLAYERS}");
        }

        // Test için ekrana küçük butonlar çizer (12. haftada gerçek Lobi UI gelince sileceğiz)
        private void OnGUI()
        {
            if (NetworkManager.Singleton == null) return;

            GUILayout.BeginArea(new Rect(10, 10, 250, 250));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                if (GUILayout.Button("Host Başlat (Odayı Kur)", GUILayout.Height(40)))
                    NetworkManager.Singleton.StartHost();

                if (GUILayout.Button("Client Başlat (Odaya Katıl)", GUILayout.Height(40)))
                    NetworkManager.Singleton.StartClient();
            }
            else
            {
                GUILayout.Label($"Bağlantı Aktif! Oyuncu Sayısı: {NetworkManager.Singleton.ConnectedClientsIds.Count}/{MAX_PLAYERS}");
            }
            GUILayout.EndArea();
        }
    }
}