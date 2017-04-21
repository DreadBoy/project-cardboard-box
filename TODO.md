
##Bugs
- [ ] If you send set of command and quickly follow up with second set of commands while first set is still executing, you don't get new hand before first set is done executing
- [ ] Display hint at final destination not player's current destination

##Planned features
- [x] When player dies, client should change screen and display "game over" screen
- [ ] When there's only 1 player alive, server should display end screen with scoreboard
- [ ] When player dies or is victorious, server should notify it of outcome
- [ ] Assign player colour to each player. Display all color cards near top edge of screen on server and change background (which one?) of client's chips/sections/buttons. Also change colour of hints.
- [x] Display hints on server when player is assembling command
- [ ] When player launches client for the first time, ask him for name and colour
- [ ] When player waits for other players, have an option to change colour


##Screens for name and colour
When user joins for first time, ask him about nickname. Simple text "What's your nickname?" with input below. Under input is disabled button "Confirm" and further down is "Skip" button. When entered or skipped, screen swipes left and from right next screen with colour selection. "What's your colour?" with 10 swatches and same "Confirm" and "Skip" buttons. 

When player joins a server, he sees "Ready?" button and "Change nickname" button on left and "change colour" button on right. If going left, buttons now says "Change Colour" and "Back to game". Similar for right screen but buttons are switched. Player is always on click away from game screen and other screen.

##Colours
Hints are displayed in players' colours and when there's no hint, there's coloured circle below players' characters. In Lobby screen, each character has coloured circle at their feet and name above then heads. 