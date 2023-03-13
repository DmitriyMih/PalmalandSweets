using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(PathCreator))]
public class PathController : MonoBehaviour
{
    [SerializeField] private Distributor distributor;
    private PathCreator pathCreator;

    [SerializeField] private float followingSpeed = 1f;
    public float FollowingSpeed => followingSpeed;

    private float tempSpeed;

    [SerializeField] private List<PathFollower> itemFollowersList = new List<PathFollower>();
    [SerializeField] private List<GuideFollower> guideFollowersList = new List<GuideFollower>();

    private void Awake()
    {
        pathCreator = GetComponent<PathCreator>();
    }

    private void Start()
    {
        DebugInitializationList();
    }

    private void DebugInitializationList()
    {
        for (int i = 0; i < itemFollowersList.Count; i++)
        {
            PathFollower follower = itemFollowersList[i];
            if (follower == null)
                continue;

            follower.transform.parent = transform;
            follower.ChangePathController(this);
            follower.SetMoveDistance(itemFollowersList.Count - i);
        }

        UpdateIndexes();
    }

    public PathCreator GetPathCreator()
    {
        return pathCreator;
    }

    public bool HasPathCreator()
    {
        return pathCreator;
    }

    #region Followers
    public bool HasFollowerInList(PathFollower pathFollower)
    {
        return itemFollowersList.Contains(pathFollower);
    }

    public int GetFollowerIndex(PathFollower pathFollower)
    {
        return itemFollowersList.IndexOf(pathFollower);
    }

    public PathFollower GetFollowerByIndex(int index)
    {
        return (index > 0 || index < itemFollowersList.Count) ? itemFollowersList[index] : null;
    }

    public int GetFollowersCount()
    {
        return itemFollowersList.Count;
    }
    #endregion

    #region Guiders
    public bool HasGuiderInList(GuideFollower guideFollower)
    {
        return guideFollowersList.Contains(guideFollower);
    }

    public int GetGuiderIndex(GuideFollower guideFollower)
    {
        return guideFollowersList.IndexOf(guideFollower);
    }

    public GuideFollower GetGuiderByIndex(int index)
    {
        return (index > 0 || index < guideFollowersList.Count) ? guideFollowersList[index] : null;
    }

    public int GetGuidersCount()
    {
        return guideFollowersList.Count;
    }
    #endregion

    private void Update()
    {
        if (tempSpeed != followingSpeed)
        {
            UpdateItemsMoveSpeed();
            tempSpeed = followingSpeed;
        }
    }

    private void UpdateItemsMoveSpeed()
    {
        if (distributor != null)
            distributor.ChangeSpeedAcceleration(followingSpeed);

        for (int i = 0; i < itemFollowersList.Count; i++)
        {
            if (itemFollowersList[i] == null)
                continue;

            itemFollowersList[i].ChangeMoveSpeed(followingSpeed);
        }
    }

    [ContextMenu("Update Indexes")]
    public void UpdateIndexes()
    {
        itemFollowersList.Clear();
        guideFollowersList.Clear();

        itemFollowersList.AddRange(GetComponentsInChildren<PathFollower>());

        if (itemFollowersList.Count == 0)
            return;

        if (!itemFollowersList[0].HasGuideFollower())
            itemFollowersList[0].AddGuides();

        Debug.Log("Update Indexes");

        GuideFollower tempFollower = itemFollowersList[0].GetGuideFollower();
        List<PathFollower> tempGuideChilds = new List<PathFollower>();

        //  Index updatings
        //  Search for followers and guides
        for (int i = 0; i < itemFollowersList.Count; i++)
        {
            PathFollower follower = itemFollowersList[i];

            follower.SetIndex(i);
            follower.name = "Sphere Follower " + i;

            //  Updating child objects of the guides
            if (follower.HasGuideFollower())
            {
                GuideFollower currentFollower = follower.GetGuideFollower();
                currentFollower.name = "Guide Follower " + guideFollowersList.Count;

                //Debug.Log($"Find Follower: {currentFollower.name} | Temp Childs: {tempGuideChilds.Count}");

                tempFollower.UpdateChilds(tempGuideChilds);
                tempGuideChilds.Clear();

                tempFollower = currentFollower;
                guideFollowersList.Add(currentFollower);
            }
            else
                tempGuideChilds.Add(follower);
        }

        if (tempFollower != null)
            tempFollower.UpdateChilds(tempGuideChilds);
    }

    #region Balls Behavior
    public void AddItemInPath(PathFollower sphereItem, int index = -1)
    {
        if (sphereItem == null)
            return;

        sphereItem.ChangeMoveSpeed(followingSpeed);
        sphereItem.ChangePathController(this);

        if (index != -1)
        {
            index = Mathf.Clamp(index, 0, itemFollowersList.Count - 1);
            itemFollowersList.Insert(index, sphereItem);
        }
        else
            itemFollowersList.Add(sphereItem);

        sphereItem.transform.parent = transform;

        UpdateIndexes();
    }

    private void SetGuideBehaviours()
    {
        switch (guideFollowersList.Count)
        {
            case 0:
                Debug.Log("Guides Is Null");
                break;

            case 1:
                guideFollowersList[0].SetBehavior(Behavior.FollowThePath);
                break;

            case 2:
                guideFollowersList[0].SetBehavior(Behavior.Stop);
                guideFollowersList[1].SetBehavior(Behavior.FollowThePath);
                break;

            default:
                for (int i = 0; i < guideFollowersList.Count; i++)
                    guideFollowersList[i].SetBehavior(i == guideFollowersList.Count - 1 ? Behavior.FollowThePath : Behavior.FollowBack);
                break;
        }
    }

    public void RemoveGuideFollower(GuideFollower guideFollower)
    {
        guideFollowersList.Remove(guideFollower);
    }

    public void RemoveSphereItem(PathFollower sphereItem)
    {
        if (sphereItem == null || !HasFollowerInList(sphereItem))
            return;

        Debug.Log("Remove Item");
        sphereItem.ChangePathController(null);
        int currentIndex = GetFollowerIndex(sphereItem);

        if (currentIndex == 0)
            itemFollowersList[1].AddGuides();
        else if (currentIndex == itemFollowersList.Count - 1)
            Debug.Log("This Last Element");
        else
            itemFollowersList[currentIndex + 1].AddGuides();

        itemFollowersList.Remove(sphereItem);

        StartCoroutine(CooldownActions(0.01f));
    }

    [SerializeField] private List<PathFollower> tempList = new List<PathFollower>();
    [SerializeField] private Dictionary<PathFollower, int> destroyDictionary = new Dictionary<PathFollower, int>();
    public void RemoveSphereItems(List<PathFollower> sphereItemsList)
    {
        int endIndex = 0;
        tempList = sphereItemsList;

        for (int i = 0; i < sphereItemsList.Count; i++)
        {
            PathFollower sphereItem = sphereItemsList[i];
            if (sphereItem == null || !HasFollowerInList(sphereItem))
                continue;

            int index = GetFollowerIndex(sphereItem);
            if (index > endIndex)
                endIndex = index;

            Debug.Log($"End Index: {endIndex} | Index: {index}");

            sphereItem.ChangePathController(null);
            sphereItem.transform.parent = null;
            sphereItem.name = "Sphere Follower";

            //itemFollowersList.Remove(sphereItem);
            itemFollowersList.RemoveAt(index);
        }

        Debug.Log($"| Last | End Index: {endIndex}");
        if (endIndex < itemFollowersList.Count && endIndex > 0)
            itemFollowersList[endIndex].AddGuides();

        StartCoroutine(CooldownActions(0.1f));
    }

    private IEnumerator CooldownActions(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        UpdateIndexes();
        SetGuideBehaviours();
    }
    #endregion
}