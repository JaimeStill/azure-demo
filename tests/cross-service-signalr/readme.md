# Cross Service SignalR

1. Run the processor API

    ```bash
    dotnet run ./apis/processor-api
    ```

2. Run the core API

    ```bash
    dotnet run ./apis/core-api
    ```

3. Run the CLI

    > You can pass any [`Intent`](../../src/Arma.Demo.Core/Processing/Intent.cs) as an argument with `-i`

    ```bash
    dotnet run ./app/core-cli -- process
    ```

https://user-images.githubusercontent.com/14102723/222603284-c95920da-1801-4cea-8146-ac4c587ecc05.mp4

