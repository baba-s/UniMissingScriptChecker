using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniMissingScriptChecker
{
	/// <summary>
	/// Missing Script が存在したら Unity を再生できなくするエディタ拡張
	/// </summary>
	[InitializeOnLoad]
	public static class MissingScriptChecker
	{
		//================================================================================
		// クラス
		//================================================================================
		/// <summary>
		/// Missing Script の情報を管理するクラス
		/// </summary>
		public sealed class MissingScriptData
		{
			/// <summary>
			/// 参照が設定されていないパラメータを所持するコンポーネント
			/// </summary>
			public GameObject GameObject { get; }

			public MissingScriptData( GameObject gameObject )
			{
				GameObject = gameObject;
			}
		}

		//================================================================================
		// 関数(static)
		//================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		static MissingScriptChecker()
		{
			EditorApplication.playModeStateChanged += OnChange;
		}

		/// <summary>
		/// Unity のプレイモードの状態が変化した時に呼び出されます
		/// </summary>
		private static void OnChange( PlayModeStateChange state )
		{
			if ( state != PlayModeStateChange.ExitingEditMode ) return;

			var settings = MissingScriptCheckerSettings.LoadFromEditorPrefs();

			if ( !settings.IsEnable ) return;

			var list = Validate().ToArray();

			if ( list.Length <= 0 ) return;

			var logFormat = settings.LogFormat;

			foreach ( var n in list )
			{
				var message = logFormat;
				message = message.Replace( "[GameObjectName]", n.GameObject.name );
				message = message.Replace( "[GameObjectRootPath]", GetRootPath( n.GameObject ) );

				Debug.LogError( message, n.GameObject );
			}

			EditorApplication.isPlaying = false;
		}

		/// <summary>
		/// Missing Script の一覧を返します
		/// </summary>
		private static IEnumerable<MissingScriptData> Validate()
		{
			var gameObjects = Resources
					.FindObjectsOfTypeAll<GameObject>()
					.Where( c => c.scene.isLoaded )
					.Where( c => c.hideFlags == HideFlags.None )
				;

			foreach ( var gameObject in gameObjects )
			{
				var count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount( gameObject );

				if ( count <= 0 ) continue;

				var data = new MissingScriptData( gameObject );

				yield return data;
			}
		}

		/// <summary>
		/// ゲームオブジェクトのルートからのパスを返します
		/// </summary>
		private static string GetRootPath( this GameObject gameObject )
		{
			var path   = gameObject.name;
			var parent = gameObject.transform.parent;

			while ( parent != null )
			{
				path   = parent.name + "/" + path;
				parent = parent.parent;
			}

			return path;
		}
	}
}