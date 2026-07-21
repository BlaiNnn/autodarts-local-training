# autodarts-local-training
Train your darts skills with training plans using Autodarts.io in your local environment

## TODO
- Feature: More modes as listed below
- Feature: nicer & scaling UI
- Feature: German translation
- Feature: Make statistics available for user with Charts
- Feature: Add multi user option?
- Feature: Add an option to make corrections
- Feature: Add an option to go back to last turn / goal
- GitHub: Add release pipeline
- GitHub: Add branch protection
- Bugfix: Don't always start Ui in full-screen mode
- Refactor: add extension methods for throw formatting
=> Perhaps this has been taken care of with PR6. If not, please describe the issue.
- Refactor: move autodarts status handling to core project
- Refactor: remove enum "TrainingMode", list implementations that inherit ITrainingMode instead
- Refactor: rename project folder to match project names


## missing modes for letsplaydarts-trainingplans
- Aufwärmroutine
  => Nur Anzeigen was gespielt werden soll
  => Keine Aufzeichnung, keine Wertung
  => 3 x 3 Darts auf BullsEye
  => 3 x 3 Darts auf D20, D10, D5
  => 3 x 3 Darts auf D16, D8, D4
  => 3 x 3 Darts auf D12, D6, D3
  => Gespielt wird jeweils der erste Dart auf's erste Doppel, der zweite auf 's zweite Doppel und der dritte auf's dritte Doppel

- Round the Board Single-Felder
  => 3 Darts pro Zahl / 1 Punkt pro Treffer
  => Einmal durchspielen 1-20


- 33 Darts auf 20
  => 1 Punkt pro Single (und Doppel) / 3 Punkte pro Triple

- 33 Darts auf 19
  => 1 Punkt pro Single (und Doppel) / 3 Punkte pro Triple

- 33 Darts auf Bull
  => 1 Punkt pro Single / 2 Punkte pro DBull

- 3-Darts-Checkouts
  => 40 Checkouts zwischen 3 und 61
  3 5 7 9 11 13 15 17 19 21
  23 25 27 29 31 33 35 37 39 41
  42 43 44 45 46 47 48 49 50 51
  52 53 54 55 56 57 58 59 60 61
  => 3 Darts = 1 Punkt / 2 Darts = 2 Punkte

- Catch 40
  => 40 Checkouts zwischen 61 und 100
  61 62 63 64 65 66 67 68 69 70
  71 72 73 74 75 76 77 78 79 80
  81 82 83 84 85 86 87 88 89 90
  91 92 93 94 95 96 97 98 99 100
  => (max. 6 Darts: 2 Darts = 3 Punkte / 3 Darts = 2 Punkte / 4-6 Darts = 1 Punkt)

- 51 Darts auf 20
  => 1 Punkt pro Single (und Doppel) / 3 Punkte pro Triple

- 51 Darts auf 19
  => 1 Punkt pro Single (und Doppel) / 3 Punkte pro Triple

- 51 Darts auf 18
  => 1 Punkt pro Single (und Doppel) / 3 Punkte pro Triple

- Shanghai
  => 3 Darts je Ziel: Single = 1 Punkt / Double = 2 Punkte / Triple = 3 Punkte
  => Einmal durchspielen 1-20

- Cricket-Scoring
  => 3 Darts je Ziel: Single = 1 Punkt / Double = 2 Punkte / Triple = 3 Punkte / maximal 9 Punkte pro Ziel erreichbar
  => 15 16 17 18 19 20 Bull

- Bob's 27
  => je 3 Darts auf alle Doppelfelder / Start bei 27 Punkten / je getroffenes Doppel wird der Wert
  addiert und bei keinem Treffer wird der Wert des Doppel abgezogen
  => Einmal durchspielen 1-20-Bull

- Doppel-Routine
  => 10 x 3 Darts auf die Doppel / 1 Punkt je Treffer
  => Gespielt wird jeweils der erste Dart auf's erste Doppel, der zweite auf 's zweite Doppel und der dritte auf's dritte Doppel
  ==> D20 / D10 / D5
  ==> D16 / D8 / D4
  ==> D12 / D6 / D3

- Round the Board - Doppelfelder
  => 3 Darts pro Zahl / 1 Punkt pro Treffer
  => Einmal durchspielen 1-20 (Bull?)

- 300 Darts Highscore
  => so viele Punkte wie möglich
  => Summe 300 Darts / 100 = Punkte

- 120 Checkout
  => 9 Darts zum Checken / Start bei 120 Punkten / plus 10 Punkte bei gelungenem
  Check / minus 1 Punkt bei verpasstem Check
  => 25 Runden
  => Checkout-Zahl nach 25 Runden gleich Ergebniss

- 3-Dart-Checkouts (20)
  => 1 Punkt pro Checkout
  40 60 68 72 76 80 84 88 90 92
  96 98 100 108 112 116 120 140 160 170

- 3-Dart-Checkouts (19)
  => 1 Punkt pro Checkout
  38 59 69 73 79 89 93 99 107 119
  126 129 133 134 137 139 149 154 157 167

- 3-Dart-Checkouts (18)
  => 1 Punkt pro Checkout
  36 58 62 66 68 70 74 78 86 90
  94 102 104 108 118 122 134 144 154 164

