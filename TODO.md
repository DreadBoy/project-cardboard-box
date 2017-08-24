## Bugs
- [ ] Display hint at final destination not player's current destination

## Planned features
- [ ] When player dies or server closes, client should change screen and display "game over" screen
- [ ] When there's only 1 player alive, server should display end screen with scoreboard
- [ ] When player dies or is victorious, server should notify it of outcome
- [x] Assign player colour to each player. 
- [ ] Display all color cards near top edge of screen on server and change background (which one?) of client's chips/sections/buttons. 
- [x] Also change colour of hints.
- [x] Display hints on server when player is assembling command
- [x] When player launches client for the first time, ask him for name and colour
- [x] When player waits for other players, have an option to change name and colour

- [x] When server is launched, display some introduction and QR code. 
- [x] When first player joins, animates QR code to both left and right of title screen and hide text
- [ ] When client is waiting for a game longer then 10 seconds, display little "Help?" button.
- [ ] When "Help?" button is clicked, show dialog, explaning that there's PC game and common troubleshooting

## Screens for name and colour [x]
When user joins for first time, ask him about nickname. Simple text "What's your nickname?" with input below. Under input is disabled button "Confirm" and further down is "Skip" button. When entered or skipped, screen swipes left and from right next screen with colour selection. "What's your colour?" with 10 swatches and same "Confirm" and "Skip" buttons. 

When player joins a server, he sees "Ready?" button and "Change nickname" button on left and "change colour" button on right. If going left, buttons now says "Change Colour" and "Back to game". Similar for right screen but buttons are switched. Player is always on click away from game screen and other screen.

## Colours [x]
Hints are displayed in players' colours and when there's no hint, there's coloured circle below players' characters. In Lobby screen, each character has a name above then heads. 

## Limits
Limit number of players that can play at one time. Lobby and podium currently have hardcoded positions for players, either do this better or limit number of players. If limit is reached, clients should display error message.

## Game over screens
When game is over, server switches to podium. When player dies or wins, he is notified of this and this message stays up until end of game. When game is finished server disconects all players and stays on podium screen. When first player connects, it switches back to lobby.