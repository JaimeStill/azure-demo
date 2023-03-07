# Cross Service SignalR

* [Demonstration](#demonstration)
* [Notes](#notes)

The purpose of this test is to demonstrate long-running, cross-service synchronization of a formalized process. This is done by leveraging a stand-alone SignalR server that acts as a message broker between any interested clients and the service that is performing some external process. To standardize the implementation of this workflow, a [`Sync`](../../src/Arma.Demo.Core/Sync/) library has been written.

In this case, there are three servers and a CLI client. The servers are:

* An [API server](./apis/core-api/) that:
    * Receives [`Package`](../../src/Arma.Demo.Core/Processing/Package.cs) objects via [`HttpPost`](./apis/core-api/Controllers/ProcessController.cs).
    * Pushes the `Package` out to the SignalR server in a [`SyncMessage`](../../src/Arma.Demo.Core/Sync/SyncMessage.cs) via its [`ProcessorService`](./apis/core-api/Services/ProcessorService.cs). `ProcessorService` derives from [`SyncService<T>`](../../src/Arma.Demo.Core/Sync/SyncService.cs), where `T` is `Package`. The `SyncService` encapsulates a `HubConnection` client and defines the standard [`ISyncService`](../../src/Arma.Demo.Core/Sync/ISyncService.cs) API, which maps to the features exposed by the [`ISyncHub`](../../src/Arma.Demo.Core/Sync/ISyncHub.cs) contract implemented by [`SyncHub`](../../src/Arma.Demo.Core/Sync/SyncHub.cs).
* A [Processor server](./apis/processor/) that:
    * Registers its own [`ProcessorService`](./apis/processor/Services/ProcessorService.cs), which on [app startup](./apis/processor/Program.cs#L21) registers with the [`SyncGroupProvider`](../../src/Arma.Demo.Core/Sync/SyncGroupProvider.cs) as a service via the [`SyncHub.RegisterService`](../../src/Arma.Demo.Core/Sync/SyncHub.cs) method.
    * Whenever a [`SyncHub.SendPush`](../../src/Arma.Demo.Core/Sync/SyncHub.cs) action is executed, all registered services are broadcast the provided `SyncMessage`. This message is intercepted through the registered [`OnPush`](./apis/processor/Services/ProcessorService.cs#L20) service hook initially exposed through `SyncService`.
    * When a `SyncMessage` is received through the `OnPush` event, the service executes the [`ProcessPackage`](./apis/processor/Services/ProcessorService.cs#L42) method, simulating a long-running processes that broadcasts progress out through the underlying SignalR client.
* A [Sync server](./apis/sync-server/) that:
    * Exposes a [`ProcessorHub`](./apis/sync-server/Hubs/ProcessorHub.cs), which inherits from the abstract [`SyncHub`](../../src/Arma.Demo.Core/Sync/SyncHub.cs) class provided by the Sync library.
    * Through this server, services can register to receive messages and execute corresponding functionality without ever having to be directly invoked by another external service.

## Demonstration

1. Run the **Sync Server**:

    ```bash
    dotnet run ./apis/sync-server
    ```

    ![image](https://user-images.githubusercontent.com/14102723/222829965-4e789b10-fa1c-4b00-9d4c-d918e96f14ab.png)

2. Run the **Processor** server;

    ```bash
    dotnet run ./apis/processor
    ```

    ![image](https://user-images.githubusercontent.com/14102723/222830080-9773080f-ace8-45be-a9af-53223f2360ec.png)

3. Run the **API** server:

    ```bash
    dotnet run ./apis/core-api
    ```

    ![image](https://user-images.githubusercontent.com/14102723/222830359-a58227b8-fd65-4e0b-b2ab-9dab61a2816e.png)

4. Run the **CLI** app:

    > You can pass any [`Intent`](../../src/Arma.Demo.Core/Processing/Intent.cs) as an argument with `-i`

    ```bash
    dotnet run ./app/core-cli -- process
    ```

    https://user-images.githubusercontent.com/14102723/222830888-380bef11-e9b5-458f-9a7e-61c1830f003e.mp4
    
If you look at the output from the servers after executing the CLI command, you'll see logged output events:

**API Server**

![image](https://user-images.githubusercontent.com/14102723/222831058-557f1181-f726-4632-8ec8-7a73c3b3a7ed.png)

**Processor Server**

![image](https://user-images.githubusercontent.com/14102723/223285387-6d3ab6f0-0d32-405a-97db-280e74ba29db.png)

**Sync Server**

![image](https://user-images.githubusercontent.com/14102723/222831242-742e5d03-05ab-48e1-afdf-992724cb46b9.png)

## Notes

With this implementation, the tracking of any internal data can be standardized by encapsulating it into a publicly exposed contract (i.e. [`Resource`](../../src/Arma.Demo.Core/Processing/Resource.cs) inside of a `Package`) and transmitting it through a [`SyncMessage`](../../src/Arma.Demo.Core/Sync/SyncMessage.cs).

Multilple iterations of this workflow could be chained together to facilitate various process complexities.

All of the infrastructure needed to leverage this standardized flow is exposed in a [NuGet package](https://www.nuget.org/packages/Arma.Demo.Core). All you need to do is define the infrastructure relative to the data you want to track and the services you want to leverage.