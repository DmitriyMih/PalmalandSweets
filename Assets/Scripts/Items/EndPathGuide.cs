using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPathGuide : BaseGuideFollower
{
    [SerializeField] private float endFollowSpeed = 3f;

    protected override void FixedUpdate()
    {
        Move(endFollowSpeed);
    }
}