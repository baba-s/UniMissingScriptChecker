using System;
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
		// イベント(static)
		//================================================================================
		/// <summary>
		/// Missing Script の情報をログ出力する時に呼び出されます
		/// </summary>
		public static event Action<MissingScriptData> OnLog;

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

			var list = Validate().ToArray();

			if ( list.Length <= 0 ) return;

			foreach ( var n in list )
			{
				if ( OnLog != null )
				{
					OnLog( n );
				}
				else
				{
					Debug.LogError( $"Missing Script が存在します：{n.GameObject.name}", n.GameObject );
				}
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
				var components = gameObject.GetComponents<Component>();

				foreach ( var component in components )
				{
					if ( component != null ) continue;

					var data = new MissingScriptData( gameObject );

					yield return data;
				}
			}
		}
	}
}