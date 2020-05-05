# Uni Missing Script Checker

Missing Script が存在したら Unity を再生できなくするエディタ拡張

## 使用例

![2020-05-05_195706](https://user-images.githubusercontent.com/6134875/81059029-9de99f80-8f0a-11ea-9cf2-0525c53d2cfd.png)

![Image (17)](https://user-images.githubusercontent.com/6134875/81059033-9fb36300-8f0a-11ea-9077-d2688114bf98.gif)

## ログ出力のカスタマイズ

```cs
using UniMissingScriptChecker;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Example
{
    static Example()
    {
        MissingScriptChecker.OnLog += data =>
        {
            Debug.LogError( $"参照が設定されていません：{data.GameObject.name}", data.GameObject );
        };
    }
}
```

* Missing Script が存在する場合に出力されるログはカスタマイズできます  