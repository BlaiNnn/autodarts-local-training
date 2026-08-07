# autodarts-local-training
Train your darts skills with training plans using Autodarts.io in your local environment

## TODO
- Feature: More modes as listed below
- Feature: nicer & scaling UI
- Feature: Show turn number on every mode 
- Feature: German translation
- Feature: Make statistics available for user with Charts
- Feature: Add multi user option?
- Feature: Add an option to make corrections
- Feature: Add an option to go back to last turn / goal
- GitHub: Add release pipeline
- GitHub: Add branch protection
- Bugfix: Don't always start Ui in full-screen mode
- Bugfix: Show target 25 as "Bull"
- Bugfix: Add Bonus in mode Shanghai 
- Refactor: add extension methods for throw formatting
=> Perhaps this has been taken care of with PR6. If not, please describe the issue.
- Refactor: move autodarts status handling to core project
- Refactor: remove enum "TrainingMode", list implementations that inherit ITrainingMode instead
- Refactor: rename project folder to match project names
- Refactor: group similar modes with customization options


## missing modes for letsplaydarts-trainingplans
- Aufwärmroutine
  => Nur Anzeigen was gespielt werden soll
  => Keine Aufzeichnung, keine Wertung
  => 3 x 3 Darts auf BullsEye
  => 3 x 3 Darts auf D20, D10, D5
  => 3 x 3 Darts auf D16, D8, D4
  => 3 x 3 Darts auf D12, D6, D3
  => Gespielt wird jeweils der erste Dart auf's erste Doppel, der zweite auf 's zweite Doppel und der dritte auf's dritte Doppel

- 120 Checkout
  => 9 Darts zum Checken / Start bei 120 Punkten / plus 10 Punkte bei gelungenem
  Check / minus 1 Punkt bei verpasstem Check
  => 25 Runden
  => Checkout-Zahl nach 25 Runden gleich Ergebniss

