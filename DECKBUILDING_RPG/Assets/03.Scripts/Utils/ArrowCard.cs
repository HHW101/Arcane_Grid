using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCard : ArrowBase
{
    private Vector2 start = Vector2.zero;
    public override void Init(Vector2 start, Vector2 end)
    {
        base.Init(start, end);
        this.start = start;

    }
    public void DrawArrow(Vector2 end)
    {
        DrawArrow(start,end);
    }
    
}
