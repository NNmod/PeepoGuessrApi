[![title-banner](https://cdn.nnmod.com/ppg-banner.png)](https://ppg.nnmod.com/)

## What is PeepoGuessr
PeepoGuessr is a game 'find a place' in the special worlds from minecraft server Pepeland.
Guessr where you are, depends on environment around you.

## Development
PeepoGuessr is not ready to use project and require changes to startup your own game based on PeepoGuessr.

### Requirements
PeepoGuessr Api require [PeepoGuessr Vue](https://github.com/NNmod/PeepoGuessrVue) project to work.

### Configuration
PeepoGuessr Api use configuration json below:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "AccountDb": "",
    "MaintenanceDb": "",
    "LobbyDb": "",
    "GameDb": ""
  },
  "Twitch": {
    "ClientId": "",
    "ClientSecret": "",
    "ClientRedirect": "https://ppg.nnmod.com/api/account/user/sign-in/processing"
  },
  "Cdn": {
    "Maps": "https://ppg.cdn.nnmod.com/maps"
  },
  "Game": {
    "Limit": 50
  }
}
```
