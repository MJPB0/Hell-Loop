using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerAnimationHelper : MonoBehaviour
{
    private Container container;

    private void Start()
    {
        container = GetComponentInParent<Container>();
    }

    public void ContainerOpened()
    {
        container.ContainerOpened();
    }
}
