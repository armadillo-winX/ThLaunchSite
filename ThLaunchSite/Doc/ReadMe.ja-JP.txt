--------------------------------------------------
東方管制塔 NX ver2.x.x

Copyright © 2023- 珠音茉白/東方管制塔開発部
--------------------------------------------------


§はじめに
本ツールは上海アリス幻樂団による「東方Project」作品群に特化したゲームランチャーです。


§機能紹介
東方管制塔 NX の主な機能を紹介します。

・ゲームを起動する
・VsyncPatch、thpracなど動作補助ツールを適用してゲームを起動する
・環境カスタムプログラムを起動する
・ゲームの実行履歴、プレイ時間を記録する
・ゲームのウィンドウを自由にリサイズする
　　初期のウィンドウサイズが固定された作品向けの機能です
・ゲームのウィンドウをキャプチャする
・各種外部ツールを登録し、起動する


§アイコンについて
アイコンに使われているキャラクターは東方管制塔のモチーフキャラになります。
キャラクターデザイン:藤花陽菜(https://x.com/Philo_messiah1)

アイコンに使われているイラストは珠音茉白が作成したものです。


§免責事項
2条項BSDライセンスが適用されています。
License.txtを参照してください。


§推奨動作環境
Microsoft Windows 10/11
.NET Desktop Runtime 8.0 が必要です。


§開発環境
Microsoft Windows 11 Insider Preview Canary
Microsoft Visual Studio Community 2022 Current
.NET 8.0


§対応作品
現在以下の作品に対応しています。
・東方紅魔郷　～ the Embodiment of Scarlet Devil.
・東方妖々夢　～ Perfect Cherry Blossom.
・東方永夜抄　～ Imperishable Night.
・東方花映塚　～ Phantasmagoria of Flower View.
・東方風神録　～ Mountain of Faith.
・東方地霊殿　～ Subterranean Animism.
・東方星蓮船　～ Undefined Fantastic Object.
・東方神霊廟　～ Ten Desires.
・東方輝針城　～ Double Dealing Character.
・東方紺珠伝　～ Legacy of Lunatic Kingdom.
・東方天空璋　～ Hidden Star in Four Seasons.
・東方鬼形獣　～ Wily Beast and Weakest Creature.
・東方虹龍洞　～ Unconnected Marketeers.
・東方獣王園　～ Unfinished Dream of All Living Ghost.

・東方文花帖　～ Shoot the Bullet.
・ダブルスポイラー　～ 東方文花帖
・妖精大戦争　～ 東方三月精
・弾幕アマノジャク　～ Impossible Spell Card.
・秘封ナイトメアダイアリー　～ Violet Detector.
・バレットフィリア達の闇市場 〜 100th Black Market.


§ソースコード
本ソフトウェアはオープンソースにて開発されており、ソースコードはGitHubで2条項BSDライセンスで公開されています。
https://github.com/armadillo-winX/ThLaunchSite


§フィードバック・お問い合わせ
フィードバックはGitHubリポジトリにissueを建てていただくとありがたいです。

GitHubをご利用でない場合、また、そのほかのお問い合わせに関しては開発者IOインスタンス(Misskey)アカウントないしXアカウントまで

開発者IOインスタンス(Misskey)アカウント
https://misskey.io/@mashironn

開発者Xアカウント
https://x.com/PhotonSak1054


§よくあるかもしれない質問

Q.東方管制塔とは?
A.東方Project作品群専用の高機能軽量ゲームランチャーです。

Q.ソースコードとかってありますか?
A.§フィードバック・お問い合わせの項にGitHubリポジトリへのリンクを貼ってあります。

Q.動かないです。
A.本ツールの動作には .NET Runtime 8.0 が必要ですので下のリンクからインストールしてみてください。
https://dotnet.microsoft.com/ja-jp/download/dotnet/8.0
それでも動かない場合、フィードバックをご提供いただくか、開発者に直接ご連絡ください。

Q.Is English supported?
A.Not now but I want to support English or other languages in the future.

Q.起動したら「~~の生成に失敗しました。」というメッセージが出ました。
A.以下のケースが考えられます。
・ThLaunchSite.exeが存在するフォルダのアクセス権が管理者権限もしくはそれ以上の権限に設定されている。

Q.起動したら「~~設定の構成に失敗しました。」というメッセージが出ました。
A.以下のケースが考えられます。
・settingsフォルダかsettingsフォルダ内の設定ファイルのアクセス権が管理者権限もしくはそれ以上の権限に設定されている。
・settingsフォルダかsettingsフォルダ内の設定ファイルが別のソフトウェアによって使用されていてThLaunchSite.exeがアクセスできない。
・設定ファイルが不正な、あるいは破壊されたxmlファイルである。

Q.エラーとか怖いしよくわかんない！
A.開発者にお気軽にご連絡ください。