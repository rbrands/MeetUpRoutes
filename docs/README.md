# Über MeetUpRoutes

Diese Web-App ist als Ergänzung zum [MeetUpPlanner](https://www.meetupplanner.de) gedacht, um Routen zu verwalten und ist als eine Art "Meta-Verzeichnis" oder einfacher als Link-Sammlung zu verstehen. MeetUpRoutes soll und kann natürlich nicht Komoot und ähnliche Web-Anwendungen ersetzen, sondern ergänzt diese, um eine Vereins/Club-spezifische Routensammlung anzulegen.

Der aktuelle Stand der Entwicklung von MeetUpRoutes kann am öffentlichen Projekt-Board [https://github.com/rbrands/MeetUpRoutes/projects/1](https://github.com/rbrands/MeetUpRoutes/projects/1) verfolgt werden.

## Berechtigungskonzept
Basis für den Zugriff auf die Routen und die Erstellung der Routen ist das folgende <em>Berechtigungskonzept</em>:
- Ohne weitere Anmeldung können die öffentlichen Routen eingesehen werden, es können keine Routen erstellt und kommentiert werden.
- Eine Benutzer:in kann sich authentifizieren über die Online-Services von Microsoft, Google, Twitter oder GitHub. Eine Benutzer:in muss dann aber noch bestätigt werden. Die Bestätigung kann automatisch erfolgen, wenn im Profil das Schlüsselwort zum MeetUpPlanner eingegeben wird. Alternativ kann eine Benutzer:in durch einen Club-Administrator bestätigt werden. Durch die Bestätigung wird eine Benutzer:in zum <em>Clubmitglied</em>. 
- Clubmitglieder (also bestätigte Benutzer:innen, s.o.) können neben den öffentlichen Routen auch Routen sehen, die nur für Clubmitglieder freigegeben wurden. Außerdem können Clubmitglieder Routen erstellen, kommentieren usw. Allerdings müssen neu estellte Routen noch freigegeben werden - durch einen Reviewer (s.u.) - bevor sie für andere Benutzer:innen/Mitglieder sichtbar sind.
- Durch das Feature "Nur sichtbar für Club-Mitglieder" können z.B. private Links aus Komoot geteilt werden, ohne dass die Komoot-Strecke öffentlich sein muss.
- Wenn ein <em>Clubmitglied</em> den Status <em>Autor:in</em> hat, werden erstellte Routen sofort veröffentlicht, d.h. müssen nicht erst durch einen <em>Reviewer</em> freigegeben werden.
- Ein <em>Reviewer</em> kann Routen, die von Clubmitgliedern erstellt werden für die Veröffentlichung freigeben.
- Eine <em>Club-Administrator:in</em> verwaltet die <em>Benutzer:innen</em>, d.h. vergibt den Status <em>Clubmitglied</em>, <em>"Autor:in"</em> und <em>Reviewer</em>.

## Tags
Weiteres Basisfeature von MeetUpRoutes ist die Vergabe von "<em>Tags</em>" zur Kategorisierung von Routen. Die Tags können flexibel von Club-Administrator:innen definiert werden. Dabei kann festgelegt werden, ob Tags aus einer Kategorie zwingend ausgewählt werden müssen oder optional sind. Außerdem können die Tags definiert werden, die nur mit "Reviewer"-Berechtigung vergeben werden können.
Die Tags können dann für eine Suche nach Routen verwendet werden.
