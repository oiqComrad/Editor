Чтобы запустить проект, вам нужно нужно поставить флажок в свойствах у файлов Form1.resx и Properties\Resource.resx.
Если после этого проект все равно не запуститься, попробуйте изменить значение <TargetFramework> в файле TextEditor.csproj c 
net5.0-windows на netcoreapp3.1. А так же изменить значение WarningLevel( от 0 до 4). Почему-то у меня он иногда слетал.

Доп. функционал у меня не реализован. Также у меня нет ни одного доп класса, кроме program.cs и form1.cs, потому что
у меня не получилось ни одного метода, который можно было бы засунуть в другой класс без нарушения инкапсуляции.
Прошу за это не снижать.
