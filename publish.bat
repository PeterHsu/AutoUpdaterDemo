:: �n�w��7-Zip
:: �ؿ��W�٭n�M�����ɦW�٤@��(�w�])
SET projectName=AutoUpdaterDemo
DEL %projectName%.zip
:: �G�p�ɭn�Υ������Ҫ��պA 
Copy .\%projectName%\App.prod.config .\%projectName%\bin\Debug\%projectName%.exe.config /Y
:: ���Y�n�G�p���ɮ�
:: �ӤH�ߺD�պA����b.exe.config�A�ӬO��bConfig�ؿ��U
"C:\Program Files\7-Zip\7z.exe" a -tzip %projectName%.zip^
 .\%projectName%\bin\Debug\%projectName%.exe.config^
 .\%projectName%\bin\Debug\*.exe^
 .\%projectName%\bin\Debug\*.dll^
 .\%projectName%\bin\Debug\*.pdb^
 .\%projectName%\bin\Debug\Config\
 :: �A�N���ݭn���ɮױq���Y�ɲ���
 :: �Ҧpprofile.json�O�ϥΪ̦ۭq���պA���i�л\
"C:\Program Files\7-Zip\7z.exe" d %projectName%.zip^
 %projectName%.vshost.exe^
 Config\profile.json
 :: �����ۤv�Nzip�ɧ�W�A�Ҧp�HprojectName%.1.2018.0525.0.zip