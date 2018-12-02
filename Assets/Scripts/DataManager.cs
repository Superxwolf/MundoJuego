/*
#if !(UNITY_IOS || UNITY_ANDROID)
  #define TestHash
#endif
*/
using UnityEngine;
using MiniJSON;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

public abstract class DataManager
{
	public class Data
	{
		string filePath;
		Dictionary<string, object> Values;

		public Data()
		{
			Values = new Dictionary<string, object>();
		}

		public Data(string filePath)
		{
			this.filePath = filePath;
			Values = Load(filePath);
		}

		public T Get<T>(string key)
		{
			if (!Values.ContainsKey(key))
			{
				Debug.LogWarning("Key \"" + key + "\" doesn't exist in DataManager");
				return default(T);
			}
			object value = Values[key];

			if (value is T) return (T)value;

			else
			{
				try
				{
					return (T)Convert.ChangeType(value, typeof(T));
				}
				catch
				{
					Debug.LogWarning("Key \"" + key + "\" isn't of type \"" + typeof(T) + "\" in DataManager");
					return default(T);
				}
			}
		}

		public void Set<T>(string key, T value)
		{
			if (Values.ContainsKey(key)) Values[key] = value;
			else Values.Add(key, value);
		}

		public List<T> GetList<T>(string key)
		{
			if (!Values.ContainsKey(key))
			{
				Debug.LogWarning("Key \"" + key + "\" doesn't exist in DataManager");
				return default(List<T>);
			}
			object value = Values[key];

			if (value is List<T>) return (List<T>)value;

			else
			{
				try
				{
					return (List<T>)Convert.ChangeType(value, typeof(List<T>));
				}
				catch
				{
					Debug.LogWarning("Key \"" + key + "\" isn't of type \"" + typeof(List<T>) + "\" in DataManager");
					return default(List<T>);
				}
			}
		}

		public bool Exists(string key)
		{
			return Values.ContainsKey(key);
		}

		public bool IsType<T>(string key, bool strict = true)
		{
			if (!Values.ContainsKey(key))
			{
				Debug.LogWarning("Key \"" + key + "\" doesn't exist in DataManager");
				return false;
			}
			object value = Values[key];

			if (value is T) return true;

			if (!strict)
			{
				try
				{
					T test = (T)Convert.ChangeType(value, typeof(T));
					return true;
				}
				catch
				{
					return false;
				}
			}

			return false;
		}

		public void Save()
		{
			if(string.IsNullOrEmpty(filePath))
			{
				Debug.LogError("Trying to save a temporary Data, please assign it a filePath first");
			}
			DataManager.Save(DataManager.filePath, GetValues());
		}

		public string getFilePath()
		{
			return filePath;
		}

		public void setFilePath(string newFilePath)
		{
			filePath = newFilePath;
		}

		public Dictionary<string, object> GetValues()
		{
			return Values;
		}
	}

    static Dictionary<string, object> tempValues = new Dictionary<string, object>();

    public static void TempSet<T>(string key, T value)
    {
        if (tempValues.ContainsKey(key)) tempValues[key] = value;
        else tempValues.Add(key, value);
    }

    public static T TempGet<T>(string key)
    {
        if (!tempValues.ContainsKey(key))
        {
            Debug.LogWarning("Temporary key \"" + key + "\" doesn't exist in DataManager");
            return default(T);
        }
        object value = tempValues[key];

        if (value is T) return (T)value;

        else
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                Debug.LogWarning("Key \"" + key + "\" isn't of type \"" + typeof(T) + "\" in DataManager");
                return default(T);
            }
        }
    }

    #region File Managers
#if TestHash || ((UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR)
    class DataWriter
    {
        BinaryWriter writer;
        public DataWriter(string filePath)
        {
            writer = new BinaryWriter(File.Open(filePath, FileMode.Create));
        }

        public void Write(byte[] arr)
        {
            writer.Write(arr);
        }

        public void Write(string str)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(str);
            string encodedText = Convert.ToBase64String(bytesToEncode);
            writer.Write(encodedText);
        }

        public void Close()
        {
            writer.Close();
        }
    }

    class DataReader
    {
        BinaryReader reader;

        public DataReader(string filePath)
        {
            reader = new BinaryReader(File.Open(filePath, FileMode.Open));
        }

        public void Read(byte[] buf, int index, int count)
        {
            reader.Read(buf, index, count);
        }

        public string ReadString()
        {
            string encodedText = reader.ReadString();
            byte[] decodedBytes = Convert.FromBase64String(encodedText);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);
            return decodedText;
        }

        public void Close()
        {
            reader.Close();
        }
    }
#else
    class DataWriter
{
    StreamWriter writer;
    public DataWriter(string filePath)
    {
        writer = new StreamWriter(filePath);
    }

    public void Write(byte[] arr) {}

    public void Write(string str)
    {
        writer.Write(str);
    }

    public void Close()
    {
        writer.Close();
    }
}

class DataReader
{
    StreamReader reader;

    public DataReader(string filePath)
    {
        reader = new StreamReader(filePath);
    }

    public void Read(byte[] buf, int index, int count)
    {
        for (int i = 0; i < count; i++)
            buf[i] = 0;
    }

    public string ReadString()
    {
        return reader.ReadToEnd();
    }

    public void Close()
    {
        reader.Close();
    }
}
#endif
#endregion

#if UNITY_EDITOR
  #if TestHash
    static string filePath = Application.dataPath + "/SaveDataHashed.txt";
  #else
    static string filePath = Application.dataPath + "/SaveData.txt";
#endif
#else
    static string filePath = Application.persistentDataPath + "/Game.dat";
#endif

	static string salt1 = "&*$";
	static string salt2 = "%=^";

	static Data GameData;

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
		GameData = new Data(filePath);
        //Set("appVersion", 1.1f);
    }

	private static Dictionary<string, object> Load(string filePath)
	{
		if (File.Exists(filePath))
		{
			DataReader file = null;
			try
			{
				file = new DataReader(filePath);

				byte[] hash = new byte[20];
				file.Read(hash, 0, 20);

				string json = file.ReadString();

				if (hash.SequenceEqual(Hash(json)))
				{
					var v = Json.Deserialize(json) as Dictionary<string, object>;

					if (v == null)
						return new Dictionary<string, object>();

					return v;
				}

				else
				{
					return new Dictionary<string, object>();
				}
			}
			catch
			{
				return new Dictionary<string, object>();
			}
			finally
			{
				if (file != null) file.Close();
			}
		}
		else
		{
			return new Dictionary<string, object>();
		}
	}

    public static void Set<T>(string key, T value)
    {
		GameData.Set(key, value);
    }

    public static T Get<T>(string key)
    {
		return GameData.Get<T>(key);
    }

    public static List<T> GetList<T>(string key)
    {
		return GameData.GetList<T>(key);
    }

    public static void Save()
    {
		Save(filePath, GameData.GetValues());
    }

	public static void Save(string filePath, Data data)
	{
		Save(filePath, data.GetValues());
	}

	public static void Save(string filePath, Dictionary<string, object> values)
	{
		string json = Json.Serialize(values);
		var file = new DataWriter(filePath);
		file.Write(Hash(json));
		file.Write(json);
		file.Close();
	}

    public static bool Exists(string key)
    {
		return GameData.Exists(key);
    }

    public static bool IsType<T>(string key, bool strict = true)
    {
		return GameData.IsType<T>(key, strict);
    }

    static byte[] Hash(string input)
    {
#if TestHash || !UNITY_EDITOR
        return (new SHA1Managed()).ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt1+input+salt2));
#else
        // 20 bytes to match the length of the read buffer at hash verification
        return new byte[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
#endif
    }
}