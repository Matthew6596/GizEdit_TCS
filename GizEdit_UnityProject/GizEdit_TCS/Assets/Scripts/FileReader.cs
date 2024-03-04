using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Windows;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using TMPro;

public class FileReader : MonoBehaviour
{

    public GameObject loadingTxt, backBtn, mainMenuGroup, optionsGroup, loadBar;
    public TMP_Dropdown greyboxDrop;

    private GUIStyle style;
    private string path = "";
    private bool windowOpen = false, selectingOption = true, creatingNew = false;
    byte[] fbytes;
    int greyboxLvl;

    GameManager gm;

    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 20;

        style.normal.textColor = Color.white;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnGUI()
    {
        //Instructions
        //GUI.Label(new Rect(10, 10, Screen.width*(2/3), Screen.height*(2/3)), "Press [spacebar] to open File Selection Window.", style);
        //GUI.Label(new Rect(10, 60, Screen.width*(2/3), Screen.height*(2/3)), "LOADING FILE...", style);

    }

    //Menu and input methods
    public void BackBtn()
    {
        if (creatingNew)
        {
            selectingOption = true;
            mainMenuGroup.SetActive(true);
            optionsGroup.SetActive(false);
            creatingNew = false;
        }
        else
        {
            selectingOption = false;
            optionsGroup.SetActive(false);
        }
    }

    public void LoadBtn()
    {
        selectingOption = false;
        mainMenuGroup.SetActive(false);
    }

    public void NewBtn()
    {
        //SceneManager.LoadScene("GizView");
        creatingNew = true;
        optionsGroup.SetActive(true);
        mainMenuGroup.SetActive(false);
        selectingOption = true;
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    public void StartBtn()
    {
        loadingTxt.SetActive(true);
        optionsGroup.SetActive(false);
        //StartCoroutine(pauseBeforeFileLoad(this.path));
    }

    public void GreyBoxDrop()
    {
        gm.greyboxLvl = greyboxDrop.value;
    }

    // Update is called once per frame
    void Update()
    {

        //if we don't have an open window yet, and the spacebar is down...
        if (!windowOpen&&!selectingOption)
        {
            FileSelector.GetFile(GotFile, ".GIZ"); //generate a new FileSelector window
            windowOpen = true; //record that we have a window open
        }
    }

    //This is called when the FileSelector window closes for any reason.
    //'Status' is an enumeration that tells us why the window closed and if 'path' is valid.
    void GotFile(FileSelector.Status status, string _path)
    {
        if (status == FileSelector.Status.Successful)
        {
            this.path = _path;
            optionsGroup.SetActive(true);
            selectingOption = true;
            
        }else if(status == FileSelector.Status.Cancelled)
        {
            selectingOption = true;
            mainMenuGroup.SetActive(true);
        }
        this.windowOpen = false;
    }
}
