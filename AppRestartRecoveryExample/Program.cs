namespace AppRestartRecoveryExample
{
    using Windows.Win32;

    class Program
    {
        const string Secret = "2bdd8ed6-ed88-4c21-90fd-3f5c44aff587";

        static void Main(string[] args)
        {
            if (args.Any(x => x == Secret))
            {
                Console.WriteLine("This application was restarted successfully");
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey(true);
            }
            else
            {
                var waitTime = DateTime.Now + TimeSpan.FromMinutes(1);
                var registered = false;
                while (true)
                {
                    Console.WriteLine("What would you like to do?");
                    Console.WriteLine("1: Register for recovery");
                    Console.WriteLine("2: Register for restart");
                    Console.WriteLine("3: Crash");

                    if (!int.TryParse(Console.ReadLine(), out var choice) || choice is < 1 or > 3)
                    {
                        Console.WriteLine("Invalid choice. Try again.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1: // Register for recovery
                        {
                            unsafe
                            {
                                PInvoke.RegisterApplicationRecoveryCallback(
                                    pRecoveyCallback: _ => {
                                        Console.WriteLine("You are now in the recovery callback. Press Space to end recovery successfully. Press Esc to end recovery unsuccessfully. Press any other key to signal that recovery is still in progress. If you don't do anything for five seconds then the application will terminate");
                                        bool success;
                                        while (true)
                                        {
                                            switch (Console.ReadKey(true).Key)
                                            {
                                                case ConsoleKey.Spacebar:
                                                    success = true;
                                                    goto Complete;
                                                case ConsoleKey.Escape:
                                                    success = false;
                                                    goto Complete;
                                            }
                                            PInvoke.ApplicationRecoveryInProgress(out var canceled).ThrowOnFailure();
                                            Console.WriteLine($"The user has {(canceled ? "" : "not ")}requested recovery cancellation.");
                                        }
                                        Complete:
                                        Console.WriteLine($"Signaling the end of a{(success ? " successful" : "n unsuccessful")} recovery");
                                        PInvoke.ApplicationRecoveryFinished(success);
                                        return 0;
                                    },
                                    pvParameter: default,
                                    dwPingInterval: 5000,
                                    dwFlags: default
                                );
                            }
                            registered = true;
                            Console.WriteLine("Registered for recovery!");
                        }
                            break;
                        case 2: // Register for restart
                            {
                                PInvoke.RegisterApplicationRestart(
                                    Secret,
                                    default
                                ).ThrowOnFailure();
                                registered = true;
                                Console.WriteLine("Registered for restart!");
                            }
                            break;
                        case 3: // Crash
                            {
                                if (registered)
                                {
                                    while (true)
                                    {
                                        var now = DateTime.Now;
                                        var interval = waitTime - now;
                                        if (interval > TimeSpan.Zero)
                                        {
                                            Console.WriteLine($"Sleeping until {waitTime:T} because you registered for either restart or recovery and WER doesn't trigger that until this application has been alive for at least a minute...");
                                            Thread.Sleep(interval);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                Environment.FailFast("Uh oh!");
                            }
                            break;
                    }
                }
            }
        }
    }
}