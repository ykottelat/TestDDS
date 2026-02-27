# TestDDS - Cyclone DDS C# Publisher (CycloneDDS.NET)

This repository now contains a C# component that publishes a simple topic to a Cyclone DDS domain.

## Project

- `src/CyclonePublisherComponent/CycloneDdsPublisher.cs`: reusable publisher component.
- `src/CyclonePublisherComponent/SimpleMessage.cs`: simple topic type.
- `src/CyclonePublisherComponent/Program.cs`: runnable example that publishes once per second.

## Run

```bash
cd src/CyclonePublisherComponent
dotnet restore
dotnet run
```

> Dependency: [`CycloneDDS.NET` 0.1.25](https://www.nuget.org/packages/CycloneDDS.NET/0.1.25)

By default it publishes on:

- Domain ID: `0`
- Topic Name: `SimpleTopic`
- Type: `SimpleMessage` (`Message`, `TimestampUnixMs`)

Press `Ctrl+C` to stop.
