# Filter Service Calendar

Forecast upcoming filter service dates from calendar and operating-hour intervals.

![Sample run from Filter Service Calendar](docs/demo.webp)

## Run

Install .NET 8 SDK and runtime. Run these commands from the repository root.

```sh
dotnet build --nologo
dotnet bin/Debug/net8.0/App.dll sample.json
```

Replace `sample.json` with a JSON file following the same structure. The app prints a JSON report to standard output.

Enter a reference date, service history, both replacement intervals, and estimated daily operating hours. Use the tool to plan reminders from your own maintenance schedule.

## Verify

Run `python check.py` with Python 3.11 or newer after the build step when applicable. The check verifies the bundled scenario and confirms that an invalid input returns an error.
