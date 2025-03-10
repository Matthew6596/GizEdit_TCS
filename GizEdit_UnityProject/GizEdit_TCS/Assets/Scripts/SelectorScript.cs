using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorScript : MonoBehaviour
{
    //PUBLIC
    public Transform gizParents;

    //PUBLIC UI STUFF
    [Header("UI STUFF")]
    public GameObject loadingStuff;
    public GameObject loadingBar;
    public GameObject propsPanel;
    public Transform propsSection;

    //PRIVATE
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.gm;
    }

    public void QuitApp()
    {
        Application.Quit();
    }
    public void ToggleToolbarColumn(Transform _col)
    {
        bool toggl = !_col.GetChild(1).gameObject.activeSelf;
        for(int i=1; i<_col.childCount; i++)
        {
            _col.GetChild(i).gameObject.SetActive(toggl);
        }
    }
    public void SetShownGizmos(Transform togglesParent)
    {
        for(int i=0; i<20; i++)
            gizParents.GetChild(i).gameObject.SetActive(togglesParent.GetChild(i).gameObject.GetComponent<Toggle>().isOn);
    }

}
