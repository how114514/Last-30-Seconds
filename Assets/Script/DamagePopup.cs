using TMPro;
using UnityEngine;

/// <summary>
/// World-space damage number, floats up and self-destructs.
/// </summary>
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float m_Lifetime = 0.8f;
    [SerializeField] private float m_FloatSpeed = 1f;

    private void Start()
    {
        Destroy(gameObject, m_Lifetime);
    }

    private void Update()
    {
        transform.position += Vector3.up * m_FloatSpeed * Time.deltaTime;
    }

    /// <summary>Spawn with random horizontal spread.</summary>
    public static void Spawn(DamagePopup prefab, Vector3 position, int damage, float spreadX = 0.5f)
    {
        var popup = Instantiate(prefab, position, Quaternion.identity);
        popup.transform.position += Vector3.right * Random.Range(-spreadX, spreadX);
        popup.SetDamage(damage);
    }

    public void SetDamage(int damage)
    {
        var text = GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = damage.ToString();
    }
}
