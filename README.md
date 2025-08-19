# Exchange Rate
Сайт написанный на Blazor (Server Side) для просмотра курса опредленной валюты с возможностью скачать данный курс в виде Excel таблицы. Данные берутся из API: https://www.exchangerate-api.com/
Данный API обновляет раз в 1.5 дня примерно.

## API Overview:
#### ExchangeRateController (/api)
 + GET /get/{baseCode}
 + GET /rates
 + GET /excel/{baseCode}

## To Run:
1) Откройте Redis в 6379 порту
2) Поменяйте API Key на свой в appsettings.json
3) dotnet run
4) Откройте в браузере http://localhost:5056

## Stack
  + C#
  + ASP.NET Core
  + Blazor + Radzen Components
  + Redis (для кеширования курсов валют из exchangerate-api)
  + EPPlus (для написания Excel файла)
  + Quartz.NET (для обновления курса валют)
