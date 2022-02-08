using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(ARAnchorManager))]
public class ARDrawManager : MonoBehaviour
{

    [SerializeField]
    private ARAnchorManager anchorManager = null;

    [SerializeField]
    private Material material = null;

    [SerializeField] 
    private Camera arCamera = null;

    [SerializeField]
    private ARLine line = null;

    private Color color = Color.white;

    public float width = 0.01f;

    public List<ARLine> undoList = new List<ARLine>();

    public List<ARLine> redoList = new List<ARLine>();

    private bool drawMode = true;

    private TextMeshProUGUI popUp = null;

    private Image popUpbackground = null;

    private Slider slider = null;

    void Start()
    {
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        popUp = GameObject.Find("TextField").GetComponent<TextMeshProUGUI>();
        popUpbackground = GameObject.Find("Popupbackground").GetComponent<Image>();
        popUpbackground.enabled = false;
        var pen = GameObject.Find("Pen").GetComponent<Button>().colors;
        pen.normalColor = new Color(0.3f,0.3f,0.3f);
    }

    void Update () 
    {
        if(drawMode) 
        {
            DrawMode();
        } else 
        {
            EraserMode();
        }
    }

    void showText(string Message) 
    {
        popUpbackground.enabled = true;
        popUp.text = Message;
        Invoke("hideText", 2f);
    }

    void hideText() 
    {
        popUpbackground.enabled = false;
        popUp.text = string.Empty;
    }

    void DrawMode ()
    {
        if (Input.touchCount == 0)
            return;   

        if(EventSystem.current.IsPointerOverGameObject(0)) 
            return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0.3f));
        
        if(touch.phase == TouchPhase.Began)
        {
            ARAnchor anchor = anchorManager.AddAnchor(new Pose(touchPosition, Quaternion.identity));

            line = new ARLine(transform, anchor, touchPosition, slider.value, color, material);
        }
        else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            line.AddPoint(touchPosition);
        }
        else if(touch.phase == TouchPhase.Ended)
        {
            undoList.Add(line);
            line.BakeTheMesh();
            // redoList.Clear();
        }
	}

    void EraserMode ()
    {
        if (Input.touchCount == 0)
            return;   

        if(EventSystem.current.IsPointerOverGameObject(0)) 
            return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = touch.position;

        if(touch.phase == TouchPhase.Began||touch.phase == TouchPhase.Moved)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;
            Debug.Log("check1");
            if (Physics.Raycast(ray, out hit, 3f)) 
            {
                Destroy(hit.transform.gameObject);
            }
        }
    }

    public void switchToPenMode() 
    {
        if(!drawMode) 
        {
            var pen = GameObject.Find("Pen").GetComponent<Button>().colors;
            var eraser = GameObject.Find("Eraser").GetComponent<Button>().colors;
            pen.normalColor = new Color(0.76f,0.76f,0.76f);
            eraser.normalColor = new Color(1f,1f,1f);
            drawMode = true;
            showText("Switched to Pen");
        }
        else return;
    }

    public void switchToEraserMode() 
    {
        if(drawMode) 
        {
            var pen = GameObject.Find("Pen").GetComponent<Button>().colors;
            var eraser = GameObject.Find("Eraser").GetComponent<Button>().colors;
            eraser.normalColor = new Color(0.76f,0.76f,0.76f);
            pen.normalColor = new Color(1f,1f,1f);
            drawMode = false;
            showText("Switched to Eraser");
        }
        else return;
    }

    public void colorRed()
    {
        this.color = Color.red;
        showText("Color red has been selected");
    }

    public void colorOrange()
    {
        this.color = new Color(1.0f, 0.64f, 0.0f);
        showText("Color orange has been selected");
    }

    public void colorYellow()
    {
        this.color = Color.yellow;
        showText("Color yellow has been selected");
    }

    public void colorGreen()
    {
        this.color = Color.green;
        showText("Color green has been selected");
    }


    public void colorCyan()
    {
        this.color = Color.cyan;
        showText("Color cyan has been selected");
    }

    public void colorBlue()
    {
        this.color = new Color(0.0f, 0.0f, 1.0f);
        showText("Color blue has been selected");
    }

    public void colorBlack()
    {
        this.color = Color.black;
        showText("Color black has been selected");
    }

    public void colorWhite()
    {
        this.color = Color.white;
        showText("Color white has been selected");
    }

    public void undo()
    {
        if(!undoList.Any())
        {
            return;
        }
        ARLine lastLine = undoList[undoList.Count-1];
        undoList.RemoveAt(undoList.Count-1);
        GameObject go = lastLine.go;     
        LineRenderer lastLinelr = go.GetComponent<LineRenderer>();
        this.redoList.Add(lastLine);
        go.SetActive(false);
        showText("Removed last Line");
    }

    public void redo()
    {
         if(!redoList.Any())
        {
            return;
        }
        ARLine lastLine = redoList[redoList.Count-1];
        redoList.RemoveAt(redoList.Count-1);
        GameObject go = lastLine.go;
        go.SetActive(true);
        showText("Redo last Line");
    }

}