# DiaryRuSearcher

## About

Search desktop programm for [http://diary.ru](http://diary.ru).

## Motivation

I want to create search programm for this webservice, update my encoding skills, WPF skills, DataBinding skills, architect skills.

## Technical information

### Skills:
-  Parsing JSON with floating key and Json.Net library
-  Encoding request content in windows-1251
-  async await
-  Resources (for icons)
-  WPF via XAML (styles, DataBinding, DataTemplates, ListView (and GridView inner with cell template))
-  IValueConverter via StaticResource

### Uses
-  Visual Studio 2013
-  WPF
-  .NET 4.5
-  Newtonsoft.Json (Json.Net)
-  sqlite-net

### For Install Dependency
    install-package Newtonsoft.Json
    install-package sqlite-net
    System.Data.SQLite (Install with NuGet and next from [http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki](http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki) setup bundle for x86 (how support x64?) and manualy add reference to your project.)

### For compilation API Keys
    Write your API key (appkey and key variables) to: \\DiaryRuSearcher\\DiaryAPIClient\\DiaryAPIClientMain.cs