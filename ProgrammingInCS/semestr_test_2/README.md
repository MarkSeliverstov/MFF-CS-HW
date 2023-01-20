Zadání úlohy
Vaším úkolem je naprogramovat rozšířený preprocessor jazyka C#. Jedná se o nástroj, který přečte zdrojový soubor jazyka C#, řídí se speciálními direktivami preprocesoru začínajícími znakem křížek (#) a produkuje výstupní zdrojový kód C# do jiného souboru.

Pokud je program spuštěn s jiným počtem parametrů na příkazové řádce nežli 1, tak na standardní výstup vypíše text Missing argument, a ukončí se. Jediný parametr příkazové řádky je jméno souboru s příponou .cse. Pokud název souboru nemá správnou příponu, program na standardní výstup vypíše Unsupported file a ukončí se. Jinak program argument na příkazové řádce chápe jako jméno souboru, který se má zpracovat. Výsledek zpracování program uloží do výstupního souboru se stejným jménem, ale s příponou .cs. Tedy např. při použití Program.exe library.cse program vygeneruje soubor library.cs. Pokud vstupní soubor nelze otevřít nebo z něj nelze číst nebo při čtení dojde k libovolné chybě, program se korektně ukončí. Pokud program již začal s psáním do výstupního souboru v situaci kdy došlo k nějaké chybě, je povoleno nechat rozpracovaný výstup na disku.

Program pracuje ve dvou režimech, pasivní a aktivní, mezi kterými se přepíná v závislosti jaké instrukce čte. Program zpracovává vstupní soubor po řádcích. Pro každý řádek se podívá jestli první nebílý znak na řádku je křížek (#) a pokud ano, provede instrukci preprocesoru podle specifikace níže a vstupní řádek nezapisuje do výstupu. Pokud se nejedná o instrukci preprocesoru, a program je v aktivním režimu, pak okopíruje vstupní řádek do výstupního souboru beze změny. Pokud se nejedná o instrukci preprocesoru a program je v pasivním módu, tak se řádek ignoruje.

Direktivy preprocesoru
Program podporuje následující příkazy. Pro účely chybových hlášení předpokládejte následující konvenci:

Znak F značí jméno souboru, ve kterém došlo k chybě. Znak L značí číslo řádku, na kterém došlo k chybě.

#define Symbol
Pokud je program v aktivním režimu, pak se deklaruje existence symbolu z názvem Symbol. Pokud je symbol již deklarovaný nebo pokud je program v pasivním režimu, příkaz se ignoruje.

#undef Symbol
Pokud je program v aktivním režimu, pak se zapomene deklarace symbolu s názvem Symbol. Pokud symbol nebyl deklarovaný nebo pokud je program v pasivním režimu, příkaz se ignoruje.

#include file
Pokud je program v aktivním režimu, pak se zpracuje soubor s názvem file podle těchto pravidel (až na kontrolu správné přípony souboru) a zpracovaný výstup se vloží do aktuálního místa ve výstupním souboru (klidně na více řádků). Zpracování vnořeného souboru používá stejnou tabulku symbolů, tedy vnořený soubor může deklarovat nové nebo zapomínat staré symboly i pro zpracování jiných souborů. Pokud je program v pasivním režimu, příkaz se ignoruje.

Pokud soubor file neexistuje, nebo nejde otevřít, nebo z něj nelze číst, program vypíše následující text: F#L: #include invalid file name 'file' na standardní výstup a ukončí se.

Podmíněný příkaz #if, #else, #endif
Příkaz je rozdelen do několika řádků následujícím způsobem:

#if Symbol
Code1
#else
Code2
#endif
Sekvence Code1 reprezentuje všechny řádky mezi #if a #else. Sekvence Code2 reprezentuje všechny řádky mezi #else a #endif. Tyto sekvence můžou být rovněž prázdné (nicméně příkazy #if, #else i #endif musí být ukončeny znakem nového řádku a musí stát na řádku samostatně). Blok #else je nepovinný. Sémantika je pak stejná, jako by byl blok else (sekvence řádků Code2) prázdný.

Pokud je symbol Symbol deklarován, pak je kód mezi #if a #else (Code1) aktivní. V opačném případě je aktivní kód mezi #else a #endif (Code2). Program je v rámci podmíněného příkazu aktivní, pokud jsou aktivní všechny bloky, ve kterých se program momentálně nachází. Hlavní blok (mezi začátkem a koncem souboru zpracovávaného souboru) je vždy aktivní.

Je tedy nutné i v případě, že je program pasivní stále zpracovávat příkazy #if, #else, #endif a pamatovat si v kterých částech byl symbol definovaný nebo ne.

Pokud příkaz #if není ukončen příkazem #endif (tedy program narazí v rámci zpracování příkazu #if na konec souboru), program vypíše F#L: Missing #endif na standardní výstup a ukončí se, kde L je číslo posledního řádku v souboru.

Pokud se příkaz #else vyskytuje mimo blok #if-#endif, program vypíše F#L: Standalone #else na standardní výstup a ukončí se.

Pokud se příkaz #endif objeví bez párového předcházejícího příkazu #if, program vypíše F#L: Standalone #endif na standardní výstup a ukončí se.

Garance
Můžete předpokládat, že Symbol nikdy neobsahuje bílé znaky a vždy obsahuje alespoň jeden znak a je od příkazu oddělen alespoň jednou mezerou (tedy že příkazy #define, #undef a #if jsou vždy zadané správné)
Můžete předpokládat, že se podmíněný příkaz vyskytuje vždy celý v rámci jednoho souboru. Není nutné nijak řešit případy, kdy by část podmíněného příkazu byla v jiném souboru vloženém pomoci příkazu #include.
Můžete předpokládat, že ve vstupním programu nejsou žádné jiné direktivy preprocesoru než ty specifikované výše.
Příklady
Zpracování vstupního souboru input.cse s následujícím obsahem:

#define DEBUG
#define EXTRA_DEBUG

Console.WriteLine("Hello");

#if DEBUG
    Console.WriteLine("Debug");
  #if EXTRA_DEBUG
    Console.WriteLine("Extra");
  #endif
#else
    Console.WriteLine("Release");
#endif

#undef EXTRA_DEBUG

#if EXTRA_DEBUG
    Console.WriteLine("Extra");
#endif

return 0;
vyprodukuje výstupní soubor input.cs s následujícím obsahem:


Console.WriteLine("Hello");

    Console.WriteLine("Debug");
    Console.WriteLine("Extra");



return 0;