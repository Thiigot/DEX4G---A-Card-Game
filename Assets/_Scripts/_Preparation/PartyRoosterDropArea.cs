using UnityEngine;

public class PartyRosterDropArea : MonoBehaviour
{
    [SerializeField] private Vector2 size = new Vector2(8f, 2f);
    [SerializeField] private bool drawGizmo = true;
    [SerializeField] private Color gizmoColor = new Color(0.4f, 0.8f, 1f, 0.25f);

    public bool IsInside(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        return Mathf.Abs(localPosition.x) <= size.x * 0.5f &&
               Mathf.Abs(localPosition.y) <= size.y * 0.5f;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmo)
            return;

        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, new Vector3(size.x, size.y, 0.1f));
    }
}
