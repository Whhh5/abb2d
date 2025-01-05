using UnityEngine;

public class Trigger3D : MonoBehaviour
{
    [SerializeField]
    private bool m_IsEnter = false;
    [SerializeField]
    private GameObject m_IgnoreObj = null;
    public bool IsEnter()
    {
        return m_IsEnter;
    }

    private void Update()
    {
        var arrHit = Physics.BoxCastAll(transform.position, 0.1f * Vector3.one, Vector3.down, Quaternion.Euler(Vector3.zero), 0.1f);

        if (arrHit != null)
            foreach (var item in arrHit)
            {
                if (item.collider.gameObject != m_IgnoreObj)
                {
                    m_IsEnter = true;
                    return;
                }
            }
        m_IsEnter = false;
    }
}
