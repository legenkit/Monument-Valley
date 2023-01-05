using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public bool Walking;
    public Transform indicator;
    [Header("Plaform Details")]
    public Transform CurrentBlock;
    public Transform ClickedBlock;
    public List<Transform> FinalPath;


    private float blend;
    void Start()
    {
        RayCastDown();
    }

    void Update()
    {
        RayCastDown();
        MyInput();
        Updater();
    }

    void Updater()
    {
        transform.up = CurrentBlock.up;
        transform.parent = CurrentBlock.transform;
    }

    void MyInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit MouseHit;
            if (Physics.Raycast(MouseRay, out MouseHit))
            {
                if (MouseHit.transform.GetComponent<Walkable>())
                {
                    ClickedBlock = MouseHit.transform;
                    FinalPath.Clear();
                    FindPath();

                    blend = transform.position.y - ClickedBlock.position.y > 0 ? -1 : 1;

                    indicator.position = MouseHit.transform.GetComponent<Walkable>().GetWalkPoint();
                    Sequence s = DOTween.Sequence();
                    s.AppendCallback(() => indicator.GetComponentInChildren<ParticleSystem>().Play());
                    s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.white, .1f));
                    s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.black, .3f).SetDelay(.2f));
                    s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.clear, .3f));
                }
            }
        }
    }

    void FindPath()
    {
        List<Transform> NextBloks = new List<Transform>();
        List<Transform> PastBloks = new List<Transform>();

        foreach (WalkPath path in CurrentBlock.GetComponent<Walkable>().PossiblePaths)
        {
            if (path.isActive)
            {
                NextBloks.Add(path.Target);
                path.Target.GetComponent<Walkable>().PreviousBlock = CurrentBlock;
            }
        }
        PastBloks.Add(CurrentBlock);

        ExploreBlocks(NextBloks, PastBloks);
        BuildPath();
    }

    void ExploreBlocks(List<Transform> nextBlocks , List<Transform> visitedBlocks)
    {
        Transform current = nextBlocks.First();
        nextBlocks.Remove(current);

        if (current == ClickedBlock)
            return;

        foreach (WalkPath path in current.GetComponent <Walkable>().PossiblePaths)
        {
            if(!visitedBlocks.Contains(path.Target) && path.isActive)
            {
                nextBlocks.Add(path.Target);
                path.Target.GetComponent<Walkable>().PreviousBlock = current;
            }
        }
        visitedBlocks.Add(current);

        if (nextBlocks.Any())
            ExploreBlocks(nextBlocks, visitedBlocks);
    }

    void BuildPath()
    {
        Transform Block = ClickedBlock;
        while(Block != CurrentBlock)
        {
            FinalPath.Add(Block);
            if (Block.GetComponent<Walkable>().PossiblePaths != null && Block.GetComponent<Walkable>().PreviousBlock)
                Block = Block.GetComponent<Walkable>().PreviousBlock;
            else
                return;
        }
        FinalPath.Insert(0 , ClickedBlock);

        FollowPath();
    }

    void FollowPath()
    {
        Sequence s = DOTween.Sequence();

        Walking = true;

        for(int i = FinalPath.Count() -1; i > 0; i--)
        {
            float time = FinalPath[i].GetComponent<Walkable>().IsStair ? 1.5f : 1;
            s.Append(transform.DOMove(FinalPath[i].GetComponent<Walkable>().GetWalkPoint(), 0.2f * time).SetEase(Ease.Linear));
        }

        if (ClickedBlock.GetComponent<Walkable>().IsButton)
        {
            s.AppendCallback(() => GameManager.instance.MoveObject(ClickedBlock.GetComponent<Walkable>().Operations));
        }

        s.AppendCallback(() => ClearPath());
    }

    void ClearPath()
    {
        foreach(Transform t in FinalPath)
        {
            t.GetComponent<Walkable>().PreviousBlock = null;
        }
        FinalPath.Clear();
        Walking = false;
    }

    void RayCastDown()
    {
        RaycastHit PlayerHit;
        Ray PlayerRay = new Ray(transform.position + transform.up*0.5f, -transform.up);
        if(Physics.Raycast(PlayerRay , out PlayerHit, 0.8f))
        {
            CurrentBlock = PlayerHit.transform;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + transform.up * 0.5f, transform.position - transform.up * 0.3f);
    }
}
