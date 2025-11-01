# 🎙️ ScenarioVoice Lite

**ScenarioVoice Lite** は、  
日本語音声合成エンジン「VOICEVOX」と「AquesTalk」に対応した  
テキスト読み上げ・動画編集ソフト連携ツールです。

※ScenarioVoice Liteは、ScenarioVoiceから一括合成機能を削りシンプルな音声合成・動画編集連携に特化しています。よりシンプルに音声合成したい方、TTSの仕組みを学んでみたい方向けに作成しました。

---

## 🚀 主な機能

- **テキスト文章から音声ファイルを生成**
  - VOICEVOX または AquesTalk に対応  
  - 各種パラメータ（話速・音高・音量・抑揚など）の調整が可能  
  - アクセントの編集にも対応
  - エコー処理にも対応

- **プリセット機能**
  - よく使う音声設定を保存・呼び出し可能  

- **動画編集ソフトとの連携**
  - [AviUtl](https://spring-fragrance.mints.ne.jp/aviutl/)  
    - 利用には別途「かんしくん」が必要です  
  - [ゆっくりMovieMaker4（YMM4）](https://manjubox.net/ymm4/) とも連携可能  

---

## 🔧 動作環境

- Windows 10 / 11  
- .NET Framework 4.7.2 以降  
- VOICEVOX / AquesTalk（DLL追加で対応）

---

## 💡 使い方
1. VOICEVOX起動後、本アプリを起動します。  
2. 字幕テキストを入力します。  
3. 音声を選択します。  
4. 各種パラメータ・アクセントを編集します。
5. 音声合成をクリックします。
6. 再生ボタンを押下すると音声を再生します。

---

## 📸 スクリーンショット
メイン画面<BR>
<img width="663" height="494" alt="image" src="https://github.com/user-attachments/assets/72fd98f0-6d01-4dc9-9c5d-30c37df9404b" />

---

## ⚙️ AquesTalk の利用について

- AquesTalk に関する DLL は本リポジトリには含まれていません。  
- 利用される場合は、株式会社アクエストの公式サイトから  
  正規の DLL を入手し、ライセンス条件をご確認ください。  

> **注:**  
> AquesTalk のライセンスや利用条件は株式会社アクエストが定める規約に従います。  

---

## 🔊 VOICEVOX の利用について

本アプリは、**VOICEVOX の中間データを利用して音声合成を行っています。**  
利用者は VOICEVOX の利用規約を必ずご確認ください。  

- [VOICEVOX 公式サイト](https://voicevox.hiroshiba.jp/)  
- [利用規約](https://voicevox.hiroshiba.jp/term/)

---

## 🧩 技術スタック
- Windows / .NET Framework（4.7.2以降 で動作想定）
- C#で開発したWindowsアプリケーション
- 利用ライブラリ：NLog/NAudio/Newsonsoft.Json

---

## 📘 ライセンス

本ソフトウェアは **MIT License** で公開しています。  
詳細は [LICENSE](./LICENSE) ファイルをご確認ください。

---

## 🌐 詳細・更新情報

詳しい使い方や更新履歴は、以下の公式ホームページをご覧ください。  
全ての機能を利用可能なフルバージョンは、ホームページからダウンロード先へのリンクがあります。  
👉 **[公式サイトはこちら](http://www.yasui-kamo.com/labo/ttsutility/)**
