using UnityEngine;

public class DieEventProxy : MonoBehaviour
{
    public void OnDieAnimEnd()
    {
        var npc = GetComponentInParent<ANPC>();
        if (npc != null)
            npc.OnDieAnimEnd();
    }
}