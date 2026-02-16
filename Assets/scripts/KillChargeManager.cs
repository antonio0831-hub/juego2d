using UnityEngine;
using UnityEngine.UI;

public class KillChargeManager : MonoBehaviour
{
    [Header("Objetivos")]
    public int targetKills = 5;   // carga especial
    public int portalKills = 4;   // aparece portal

    [Header("Estado")]
    public int kills = 0;
    public bool charged = false;

    [Header("UI (segmentos 0..4)")]
    public Image[] segments;

    [Header("Habilidad al cargar (opcional)")]
    public MonoBehaviour abilityToEnable;

    [Header("Portal (aparece 1 vez)")]
    public GameObject portalObject;                 // arrastra el portal (en escena) o un prefab instanciado ya
    public string portalUnlockedKey = "PORTAL_4K_UNLOCKED"; // PlayerPrefs key
    private bool portalUnlocked;

    void Awake()
    {
        // si ya se desbloqueó antes, no volverá a activarse jamás
        portalUnlocked = false;

        kills = 0;
        charged = false;

        if (abilityToEnable != null) abilityToEnable.enabled = false;

        // si ya estaba desbloqueado, deja el portal activo (o como tú quieras)
        if (portalObject != null)
            portalObject.SetActive(portalUnlocked);

        ApplyUI();
    }

    public void AddKill(int amount = 1)
    {
        if (charged) return;

        kills = Mathf.Clamp(kills + Mathf.Max(1, amount), 0, targetKills);
        Debug.Log($"[KillCharge] {kills}/{targetKills}");

        // ✅ 4 kills -> activar portal SOLO si nunca se activó
        if (!portalUnlocked && kills >= portalKills)
        {
            portalUnlocked = true;

            if (portalObject != null)
            {
                portalObject.SetActive(true);

                // si tiene animación, dispara su trigger "appear"
                var a = portalObject.GetComponent<Animator>();
                if (a != null)
                {
                    a.ResetTrigger("appear");
                    a.SetTrigger("appear");
                }
            }
        }

        ApplyUI();

        // ✅ 5 kills -> cargar habilidad
        if (kills >= targetKills)
        {
            charged = true;
            if (abilityToEnable != null) abilityToEnable.enabled = true;
        }
    }

    public void Consume()
    {
        kills = 0;
        charged = false;

        if (abilityToEnable != null) abilityToEnable.enabled = false;

        ApplyUI();
        Debug.Log("[KillCharge] Reset a 0 (el portal NO se resetea)");
    }

    void ApplyUI()
    {
        if (segments == null || segments.Length == 0) return;

        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] == null) continue;
            segments[i].gameObject.SetActive(i < kills); // 0 kills -> nada visible
        }
    }
}
