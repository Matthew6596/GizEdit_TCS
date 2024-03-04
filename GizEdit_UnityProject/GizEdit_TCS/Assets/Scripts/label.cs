using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class label : MonoBehaviour
{
    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startDelay());
        cam = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        transform.LookAt(cam);
    }

    IEnumerator startDelay()
    {
        yield return new WaitForSeconds(0.9f);
        transform.GetChild(1).GetComponent<TMP_Text>().text = gameObject.name;
        Transform b1 = transform.GetChild(0), b2 = transform.GetChild(2);
        float len = gameObject.name.Length * 0.005f;
        b1.localScale = new Vector3(len, b1.localScale.y, b1.localScale.z);
        b2.localScale = new Vector3(len + .005f, b2.localScale.y, b2.localScale.z);
    }
}
