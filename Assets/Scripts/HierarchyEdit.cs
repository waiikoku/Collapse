using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class HierarchyEdit : MonoBehaviour
{
    [SerializeField] private GameObject[] prefix;
    private Transform _transform;
    private List<string> headName;
    private List<Transform> headBranch;
    private Transform[] storeChild;

    [SerializeField] private bool direct = false;
    private void Awake()
    {
        _transform = transform;
        headName = new List<string>();
    }

    private void Start()
    {
        if (direct)
        {
            StartCoroutine(Sort(_transform));
        }
        else
        {
            StartCoroutine(GetAllOut());
        }

    }

    private IEnumerator Sort(Transform current)
    {
        IOrderedEnumerable<Transform> orderedChildren = current.Cast<Transform>().OrderBy(tr => tr.name);
        yield return new WaitForFixedUpdate();
        foreach (Transform child in orderedChildren)
        {
            Undo.SetTransformParent(child, null, "Reorder children");
            Undo.SetTransformParent(child, current, "Reorder children");
            if (child.name.Contains("("))
            {
                string tempName = child.name;
                int whatIndex = tempName.IndexOf("(");
                child.name = tempName.Substring(0, whatIndex - 1);
                if (headName.Contains(child.name) == false)
                {
                    headName.Add(child.name);
                }
            }
            else
            {
                if (headName.Contains(child.name) == false)
                {
                    headName.Add(child.name);
                }
            }
            yield return null;
        }
        storeChild = _transform.GetComponentsInChildren<Transform>();
        headBranch = new List<Transform>(4);
        yield return new WaitForFixedUpdate();
        foreach (var name in headName)
        {
            GameObject parentHi = new GameObject();
            parentHi.transform.parent = _transform;
            parentHi.name = name;
            headBranch.Add(parentHi.transform);
        }
        yield return new WaitForFixedUpdate();
        foreach (var child in storeChild)
        {
            foreach (var head in headBranch)
            {
                if (child.name == head.name)
                {
                    child.parent = head;
                    break;
                }
                yield return null;
            }
            yield return null;
        }
    }

    public IEnumerator GetAllOut()
    {
        yield return new WaitForSecondsRealtime(3f);
        Transform[] headObj = new Transform[_transform.childCount];
        print(headObj.Length);
        yield return new WaitForEndOfFrame();
        List<Transform> allChild =new List<Transform>();
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < headObj.Length; i++)
        {
            headObj[i] = _transform.GetChild(i);
            Transform[] tf = headObj[i].GetComponentsInChildren<Transform>();
            allChild.AddRange(tf);
            print(tf.Length);
        }
        yield return new WaitForEndOfFrame();
        foreach (var child in allChild)
        {
            child.parent = _transform;
        }
        yield return new WaitForEndOfFrame();
        foreach (var head in headObj)
        {
            Destroy(head.gameObject);
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(Sort(_transform));
    }
}
#endif