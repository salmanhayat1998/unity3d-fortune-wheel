using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

public class wheelWindow : MonoBehaviour
{
    [MenuItem("Fortune Wheel/Add Spin Wheel")]
    public static void addSpinWheel()
    {
        var parent = FindObjectOfType<Canvas>();
        if (parent)
        {
            SpawnWheelPrefab(parent.transform);
           
        }
        else
        {
            createCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1080, 1920), ScreenMatchMode.MatchWidthOrHeight);
            parent = FindObjectOfType<Canvas>();
            SpawnWheelPrefab(parent.transform);
        }
    }
    private static void SpawnWheelPrefab(Transform parent)
    {
        var prefab = AssetDatabase.LoadAssetAtPath("Assets/Wheel/Prefabs/Background.prefab", typeof(GameObject));
        GameObject g = Instantiate(prefab, parent) as GameObject;
        g.name = g.name.Replace("(Clone)", "");
    }
    private static void createCanvas(RenderMode renderMode,Vector2 res,ScreenMatchMode screenMatchMode) 
    {
        GameObject canvas = new GameObject("FortuneWheel Canvas");
        canvas.AddComponent<Canvas>().renderMode= renderMode;
        CanvasScaler cs=canvas.AddComponent<CanvasScaler>();
        cs.referenceResolution = res;
        cs.screenMatchMode = screenMatchMode;
        canvas.AddComponent<GraphicRaycaster>();
        GameObject eventsystem = new GameObject("Event System");
        eventsystem.AddComponent<EventSystem>();
        eventsystem.AddComponent<StandaloneInputModule>();

    }

   
}
