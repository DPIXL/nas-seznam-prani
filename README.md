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

## Náš Seznam Přání V1
- První prototyp aplikace, s hard-coded uživatelema a hodnotama, se základními funkcemi a bez velkého počtu QOL možností, jako přidání vlastního obrázku.

## Náš Seznam Přání V2
- Clean-up aplikace, aby fungovala více dynamicky, a příprava na expanzi pro veřejné použití.

## Náš Seznam Přání V3
- Mometálně probíhá práce na třetí verzi Našeho Seznamu Přáni, která bude podporovat vícero rodin, a přestane tím pádem být exkluzivně pro mé osobní použití.
- Mezi nové funkce bude patřit například tvorba vlastní "rodiny", registrace uživatele (momentálně je aplikace soukromá, takže tvorba uživatelů probíhá přímo skrze mě přes databázi), a možnost patřit do vícero rodin najednou. Přidá se registrační pohled, pohled pro tvorbu rodiny a pozvánky pro členy, a také pohled pro výběr rodiny, pokud patříte do více než jedné.
