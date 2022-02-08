using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI;


public class EventManager : MonoBehaviour
{

    [SerializeField]
    private Material material = null;

    [SerializeField]
    private ARAnchorManager anchorManager = null;

    private TextMeshProUGUI popUp = null;

    const string LINE_SUB = "/line";

    private Image popUpbackground = null;

    // Get all Lines 
    GameObject[] GetAllLinesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Line");
    }

    private Button colorLibrary;
    private Button close;
    private Button blue;
    private Button red;
    private Button orange;
    private Button yellow;
    private Button green;
    private Button cyan;
    private Button black;
    private Button white;

    void Start()
    {
        popUpbackground = GameObject.Find("Popupbackground").GetComponent<Image>();
        popUpbackground.enabled = false;
        popUp = GameObject.Find("TextField").GetComponent<TextMeshProUGUI>();
        close = GameObject.Find("CloseLibrary").GetComponent<Button>();
        close.gameObject.SetActive(false);
        blue = GameObject.Find("Blue").GetComponent<Button>();
        blue.gameObject.SetActive(false);
        red = GameObject.Find("Red").GetComponent<Button>();
        red.gameObject.SetActive(false);
        orange = GameObject.Find("Orange").GetComponent<Button>();
        orange.gameObject.SetActive(false);
        yellow = GameObject.Find("Yellow").GetComponent<Button>();
        yellow.gameObject.SetActive(false);
        green = GameObject.Find("Green").GetComponent<Button>();
        green.gameObject.SetActive(false);
        cyan = GameObject.Find("Cyan").GetComponent<Button>();
        cyan.gameObject.SetActive(false);
        black = GameObject.Find("Black").GetComponent<Button>();
        black.gameObject.SetActive(false);
        white = GameObject.Find("White").GetComponent<Button>();
        white.gameObject.SetActive(false);

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

    // Destroy all Lines
    public void DestroyLines()
    {
        GameObject[] lines = GetAllLinesInScene();
        foreach (GameObject currentLine in lines)
        {
            LineRenderer line = currentLine.GetComponent<LineRenderer>();
            Destroy(currentLine);
        }
        showText("All Lines have been destroyed");
    }


    public void SaveLines() 
    {
        GameObject[] lines = GetAllLinesInScene();
        string path = Application.persistentDataPath + LINE_SUB + SceneManager.GetActiveScene().buildIndex;

        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
        {
            writer.Write(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                LineRenderer lr = lines[i].GetComponent<LineRenderer>();
                LineData data = new LineData(lr);
                Color savedColor = lr.startColor;
                writer.Write(savedColor.r);
                writer.Write(savedColor.g);
                writer.Write(savedColor.b);
                writer.Write(savedColor.a);
                writer.Write(lr.startWidth);
                writer.Write(data.positionCount);
                for (int j = 0; j < data.positionCount; j++)
                {
                    writer.Write(data.positions[j].x);
                    writer.Write(data.positions[j].y);
                    writer.Write(data.positions[j].z);
                    Debug.Log($"Saveposition: {data.positions[j]}");
                }
            }
        }
        showText("Saved all Lines");
        Debug.Log($"Saved Binary to {path}");
    }


    public void LoadLines() 
    {
        string path = Application.persistentDataPath + LINE_SUB + SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Starting loading process");
        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            var linesCount = reader.ReadInt32();
            Debug.Log($"linesCount: {linesCount}");
            for (int i = 0; i < linesCount; i++)
            {
                Color savedColor = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Debug.Log($"color: {savedColor.ToString()}");
                float width = reader.ReadSingle();
                Debug.Log($"width: {width}");
                var positionsCount = reader.ReadInt32();
                Debug.Log($"positionCount: {positionsCount}");
                Vector3[] positions = new Vector3[positionsCount];
                Vector3 initialTouchPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Debug.Log($"initialTouchPosition: {initialTouchPosition}");
                ARAnchor anchor = anchorManager.AddAnchor(new Pose(initialTouchPosition, Quaternion.identity));
                positions[0] = initialTouchPosition;
                ARLine line = new ARLine(transform, anchor, initialTouchPosition, width, savedColor, material);
                for (int j = 1; j < positionsCount; j++)
                {
                    positions[j] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Debug.Log($"loadedpositions: {positions[j]}");
                }
                GameObject go = line.go;     
                LineRenderer lineRender = go.GetComponent<LineRenderer>();
                Debug.Log($"positions array: {positions}");
                lineRender.positionCount = positions.Length;
                lineRender.SetPositions(positions);
            }
        }
        showText("Loaded all Lines");
        Debug.Log($"Loaded Binary from {path}");
    }

    public void OpenColorLibrary() 
    {
        colorLibrary = GameObject.Find("ColorLibrary").GetComponent<Button>();
        colorLibrary.gameObject.SetActive(false);

        
        close.gameObject.SetActive(true);
        blue.gameObject.SetActive(true);
        red.gameObject.SetActive(true);
        orange.gameObject.SetActive(true);
        cyan.gameObject.SetActive(true);
        green.gameObject.SetActive(true);
        black.gameObject.SetActive(true);
        white.gameObject.SetActive(true);
        yellow.gameObject.SetActive(true);
    }

    public void CloseColorLibrary() 
    {
        colorLibrary.gameObject.SetActive(true);

        close.gameObject.SetActive(false);
        blue.gameObject.SetActive(false);
        red.gameObject.SetActive(false);
        orange.gameObject.SetActive(false);
        cyan.gameObject.SetActive(false);
        green.gameObject.SetActive(false);
        black.gameObject.SetActive(false);
        white.gameObject.SetActive(false);
        yellow.gameObject.SetActive(false);
    }

    public void ClearPopUp() 
    {
        popUp.text = string.Empty;
    }
}
