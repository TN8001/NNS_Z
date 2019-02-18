まさかの更新 
# NNS_Z ニコ生サーチ(雑) on the Edge
![アプリスクリーンショット](AppImage.png)
## 概要
[Windows Community Toolkit](https://github.com/Microsoft/WindowsCommunityToolkit)にEdgeベースのWebViewが入ったので、試用がてら動かなくなっていたNNS_Zをバージョンアップ。  
WebViewを使ってみたかっただけなので実用性は低いです。（[NNS_T](https://github.com/TN8001/NNS_T)をお使いください）  
MSHTMLの依存がなくなったので一応バイナリ用意しました（Windows 10 April 2018 Update以降が必要 だと思います）

## ライセンス
CC0 1.0 Universal

[![CC0](http://i.creativecommons.org/p/zero/1.0/88x31.png)](LICENSE)
## 注意事項
* 一切責任は持ちません
* 不要要素の非表示中心であまりレイアウトはいじっていませんが、環境によっては表示が崩れるかもしれません
* 検索オプションは保存されません
* 実行ファイルのフォルダに設定ファイルを作るので書き込めるフォルダにしてください（Program Files等は避けてください）

## WebViewの感想
WebBrowserのレンダリングエンジンをIEからEdgeに変更できればよかったのでしょうがいろいろ難しいのでしょう、WebViewは全く別のコントロールです。

最低限のイベントはありますが、操作周りはInvokeScriptAsyncによるJavaScriptの実行に頼るしかありません。  
JS全くわからない身としては「勘弁してくれよ」と言いたいですねｗ



旧版が動かなかったので直接比較はできていませんが、WebBrowserに比べcssの挿入タイミング（NavigatedとDOMContentLoaded）が遅い（タイミングでなく反映か？）ような印象です。  
そのため再検索時に表示が大変バタバタします。


方法があるとは思いますが、Edgeの開発者ツールの出し方がわからずデバッグが面倒でした。

---
感想としては「あんまりうれしくないなぁ」となりますが、.NET Framework4.8 XAML Islandsには非常に期待感を持っています。  
UWPは制限事項が多く、クラシックデスクトップアプリを選択していましたがUWPでしかできない（できても非常に面倒な）ことも増えてきてモヤモヤしていたところ、向こうから歩み寄りがあるとは思っていませんでした。
