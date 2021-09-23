using UnityEditor;
using UnityEngine;

namespace com.CookieDeveloper.MyMonsterDK
{
	public class KeywordReplacer : UnityEditor.AssetModificationProcessor
	{
		public static void OnWillCreateAsset(string path)
		{
			path = path.Replace(".meta", "");
			int index = path.LastIndexOf(".");
			string file;
			try
			{
				file = path.Substring(index);
			}
			catch (System.ArgumentOutOfRangeException)
			{
				return;
			}
			if (!file.Contains(".cs")) return;
			index = Application.dataPath.LastIndexOf("Assets");
			path = Application.dataPath.Substring(0, index) + path;
			file = System.IO.File.ReadAllText(path);
			string[] splitPath = path.Split('/');
			string folder = splitPath[splitPath.Length - 2];

			file = file.Replace("#NAMESPACE#", "com." + PlayerSettings.companyName + "." + PlayerSettings.productName);
			file = file.Replace("#CREATIONDATE#", System.DateTime.Now + "");
			file = file.Replace("#PROJECTNAME#", PlayerSettings.productName);
			file = file.Replace("#DEVELOPERS#", PlayerSettings.companyName);

			System.IO.File.WriteAllText(path, file);
			AssetDatabase.Refresh();
		}
	}
}