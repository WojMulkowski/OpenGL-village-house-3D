### Projekt
## 1 Podstawowy model
Wszystkie dane modelu, tj. współrzędne wierzchołków, wektory normalne, współrzędne teksturowania są przekazywane bezpośrednio w kodzie (czyli nie korzystamy z gotowców z tematu 1). Można wykorzystać pliki obj. Jeżeli podstawowa geometria się nie zgadza, nie ocenia się kolejnych punktów.
5 p.Bryła domu
Bryła domu jest zgodna z opisem, tj. ma dwa piętra, trójkątny dach i komin.
5 p.Okna i drzwi
Okna i drzwi są w prawidłowych miejscach. Drzwi są tylko na jednej ścianie. Ona i drzwi stanowią oddzielny obiekt geometryczny, tj. nie są naklejką na ścianę, ale wgłębieniem lub uwypukleniem.
3 p.Obsługa klawiatury
Strzałki pozwalają obracać model domu wokół własnej osi.
## 2 Teksturowanie
Ocenie podlega poprawność doboru i nałożenia tekstur. Widoczne miejsca łączenia tekstur (poza oczywiście krawędziami) obniżają punktację.
2 p.Tekstura ścian
Ściany mają własną teksturę, stosowną dla tego typu obiektu (mogą być cegły, tynk, drewno itp.).
2 p.Tekstura dachu
Dach ma własną teksturę w postaci dachówek, strzechy lub podobnej. Musi być inna niż tekstura ścian.
2 p.Tekstura komina
Komin ma własną teksturę, różną od poprzednich.
4 p.Tekstura tła
Dom stoi pośrodku pewnego podłoża (trawy, pola itp.) a w tle widoczna jest linia horyzontu z teksturą nieba (dowolną). Poruszanie strzałkami nie porusza niebem. Dopuszczalny jest również brak obrotu podłoża.
2 p.Tekstura okien i drzwi
Okna i drzwi są narysowane przy pomocy tekstury. Dodatkowe punkty przyznaje się, gdy są oddzielnym geometrycznie obiektem (tj. okna i drzwi są 3D).
## 3 Cieniowanie
Przyznawane są punkty zależnie od zastosowanego modelu oświetlenia. Ocenie może podlegać więcej niż jeden model oświetlenia, pod warunkiem, że zastosowano więcej niż jeden program cieniujący w sposób sensowny.
5 p.Model światła rozproszonego
Poprawnie zaimplementowano model światła rozproszonego a większość obliczeń znajduje się w programie cieniującym fragmenty.
10 p.Model oświetlenia Phonga
Poprawnie zaimplementowano model oświetlenia Phonga a większość obliczeń znajduje się w programie cieniującym fragmenty.
5 p.Oddzielne programy cieniujące
Oświetlenie to zostało zastosowane do oświetlenia powierzchni błyszczących (np. okien, metalowego komina). Błyszczące elementy są oświetlane przez jeden program cieniujący wykorzystujący model Phonga, a pozostałe oświetla program cieniujący bez odbłysku z modelu Phonga.
5 p.Specjalistyczna tekstura
Oświetlenie to zostało zastosowane do oświetlenia powierzchni błyszczących (np. okien, metalowego komina). Do odróżnienia powierzchni błyszczących od pozostałych, zastosowano specjalistyczne tekstury.
