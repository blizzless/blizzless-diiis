# Server Commands List

## Account  Commands

| Command  Group | Command        | Example                             | Description                                    |
| -------------- | -------------- | ----------------------------------- | ---------------------------------------------- |
| Account Group  | `show`         | `!account show test@`               | Shows information about given account          |
|                | `add`          | `!account add test@ 123456 test`    | Allows you to add a new user account           |
|                | `setpassword`  | `!account setpassword test@ 123654` | Allows you to set a new password for account   |
|                | `setbtag`      | `!account setbtag test@ NonTest`    | Allows you to change battle tag for account    |
|                | `setuserlevel` | `!account setuserlevel admin`       | Allows you to set a new user level for account |
| Mute Command   | `mute`         | `!mute test@`                       | Disable chat functions for user                |

## Game  Commands

| Command  Group          | Command     | Example                  | Description                                                  |
| ----------------------- | ----------- | ------------------------ | ------------------------------------------------------------ |
| Spawn command           | `spawn`     | `!spawn 6632`            | Spawn a mob by ID                                            |
| Level up command        | `levelup`   | `!levelup 2`             | Levels you character                                         |
| Unlock Artisans command | `unlockart` | `!unlockart`             | Unlock all artisans for you in Campaign                      |
| Platinum command        | `platinum`  | `!platinum 100`          | Platinum for you                                             |
| Gold command            | `gold`      | `!gold 100`              | Gold for you?                                                |
| Item command            | `item`      | `!item p71_ethereal_10`  | Get any Item by Name                                         |
| Teleport command        | `tp`        | `!tp 71150`              | Teleport character to World by ID                            |
| SpeedHack command       | `speed`     | `!speed 2`               | Increase you speed character                                 |
| Lookup Command          | `lookup`    | `!lookup item axe`       | Display all founded in game objects with entered text in Name |
|                         |             | `!lookup world Tristram` |                                                              |
|                         |             | `!lookup actor zombie`   |                                                              |
|                         |             | `!lookup power Punch`    |                                                              |

# All Commands
Available commands from `!commands`: 

### !account
Provides account management commands.

### !mute
Disables chat messages for the account for some defined time span.

### !tag
Switch private Tag for connect

### !powerful
Makes your character with absurd amount of damage. Useful for testing.

### !resourceful
Makes your character with full resource. Useful for testing.

### !info
Get current game information.

### !followers
Manage your followers.

### !difficulty
Changes difficulty of the game

### !heal
Heals yourself

### !invulnerable
Makes you invulnerable

### !spawn
Spawns a mob.

Usage: spawn [actorSNO] [amount]

### !levelup
Levels your character.

Optionally specify the number of levels: !levelup [count]

### !unlockart
Unlock all artisans: !unlockart

### !platinum
Platinum for your character.

Optionally specify the number of levels: !platinum [count]

### !stashup
Upgrade Stash.

### !gold
Gold for your character.

Optionally specify the number of gold: !gold [count]

### !achiplatinum
Platinum for your character.

Optionally specify the number of levels: !platinum [count]

### !eff
Platinum for your character.

Optionally specify the number of levels: !eff [count]

### !item
Spawns an item (with a name or type).

Usage: item [type <type>|<name>] [amount]

### !drop
Drops an epic item for your class.

Optionally specify the number of items: !drop [1-20]

### !tp
Transfers your character to another world.

### !conversation
Starts a conversation.

 Usage: conversation snoConversation

### !speed
Modify speed walk of you character.

Usage: !speed <value>

Reset: !speed

### !commands
Lists available commands for your user-level.

### !help
usage: help <command>

Type 'commands' to get a list of available commands.

### !quest
Retrieves information about quest states and manipulates quest progress.

 Usage: quest [triggers | trigger eventType eventValue | advance snoQuest]

### !lookup
Searches in sno databases.

Usage: lookup [actor|conv|power|scene|la|sp|weather] <pattern>

# Item List

You can use the official website to search items: https://eu.diablo3.blizzard.com/en-us/item/

```c
// Sample: Firebird's Breast
// Url: https://eu.diablo3.blizzard.com/en-us/item/firebirds-breast-Unique_Chest_Set_06_x1
// Name: Unique_Chest_Set_06_x1
```

You can also access the elements created for testing during game development :)
