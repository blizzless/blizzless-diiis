 

![](pictures/logo.png)

# DiIiS Project

DiIiS is a fully-functional open-source local server for [Diablo III: Reaper of Souls](https://eu.diablo3.blizzard.com)

## Features

- Implemented account creation system, authorization and lobby.
- Fully implemented chat system.
- Fully implemented clan system.
- Opened all cosmetics in the in-game store.
- Implemented basic DRLG.
- Implemented item generator with in-game affixes.
- Implemented the basic mechanics of almost all active abilities for all classes.
- Implemented a system of set items.
- Implemented all main scripts for all story quests 5 acts.
- Implemented basic scripts and generator for "Adventure Mode".
- Created the basis for the "Challenge Nephalem Rifts" mode.
- Implemented artificial intelligence for 80% of minions.
- Implemented personal artificial intelligence for 40% of all monsters.
- Implemented personal artificial intelligence for half of the Bosses.
- Implemented LAN
- NAT support is hidden, but possible ;)

## Installation

### General steps
1. Install [PostgreSQL 9.5.25](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads).
2. Create databases in PostgreSQL: `diiis` and `worlds`
3. Change you account and password in `database.Account.config` and `database.Worlds.conifg`
4. Restore `worlds.backup` to `worlds` database
5. Compile by [VS 2019/2022](https://visualstudio.microsoft.com/)
6. Launch wait until server start, it creates a hierarchy.
7. Create account using console: `!account add Login Password Tag`
8. Install certificate `bnetserver.p12`, password - `123` (the game verifies the CA root certificates).
9. Use Client Diablo 3 `2.7.3.82785`.
9. Add redirects to the `hosts` file (`%WinDir%\System32\drivers\etc\hosts`):  
    `127.0.0.1 us.actual.battle.net`  
    `127.0.0.1 eu.actual.battle.net`
11. Launch client (`x64` or `x86`) with arguments `"Diablo III64.exe" -launch -uid diablo3_engb`
10. Login to the game using your credentials =)

### Using Docker
Run `docker-compose up` inside `db` folder and continue from the 5th step in section above

## Playing with friends

1. Create new accounts using the console command:
    `!account add Login Password Tag`
2. Copy the [config.ini](configs/config.ini) file to the server folder (It overwrites the default settings)
3. In the IP fields - write your IP within the network. Update the parameter entries: `BindIP` and `PublicIP`.
4. Other players must specify your IP address in the `hosts` file (`%WinDir%\System32\drivers\etc\hosts`).
    `192.168.1.1 us.actual.battle.net`
    `192.168.1.1 eu.actual.battle.net`
5. Launch client (`x64` or `x86`) with arguments `"Diablo III64.exe" -launch -uid diablo3_engb`
6. Login to the game using your credentials
7. After that, when creating a game (in client), indicate the creation of a public game.
7. Other players, when connecting, must also indicate a public game, and at the start they will connect to you.

## Flexible configuration

Using the configuration file you can easily override the [global world parameters](docs/game-world-settings.md).

## Minimum system requirements

Minimum system requirements for server

- CPU: Xeon E5-2620V3 (2.40 GHz and 6 cores)
- RAM: 4GB
- HDD/SSD: 500MB

## Screenshots

You can see more screenshots [here](SCREENSHOTS.md)

![](pictures/ingame-screen-1.png)

