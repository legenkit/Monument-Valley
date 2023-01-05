using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour
{
    public enum OperationType
    {
        Move,
        Rotate
    }

    [Header("References")]
    public List<WalkPath> PossiblePaths = new List<WalkPath>();
    public Transform PreviousBlock;

    #region VARIABLES
    [Header("Offset")]
    [SerializeField] float walkPointOffset = 1f;
    [SerializeField] float StairOffset = 0.4f;

    [Header("Stats")]
    public bool IsStair;
    public bool MovingGround;
    public bool IsButton;
    public bool RontRotate;

    [Header("For Button Operations")]
    public OperationType Operation_Type;
    public ButtonOperations[] Operations;

    #endregion

    #region Public Methods
    public Vector3 GetWalkPoint()
    {
        float stair = IsStair ? StairOffset : 0;
        return transform.position + transform.up * (walkPointOffset - stair);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(GetWalkPoint(), .1f);

        if (PossiblePaths == null)
            return;
        foreach (WalkPath p in PossiblePaths)
        {
            if (p.Target == null)
                return;
            Gizmos.color = p.isActive ? Color.black : Color.clear;
            Gizmos.DrawLine(GetWalkPoint(), p.Target.GetComponent<Walkable>().GetWalkPoint());
        }
    }
}

[System.Serializable]
public class WalkPath
{
    public Transform Target;
    public bool isActive = true;
}

[System.Serializable]
public class ButtonOperations
{
    public Transform OperativeObj;
    public Vector3 ToMovePos;
    public Vector3 RotationAngle;
    public float time;
}
