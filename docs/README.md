# Über MeetUpRoutes

Diese Web-App ist als Ergänzung zum [MeetUpPlanner](https://www.meetupplanner.de) gedacht, um Touren zu verwalten und ist als eine Art "Meta-Verzeichnis" oder einfacher als Link-Sammlung zu verstehen. MeetUpRoutes soll und kann natürlich nicht komoot und ähnliche Web-Anwendungen ersetzen, sondern ergänzt diese, um eine Vereins/Club-spezifische Tourensammlung anzulegen.

Der aktuelle Stand der Entwicklung von MeetUpRoutes kann am öffentlichen Projekt-Board https://github.com/rbrands/MeetUpRoutes/projects/1 verfolgt werden.

Basis für den Zugriff auf die Touren und die Erstellung der Touren ist das folgende <b>Berechtigungskonzept</b>:
- Ohne weitere Anmeldung können die öffentlichen Touren eingesehen werden, es können keine Touren erstellt und kommentiert werden.
- Eine Benutzer:in kann sich authentifizieren über die Online-Services von Microsoft, Google, Twitter oder GitHub. Eine Benutzer:in muss dann aber noch bestätigt werden. Die Bestätigung kann automatisch erfolgen, wenn im Profil das Schlüsselwort zum MeetUpPlanner eingegeben wird. Alternativ kann eine Benutzer:in durch einen Club-Administrator bestätigt werden. Durch die Bestätigung wird eine Benutzer:in zum <em>Clubmitglied</em>. 
- Clubmitglieder (also bestätigte Benutzer:innen, s.o.) können neben den öffentlichen Touren auch Touren sehen, die nur für Clubmitglieder freigegeben wurden. Außerdem können Clubmitglieder Touren erstellen, kommentieren usw. Allerdings müssen neu estellte Touren noch freigegeben werden - durch einen Reviewer (s.u.) - bevor sie für andere Benutzer:innen/Mitglieder sichtbar sind.
- Wenn ein <em>Clubmitglied</em> den Status <em>Autor:in</em> hat, werden erstellte Touren sofort veröffentlicht, d.h. müssen nicht erst durch einen <em>Reviewer</em> freigegeben werden.
- Ein <em>Reviewer</em> kann Touren, die von Clubmitgliedern erstellt werden für die Veröffentlichung freigeben.
- Eine <em>Club-Administrator:in</em> verwaltet die <em>Benutzer:innen</em>, d.h. vergibt den Status <em>Clubmitglied</em>, <em>"Autor:in"</em> und <em>Reviewer</em>.

