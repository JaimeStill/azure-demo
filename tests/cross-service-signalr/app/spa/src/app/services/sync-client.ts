import {
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel
} from '@microsoft/signalr';

import { SyncMessage } from '../models';
import { v4 as uuid } from 'uuid';

export abstract class SyncClient<T> {
    protected key: string;
    protected connection: HubConnection;

    endpoint: string;
    messages: string[] = new Array<string>();

    onRegistered: (guid: string) => void = (guid: string) => this.key = guid;
    onPush: (message: SyncMessage<T>) => void;
    onNotify: (message: SyncMessage<T>) => void;
    onComplete: (message: SyncMessage<T>) => void;
    onReturn: (message: SyncMessage<T>) => void;
    onReject: (message: SyncMessage<T>) => void;

    protected registerEvents = () => {
        this.connection.onclose(async () => await this.start());

        this.connection.on("Registered", this.onRegistered);
        this.connection.on("Push", this.onPush);
        this.connection.on("Notify", this.onNotify);
        this.connection.on("Complete", this.onComplete);
        this.connection.on("Return", this.onReturn);
        this.connection.on("Reject", this.onReject);
    }

    protected initialize = () => {
        this.connection = new HubConnectionBuilder()
            .withUrl(this.endpoint)
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        this.registerEvents();
    }

    constructor(endpoint: string) {
        this.endpoint = endpoint;
        this.initialize();
    }

    state = (): HubConnectionState => this.connection.state;

    start = async () => {
        try {
            this.messages.push(`Connecting to ${this.endpoint}`);
            await this.connection.start();
            this.messages.push(`Connected to ${this.endpoint}`);
        } catch (err) {
            console.log(err);
            setTimeout(this.start, 5000);
        }
    }

    register = async () => await this.connection.invoke("RegisterListener", uuid())
}