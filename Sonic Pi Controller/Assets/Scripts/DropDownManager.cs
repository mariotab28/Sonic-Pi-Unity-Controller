using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownManager : MonoBehaviour
{
    public GameObject container;
    public GameObject synthPF;
    public GameObject sleepPF;
    public GameObject samplePF;
    public void HandleInputData(int val)
    {
        print(val);
        switch(val)
        {
            case 0:
                Instantiate(synthPF, container.transform);
                break;
            case 1:
                Instantiate(samplePF, container.transform);
                break;
            case 2:
                Instantiate(sleepPF, container.transform);
                break;
        }
    }
}
