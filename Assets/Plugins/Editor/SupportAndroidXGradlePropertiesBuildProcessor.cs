using System.IO;
using UnityEditor.Android;
using UnityEngine;

/**
 * 让Unity支持androidx的插件
 */
public class SupportAndroidXGradlePropertiesBuildProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder
    {
        // 优先级
        get { return 999; }
    }
    public void OnPostGenerateGradleAndroidProject(string path)
    {
        Debug.Log("Bulid path : " + path);
        string gradlePropertiesFile = path + "/gradle.properties";
        if (File.Exists(gradlePropertiesFile))
        {
            File.Delete(gradlePropertiesFile);
        }
        StreamWriter writer = File.CreateText(gradlePropertiesFile);
        writer.WriteLine("org.gradle.jvmargs=-Xmx4096M");
        writer.WriteLine("android.useAndroidX=true");
        writer.WriteLine("android.enableJetifier=true");
        writer.Flush();
        writer.Close();
    }
}
