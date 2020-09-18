using System.Collections;
using System.Collections.Generic;
using SWS;
using UnityEngine;
using UnityEditor;
public class WayPointNormalFixed : EditorWindow
{
    private LayerMask hitMask;

    private GameObject facePoint;

    private PathManager pathManager;
    private Transform[] pointTrans;

    private Mesh showMesh;

    List<GameObject> selectGameObjects = new List<GameObject>();

 
    //打开窗口
    [MenuItem("Window/路径点朝向修正")]
    static void Open()
    {
        var window = (WayPointNormalFixed)EditorWindow.GetWindow(typeof(WayPointNormalFixed), false, "路径点朝向修正");
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += SceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= SceneGUI;
    }
    private void SceneGUI(SceneView sceneView)
    {

        if (Event.current.type == EventType.MouseDown
            && Event.current.button == 0)
        {
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            UnityEngine.Debug.DrawRay(ray.origin, ray.direction, Color.blue, 10);
            if (Physics.Raycast(ray, out hit))
            {
                UnityEngine.Debug.Log(hit.collider.gameObject);
            }
        }


        GameObject[] selects = Selection.gameObjects;
        selectGameObjects.ForEach((a) =>
        {
            var mr = a.GetComponent<MeshRenderer>();
            if (mr && mr.sharedMaterial)
                mr.material.color = Color.red;
        });
        selectGameObjects.Clear();
        for (int i = 0; i < selects.Length; i++)
        {
            if (selects[i].name.StartsWith("Waypoint"))
            {
                selectGameObjects.Add(selects[i]);
            }
        }
        selectGameObjects.ForEach((a) =>
        {
            var mr = a.GetComponent<MeshRenderer>();
            if (mr && mr.sharedMaterial)
                mr.material.color = Color.blue;
        });
        if (facePoint != null)
        {
            selectGameObjects.ForEach((a) => { Debug.DrawLine(a.transform.position, facePoint.transform.position, Color.green); });
           
        }
       
    }
    void OnGUI()
    {
        EditorGUILayout.LabelField("选择路径点--PathManager的物体:");
         
        pathManager = (PathManager)EditorGUILayout.ObjectField(pathManager, typeof(PathManager));
        if (pathManager)
        {
            pointTrans = pathManager.waypoints;
        }

        if (pointTrans==null||pointTrans.Length==0)
        {
            return;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("替换的Mesh网格:");
        showMesh = (Mesh)EditorGUILayout.ObjectField(showMesh, typeof(Mesh));
       
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("显示路径点"))
        {
            ShowOrHidePoint(true);
        }
        if (GUILayout.Button("删除显示"))
        {
            ShowOrHidePoint(false);
        }
        EditorGUILayout.EndHorizontal();
        facePoint=(GameObject)EditorGUILayout.ObjectField(facePoint, typeof(GameObject));
        if (facePoint==null)
        {
            return;
        }
        if (GUILayout.Button("开始修正"))
        {
            int sucessCount=0;
            int failedCount = 0;
            for (int i = 0; i < selectGameObjects.Count; i++)
            {
                GameObject sg = selectGameObjects[i];
                //--获取修正朝向
                Vector3 tempDir = (sg.transform.position-facePoint.transform.position).normalized;

                RaycastHit tempOneHit;
                if (Physics.Raycast(sg.transform.position- tempDir, tempDir, out tempOneHit,
                    10f))
                {
                    sg.transform.forward=tempOneHit.normal;
                    sg.transform.position = tempOneHit.point;
                }
                else
                {
                    Debug.LogError("修正失败，"+sg.name);
                    failedCount++;
                }
            }
            EditorUtility.DisplayDialog("朝向修正",string.Format("修正成功:{0}个，失败:{1}个", sucessCount, failedCount) , "确认");
        }
    }

    void ShowOrHidePoint(bool isShow)
    {
        if (pathManager==null|| pathManager.waypoints==null)
        {
            return;
        }
        for (int i = 0; i < pathManager.waypoints.Length; i++)
        {
            GameObject go = pathManager.waypoints[i].gameObject;
            go.transform.localScale=Vector3.one;
            var mf = go.GetComponent<MeshFilter>();
            var mr= go.GetComponent<MeshRenderer>();
            if (isShow)
            {
                var mat= new Material(Shader.Find("Unlit/TrueColor"));
                if (mf==null)
                {
                    var tempmf=go.AddComponent<MeshFilter>();
                    tempmf.sharedMesh = showMesh;
                }
                if (mr==null)
                {
                    var tempmr= go.AddComponent<MeshRenderer>();
                    tempmr.sharedMaterial = mat;
                }
            }
            else
            {
                if (mf)
                {
                    Object.DestroyImmediate(mf);
                }
                if (mr)
                {
                    Object.DestroyImmediate(mr);
                }
            }
        }
       
    }

}
