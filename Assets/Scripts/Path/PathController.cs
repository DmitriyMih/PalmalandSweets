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

    [SerializeField] private List<PathFollower> itemsList = new List<PathFollower>();
    [SerializeField] private List<GuideFollower> guideFollowers = new List<GuideFollower>();

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
        for (int i = 0; i < itemsList.Count; i++)
        {
            PathFollower follower = itemsList[i];
            if (follower == null)
                continue;

            follower.transform.parent = transform;
            follower.ChangePathController(this);
            follower.SetMoveDistance(itemsList.Count - i);
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
        return itemsList.Contains(pathFollower);
    }

    public int GetFollowerIndex(PathFollower pathFollower)
    {
        return itemsList.IndexOf(pathFollower);
    }
    #endregion

    #region Guiders
    public bool HasGuiderInList(GuideFollower guideFollower)
    {
        return guideFollowers.Contains(guideFollower);
    }

    public int GetGuiderIndex(GuideFollower guideFollower)
    {
        return guideFollowers.IndexOf(guideFollower);
    }

    public GuideFollower GetGuiderByIndex(int index)
    {
        return (index > 0 || index < guideFollowers.Count) ? guideFollowers[index] : null;
    }

    public int GetGuidersCount()
    {
        return guideFollowers.Count;
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

        for (int i = 0; i < itemsList.Count; i++)
        {
            if (itemsList[i] == null)
                continue;

            itemsList[i].ChangeMoveSpeed(followingSpeed);
        }
    }

    private void UpdateIndexes()
    {
        itemsList.Clear();
        guideFollowers.Clear();

        itemsList.AddRange(GetComponentsInChildren<PathFollower>());

        if (itemsList[0].HasGuideFollower())
            itemsList[0].AddGuides();

        GuideFollower tempFollower = itemsList[0].GetGuideFollower();
        List<PathFollower> tempGuideChilds = new List<PathFollower>();

        //  Index updatings
        //  Search for followers and guides
        for (int i = 0; i < itemsList.Count; i++)
        {
            PathFollower follower = itemsList[i];

            follower.SetIndex(i);
            follower.name = "Sphere Follower " + i;

            //  Updating child objects of the guides
            if (follower.HasGuideFollower())
            {
                GuideFollower currentFollower = follower.GetGuideFollower();
                currentFollower.name = "Guide Follower " + guideFollowers.Count;

                //Debug.Log($"Find Follower: {currentFollower.name} | Temp Childs: {tempGuideChilds.Count}");

                tempFollower.UpdateChilds(tempGuideChilds);
                tempGuideChilds.Clear();

                tempFollower = currentFollower;
                guideFollowers.Add(currentFollower);
            }
            else
                tempGuideChilds.Add(follower);
        }

        if (tempFollower != null)
            tempFollower.UpdateChilds(tempGuideChilds);
    }

    #region Balls Behavior
    public void AddWithOffset(PathFollower newItem, PathFollower itemInList, Direction direction)
    {
        int index = itemsList.IndexOf(itemInList);
        Debug.Log("Index: " + index);

        if (direction == Direction.Forward)
        {

        }
        else
        {

        }
    }

    public void AddItemInPath(PathFollower sphereItem, int index = -1)
    {
        if (sphereItem == null)
            return;

        sphereItem.ChangeMoveSpeed(followingSpeed);
        sphereItem.ChangePathController(this);

        if (index != -1)
        {
            index = Mathf.Clamp(index, 0, itemsList.Count - 1);
            itemsList.Insert(index, sphereItem);
        }
        else
            itemsList.Add(sphereItem);

        sphereItem.transform.parent = transform;

        UpdateIndexes();
    }

    public void RemoveSphereItem(PathFollower sphereItem)
    {
        if (sphereItem == null)
            return;

        sphereItem.ChangePathController(null);
        itemsList.Remove(sphereItem);

        UpdateIndexes();
    }
    #endregion
}