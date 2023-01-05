using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region VARIABLE

    [Header("References and Conditions")]
    public PlayerController Player;
    public List<PathCondition> pathConditions = new List<PathCondition>();
    public List<Transform> pivots;

    #endregion

    #region Script Initialization
    private void Awake()
    {
        instance = this;
    }
    #endregion
    void Update()
    {
        CheckConditions();

        if (Player.Walking)
            return;

        MyInputs();
    }

    void CheckConditions()
    {
        foreach (PathCondition pc in pathConditions)
        {
            int count = 0;

            for (int i = 0; i < pc.Conditions.Count; i++)
            {
                if (pc.Conditions[i].ConditionObject.eulerAngles == pc.Conditions[i].eulerAngle && pc.Conditions[i].ConditionObject.localPosition == pc.Conditions[i].Position)
                    count++;
            }
            foreach (SinglePath sp in pc.Paths)
            {
                sp.Block.PossiblePaths[sp.index].isActive = (count == pc.Conditions.Count);

            }
            foreach(ObjectsActivation OA in pc.Objects)
            {
                if (count == pc.Conditions.Count)
                    OA.ObjToActivate.SetActive(true);
            }
        }
    }

    void MyInputs()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int multiplier = Input.GetKey(KeyCode.RightArrow) ? 1 : -1;
            pivots[0].DOComplete();
            pivots[0].DORotate(new Vector3(0, 90 * multiplier, 0), .6f, RotateMode.WorldAxisAdd).SetEase(Ease.OutBack);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    public void RotateRightPivot(Transform obj)
    {
        obj.DOComplete();
        obj.DORotate(new Vector3(0, 0, 90), .6f).SetEase(Ease.OutBack);
    }

    public void MoveObject(ButtonOperations[] operations)
    {
        Sequence s = DOTween.Sequence();
        foreach (ButtonOperations OP in operations)
        {
            s.Append(OP.OperativeObj.DOMove(OP.ToMovePos, OP.time).SetEase(Ease.Linear));
        }
    }
}

[System.Serializable]
public class PathCondition
{
    public string PathConditaionName;
    public List<Condition> Conditions;
    public List<SinglePath> Paths;
    public List<ObjectsActivation> Objects;
}

[System.Serializable]
public class Condition
{
    public Transform ConditionObject;
    public Vector3 eulerAngle;
    public Vector3 Position;
}

[System.Serializable]
public class SinglePath
{
    public Walkable Block;
    public int index;
}

[System.Serializable]
public class ObjectsActivation
{
    public GameObject ObjToActivate;
}
