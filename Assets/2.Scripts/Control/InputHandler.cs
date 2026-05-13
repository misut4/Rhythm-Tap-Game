using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    [Header("References")]
    private List<RaycastResult> _results = new List<RaycastResult>();
    private PointerEventData _ped;

    private void Awake()
    {
        _ped = new PointerEventData(EventSystem.current);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        // ── Mouse (editor + PC standalone) ──────────────────────────
        if (Input.GetMouseButtonDown(0))
        {
            TryHitAt(Input.mousePosition);
        }


        // ── Touch (mobile) ──────────────────────────────────────────
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
                TryHitAt(touch.position);
        }
    }

    // ── Internal ────────────────────────────────────────────────────

    private void TryHitAt(Vector2 screenPos)
    {
        if (EventSystem.current == null) return;
        _ped.position = screenPos;

        _results.Clear();

        EventSystem.current.RaycastAll(_ped, _results);
        for (int i = 0; i < _results.Count; i++)
        {
            Tile tile = _results[i].gameObject.GetComponent<Tile>();
            if (tile != null)
            {
                tile.TryHit();
                return; // stop at the first tile hit (topmost in UI order)
            }
        }
    }
}