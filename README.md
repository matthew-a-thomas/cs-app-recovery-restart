# cs-app-recovery-restart

Demonstration of Windows Application Recovery and Restart in a C# application

https://docs.microsoft.com/en-us/windows/win32/recovery/application-recovery-and-restart-portal

> An application can use Application Recovery and Restart (ARR) to save data and state information before the application exits due to an unhandled exception or when the application stops responding. The application is also restarted, if requested.

## Caveats

Windows Error Reporting will not trigger either your restart or your recovery callback unless the application has been running for at least 60 seconds.

## Dump files

You can combine this with some registry settings to gather mini (or full, or custom) dump files into the location of your choosing. That dump file will be available by the time ARR triggers your recovery or restart.

https://docs.microsoft.com/en-us/windows/win32/wer/collecting-user-mode-dumps
