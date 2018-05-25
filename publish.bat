:: 要安裝7-Zip
:: 目錄名稱要和執行檔名稱一樣(預設)
SET projectName=AutoUpdaterDemo
DEL %projectName%.zip
:: 佈署時要用正式環境的組態 
Copy .\%projectName%\App.prod.config .\%projectName%\bin\Debug\%projectName%.exe.config /Y
:: 壓縮要佈署的檔案
:: 個人習慣組態不放在.exe.config，而是放在Config目錄下
"C:\Program Files\7-Zip\7z.exe" a -tzip %projectName%.zip^
 .\%projectName%\bin\Debug\%projectName%.exe.config^
 .\%projectName%\bin\Debug\*.exe^
 .\%projectName%\bin\Debug\*.dll^
 .\%projectName%\bin\Debug\*.pdb^
 .\%projectName%\bin\Debug\Config\
 :: 再將不需要的檔案從壓縮檔移除
 :: 例如profile.json是使用者自訂的組態不可覆蓋
"C:\Program Files\7-Zip\7z.exe" d %projectName%.zip^
 %projectName%.vshost.exe^
 Config\profile.json
 :: 完成自己將zip檔改名，例如％projectName%.1.2018.0525.0.zip