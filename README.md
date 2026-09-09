# Náš Seznam Přání
Neboli osobní rodinná wishlist aplikace.

**"Náš Seznam Přání"** je webová stránka (ke které zároveň existuje jednoduchá android aplikace využívající WebView), která vznikla pro účel zjednodušení procesu vzájemného vybírání dárků pro členů rodiny.
Aplikace primárně vznikla na požadavek mojí drahé Matky, které moc děkuji za příležitost procvičit si práci s ASP .NET server-side webem, a hlavně první příležitost tvořit web s reálným využitím.

## Technická stránka věci
Aplikace je postavená na ASP .NET MVC, běží na Free MS Azure hostingu, s Aiven (opět free) MySql databází.
Mezi funkce webu patří například:
- Schopnost získat náhledový obrázek přímo z odkazu na dárek.
- Nahrání vlastního obrázku, pokud velkoobchod zablokuje přístup k náhledu obrázku
- Uživatelské účty (pro rodinu) a vše co to obnáší (změna hesla, zašifrované heslo v databázi, etc.)
- Rezervace dárku, aby ostatní uživatelé věděli že dárek už někdo kupuje.

- Mimo jiné adaptivní zobrazení pro mobilní telefony nebo používání stránky přes dedikovanou mobilní aplikaci
