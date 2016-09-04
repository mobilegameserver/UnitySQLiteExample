using UnityEngine;
using UnityEngine.UI;

using System.IO;

using SQLite4Unity3d;

public class test
{
	public int test_id { get; set; }
	public int test_seq { get; set; }
}

public class UnitySQLiteExample : MonoBehaviour
{

	public Text text;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Test()
	{
		var dbName = "sqlite3.sq3";
		var dbPath = string.Empty;

		var persistentDataPath = string.Format("{0}/{1}", Application.persistentDataPath, dbName);

#if UNITY_EDITOR
		dbPath = string.Format(@"Assets/StreamingAssets/{0}", dbName);

#elif UNITY_IOS
        if (File.Exists(persistentDataPath) == false)
        {
            var rawPath = Application.dataPath + "/Raw/" + dbName;
            File.Copy(rawPath, persistentDataPath);
        }
        dbPath = persistentDataPath;

#elif UNITY_ANDROID
        if (File.Exists(persistentDataPath) == false)
        {
            var androidPath = new WWW("jar:file://" + Application.dataPath + "!/assets/" + dbName);

            // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            while (!androidPath.isDone) { }  

            File.WriteAllBytes(persistentDataPath, androidPath.bytes);
        }
        dbPath = persistentDataPath;
#endif

		var dbConn = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

		int affectedRows = 0;

		//var cmdCreate = dbConn.CreateCommand("CREATE TABLE test (test_id INTEGER, test_seq INTEGER)");
		//affectedRows = cmdCreate.ExecuteNonQuery();
		//text.text = affectedRows.ToString();

		var cmdInsert = dbConn.CreateCommand("INSERT INTO test VALUES(?, ?)", 1, 2);
		affectedRows = cmdInsert.ExecuteNonQuery();
		text.text = affectedRows.ToString();

		var cmdSelect = dbConn.CreateCommand("SELECT test_id, test_seq FROM test");
		var rows = cmdSelect.ExecuteQuery<test>();

		Debug.Log(rows.Count);

		foreach (var row in rows)
		{
			Debug.Log(row.test_id);
			Debug.Log(row.test_seq);
		}

		dbConn.Close();
		dbConn.Dispose();

		Debug.Log("OK");
		text.text = "OK";
	}
}
