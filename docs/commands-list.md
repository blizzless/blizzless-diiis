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

# Item List

You can use the official website to search items: https://eu.diablo3.blizzard.com/en-us/item/

```c
// Sample: Firebird's Breast
// Url: https://eu.diablo3.blizzard.com/en-us/item/firebirds-breast-Unique_Chest_Set_06_x1
// Name: Unique_Chest_Set_06_x1
```

You can also access the elements created for testing during game development :)
