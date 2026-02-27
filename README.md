# TestDDS - Cyclone DDS C# Publisher (CycloneDDS.NET)

This repository now contains a C# component that publishes a simple topic to a Cyclone DDS domain using the pjanec/CycloneDds.NET implementation.

## Project

- `src/CyclonePublisherComponent/CycloneDdsPublisher.cs`: reusable publisher component.
- `src/CyclonePublisherComponent/SimpleMessage.cs`: simple topic type.
- `src/CyclonePublisherComponent/CycloneDdsSubscriber.cs`: reusable subscriber component.
- `src/CyclonePublisherComponent/Program.cs`: runnable example that publishes and receives on separate threads.

## Run

```bash
cd src/CyclonePublisherComponent
dotnet restore
dotnet run
```

> Dependency: [`CycloneDDS.NET` 0.1.25](https://www.nuget.org/packages/CycloneDDS.NET/0.1.25) from [pjanec/CycloneDds.NET](https://github.com/pjanec/CycloneDds.NET).

By default it publishes on:

- Domain ID: `0`
- Topic Name: `SimpleTopic`
- Type: `SimpleMessage` (`Message`, `TimestampUnixMs`)

The sample starts dedicated sender and receiver threads (`TaskCreationOptions.LongRunning`) so publishing and receiving run concurrently.

Press `Ctrl+C` to stop.
