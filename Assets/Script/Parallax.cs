using UnityEngine;

/// <summary>
/// Moves the object horizontally based on camera movement and a multiplier.
/// 1 = follows camera, 0 = static, 0.5 = half speed.
/// </summary>
public class Parallax : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float m_Coefficient = 0.5f;

    private Transform m_Camera;
    private float m_StartX;
    private float m_CameraStartX;

    private void Start()
    {
        m_Camera = Camera.main.transform;
        m_StartX = transform.position.x;
        m_CameraStartX = m_Camera.position.x;
    }

    private void LateUpdate()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main?.transform;
            if (m_Camera == null) return;
            m_CameraStartX = m_Camera.position.x;
        }

        float deltaX = m_Camera.position.x - m_CameraStartX;
        var pos = transform.position;
        pos.x = m_StartX + deltaX * m_Coefficient;
        transform.position = pos;
    }
}
