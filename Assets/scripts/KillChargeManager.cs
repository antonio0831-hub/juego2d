using UnityEngine;
using UnityEngine.UI;

public class KillChargeManager : MonoBehaviour
{
    [Header("Objetivos")]
    public int targetKills = 5;   // Carga para ataque especial
    public int portalKills = 4;   // Carga para el portal

    [Header("Estado")]
    public int kills = 0;
    public bool charged = false;

    [Header("UI (segmentos 0..4)")]
    public Image[] segments;

    [Header("Habilidad al cargar")]
    public MonoBehaviour abilityToEnable; // Aquí va el script 'ataque_especial'

    [Header("Portal")]
    public GameObject portalObject; 
    private bool portalUnlocked = false;

    void Awake()
    {
        kills = 0;
        charged = false;
        portalUnlocked = false;

        // Desactivamos la habilidad al empezar
        if (abilityToEnable != null) abilityToEnable.enabled = false;

        // Aseguramos que el portal esté apagado
        if (portalObject != null) portalObject.SetActive(false);

        ApplyUI();
    }

    public void AddKill(int amount = 1)
    {
        if (charged) return;

        kills = Mathf.Clamp(kills + amount, 0, targetKills);
        Debug.Log($"[KillCharge] {kills}/{targetKills}");

        // --- LÓGICA DEL PORTAL (4 Kills) ---
        if (!portalUnlocked && kills >= portalKills)
        {
            portalUnlocked = true;
            if (portalObject != null)
            {
                portalObject.SetActive(true);
                var a = portalObject.GetComponent<Animator>();
                if (a != null) a.SetTrigger("appear");
            }
        }

        // --- LÓGICA DE HABILIDAD (5 Kills) ---
        if (kills >= targetKills)
        {
            charged = true;
            if (abilityToEnable != null) abilityToEnable.enabled = true;
        }

        ApplyUI();
    }

    public void Consume()
    {
        kills = 0;
        charged = false;
        
        // Al usar el ataque, se desactiva el script hasta volver a cargar
        if (abilityToEnable != null) abilityToEnable.enabled = false;

        ApplyUI();
    }

    void ApplyUI()
    {
        if (segments == null || segments.Length == 0) return;

        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] != null)
            {
                segments[i].gameObject.SetActive(i < kills);
            }
        }
    }
}