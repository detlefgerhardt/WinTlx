# WinTelex
A Windows terminal software for the i-Telex network

## Features
- No i-telex hardware needed (direct TCP/IP connection)
- Outgoing and incoming connection (fixed port 134)
- Subscriber server query
- Automatic ITA2 code conversion
- Resizable and scrollable terminal window
- Inactivity timeout (2 minutes)
- copy and paste function
- Sending text files
- Logging all communication to a text file.
- Runs on any Windows version with .NET Framework 4.5 and above 

![Screenshot](https://github.com/detlefgerhardt/WinTelex/blob/master/WinTelexScreen.png)

## Version history

```
1.0.0.1 - logging, improved error handling, inactivity timer
1.0.0.2 - direct entry of peer address, port and extension is now possible
        - fixed error on incoming connection (there still seems to be a problem)
        - last line was not shown in terminal window
        - Terminal windows is now scrollable
1.0.0.3 - copy/paste implemented (context menu and ctrl-c/ctrl-v)
1.0.0.4 - Removed expire date
        - The size of the terminal window can now be changed dynamically.
        - First GitHub release
1.0.0.5 - Show messages when connecting to subscribe server
```

## Credits

An [i-Telex-Library](https://sourceforge.net/projects/itelex) for Arduino/Atmel by Fred Sonnenrein.

The [piTelex project](https://github.com/fablab-wue/piTelex) by FabLab Würzburg.
