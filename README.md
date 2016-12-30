#Name
HAPControleApplication  

#詳細
HAPをWindowsから操作するリモコンアプリです。  
Windows 10/HAP-S1で動作確認をしています。 HAP-Z1ESで動作するかは未確認です。  
Windows Vista/7/8/8.1/10で動作するようです。  
動作には.NET Framework 4.6が必要です。インストールしていない場合はインストールしてから起動する必要があります。
現在ベータ版です。  
1. IPアドレスを一度間違えると強制終了し、今後起動しなくなります。
2. HAPのIPアドレスの変更はサポートしていませんので、IPアドレスが変わるとアプリケーションが起動しなくなります。  
(これらの場合は、マイドキュメントフォルダ直下のHapControleAppフォルダ内のipaddファイルを削除することによって再度手動入力できるようになります。)
3. たまに、HAPがHTTPエラーの500を返すことがあります。その場合、アプリが強制終了します。
4. ウィンドウサイズの変更はサポートしていません。
5. ある程度の長さまでの曲名、アーティスト名、アルバム名であれば折り返して表示しますが、とても長い場合は見切れます。
6. 他にも気づいていないだけで、バグがあるかもしれません。  
