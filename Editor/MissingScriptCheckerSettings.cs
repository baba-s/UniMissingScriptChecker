using System;
using UnityEditor;
using UnityEngine;

namespace UniMissingScriptChecker
{
	/// <summary>
	/// 設定を管理するクラス
	/// </summary>
	[Serializable]
	internal sealed class MissingScriptCheckerSettings
	{
		//================================================================================
		// 定数
		//================================================================================
		private const string KEY = "UniMissingScriptChecker";

		//================================================================================
		// 変数(SerializeField)
		//================================================================================
		[SerializeField] private bool   m_isEnable  = false;
		[SerializeField] private string m_logFormat = "参照が設定されていません：[GameObjectRootPath]";

		//================================================================================
		// プロパティ
		//================================================================================
		public bool IsEnable
		{
			get => m_isEnable;
			set => m_isEnable = value;
		}

		public string LogFormat
		{
			get => m_logFormat;
			set => m_logFormat = value;
		}

		//================================================================================
		// 関数(static)
		//================================================================================
		/// <summary>
		/// EditorPrefs から読み込みます
		/// </summary>
		public static MissingScriptCheckerSettings LoadFromEditorPrefs()
		{
			var json = EditorPrefs.GetString( KEY );
			var settings = JsonUtility.FromJson<MissingScriptCheckerSettings>( json ) ??
			               new MissingScriptCheckerSettings();

			return settings;
		}

		/// <summary>
		/// EditorPrefs に保存します
		/// </summary>
		public static void SaveToEditorPrefs( MissingScriptCheckerSettings setting )
		{
			var json = JsonUtility.ToJson( setting );

			EditorPrefs.SetString( KEY, json );
		}
	}
}